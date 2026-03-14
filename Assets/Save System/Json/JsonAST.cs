using System.Collections.Generic;
using System.Text;

public class JsonAST
{
    public JsonValue value { get; set; }

    public JsonAST()
    {
        this.value = null;
    }

    public string ToStringJson(bool pretty_print = true)
    {
        return this.value.ToStringJson(pretty_print);
    }
}

public abstract class JsonValue
{
    public abstract string ToStringJson(bool pretty_print = true);

    public abstract string ToStringJson(int tab_level, bool pretty_print = true); 

    protected void AppendTabs(StringBuilder sb, int count)
    {
        for (int i = 0; i < count; ++i)
        {
            sb.Append("\t");
        }
    }
}

public class JsonObject : JsonValue
{
    public Dictionary<string, JsonValue> value;

    public JsonValue this[string key]
    {
        get { return this.value[key]; }
        set { this.value[key] = value; }
    }

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        foreach (string key in this.value.Keys)
        {
            if (pretty_print)
            {
                sb.Append("\n");
                this.AppendTabs(sb, tab_level + 1);
            }
            sb.Append("\"");
            sb.Append(key);
            sb.Append("\"");
            sb.Append(" : ");
            sb.Append(this.value[key].ToStringJson(tab_level + 1, pretty_print));
            sb.Append(",");
        }

        // Remove the extra comma
        sb.Remove(sb.Length - 1, 1);

        if (pretty_print)
        {
            sb.Append("\n");
            this.AppendTabs(sb, tab_level);
        }

        sb.Append("}");

        return sb.ToString();
    }
}

public class JsonArray : JsonValue
{
    public List<JsonValue> value;

    public JsonValue this[int index]
    {
        get { return this.value[index]; }
        set { this.value[index] = value; }
    }

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (int i = 0; i < this.value.Count; ++i)
        {
            if (pretty_print)
            {
                sb.Append("\n");
                this.AppendTabs(sb, tab_level + 1);
            }
            sb.Append(this.value[i].ToStringJson(tab_level + 1, pretty_print));
            if (i < this.value.Count - 1)
            {
                sb.Append(", ");
            }
        }

        if (pretty_print)
        {
            sb.Append("\n");
            this.AppendTabs(sb, tab_level);
        }

        sb.Append("]");

        return sb.ToString();
    }
}

public class JsonString : JsonValue
{
    public string value;

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        return "\"" + value + "\"";
    }
}

public class JsonFloat : JsonValue
{
    public float value;

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        return value.ToString();
    }
}

public class JsonInt : JsonValue
{
    public int value;

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        return value.ToString();
    }
}

public class JsonBool : JsonValue
{
    public bool value = true;

    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        return (value ? "true" : "false");
    }
}

public class JsonNull : JsonValue
{
    override public string ToStringJson(bool pretty_print = true)
    {
        return this.ToStringJson(0, pretty_print);
    }

    override public string ToStringJson(int tab_level, bool pretty_print = true)
    {
        return "null";
    }
}
