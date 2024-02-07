using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KeySetting { public static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>(); }

public class KeyClass
{
    private List<string> defaultKeyName = new List<string>();
    private List<KeyCode> defaultKeyCode = new List<KeyCode>();


    public KeyClass()
    {
    }

    //Ű ���� �⺻�� ����
    internal void Key_Setting_Default(List<string> keynamelist, List<KeyCode> keycodelist)
    {
        if (keynamelist != null && keycodelist != null)
            Key_List_Setting_Default(keynamelist, keycodelist);
    }

    //������ Ű ���ڷ� ��������
    internal List<string> Key_GetKey()
    {
        List<string> returnlist = new List<string>();
        if (KeySetting.keys != null)
            returnlist.AddRange(KeySetting.keys.Keys);
        else return null;
        return returnlist;
    }

    //������ Ű KeyCode�� ��������
    internal List<KeyCode> Key_GetCode()
    {
        List<KeyCode> returnlist = new List<KeyCode>();
        if (KeySetting.keys != null)
            returnlist.AddRange(KeySetting.keys.Values);
        else return null;
        return returnlist;
    }

    //Ű ����Ʈ ���� �⺻�� ����
    private void Key_List_Setting_Default(List<string> keynamelist, List<KeyCode> keycodelist)
    {
        defaultKeyName = keynamelist;
        defaultKeyCode = keycodelist;

        KeySetting.keys.Clear();
        for (int i = 0; i < keynamelist.Count; i++)
        {
            KeySetting.keys.Add(keynamelist[i], keycodelist[i]);
        }
    }


    //Ű ����Ʈ �����ϱ�
    internal bool Key_List_Setting_Keys(string keyname, KeyCode keycode)
    {
        if (KeySetting.keys.ContainsValue(keycode) == false)
        {
            KeySetting.keys[keyname] = keycode;
            return true;
        }
        else
         {
            string tempname;
            tempname = KeySetting.keys.FirstOrDefault(x => x.Value == keycode).Key;
            KeySetting.keys[tempname] = KeyCode.None;
            KeySetting.keys[keyname] = keycode;
        }

        return false;
    }
}

