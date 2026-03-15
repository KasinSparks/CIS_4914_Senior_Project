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
        //this.TestWordInfoSave();
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
        WordInfo info = SaveSystemTable.Get<WordInfo>(System.Guid.Parse("b4c9e3da-aa17-49ed-a6ec-6d6a51d070d9"));
        Debug.Log(info.GetInfo());
        Debug.Log(info.GetWords());
        HighlightedWords hw = GameObject.Find("-----UI-----/UI_Book/Panel/Info").GetComponent<HighlightedWords>();
        hw.GetDict().Add(info.GetWords()[0].ToUpper(), info);
        hw.GetDict().Add(info.GetWords()[1].ToUpper(), info);
        WordInfo[] temp = hw.words;
        hw.words = new WordInfo[2];
        hw.words[0] = info;
        hw.words[1] = temp[0];
        hw.TEST();
    }
}