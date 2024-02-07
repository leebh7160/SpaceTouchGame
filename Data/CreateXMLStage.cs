using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CreateXMLStage
{
    private string path = "./Assets/Resources/Stage.xml";

    public CreateXMLStage()
    { }

    //https://wergia.tistory.com/53
    internal void CreateXML(List<StageDataList> dataList)
    {
        XmlDocument xmlDoc = new XmlDocument();
        //xml�� �����Ѵ�(xml�� ������ ���ڵ� ����� �����ش�)
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes"));

        //��Ʈ ��� ����
        XmlNode root = xmlDoc.CreateNode(XmlNodeType.Element, "StageInfo", string.Empty);
        xmlDoc.AppendChild(root);

        for (int i = 0; i < dataList.Count; i++)
        {
            //�ڽ� ��� ����
            XmlNode child = xmlDoc.CreateNode(XmlNodeType.Element, "Stage" + i, string.Empty);
            root.AppendChild(child);

            //�ڽ� ��忡 �� �Ӽ� ����
            XmlElement stage = xmlDoc.CreateElement("Stage");
            stage.InnerText = dataList[i].Stage.name;
            child.AppendChild(stage);

            //�÷��̾� ���� ��ġ
            XmlElement startPosX = xmlDoc.CreateElement("StartPosX");
            startPosX.InnerText = dataList[i].StageStartPos.x.ToString();
            child.AppendChild(startPosX);

            XmlElement startPosY = xmlDoc.CreateElement("StartPosY");
            startPosY.InnerText = dataList[i].StageStartPos.y.ToString();
            child.AppendChild(startPosY);

            //�÷��̾� �ν���
            XmlElement boost = xmlDoc.CreateElement("Boost");
            boost.InnerText = dataList[i].StageBoost.ToString();
            child.AppendChild(boost);

            //�÷��̾� ��
            XmlElement shoot = xmlDoc.CreateElement("Shoot");
            shoot.InnerText = dataList[i].StageShoot.ToString();
            child.AppendChild(shoot);

            //ī�޶� ���� ��ġ
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

            //��ü ȸ�� �ʱⰪ
            for (int j = 0; j < dataList[i].StageObjectRotate.Count; j++)
            {
                XmlElement objrotate = xmlDoc.CreateElement("ObjRotate");
                objrotate.InnerText = dataList[i].StageObjectRotate[j].ToString();
                child.AppendChild(objrotate);
            }
        }

        xmlDoc.Save(path);
    }

    internal List<StageDataList> LoadXML()
    {
        List<StageDataList> stageList = new List<StageDataList>();

        TextAsset textAsset = (TextAsset)Resources.Load(path);
        Debug.Log(textAsset);

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textAsset.text);

        for (int i = 0; i < xmlDoc.ChildNodes.Count; i++)
        {
            XmlNodeList nodes = xmlDoc.SelectNodes("StageInfo/" + "Stage" + i);

            foreach (XmlNode node in nodes)
            {
                StageDataList tempList = new StageDataList();
                Debug.Log("Stage :: " + node.SelectSingleNode("Stage").InnerText);
                Debug.Log("StartPosX :: " + node.SelectSingleNode("StartPosX").InnerText);
                Debug.Log("StartPosY :: " + node.SelectSingleNode("StartPosY").InnerText);
                Debug.Log("Boost :: " + node.SelectSingleNode("Boost").InnerText);
                Debug.Log("Shoot :: " + node.SelectSingleNode("Shoot").InnerText);
                Debug.Log("CamLimitX :: " + node.SelectSingleNode("CamLimitX").InnerText);
                Debug.Log("CamLimitY :: " + node.SelectSingleNode("CamLimitY").InnerText);
                Debug.Log("ObjRotate :: " + node.SelectNodes("ObjRotate"));

                tempList.Stage.name = node.SelectSingleNode("Stage").InnerText;
                tempList.StageStartPos.x = float.Parse(node.SelectSingleNode("StartPosX").InnerText);
                tempList.StageStartPos.y = float.Parse(node.SelectSingleNode("StartPosY").InnerText);
                tempList.StageBoost = int.Parse(node.SelectSingleNode("Boost").InnerText);
                tempList.StageShoot = int.Parse(node.SelectSingleNode("Shoot").InnerText);
                tempList.CamLimitX.x = int.Parse(node.SelectSingleNode("CamLimitX_Min").InnerText);
                tempList.CamLimitX.y = int.Parse(node.SelectSingleNode("CamLimitX_Max").InnerText);
                tempList.CamLimitY.x = int.Parse(node.SelectSingleNode("CamLimitY_Min").InnerText);
                tempList.CamLimitY.y = int.Parse(node.SelectSingleNode("CamLimitY_Max").InnerText);

                for (int j = 0; i < node.SelectNodes("ObjRotate").Count; j++)
                {
                    tempList.StageObjectRotate.Add(float.Parse(node.SelectNodes("ObjRotate")[j].ToString()));
                }
                stageList.Add(tempList);
            }
        }
        return stageList;
    }

}
