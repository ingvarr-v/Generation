using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsRecognition : MonoBehaviour
{
    public int Icoord, Jcoord;
    public string LeftM, UpM, DownM, RightM;
    public string LeftC, UpC, DownC, RightC;

    //void Awake()
    //{
    //    LevelGenerator1 LvlGen = GameObject.Find("manager").GetComponent<LevelGenerator1>();

    //    Icoord = int.Parse(this.gameObject.name.Split('_')[2]);
    //    Jcoord = int.Parse(this.gameObject.name.Split('_')[3]);

    //    if (Jcoord > 1)
    //    {
    //        LeftM = LvlGen.MapArray[Icoord, Jcoord - 1].Split('_')[0];
    //    }
    //    if (Icoord < LvlGen.MapSizeZ)
    //    {
    //        UpM = LvlGen.MapArray[Icoord + 1, Jcoord].Split('_')[0];
    //    }
    //    if (Icoord > 1)
    //    {
    //        DownM = LvlGen.MapArray[Icoord - 1, Jcoord].Split('_')[0];
    //    }
    //    if (Jcoord < LvlGen.MapSizeX)
    //    {
    //        RightM = LvlGen.MapArray[Icoord, Jcoord + 1].Split('_')[0];
    //    }
    //}

    //void Update()
    //{
         
    //}
}
