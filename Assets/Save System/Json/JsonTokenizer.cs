using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;

public enum JsonTokenTypes
{
    None,
    OpeningCurrlyBracket,
    ClosingCurrlyBracket,
    OpeningSquareBracket,
    ClosingSquareBracket,
    DigitInt,
    DigitFloat,
    Colon,
    Comma,
    String,
    True,
    False,
    Null,
}

public class JsonToken
{
    public JsonTokenTypes token_type;
    public string         token_value;
}

public class JsonTokenizer
{
    private List<JsonToken> tokens;

    private int current_index = 0;

    private string json_data = null;

    public JsonTokenizer(string file)
    {
        StreamReader reader;
        try
        {
            reader = new StreamReader(file);
        }
        catch (System.IO.FileNotFoundException ex)
        {
            throw ex;
        }

        tokens = new List<JsonToken>();

        this.json_data = reader.ReadToEnd();

        while (this.current_index < this.json_data.Length)
        {
            char peek_char = this.Peek();

            // There is a range for valid characters, but it is not checked rn.
            JsonToken token = new JsonToken();
            token.token_type  = JsonTokenTypes.None;

            // Digit
            if (peek_char >= '0' && peek_char <= '9')
            {
                tokens.Add(this.ParseNumber());
                continue;
            }

            switch (peek_char)
            {
                case '{':
                    token.token_type = JsonTokenTypes.OpeningCurrlyBracket;
                    break;
                case '}':
                    token.token_type = JsonTokenTypes.ClosingCurrlyBracket;
                    break;
                case '[':
                    token.token_type = JsonTokenTypes.OpeningSquareBracket;
                    break;
                case ']':
                    token.token_type = JsonTokenTypes.ClosingSquareBracket;
                    break;
                case ',':
                    token.token_type = JsonTokenTypes.Comma;
                    break;
                case ':':
                    token.token_type = JsonTokenTypes.Colon;
                    break;
                case '-':
                case '+':
                case '.':
                    tokens.Add(this.ParseNumber());
                    continue;
                case '"':
                    tokens.Add(this.ParseString());
                    continue;
                case 't':
                case 'f':
                case 'n':
                    tokens.Add(this.ParseLiteral());
                    continue;
                case ' ':
                case (char)0x09:
                case (char)0x0A:
                case (char)0x0D:
                    // Ignore white-space(s)
                    this.Consume(current_index, 1);
                    continue;
            }

            token.token_value = this.Consume(current_index, 1);
            tokens.Add(token);
        }

        reader.Close();
    }

    public List<JsonToken> GetTokens()
    {
        return this.tokens;
    }

    private char Peek(int amount = 0)
    {
        if (((this.current_index + amount) < 0) ||
            ((this.current_index + amount) >= this.json_data.Length))
        {
            throw new IndexOutOfRangeException("Unable to peek by the amount given");
        }

        return json_data[this.current_index + amount];
    }

    private string Consume(int start, int count)
    {
        if ((start < 0) ||
            ((start + count) > this.json_data.Length))
        {
            throw new IndexOutOfRangeException("Unable to consume the block given");
        }

        string val = this.json_data.Substring(start, count);
        this.current_index = start + count;
        return val;
    }

    private JsonToken ParseLiteral()
    {
        JsonToken token = new JsonToken();
        string val = null;
        switch (this.Peek())
        {
            case 't':
                val = this.Consume(current_index, 4);
                if (!val.Equals("true"))
                {
                    throw new JsonTokenError("Expected a 'true' literal.");
                }
                token.token_type = JsonTokenTypes.True;
                break;
            case 'f':
                val = this.Consume(current_index, 5);
                if (!val.Equals("false"))
                {
                    throw new JsonTokenError("Expected a 'false' literal.");
                }
                token.token_type = JsonTokenTypes.False;
                break;
            case 'n':
                val = this.Consume(current_index, 4);
                if (!val.Equals("null"))
                {
                    throw new JsonTokenError("Expected a 'null' literal.");
                }
                token.token_type = JsonTokenTypes.Null;
                break;
        }

        token.token_value = val;

        return token;
    }

    private JsonToken ParseString()
    {
        JsonToken ret = new JsonToken();

        if (!(this.Consume(this.current_index, 1).Equals("\"")))
        {
            throw new JsonTokenError("Failed to find the beginning \" for a string literal"); 
        }

        int count = 0;
        while (this.Peek(count) != '"')
        {
            count++;
        }

        ret.token_type  = JsonTokenTypes.String;
        ret.token_value = this.Consume(this.current_index, count);

        // Consume the end double quote
        this.Consume(this.current_index, 1);

        return ret;
    }
    
    private JsonToken ParseNumber()
    {
        JsonToken ret = new JsonToken();
        ret.token_type = JsonTokenTypes.DigitInt;
        int count = 0;

        // See if there is a sign at the beginning
        if (this.Peek() == '+')
        {
            // Drop the +, just number. It's cleaner
            this.Consume(this.current_index, 1);
        }
        else if (this.Peek() == '-')
        {
            count++;
        }

        try
        {
            char peek_char = this.Peek(count);
            while ((peek_char >= '0' && peek_char <= '9'))
            {
                peek_char = this.Peek(++count);
            }

            if (peek_char == '.')
            {
                ret.token_type = JsonTokenTypes.DigitFloat;
                count++;

                peek_char = this.Peek(count);
                if (!(peek_char >= '0' && peek_char <= '9'))
                {
                    throw new JsonTokenError("A float number must have at least one digit after the '.'");
                }

                while ((peek_char >= '0' && peek_char <= '9'))
                {
                    peek_char = this.Peek(++count);
                }
            }

        }
        catch (IndexOutOfRangeException)
        {
            // Ignore
        }

        ret.token_value = this.Consume(this.current_index, count); 

        return ret;
    }
}

public class JsonTokenError : System.Exception
{
    public JsonTokenError(string message) : base(message)
    {
    
    }
}