using System.Collections.Generic;

public class JsonAST
{
    public JsonValue value { get; set; }

    public JsonAST()
    {
        this.value = null;
    }
}

public abstract class JsonValue
{
    public abstract JsonValue GetValue();
}

public class JsonObject : JsonValue
{
    public Dictionary<string, JsonValue> value;

    public override JsonValue GetValue()
    {
        return this;
    }
}

public class JsonArray : JsonValue
{
    public List<JsonValue> value;

    override public JsonValue GetValue()
    {
        return this;
    }
}

// string, number, true, false, null
public class JsonLiteral<T> : JsonValue
{
    public T value;

    override public JsonValue GetValue()
    {
        return this;
    }
}

public class JsonBool
{
    public bool value = true;
}
public class JsonNull { }
