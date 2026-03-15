using System.IO;
using UnityEngine;
using UnityEngine.Assertions;

public class SaveSystemTest : MonoBehaviour
{
    public WordInfo word_info;

    void Start()
    {
        this.RunTests();
    }

    void RunTests()
    {
        this.TestWordInfoSave();
        this.TestLoadFromTable();
        //this.TestAddToSaveTable();
    }

    void TestWordInfoSave()
    {
        SaveSystemTable._TESTING_SaveToJsonFile(Path.Combine("SAVES", "TESTING", "word_test.json"), word_info);
    }

    void TestAddToSaveTable()
    {
        System.Guid guid1 = SaveSystemTable.Add(word_info, word_info.GetInstanceID());
        System.Guid guid2 = SaveSystemTable.Add(word_info, word_info.GetInstanceID());

        Assert.AreEqual(guid1, guid2);

        SaveSystemTable.WriteTableToDisk();
    }


    void TestLoadFromTable()
    {
        SaveSystemTable.ReadTableFromDisk();
        WordInfo info = SaveSystemTable.Get<WordInfo>(System.Guid.Parse("d484fed4-b2a3-482e-8a5f-c1d9735d4711"));
        Debug.Log(info.GetInfo());
        Debug.Log(info.GetWords());
        HighlightedWords hw = GameObject.Find("-----UI-----/UI_Book/Panel/Info").GetComponent<HighlightedWords>();
        hw.GetDict().Add(info.GetWords()[0].ToUpper(), info);
        hw.GetDict().Add(info.GetWords()[1].ToUpper(), info);
        hw.TEST(info);
    }
}