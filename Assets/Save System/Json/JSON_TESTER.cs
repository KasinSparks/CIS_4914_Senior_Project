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
        JsonObject val = (JsonObject) ((JsonObject)ast.value).value["glossary"];

        Debug.Log(((JsonLiteral<string>) (val.value["title"])).value);
        JsonObject t = (JsonObject) val.value["GlossDiv"];
        t = (JsonObject) t.value["GlossList"];
        t = (JsonObject) t.value["GlossEntry"];
        JsonObject t2 = (JsonObject) t.value["GlossDef"];
        JsonArray f = (JsonArray) t2.value["GlossSeeAlso"];
        Debug.Log(((JsonLiteral<string>) f.value[0]).value);
        Debug.Log(((JsonLiteral<string>) f.value[1]).value);

        Debug.Log(((JsonLiteral<int>) t.value["TestNumber"]).value);
        Debug.Log(((JsonLiteral<int>) t.value["TestNumber2"]).value);
        Debug.Log(((JsonLiteral<int>) t.value["TestNumber3"]).value);
        Debug.Log(((JsonLiteral<float>) t.value["TestFloat"]).value);
        Debug.Log(((JsonLiteral<float>) t.value["TestFloat2"]).value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
