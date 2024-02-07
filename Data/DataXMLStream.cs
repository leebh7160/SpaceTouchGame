using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DataXMLStream : MonoBehaviour
{
    //private string path = "./BrokenSpace_Data/StreamingAssets/Stage.xml";
    private string stagepath = "Assets/Resources/Stage.xml";
    private string optionpath = "Assets/Resources/Option.xml";

    //================================진짜 이렇게 해야되냐
    private bool saveStart = false;

    private float sound_value = 0f;
    private string move_key;
    private string shoot_key;
    //================================진짜 이렇게 해야되냐


    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void LateUpdate()
    {
        if (saveStart == true)
        {
            Debug.Log("세이브 실행" + shoot_key + " : " + move_key);
        }
    }

    #region 스테이지 XML
    //https://wergia.tistory.com/53
    //스테이지 XML 생성
    internal void CreateXML_Stage(List<StageDataList> dataList)
    {
        XmlDocument xmlDoc = new XmlDocument();
        //xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        //루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "StageInfo", string.Empty);
        xmlDoc.AppendChild(root);

        for (int i = 0; i < dataList.Count; i++)
        {
            //자식 노드 생성
            XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Stage" + i, string.Empty);
            root.AppendChild(child);

            //자식 노드에 들어갈 속성 생성
            XmlElement stagename = xmlDoc.CreateElement("StageName");
            stagename.InnerText = dataList[i].Stage.name;
            child.AppendChild(stagename);

            //플레이어 시작 위치
            XmlElement startPosX = xmlDoc.CreateElement("StartPosX");
            startPosX.InnerText = dataList[i].StageStartPos.x.ToString();
            child.AppendChild(startPosX);

            XmlElement startPosY = xmlDoc.CreateElement("StartPosY");
            startPosY.InnerText = dataList[i].StageStartPos.y.ToString();
            child.AppendChild(startPosY);

            //플레이어 부스터
            XmlElement boost = xmlDoc.CreateElement("Boost");
            boost.InnerText = dataList[i].StageBoost.ToString();
            child.AppendChild(boost);

            //플레이어 샷
            XmlElement shoot = xmlDoc.CreateElement("Shoot");
            shoot.InnerText = dataList[i].StageShoot.ToString();
            child.AppendChild(shoot);

            //카메라 제한 위치
            XmlElement camLimitX_Min = xmlDoc.CreateElement("CamLimitX_Min");
            camLimitX_Min.InnerText = dataList[i].CamLimitX.x.ToString();
            child.AppendChild(camLimitX_Min);

            XmlElement camLimitX_Max = xmlDoc.CreateElement("CamLimitX_Max");
            camLimitX_Max.InnerText = dataList[i].CamLimitX.y.ToString();
            child.AppendChild(camLimitX_Max);

            XmlElement camLimitY_Min = xmlDoc.CreateElement("CamLimitY_Min");
            camLimitY_Min.InnerText = dataList[i].CamLimitY.x.ToString();
            child.AppendChild(camLimitY_Min);

            XmlElement camLimitY_Max = xmlDoc.CreateElement("CamLimitY_Max");
            camLimitY_Max.InnerText = dataList[i].CamLimitY.y.ToString();
            child.AppendChild(camLimitY_Max);

            //객체 회전 초기값
            for (int j = 0; j < dataList[i].StageObjectRotate.Count; j++)
            {
                XmlElement objrotate = xmlDoc.CreateElement("ObjRotate");
                objrotate.InnerText = dataList[i].StageObjectRotate[j].ToString();
                child.AppendChild(objrotate);
            }
        }

        xmlDoc.Save(stagepath);
    }

    //스테이지 XML 불러오기
    internal List<StageDataList> LoadXML_Stage()
    {
        List<StageDataList> stageList = new List<StageDataList>();

        TextAsset textAsset = (TextAsset)Resources.Load("Stage");
        Debug.Log(textAsset);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("StageInfo/" + "Stage" + i);

            foreach (XmlNode node in nodes)
            {
                StageDataList tempList = new StageDataList();
                Debug.Log("StageName :: " + node.SelectSingleNode("StageName").InnerText);
                Debug.Log("StartPosX :: " + node.SelectSingleNode("StartPosX").InnerText);
                Debug.Log("StartPosY :: " + node.SelectSingleNode("StartPosY").InnerText);
                Debug.Log("Boost :: " + node.SelectSingleNode("Boost").InnerText);
                Debug.Log("Shoot :: " + node.SelectSingleNode("Shoot").InnerText);
                Debug.Log("CamLimitX_Min :: " + node.SelectSingleNode("CamLimitX_Min").InnerText);
                Debug.Log("CamLimitX_Max :: " + node.SelectSingleNode("CamLimitX_Max").InnerText);
                Debug.Log("CamLimitY_Min :: " + node.SelectSingleNode("CamLimitY_Min").InnerText);
                Debug.Log("CamLimitY_Max :: " + node.SelectSingleNode("CamLimitY_Max").InnerText);
                Debug.Log("ObjRotate :: " + node.SelectNodes("ObjRotate"));

                tempList.StageName = node.SelectSingleNode("StageName").InnerText;
                tempList.StageStartPos.x = float.Parse(node.SelectSingleNode("StartPosX").InnerText);
                tempList.StageStartPos.y = float.Parse(node.SelectSingleNode("StartPosY").InnerText);
                tempList.StageBoost = int.Parse(node.SelectSingleNode("Boost").InnerText);
                tempList.StageShoot = int.Parse(node.SelectSingleNode("Shoot").InnerText);
                tempList.CamLimitX.x = float.Parse(node.SelectSingleNode("CamLimitX_Min").InnerText);
                tempList.CamLimitX.y = float.Parse(node.SelectSingleNode("CamLimitX_Max").InnerText);
                tempList.CamLimitY.x = float.Parse(node.SelectSingleNode("CamLimitY_Min").InnerText);
                tempList.CamLimitY.y = float.Parse(node.SelectSingleNode("CamLimitY_Max").InnerText);

                tempList.StageObjectRotate = new List<float>();
                for (int j = 0; j < node.SelectNodes("ObjRotate").Count; j++)
                {
                    tempList.StageObjectRotate.Add(float.Parse(node.SelectNodes("ObjRotate")[j].InnerText));
                }
                stageList.Add(tempList);
            }
        }
        return stageList;
    }

    //스테이지 XML 저장
    internal void SaveXML_Stage(List<StageDataList> dataList)
    {
        //로드와 같음
        TextAsset textAsset = (TextAsset)Resources.Load("Stage");
        //제작과 같음
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(textAsset.text);

        for(int i = 0; i < dataList.Count; i++)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("StageInfo/" + "Stage" + i);

            foreach(XmlNode node in nodes)
            {
               node.SelectSingleNode("StageName").InnerText = dataList[i].Stage.name;
               node.SelectSingleNode("StartPosX").InnerText = dataList[i].StageStartPos.x.ToString();
               node.SelectSingleNode("StartPosY").InnerText = dataList[i].StageStartPos.y.ToString();
               node.SelectSingleNode("Boost").InnerText = dataList[i].StageBoost.ToString();
               node.SelectSingleNode("Shoot").InnerText = dataList[i].StageShoot.ToString();
               node.SelectSingleNode("CamLimitX_Min").InnerText = dataList[i].CamLimitX.x.ToString();
               node.SelectSingleNode("CamLimitX_Max").InnerText = dataList[i].CamLimitX.y.ToString();
               node.SelectSingleNode("CamLimitY_Min").InnerText = dataList[i].CamLimitY.x.ToString();
               node.SelectSingleNode("CamLimitY_Max").InnerText = dataList[i].CamLimitY.y.ToString();

                for (int objnum = 0; objnum < node.SelectNodes("ObjRotate").Count; objnum++)
                {
                    node.SelectNodes("ObjRotate")[objnum].InnerText = dataList[i].StageObjectRotate[objnum].ToString();
                }
            }
        }

        xmlDoc.Save(stagepath);
    }
    #endregion 스테이지 XML

    #region 스테이지 XML 단순화
    //스테이지 XML 만들기 단순
    internal void CreateXML_Stage_Simple(int stagenum)
    {
        XmlDocument xmlDoc = new XmlDocument();
        //xml을 선언한다(xml의 버전과 인코딩 방식을 정해준다)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        //루트 노드 생성
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "StageInfo", string.Empty);
        xmlDoc.AppendChild(root);

        XmlElement stage = xmlDoc.CreateElement("StageNum");
        stage.InnerText = stagenum.ToString();
        root.AppendChild(stage);

        xmlDoc.Save(stagepath);
    }

    //스테이지 XML 불러오기 단순
    internal int LoadXML_Stage_Simple()
    {
        int stagenum = 0;

        //TextAsset으로 로드
        TextAsset textAsset = (TextAsset)Resources.Load("Stage");
        Debug.Log(textAsset);

        //xml로드 사용
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        //xml로드로 불러온 노드 리스트로 선택
        XmlNodeList nodes = xmlDoc.SelectNodes("StageInfo/StageNum");

        stagenum = int.Parse(nodes[0].SelectSingleNode("StageNum").InnerText);

        return stagenum;
    }

    //스테이지 XML 저장하기 단순
    internal void SaveXML_Stage_Simple(int stagenum)
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Stage");
        XmlDocument xmlDoc = new XmlDocument();

        xmlDoc.LoadXml(textAsset.text);

        XmlNodeList nodes = xmlDoc.SelectNodes("StageInfo/StageNum");

        nodes[0].SelectSingleNode("StageNum").InnerText = stagenum.ToString();

        xmlDoc.Save(stagepath);
    }
    #endregion 스테이지 XML 단순화

    #region 옵션 XML 단순화
    //옵션 저장
    internal void CreateXML_Option_Simple(float soundValue, string Move, string Shoot)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "OptionInfo", string.Empty);
        xmlDoc.AppendChild(root);

        //===============================
        XmlElement soundvalue = xmlDoc.CreateElement("SoundValue");
        soundvalue.InnerText = soundValue.ToString();
        root.AppendChild(soundvalue);

        XmlElement move = xmlDoc.CreateElement("Move");
        move.InnerText = Move.ToString();
        root.AppendChild(move);

        XmlElement shoot = xmlDoc.CreateElement("Shoot");
        shoot.InnerText = Shoot.ToString();
        root.AppendChild(shoot);

        xmlDoc.Save(optionpath);
    }

    internal optionData LoadXML_Option_Simple()
    {
        optionData returnData = new optionData();

        //TextAsset으로 로드
        TextAsset textAsset = (TextAsset)Resources.Load("Option");
        Debug.Log(textAsset);

        //xml로드 사용
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        //xml로드로 불러온 노드 리스트로 선택
        XmlNodeList nodes = xmlDoc.SelectNodes("OptionInfo");

        returnData.soundValue   = float.Parse(nodes[0].SelectSingleNode("SoundValue").InnerText);
        returnData.boostKey     = nodes[0].SelectSingleNode("Move").InnerText;
        returnData.shootKey     = nodes[0].SelectSingleNode("Shoot").InnerText;

        return returnData;
    }

    private void SaveXML_Option_Simple()
    {
        TextAsset textAsset = (TextAsset)Resources.Load("Option");
        XmlDocument xmlDoc = new XmlDocument();

        XmlNodeList nodes = xmlDoc.SelectNodes("OptionInfo");
        xmlDoc.LoadXml(textAsset.text);

        //===============================================
        XmlReader xmlReader;

        XmlReaderSettings xmlReaderSetting = new XmlReaderSettings();
        xmlReaderSetting.IgnoreComments = true;
        xmlReaderSetting.IgnoreWhitespace = true;

        try
        {
            xmlReader = XmlReader.Create(optionpath, xmlReaderSetting);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return;
        }

        while(xmlReader.Read())
        {
            if(xmlReader.NodeType == XmlNodeType.Element)
            {
                nodes[0].SelectSingleNode("SoundValue").InnerText = sound_value.ToString();
                nodes[0].SelectSingleNode("Move").InnerText = move_key.ToString();
                nodes[0].SelectSingleNode("Shoot").InnerText = shoot_key.ToString();
            }
        }
        //===============================================



        /*nodes[0].SelectSingleNode("SoundValue").InnerText = sound_value.ToString();
        nodes[0].SelectSingleNode("Move").InnerText = move_key.ToString();
        nodes[0].SelectSingleNode("Shoot").InnerText = shoot_key.ToString();*/

        xmlReader.Close();
        xmlDoc.Serialize(true);
        xmlDoc.Save(optionpath);
    }

    internal void SaveXML_Start(bool start)
    {
        saveStart = start;
    }

    internal void SaveXML_Option_Save(float soundValue, string Move, string Shoot)
    {
        sound_value = soundValue;
        move_key = Move;
        shoot_key = Shoot;
        CreateXML_Option_Simple(soundValue, Move, Shoot);
    }

    #endregion 옵션 XML 단순화
}
