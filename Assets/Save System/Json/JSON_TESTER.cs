using System.IO;
using UnityEngine;

public class JSON_TESTER : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /*
        JsonTokenizer jsonTokenizer = new JsonTokenizer(Path.Combine(new string[] { "SAVES", "JSON_TESTING", "test2.json" }));
        List<JsonToken> tokens = jsonTokenizer.GetTokens();
        foreach (JsonToken token in tokens)
        {
            Debug.Log("( " + token.token_type + ", " + token.token_value + " )");
        }
        */
        JsonParser jsonParser = new JsonParser();
        JsonAST ast = jsonParser.Parse(Path.Combine(new string[] { "Assets", "Save System", "Json", "test.json" }));
        JsonObject val = (JsonObject) ((JsonObject)ast.value)["glossary"];

        Debug.Log(((JsonString) (val["title"])).value);
        JsonObject t = (JsonObject) val["GlossDiv"];
        t = (JsonObject) t["GlossList"];
        t = (JsonObject) t["GlossEntry"];
        JsonObject t2 = (JsonObject) t["GlossDef"];
        JsonArray f = (JsonArray) t2["GlossSeeAlso"];
        Debug.Log(((JsonString) f[0]).value);
        Debug.Log(((JsonString) f[1]).value);

        Debug.Log(((JsonInt) t["TestNumber"]).value);
        Debug.Log(((JsonInt) t["TestNumber2"]).value);
        Debug.Log(((JsonInt) t["TestNumber3"]).value);
        Debug.Log(((JsonFloat) t["TestFloat"]).value);
        Debug.Log(((JsonFloat) t["TestFloat2"]).value);

        StreamWriter writer = new StreamWriter(Path.Combine(new string[] { "SAVES", "JSON_TESTING", "test_output.json" }));
        writer.Write(ast.ToStringJson());
        writer.Flush();
        writer.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
