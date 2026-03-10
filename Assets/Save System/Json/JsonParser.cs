using System.Collections.Generic;

public class JsonParser
{
    private JsonAST jsonAST;
    private JsonToken[] tokens;
    private int current_index;

    public JsonParser() {}

    public JsonAST Parse(string file)
    {
        JsonTokenizer tokenizer = new JsonTokenizer(file);
        this.tokens = tokenizer.GetTokens().ToArray();
        
        this.current_index = 0;
        this.jsonAST = new JsonAST();
        this.jsonAST.value = this.ParseElement();
        return this.jsonAST;
    }

    private JsonValue ParseElement()
    {
        JsonValue ret = null;

        JsonToken peek_token = this.PeekToken();
        switch (peek_token.token_type)
        {
            case JsonTokenTypes.OpeningCurrlyBracket:
                ret = ParseJsonObject();
                break;
            case JsonTokenTypes.OpeningSquareBracket:
                ret = ParseJsonArray();
                break;
            case JsonTokenTypes.String:
                ret = ParseJsonString();
                break;
            case JsonTokenTypes.DigitInt:
                ret = ParseJsonInt();
                break;
            case JsonTokenTypes.DigitFloat:
                ret = ParseJsonFloat();
                break;
            case JsonTokenTypes.True:
            case JsonTokenTypes.False:
                this.ParseJsonBool();
                break;
            case JsonTokenTypes.Null:
                this.ParseJsonNull();
                break;
            default:
                throw new JsonParseError("Unknown token '" + peek_token.token_value + "' for element.");
        }

        return ret;
    }

    private JsonObject ParseJsonObject()
    {
        JsonObject ret = new JsonObject();
        ret.value = new Dictionary<string, JsonValue>();

        // Consume the opening currly
        this.ConsumeToken(current_index, 1);

        // Go until the closing currly
        while (this.PeekToken().token_type != JsonTokenTypes.ClosingCurrlyBracket)
        {
            // Fill up the dictionary
            JsonToken key = this.ConsumeToken(current_index, 1)[0];
            if (key.token_type != JsonTokenTypes.String)
            {
                throw new JsonParseError("Expected a string value for the object key.");
            } 
            
            if (this.ConsumeToken(current_index, 1)[0].token_type != JsonTokenTypes.Colon)
            {
                throw new JsonParseError("Expected a ':' after the object key.");
            }

            JsonValue val = this.ParseElement();

            // Consume trailing comma if there is one
            JsonToken peek_token = this.PeekToken();
            if (peek_token.token_type == JsonTokenTypes.Comma)
            {
                this.ConsumeToken(this.current_index, 1);
            }

            ret.value.Add(key.token_value, val);
        }

        // Consume the closing currly
        this.ConsumeToken(current_index, 1);

        return ret;
    }

    private JsonArray ParseJsonArray()
    {
        JsonArray ret = new JsonArray();
        ret.value = new List<JsonValue>();

        // Consume the opening square bracket 
        this.ConsumeToken(current_index, 1);

        bool is_first = true;

        // Go until the closing square bracket 
        while (this.PeekToken().token_type != JsonTokenTypes.ClosingSquareBracket)
        {
            if (!is_first)
            {
                if (this.ConsumeToken(current_index, 1)[0].token_type != JsonTokenTypes.Comma)
                {
                    throw new JsonParseError("Expected a ',' after the object key.");
                }
            }
            else
            {
                is_first = false;
            }

            // Fill up the list 
            JsonValue element = this.ParseElement();

            ret.value.Add(element);
        }

        // Consume the closing square bracket 
        this.ConsumeToken(current_index, 1);

        return ret;
    }

    private JsonLiteral<string> ParseJsonString()
    {
        JsonLiteral<string> ret = new JsonLiteral<string>();

        JsonToken string_token = this.ConsumeToken(current_index, 1)[0];

        ret.value = string_token.token_value;

        return ret;
    }

    private JsonLiteral<JsonBool> ParseJsonBool()
    {
        JsonLiteral<JsonBool> ret = new JsonLiteral<JsonBool>();
        JsonToken token = this.ConsumeToken(this.current_index, 1)[0];
        switch (token.token_type)
        {
            case JsonTokenTypes.True:
                ret.value.value = true;
                break;
            case JsonTokenTypes.False:
                ret.value.value = false;
                break;
            default:
                throw new JsonParseError("Expected a JsonBool type");
        }

        return ret;
    }

    private JsonLiteral<JsonNull> ParseJsonNull()
    {
        JsonLiteral<JsonNull> ret = new JsonLiteral<JsonNull>();
        JsonToken token = this.ConsumeToken(this.current_index, 1)[0];
        switch (token.token_type)
        {
            case JsonTokenTypes.Null:
                break;
            default:
                throw new JsonParseError("Expected a JsonNull type");
        }

        return ret;
    }

    private JsonLiteral<int> ParseJsonInt()
    {
        JsonLiteral<int> ret = new JsonLiteral<int>();
        JsonToken token = this.ConsumeToken(this.current_index, 1)[0];
        switch (token.token_type)
        {
            case JsonTokenTypes.DigitInt:
                ret.value = int.Parse(token.token_value);
                break;
            default:
                throw new JsonParseError("Expected a JsonDigitInt type");
        }

        return ret;
    }

    private JsonLiteral<float> ParseJsonFloat()
    {
        JsonLiteral<float> ret = new JsonLiteral<float>();
        JsonToken token = this.ConsumeToken(this.current_index, 1)[0];
        switch (token.token_type)
        {
            case JsonTokenTypes.DigitFloat:
                ret.value = float.Parse(token.token_value);
                break;
            default:
                throw new JsonParseError("Expected a JsonDigitFloat type");
        }

        return ret;
    }

    private JsonToken PeekToken(int peek_amount = 0)
    {
        if (this.current_index + peek_amount >= this.tokens.Length)
        {
            throw new JsonParseError("Tried to peek past end of token array");
        }

        return this.tokens[this.current_index + peek_amount];
    }

    private JsonToken[] ConsumeToken(int start, int count)
    {
        if ((start < 0) || ((start + count) > this.tokens.Length))
        {
            throw new JsonParseError("Unable to consume tokens past end of token array.");
        }

        JsonToken[] ret = this.GetSubArray<JsonToken>(this.tokens, start, start + count);
        this.current_index = start + count;
        return ret;
    }

    private T[] GetSubArray<T>(T[] original_array, int start, int end)
    {
        if (start < 0 || end >= original_array.Length)
        {
            return null;
        }

        if (start > end)
        {
            return null;
        }

        if (original_array == null)
        {
            return null;
        }

        T[] new_array = new T[(end - start) + 1];
        
        for (int i = start; i <= end; ++i)
        {
            new_array[i - start] = original_array[i];
        }

        return new_array;
    }
}

public class JsonParseError : System.Exception
{
    public JsonParseError(string message) : base(message)
    {
    
    }
}