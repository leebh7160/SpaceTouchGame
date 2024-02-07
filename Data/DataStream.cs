
using Mono.Cecil;
using System.IO;
using UnityEngine;

struct optionData
{
    public float soundValue;
    public string boostKey;
    public string shootKey;
}

struct stageData
{
    public int stageValue;
}



public class DataStream
{

    //이미 있는 것은 Resource
    //새로 만든 것은 Stream으로 해야한다
    internal void optionDataSave(float soundValue, string boost, string key)
    {
        //FileStream data = new FileStream("Assets/Resources/Option.txt", FileMode.Append, FileAccess.Write);
        //디버그 플레이 시
        StreamWriter writer = File.CreateText("Assets/Resources/Option.txt");
        writer.Write("SoundValue=" + soundValue + "\r\nBoostKey=" + boost + "\r\nShootKey=" + key);
        writer.Close();

        /*//빌드 시
        StreamWriter Stand_writer = File.CreateText(System.IO.Directory.GetCurrentDirectory() + @"/BrokenSpace_Data/StreamingAssets/Option.txt");
        Stand_writer.Write("SoundValue=" + soundValue + "\r\nBoostKey=" + boost + "\r\nShootKey=" + key);
        Stand_writer.Close();*/
    }

    internal void stageDataSave(int stage)
    {
        StreamWriter writer = File.CreateText("Assets/Resources/Stage.txt");
        writer.Write("StageNum=" + stage);
        writer.Close();
        
        /* 빌드 시
        StreamWriter Stand_writer = File.CreateText(System.IO.Directory.GetCurrentDirectory() + @"/BrokenSpace_Data/StreamingAssets/Stage.txt");
        Stand_writer.Write("StageNum=" + stage);
        Stand_writer.Close();
        */
    }

    internal optionData optionDataLoad()
    {
        optionData returnData = new optionData();

        TextAsset data = Resources.Load("Option") as TextAsset;
        string[] lines = data.text.Split('=', '\r', '\n', ' '); // 1 = sound,  4 = boost,  7 = shoot

        returnData.soundValue = float.Parse(lines[1]);
        returnData.boostKey = lines[4];
        returnData.shootKey = lines[7];
        return returnData;
    }

    internal stageData stageDataLoad()
    {
        stageData returnData = new stageData();

        TextAsset data = Resources.Load("Stage") as TextAsset;
        string[] lines = data.text.Split('=', '\r', '\n', ' '); // 1 = stage

        returnData.stageValue = int.Parse(lines[1]);
        return returnData;
    }
}
