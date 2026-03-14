using System.Collections;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class JsonParseTest
{
    private string TEST_FOLDER = Path.Combine("Assets", "Save System", "Json", "JsonTests");

    // A Test behaves as an ordinary method
    [Test]
    public void JsonParseTestSimplePasses()
    {
        // Use the Assert class to test conditions
        JsonParser parser = new JsonParser();
        JsonAST ast = parser.Parse(Path.Combine(TEST_FOLDER, "test.json"));

        JsonObject val = (JsonObject) ((JsonObject)ast.value)["glossary"];



        Assert.AreEqual(((JsonString) (val["title"])).value, "example glossary");
        JsonObject t = (JsonObject) val["GlossDiv"];
        t = (JsonObject) t["GlossList"];
        t = (JsonObject) t["GlossEntry"];
        JsonObject t2 = (JsonObject) t["GlossDef"];
        JsonArray f = (JsonArray) t2["GlossSeeAlso"];
        Assert.AreEqual(((JsonString) f[0]).value, "GML");
        Assert.AreEqual(((JsonString) f[1]).value, "XML");

        Assert.AreEqual(((JsonInt) t["TestNumber"]).value, 420);
        Assert.AreEqual(((JsonInt) t["TestNumber2"]).value, 69);
        Assert.AreEqual(((JsonInt) t["TestNumber3"]).value, -67);
        Assert.AreEqual(((JsonFloat) t["TestFloat"]).value, 3.14f);
        Assert.AreEqual(((JsonFloat) t["TestFloat2"]).value, 0.21f);
    }
/*
    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator JsonParseTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
*/
}
