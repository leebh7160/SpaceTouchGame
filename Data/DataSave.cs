using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSave
{

    internal void Data_Save_PlayerPrefs(float soundvalue, string move, string shoot)
    {
        PlayerPrefs.SetFloat("soundvalue", soundvalue);
        PlayerPrefs.SetString("move", move);
        PlayerPrefs.SetString("shoot", shoot);
        PlayerPrefs.Save();
    }

    internal optionData Data_Load_PlayerPrefs()
    {
        optionData returndata = new optionData();

        returndata.soundValue   = PlayerPrefs.GetFloat("soundvalue");
        returndata.boostKey     = PlayerPrefs.GetString("move");
        returndata.shootKey     = PlayerPrefs.GetString("shoot");

        return returndata;
    }

    internal void Data_DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
