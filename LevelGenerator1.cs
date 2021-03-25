using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator1 : MonoBehaviour
{
    public string[,] MapArray = new string[21, 21];

    public int MapSizeX=20;
    public int MapSizeZ=20;

    private int z = 0;
    private int x = 0;

    private int ModuleJ, ModuleI, ModuleX, ModuleZ;

    public List<GameObject> AllModules;
    public List<GameObject> BuildAllModules;

    private int NumBuild;
    private int Num;

    private int DM2Count = 0, GM1Count = 0, SM1Count = 0, SM2Count = 0, EM1Count = 0, CM1Count = 0, RM1Count = 0, SM3Count=0;

    private GameObject ProcessingModule;

    private List<string> ModulesAround;

    private bool SolidWall = true;

    private int CurrentI, CurrentJ;


    void Awake()
    {
        Num = 0;
        for (int ii = 0; ii < BuildAllModules.Count; ii++)
        {            AllModules.Add(null);        }        

        StartMapBuild();
        FirstModuleBuild();        
        ModulesBuild();

        //for (int i = 1; i <= MapSizeZ; i++)
        //{
        //    for (int j = 1; j <= MapSizeX; j++)
        //    {
        //        Debug.Log(MapArray[i, j]);
        //    }
        //}

    }


    public void FirstModuleBuild()
    {
        ModuleJ = Random.Range(0,0);

        ModuleJ = 1;
        ModuleI = 1;

        ModuleX = ModuleJ * 3;
        ModuleZ = ModuleI * 3;
        int FirstModule = Random.Range(0,0);
        NumBuild = FirstModule;

        Instantiate(BuildAllModules[FirstModule], new Vector3(1,0,1), Quaternion.identity);
        ProcessingModule = GameObject.Find(BuildAllModules[FirstModule].gameObject.name + "(Clone)");
        ModulesReferance();
        AllModules[Num] = ProcessingModule;

        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;
        ModCells.ModuleI = ModuleI;
        ModCells.ModuleJ = ModuleJ;


        AllModules[Num].transform.Rotate(0, 0, 0, Space.World);

        if (AllModules[Num].transform.localEulerAngles.y == 0)
        {
            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
        }
        else if (AllModules[Num].transform.localEulerAngles.y == 90)
        {
            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
            ModulesFlip();
            ModulesFlip();
            ModulesFlip();
        }
        else if (AllModules[Num].transform.localEulerAngles.y == 180)
        {
            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
            ModulesFlip();
            ModulesFlip();
        }
        else if (AllModules[Num].transform.localEulerAngles.y == 270)
        {
            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
            ModulesFlip();
        }

        BuildAllModules.RemoveAt(FirstModule);

        NamingCellsFirst();
        CellsToMapArray();
        DeadCellsToMapArrayFirst();

        //Num++;
    }

    public void ModulesBuild()
    {

        for (int i = 1; i <= MapSizeZ; i++)    //z
        {
            CurrentI = i;

            bool ModuleExistInJ = false;
            int ModuleExistJ = 0;

            for (int j = 1; j <= MapSizeX; j++)    //x
            {                

                CurrentJ = j;

                if (MapArray[i, j] == "0")
                {
                    for (int j1 = j; j1 <= MapSizeX; j1++)
                    {
                        if (MapArray[i, j1] != "0" && MapArray[i, j1] != "DeadCell")
                        {
                            ModuleExistInJ = true;
                            ModuleExistJ = j1;

                            break;
                        }
                    }                    
                }

                if (ModuleExistInJ == false)
                {

                    if (MapArray[i, j] == "0" && MapArray[i, j].Contains("DeadCell") == false)
                    {

                        if (BuildAllModules[0].GetComponent<ModuleCells>().Jsize - 1 + j >= MapSizeX) { continue; }

                        else
                        {


                            CheckIfSolidWall();
                            if (SolidWall == true) { continue; }

                            Num++;


                            Instantiate(BuildAllModules[0], new Vector3(j * 3, 0, i * 3), Quaternion.identity);
                            ProcessingModule = GameObject.Find(BuildAllModules[0].gameObject.name + "(Clone)");
                            ModulesReferance();
                            AllModules[Num] = ProcessingModule;

                            ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

                            ModuleJ = j;
                            ModuleI = i;
                            ModCells.ModuleI = ModuleI;
                            ModCells.ModuleJ = ModuleJ;

                            int zsize = ModCells.Isize;
                            int xsize = ModCells.Jsize;

                            BuildAllModules.RemoveAt(0);        //по порядку

                            NamingCells();
                            CellsToMapArray();
                            CellsRecognition();
                            ModuleSubstitutionI1();

                            //if (AllModules[Num].name == "DM2.2")

                            if (ModulesAround.Count == 1)
                            {
                                ModuleEntrancesCheck_OneAround();
                            }
                            else if (ModulesAround.Count >= 2)
                            {
                                ModuleEntrancesCheck_TwoAround();
                            }

                            if (BuildAllModules.Count <= 1) { break; }
                        }
                    }
                } 

                if (ModuleExistInJ == true)
                {

                    if (MapArray[i, j] == "0" || MapArray[i, j].Contains("DeadCell") == true) { continue; }
                    if (MapArray[i, j] != "0" && MapArray[i, j].Contains("DeadCell") == false)
                    {
                        

                        bool CantBuild = false;

                        for (int p = 0; p < BuildAllModules[0].GetComponent<ModuleCells>().Jsize; p++)
                        {
                            if (MapArray[i, j - BuildAllModules[0].GetComponent<ModuleCells>().Jsize + p] != "0" && MapArray[i, j - BuildAllModules[0].GetComponent<ModuleCells>().Jsize - 1] != "0")
                            {
                                CantBuild = true;

                            }
                        }

                        if (BuildAllModules[0].GetComponent<ModuleCells>().Jsize + j >= MapSizeX) 
                        {
                            continue;
                        }

                        if (j - BuildAllModules[0].GetComponent<ModuleCells>().Jsize < 1) 
                        {
                            continue;
                        }

                        else if (CantBuild == true)
                        {
                            continue;
                        }

                        else if (MapArray[i, j - BuildAllModules[0].GetComponent<ModuleCells>().Jsize] == "0")
                        {

                            CheckIfSolidWallLeft();
                            if (SolidWall == true) { continue; }

                            Num++;

                            Debug.Log(BuildAllModules[0]);
                            Debug.Log(j);
                            

                            Instantiate(BuildAllModules[0], new Vector3((j - BuildAllModules[0].GetComponent<ModuleCells>().Jsize) * 3, 0, i * 3), Quaternion.identity);
                            ProcessingModule = GameObject.Find(BuildAllModules[0].gameObject.name + "(Clone)");
                            ModulesReferance();
                            AllModules[Num] = ProcessingModule;

                            ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

                            ModuleJ = j - ModCells.Jsize;
                            ModuleI = i;
                            ModCells.ModuleI = ModuleI;
                            ModCells.ModuleJ = ModuleJ;

                            int zsize = ModCells.Isize;
                            int xsize = ModCells.Jsize;

                            BuildAllModules.RemoveAt(0);        //по порядку

                            NamingCells();
                            CellsToMapArray();
                            CellsRecognition();
                            ModuleSubstitutionI1();

                            if (ModulesAround.Count == 1)
                            {
                                //if (AllModules[Num].name.Contains("SM3") == false)
                                //{
                                    ModuleEntrancesCheck_OneAround();
                                //}
                            }
                            else if (ModulesAround.Count >= 2)
                            {
                                ModuleEntrancesCheck_TwoAround();
                            }

                            if (BuildAllModules.Count <= 1) { break; }

                            if (j - ModCells.Jsize - BuildAllModules[0].GetComponent<ModuleCells>().Jsize < 1)
                            {
                                DeadCellsToMapArrayLeft(AllModules[Num]);
                            }
                            else
                            {
                                j = 1;
                                ModuleExistInJ = false;
                            }

                        }


                    }


                }

                ModuleExistInJ = false;

                x = x + 3;

                if (BuildAllModules.Count <= 1) { break; }

            }
            z = z + 3;
            x = 0;

            if (BuildAllModules.Count <= 1) { break; }
        } 
    }

    public void StartMapBuild()
    {
        for (int i = 1; i <= MapSizeZ; i++)
        {
            for (int j = 1; j <= MapSizeX; j++)
            {
                MapArray[i, j] = "0";
            }
        }
    }

    public void NamingCells()
    {
        int m = 0;
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;

        //Debug.Log(AllModules[Num]);
        //Debug.Log(ModuleI);


        //Debug.Log(AllModules[Num]);

        for (int i = 1; i <= Isize; i++)    //z
        {
            for (int j = 1; j <= Jsize; j++)    //x
            {
               
                if (ModCells.Cells[m].gameObject.name.Contains("NullCell") == true)
                {                    
                    ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "NullCell" + "_" + (i+ModuleI-1).ToString() + "_" + (j + ModuleJ-1).ToString();
                }
                else
                {
                    if (ModCells.Cells[m].gameObject.name.Contains("Entrance") == true)
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Entrance" + "_" + (i + ModuleI - 1).ToString() + "_" + (j + ModuleJ-1).ToString();
                    }
                    else
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Floor" + "_" + (i + ModuleI - 1).ToString() + "_" + (j + ModuleJ-1).ToString();
                    }
                }
                
                m++;
            }
        }
    }

    public void NamingCellsIfLeft()
    {
        int m = 0;
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;

        //Debug.Log(AllModules[Num]);
        //Debug.Log(ModuleI);


        //Debug.Log(AllModules[Num]);

        for (int i = 1; i <= Isize; i++)    //z
        {
            for (int j = 1; j <= Jsize; j++)    //x
            {

                if (ModCells.Cells[m].gameObject.name.Contains("NullCell") == true)
                {
                    ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "NullCell" + "_" + (i + ModuleI - 1).ToString() + "_" + (j + ModuleJ - 1 - Jsize).ToString();
                }
                else
                {
                    if (ModCells.Cells[m].gameObject.name.Contains("Entrance") == true)
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Entrance" + "_" + (i + ModuleI - 1).ToString() + "_" + (j + ModuleJ - 1 - Jsize).ToString();
                    }
                    else
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Floor" + "_" + (i + ModuleI - 1).ToString() + "_" + (j + ModuleJ - 1 - Jsize).ToString();
                    }
                }

                m++;
            }
        }
    }

    public void NamingCellsFirst()
    {
        int m = 0;
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;

        //Debug.Log(AllModules[Num]);

        for (int i = 1; i <= Isize; i++)    //z
        {
            for (int j = 1; j <= Jsize; j++)    //x
            {

                if (ModCells.Cells[m].gameObject.name.Contains("NullCell") == true)
                {
                    ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "NullCell" + "_" + (i + ModuleI-1).ToString() + "_" + (j + ModuleJ-1).ToString();
                }
                else
                {
                    if (ModCells.Cells[m].gameObject.name.Contains("Entrance") == true)
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Entrance" + "_" + (i + ModuleI-1).ToString() + "_" + (j + ModuleJ-1).ToString();
                    }
                    else
                    {
                        ModCells.Cells[m].gameObject.name = AllModules[Num].gameObject.name + "_" + "Floor" + "_" + (i + ModuleI-1).ToString() + "_" + (j + ModuleJ-1).ToString();
                    }
                }

                m++;
            }
        }
    }

    public void CellsToMapArray()
    {
        int m = 0;
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

        for (int i = 1; i <= MapSizeZ; i++)    //z
        {
            for (int j = 1; j <= MapSizeX; j++)    //x
            {
                if (m < ModCells.Cells.Count)
                {
                    if ((i == int.Parse(ModCells.Cells[m].gameObject.name.Split('_')[2])) && (j == int.Parse(ModCells.Cells[m].gameObject.name.Split('_')[3])))
                    {
                        MapArray[i, j] = AllModules[Num].gameObject.name + "_" + ModCells.Cells[m].gameObject.name.Split('_')[1] + "_" + ModCells.Cells[m].gameObject.name.Split('_')[2] + "_" + ModCells.Cells[m].gameObject.name.Split('_')[3];
                        m++;
                        //Debug.Log(MapArray[i, j]);
                    }
                }


            }        

        }
    }

    public void DeadCellsToMapArrayFirst()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

        for (int i = 1; i <= MapSizeZ; i++)    //z
        {
            for (int j = 1; j <= MapSizeX; j++)    //x
            {
                if (i < ModCells.ModuleI && j < ModCells.ModuleJ + ModCells.Jsize)  
                {
                    MapArray[i, j] = "DeadCell" + "_" + i.ToString() + "_" + j.ToString();
                }
                if (i < ModCells.ModuleI + ModCells.Isize && j < ModCells.ModuleJ) 
                {
                    MapArray[i, j] = "DeadCell" + "_" + i.ToString() + "_" + j.ToString();
                }
            }
        }
    }

    public void DeadCellsToMapArrayLeft(GameObject Module)
    {
        ModuleCells ModCells = Module.GetComponent<ModuleCells>();

        for (int i = 1; i <= MapSizeZ; i++)    //z
        {
            for (int j = 1; j <= MapSizeX; j++)    //x
            {
                if (i < ModCells.ModuleI + ModCells.Isize && i >= ModCells.ModuleI && j < ModCells.ModuleJ)  
                {
                    MapArray[i, j] = "DeadCell" + "_" + i.ToString() + "_" + j.ToString();
                }
            }
        }
    }

    public void ModulesFlip()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;
        int m1 = Isize-1;
        int m = 0;
        List<GameObject> CellsFlip = new List<GameObject>();
        for (int l = 0; l < ModCells.Cells.Count; l++)
        {
            CellsFlip.Add(null);
        }

        int offset = (ModCells.Cells.Count-1)-(Jsize-2);

        for (int i = 1; i <= Isize; i++)
        {
            for (int j = 1; j <= Jsize; j++)
            {
                //Debug.Log(m);
                //Debug.Log(m1);
                CellsFlip[m1] = ModCells.Cells[m];

                CellsRecognition CellsRec = CellsFlip[m1].GetComponent<CellsRecognition>();

                //string CellsRecChangeLeft = CellsRec.LeftC;
                //string CellsRecChangeUp = CellsRec.UpC;
                //string CellsRecChangeRight = CellsRec.RightC;
                //string CellsRecChangeDown = CellsRec.DownC;
                if (CellsRec.LeftC == "WallAround")
                {
                    CellsRec.DownC = "WallAround";
                    CellsRec.LeftC = "";
                }
                if (CellsRec.UpC == "WallAround")
                {
                    CellsRec.LeftC = "WallAround";
                    CellsRec.UpC = "";
                }
                if (CellsRec.RightC == "WallAround")
                {
                    CellsRec.UpC = "WallAround";
                    CellsRec.RightC = "";
                }
                if (CellsRec.DownC == "WallAround")
                {
                    CellsRec.RightC = "WallAround";
                    CellsRec.DownC = "";
                }


                if (j < Jsize)
                {
                    m1 = m1 + Jsize;
                }              

                m++;
            }
            m1 = m1 - offset;
        }

        ModCells.Cells = CellsFlip;

        int flipInts = Isize;
        Isize = Jsize;
        Jsize = flipInts;
    }

    public void ModulesReferance()
    {
        if (ProcessingModule.name.Contains("DM2"))
        {
            DM2Count++;
            ProcessingModule.name = "DM2" + "." + DM2Count;
        }
        if (ProcessingModule.name.Contains("GM1"))
        {
            GM1Count++;
            ProcessingModule.name = "GM1" + "." + GM1Count;
        }
        if (ProcessingModule.name.Contains("EM1"))
        {
            EM1Count++;
            ProcessingModule.name = "EM1" + "." + EM1Count;
        }
        if (ProcessingModule.name.Contains("SM1"))
        {
            SM1Count++;
            ProcessingModule.name = "SM1" + "." + SM1Count;
        }
        if (ProcessingModule.name.Contains("SM2"))
        {
            SM2Count++;
            ProcessingModule.name = "SM2" + "." + SM2Count;
        }
        if (ProcessingModule.name.Contains("CM1"))
        {
            CM1Count++;
            ProcessingModule.name = "СM1" + "." + CM1Count;
        }
        if (ProcessingModule.name.Contains("RM1"))
        {
            RM1Count++;
            ProcessingModule.name = "RM1" + "." + RM1Count;
        }
        if (ProcessingModule.name.Contains("SM3"))
        {
            SM3Count++;
            ProcessingModule.name = "SM3" + "." + SM3Count;
        }
    }

    public void CellsRecognition()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

        for (int i = 0; i <= ModCells.Cells.Count-1; i++)
        {
            CellsRecognition CellsRec = ModCells.Cells[i].GetComponent<CellsRecognition>();

            //Debug.Log(ModCells.Cells[i].name);

            CellsRec.Icoord = int.Parse(ModCells.Cells[i].name.Split('_')[2]);
            CellsRec.Jcoord = int.Parse(ModCells.Cells[i].name.Split('_')[3]);

            if (CellsRec.Jcoord > 1)
            {
                if (MapArray[CellsRec.Icoord, CellsRec.Jcoord - 1].Contains("DeadCell") == false && CellsRec.LeftC != "WallAround") 
                {
                    CellsRec.LeftC = MapArray[CellsRec.Icoord, CellsRec.Jcoord - 1];
                    CellsRec.LeftM = MapArray[CellsRec.Icoord, CellsRec.Jcoord - 1].Split('_')[0];
                }
            }
            if (CellsRec.Icoord < MapSizeZ)
            {
                if (MapArray[CellsRec.Icoord + 1, CellsRec.Jcoord].Contains("DeadCell") == false && CellsRec.UpC != "WallAround") 
                {
                    CellsRec.UpC = MapArray[CellsRec.Icoord + 1, CellsRec.Jcoord];
                    CellsRec.UpM = MapArray[CellsRec.Icoord + 1, CellsRec.Jcoord].Split('_')[0];
                }
            }
            if (CellsRec.Icoord > 1)
            {
                if (MapArray[CellsRec.Icoord - 1, CellsRec.Jcoord].Contains("DeadCell") == false && CellsRec.DownC != "WallAround") 
                {
                    CellsRec.DownC = MapArray[CellsRec.Icoord - 1, CellsRec.Jcoord];
                    CellsRec.DownM = MapArray[CellsRec.Icoord - 1, CellsRec.Jcoord].Split('_')[0];
                }
            }
            if (CellsRec.Jcoord < MapSizeX)
            {
                if (MapArray[CellsRec.Icoord, CellsRec.Jcoord + 1].Contains("DeadCell") == false && CellsRec.RightC != "WallAround") 
                {
                    CellsRec.RightC = MapArray[CellsRec.Icoord, CellsRec.Jcoord + 1];
                    CellsRec.RightM = MapArray[CellsRec.Icoord, CellsRec.Jcoord + 1].Split('_')[0];
                }
            }
        }
        
    }

    public void ModuleSubstitutionI1()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();

        bool ModuleExist = false;

        ModulesAround = ModCells.ModulesAround;

        for (int i = 0; i <= ModCells.Cells.Count-1; i++)
        {

            if (ModCells.Cells[i].name.Contains("NullCell") == false)
            {
                if (ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM != "" && ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM != "0")
                {
                    if (ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM != AllModules[Num].gameObject.name)
                    {
                        if (ModulesAround.Count != 0)
                        {
                            for (int j = 0; j <= ModulesAround.Count - 1; j++)
                            {
                                if (ModulesAround[j] == ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM)
                                {
                                    ModuleExist = true;
                                }
                            }
                            if (ModuleExist == false)
                            {
                                ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM);
                            }
                            ModuleExist = false;
                        }
                        else if (ModulesAround.Count == 0)
                        {
                            ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().LeftM);
                        }
                    }
                }
                if (ModCells.Cells[i].GetComponent<CellsRecognition>().UpM != "" && ModCells.Cells[i].GetComponent<CellsRecognition>().UpM != "0")
                {
                    if (ModCells.Cells[i].GetComponent<CellsRecognition>().UpM != AllModules[Num].gameObject.name)
                    {
                        if (ModulesAround.Count != 0)
                        {
                            for (int j = 0; j <= ModulesAround.Count - 1; j++)
                            {
                                if (ModulesAround[j] == ModCells.Cells[i].GetComponent<CellsRecognition>().UpM)
                                {
                                    ModuleExist = true;
                                }
                            }
                            if (ModuleExist == false)
                            {
                                ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().UpM);
                            }
                            ModuleExist = false;
                        }
                        else if (ModulesAround.Count == 0)
                        {
                            ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().UpM);
                        }
                    }
                }
                if (ModCells.Cells[i].GetComponent<CellsRecognition>().DownM != "" && ModCells.Cells[i].GetComponent<CellsRecognition>().DownM != "0")
                {
                    if (ModCells.Cells[i].GetComponent<CellsRecognition>().DownM != AllModules[Num].gameObject.name)
                    {
                        if (ModulesAround.Count != 0)
                        {
                            for (int j = 0; j <= ModulesAround.Count - 1; j++)
                            {
                                if (ModulesAround[j] == ModCells.Cells[i].GetComponent<CellsRecognition>().DownM)
                                {
                                    ModuleExist = true;
                                }
                            }
                            if (ModuleExist == false)
                            {
                                ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().DownM);
                            }
                            ModuleExist = false;
                        }
                        else if (ModulesAround.Count == 0)
                        {
                            ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().DownM);

                        }
                    }
                }
                if (ModCells.Cells[i].GetComponent<CellsRecognition>().RightM != "" && ModCells.Cells[i].GetComponent<CellsRecognition>().RightM != "0")
                {
                    if (ModCells.Cells[i].GetComponent<CellsRecognition>().RightM != AllModules[Num].gameObject.name)
                    {
                        if (ModulesAround.Count != 0)
                        {
                            for (int j = 0; j <= ModulesAround.Count - 1; j++)
                            {
                                if (ModulesAround[j] == ModCells.Cells[i].GetComponent<CellsRecognition>().RightM)
                                {
                                    ModuleExist = true;
                                }
                            }
                            if (ModuleExist == false)
                            {                              
                                ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().RightM);
                            }
                            ModuleExist = false;
                        }
                        else if (ModulesAround.Count == 0)
                        {
                            ModulesAround.Add(ModCells.Cells[i].GetComponent<CellsRecognition>().RightM);
                        }
                    }
                }
            }
        }
    }

    public void ModuleEntrancesCheck_OneAround()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        GameObject NearMod = GameObject.Find(ModCells.ModulesAround[0]);
        ModuleCells NearModCells = NearMod.GetComponent<ModuleCells>();
        bool EntranceExist = false;
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;        

        if (ModCells.ModuleI > NearModCells.ModuleI + NearModCells.Isize - 1)               //Up
        {
            for (int k = NearModCells.ModuleJ; k <= NearModCells.Jsize + NearModCells.ModuleJ - 1; k++)
            {
                ModuleX = ModuleJ * 3;
                ModuleZ = ModuleI * 3;

                for (int f = 1; f <= 4; f++)
                {
                    for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
                    {
                        CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();

                        if (ModCells.Cells[k1].name.Contains("Entrance") &&
                            CellsRec.DownC.Contains(CellsRec.DownM) &&
                            CellsRec.DownC.Contains("Entrance") &&
                            CellsRec.DownC != "WallAround" &&
                            CellsRec.DownM != AllModules[Num].name)
                        {
                            EntranceExist = true;
                        }
                        if (EntranceExist == true)
                        {                           
                            break;
                        }
                    }
                    if (EntranceExist == true)
                    {
                        break;
                    }

                    if (f == 2)
                    {
                        AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
                    }
                    else if (f == 3)
                    {
                        AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
                    }
                    else if (f == 4)
                    {
                        AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
                    }

                    if (AllModules[Num].transform.localEulerAngles.y == 0)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 90)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
                        ModulesFlip();
                        ModulesFlip();
                        ModulesFlip();
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 180)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
                        ModulesFlip();
                        ModulesFlip();
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 270)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
                        ModulesFlip();
                    }

                    NamingCells();
                    CellsToMapArray();
                    CellsRecognition();
                    ModuleSubstitutionI1();

                }
                if (EntranceExist == true)
                {
                    break;
                }

                ModCells.ModuleJ++;
                ModuleX = ModuleJ * 3;
                //AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                NamingCells();
                CellsToMapArray();
                CellsRecognition();
                ModuleSubstitutionI1();
            }


        }
        else if(ModCells.ModuleJ > NearModCells.ModuleJ + NearModCells.Jsize - 1)           //Right
        {
            for (int k = NearModCells.ModuleI; k <= NearModCells.Isize + NearModCells.ModuleI - 1; k++) 
            {
                ModuleX = ModuleJ * 3;
                ModuleZ = ModuleI * 3;

                for (int f = 1; f <= 4; f++)
                {
                    //if (AllModules[Num].name == "SM2.1") { Debug.Log("1"); }
                    for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
                    {
                        CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();
                        //Debug.Log(ModCells.Cells[k1].name);
                        if (ModCells.Cells[k1].name.Contains("Entrance") &&
                            CellsRec.LeftC.Contains(CellsRec.LeftM) &&
                            CellsRec.LeftC.Contains("Entrance") &&
                            CellsRec.LeftC!= "WallAround" && 
                            CellsRec.LeftM!=AllModules[Num].name)
                        {

                            //if (AllModules[Num].name == "SM2.1") { Debug.Log("1"); }
                            EntranceExist = true;
                        }
                        if (EntranceExist == true)
                        {
                            break;
                        }
                    }
                    if (EntranceExist == true)
                    {
                        break;
                    }

                    if (f == 2)
                    {
                        AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
                    }
                    else if (f == 3)
                    {
                        AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
                    }
                    else if (f == 4)
                    {
                        AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
                    }

                    if (AllModules[Num].transform.localEulerAngles.y == 0)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 90)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
                        ModulesFlip();
                        ModulesFlip();
                        ModulesFlip();
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 180)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
                        ModulesFlip();
                        ModulesFlip();
                    }
                    else if (AllModules[Num].transform.localEulerAngles.y == 270)
                    {
                        AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
                        ModulesFlip();
                    }

                    NamingCells();
                    CellsToMapArray();
                    CellsRecognition();
                    ModuleSubstitutionI1();

                }
                if (EntranceExist == true)
                {
                    break;
                }
               
                ModCells.ModuleI++;
                ModuleZ = ModuleI * 3;
                //AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                NamingCells();
                CellsToMapArray();
                CellsRecognition();
                ModuleSubstitutionI1();
            }



        }
    }

    public void ModuleEntrancesCheck_TwoAround()
    {
        ModuleCells ModCells = AllModules[Num].GetComponent<ModuleCells>();
        GameObject NearMod1 = GameObject.Find(ModCells.ModulesAround[0]);
        ModuleCells NearMod1Cells = NearMod1.GetComponent<ModuleCells>();
        GameObject NearMod2 = GameObject.Find(ModCells.ModulesAround[1]);
        ModuleCells NearMod2Cells = NearMod2.GetComponent<ModuleCells>();
        bool EntranceDownExist = false;
        bool Entrance2Exist = false;
        int Isize = ModCells.Isize;
        int Jsize = ModCells.Jsize;

        if (ModCells.ModuleJ < NearMod1Cells.ModuleJ || ModCells.ModuleJ < NearMod2Cells.ModuleJ)                //Left
        {

            int SaveModuleI = ModuleI;
            int SaveModuleJ = ModuleJ;
            ModuleX = ModuleJ * 3;
            ModuleZ = ModuleI * 3;

            for (int f = 1; f <= 4; f++)
            {
                

                for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
                {
                    CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();

                    if (ModCells.Cells[k1].name.Contains("Entrance") &&
                        CellsRec.DownC.Contains(CellsRec.DownM) &&
                        CellsRec.DownC.Contains("Entrance") &&
                        CellsRec.DownC != "WallAround" &&
                        CellsRec.DownM != AllModules[Num].name)
                    {
                        EntranceDownExist = true;
                    }
                    if (ModCells.Cells[k1].name.Contains("Entrance") &&
                        CellsRec.RightC.Contains(CellsRec.RightM) &&
                        CellsRec.RightC.Contains("Entrance") &&
                        CellsRec.DownC != "WallAround" &&
                        CellsRec.DownM != AllModules[Num].name)
                    {
                        Entrance2Exist = true;
                    }
                    if (EntranceDownExist == true || Entrance2Exist == true)
                    {
                        break;
                    }
                }
                if (EntranceDownExist == true || Entrance2Exist == true)
                {
                    break;
                }

                if (f == 2)
                {
                    AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
                }
                else if (f == 3)
                {
                    AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
                }
                else if (f == 4)
                {
                    AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
                }

                if (AllModules[Num].transform.localEulerAngles.y == 0)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 90)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
                    ModulesFlip();
                    ModulesFlip();
                    ModulesFlip();
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 180)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
                    ModulesFlip();
                    ModulesFlip();
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 270)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
                    ModulesFlip();
                }

                NamingCells();
                CellsToMapArray();
                CellsRecognition();
                ModuleSubstitutionI1();

            }

            //if (EntranceDownExist == false || Entrance2Exist == false)
            //{

            //    EntranceDownExist = false;
            //    Entrance2Exist = false;
            //    ModuleI = SaveModuleI;
            //    ModuleJ = SaveModuleJ;
            //    ModuleX = ModuleJ * 3;
            //    ModuleZ = ModuleI * 3;
            //    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
            //    NamingCells();
            //    CellsToMapArray();
            //    CellsRecognition();
            //    ModuleSubstitutionI1();

            //    for (int f = 1; f <= 4; f++)
            //    {
            //        for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
            //        {
            //            CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();

            //            if (ModCells.Cells[k1].name.Contains("Entrance") &&
            //                CellsRec.DownC.Contains(CellsRec.DownM) &&
            //                CellsRec.DownC.Contains("Entrance") &&
            //                CellsRec.DownC != "WallAround" &&
            //                CellsRec.DownM != AllModules[Num].name)
            //            {
            //                EntranceDownExist = true;
            //            }
            //            if (ModCells.Cells[k1].name.Contains("Entrance") &&
            //                CellsRec.RightC.Contains(CellsRec.RightM) &&
            //                CellsRec.RightC.Contains("Entrance") &&
            //                CellsRec.RightC != "WallAround" &&
            //                CellsRec.RightM != AllModules[Num].name)
            //            {
            //                Entrance2Exist = true;
            //            }
            //            if (EntranceDownExist == true || Entrance2Exist == true)
            //            {
            //                break;
            //            }
            //        }
            //        if (EntranceDownExist == true || Entrance2Exist == true)
            //        {
            //            break;
            //        }

            //        if (f == 2)
            //        {
            //            AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
            //        }
            //        else if (f == 3)
            //        {
            //            AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
            //        }
            //        else if (f == 4)
            //        {
            //            AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
            //        }

            //        if (AllModules[Num].transform.localEulerAngles.y == 0)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 90)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
            //            ModulesFlip();
            //            ModulesFlip();
            //            ModulesFlip();
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 180)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
            //            ModulesFlip();
            //            ModulesFlip();
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 270)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
            //            ModulesFlip();
            //        }

            //        NamingCells();
            //        CellsToMapArray();
            //        CellsRecognition();
            //        ModuleSubstitutionI1();

            //    }
            //}

        }



        if (ModCells.ModuleJ > NearMod1Cells.ModuleJ + NearMod1Cells.Jsize - 1 || ModCells.ModuleJ > NearMod2Cells.ModuleJ + NearMod1Cells.Jsize - 1)                //Right
        {

            int SaveModuleI = ModuleI;
            int SaveModuleJ = ModuleJ;
            ModuleX = ModuleJ * 3;
            ModuleZ = ModuleI * 3;

            for (int f = 1; f <= 4; f++)
            {
                for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
                {
                    CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();

                    if (ModCells.Cells[k1].name.Contains("Entrance") &&
                        CellsRec.DownC.Contains(CellsRec.DownM) &&
                        CellsRec.DownC.Contains("Entrance") &&
                        CellsRec.DownC != "WallAround" &&
                        CellsRec.DownM != AllModules[Num].name)
                    {
                        EntranceDownExist = true;
                    }
                    if (ModCells.Cells[k1].name.Contains("Entrance") &&
                        CellsRec.LeftC.Contains(CellsRec.LeftM) &&
                        CellsRec.LeftC.Contains("Entrance") &&
                        CellsRec.LeftC != "WallAround" &&
                        CellsRec.LeftM != AllModules[Num].name)
                    {
                        Entrance2Exist = true;
                    }
                    if (EntranceDownExist == true || Entrance2Exist == true)
                    {
                        break;
                    }
                }
                if (EntranceDownExist == true || Entrance2Exist == true)
                {
                    break;
                }

                if (f == 2)
                {
                    AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
                }
                else if (f == 3)
                {
                    AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
                }
                else if (f == 4)
                {
                    AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
                }

                if (AllModules[Num].transform.localEulerAngles.y == 0)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 90)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
                    ModulesFlip();
                    ModulesFlip();
                    ModulesFlip();
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 180)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
                    ModulesFlip();
                    ModulesFlip();
                }
                else if (AllModules[Num].transform.localEulerAngles.y == 270)
                {
                    AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
                    ModulesFlip();
                }

                NamingCells();
                CellsToMapArray();
                CellsRecognition();
                ModuleSubstitutionI1();

            }

            //if (EntranceDownExist == false || Entrance2Exist == false)
            //{

            //    EntranceDownExist = false;
            //    Entrance2Exist = false;
            //    ModuleI = SaveModuleI;
            //    ModuleJ = SaveModuleJ;
            //    ModuleX = ModuleJ * 3;
            //    ModuleZ = ModuleI * 3;
            //    AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
            //    NamingCells();
            //    CellsToMapArray();
            //    CellsRecognition();
            //    ModuleSubstitutionI1();


            //    for (int f = 1; f <= 4; f++)
            //    {
            //        for (int k1 = 0; k1 <= ModCells.Cells.Count - 1; k1++)
            //        {
            //            CellsRecognition CellsRec = ModCells.Cells[k1].GetComponent<CellsRecognition>();

            //            if (ModCells.Cells[k1].name.Contains("Entrance") &&
            //                CellsRec.DownC.Contains(CellsRec.DownM) &&
            //                CellsRec.DownC.Contains("Entrance") &&
            //                CellsRec.DownC != "WallAround" &&
            //                CellsRec.DownM != AllModules[Num].name)
            //            {
            //                EntranceDownExist = true;
            //            }
            //            if (ModCells.Cells[k1].name.Contains("Entrance") &&
            //                CellsRec.LeftC.Contains(CellsRec.LeftM) &&
            //                CellsRec.LeftC.Contains("Entrance") &&
            //                CellsRec.LeftC != "WallAround" &&
            //                CellsRec.LeftM != AllModules[Num].name)
            //            {
            //                Entrance2Exist = true;
            //            }
            //            if (EntranceDownExist == true || Entrance2Exist == true)
            //            {
            //                break;
            //            }
            //        }
            //        if (EntranceDownExist == true || Entrance2Exist == true)
            //        {
            //            break;
            //        }

            //        if (f == 2)
            //        {
            //            AllModules[Num].transform.Rotate(0, 90, 0, Space.World);
            //        }
            //        else if (f == 3)
            //        {
            //            AllModules[Num].transform.Rotate(0, 180, 0, Space.World);
            //        }
            //        else if (f == 4)
            //        {
            //            AllModules[Num].transform.Rotate(0, 270, 0, Space.World);
            //        }

            //        if (AllModules[Num].transform.localEulerAngles.y == 0)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ);
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 90)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX, 0, ModuleZ + Isize * 3);
            //            ModulesFlip();
            //            ModulesFlip();
            //            ModulesFlip();
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 180)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ + Isize * 3);
            //            ModulesFlip();
            //            ModulesFlip();
            //        }
            //        else if (AllModules[Num].transform.localEulerAngles.y == 270)
            //        {
            //            AllModules[Num].transform.position = new Vector3(ModuleX + Jsize * 3, 0, ModuleZ);
            //            ModulesFlip();
            //        }

            //        NamingCells();
            //        CellsToMapArray();
            //        CellsRecognition();
            //        ModuleSubstitutionI1();

            //    }
            //}
        }
    }

    public void CheckIfSolidWall()
    {
        ModuleCells ModCells = BuildAllModules[0].GetComponent<ModuleCells>();
        SolidWall = true;


        for (int n = CurrentI; n <= CurrentI + ModCells.Isize - 1; n++) 
        {           
            for (int m = CurrentJ; m <= CurrentJ + ModCells.Jsize - 1; m++) 
            {
                //Debug.Log(n);
                //Debug.Log(m);

                if (m == CurrentJ && CurrentJ + ModCells.Jsize - 1 <= MapSizeX && CurrentJ > 1)
                {
                    if (MapArray[n, m - 1] != "0" && MapArray[n, m - 1].Contains("DeadCell")==false)
                    {
                        if (MapArray[n, m - 1].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                        {
                            GameObject LeftC = GameObject.Find(MapArray[n, m - 1]);
                            CellsRecognition CellsRecLeft = LeftC.GetComponent<CellsRecognition>();

                            if (LeftC.name.Contains("Entrance"))
                            {
                                if (CellsRecLeft.RightC != "WallAround")
                                {
                                    SolidWall = false;
                                }
                            }
                        }
                    }
                }
                    

                if (m == CurrentJ + ModCells.Jsize - 1)
                {

                    if (MapArray[n, m + 1] != "0" && MapArray[n, m + 1].Contains("DeadCell")==false)
                    {
                        if (MapArray[n, m + 1].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                        {
                            GameObject RightC = GameObject.Find(MapArray[n, m + 1]);
                            CellsRecognition CellsRecRight = RightC.GetComponent<CellsRecognition>();

                            if (RightC.name.Contains("Entrance"))
                            {
                                if (CellsRecRight.LeftC != "WallAround")
                                {
                                    SolidWall = false;
                                }
                            }
                        }
                    }
                }
                if (CurrentI > 1)
                {
                    if (n == CurrentI)
                    {
                        if (MapArray[n - 1, m] != "0" && MapArray[n - 1, m].Contains("DeadCell")==false)
                        {
                            if (MapArray[n - 1, m].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                            {
                                GameObject DownC = GameObject.Find(MapArray[n - 1, m]);
                                if (BuildAllModules[0].name.Contains("RM1"))
                                {
                                    //Debug.Log(DownC.name);
                                }
                                CellsRecognition CellsRecDown = DownC.GetComponent<CellsRecognition>();

                                if (DownC.name.Contains("Entrance"))
                                {
                                    if (CellsRecDown.UpC != "WallAround")
                                    {
                                        SolidWall = false;
                                    }
                                }
                            }
                        }                      
                    }
                }
                
            }
        }

    }

    public void CheckIfSolidWallLeft()
    {
        ModuleCells ModCells = BuildAllModules[0].GetComponent<ModuleCells>();
        SolidWall = true;


        for (int n = CurrentI; n <= CurrentI + ModCells.Isize - 1; n++)
        {
            for (int m = CurrentJ-ModCells.Jsize; m <= CurrentJ - 1; m++)
            {
                //Debug.Log(n);
                //Debug.Log(m);

                if (m == CurrentJ && CurrentJ + ModCells.Jsize - 1 <= MapSizeX && CurrentJ > 1)
                {
                    if (MapArray[n, m - 1] != "0" && MapArray[n, m - 1] != "DeadCell")
                    {
                        if (MapArray[n, m - 1].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                        {
                            GameObject LeftC = GameObject.Find(MapArray[n, m - 1]);
                            CellsRecognition CellsRecLeft = LeftC.GetComponent<CellsRecognition>();

                            if (LeftC.name.Contains("Entrance"))
                            {
                                if (CellsRecLeft.RightC != "WallAround")
                                {
                                    SolidWall = false;
                                }
                            }
                        }
                    }
                }


                if (m == CurrentJ + ModCells.Jsize - 1)
                {

                    if (MapArray[n, m + 1] != "0" && MapArray[n, m + 1] != "DeadCell")
                    {
                        if (MapArray[n, m + 1].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                        {
                            GameObject RightC = GameObject.Find(MapArray[n, m + 1]);
                            CellsRecognition CellsRecRight = RightC.GetComponent<CellsRecognition>();

                            if (RightC.name.Contains("Entrance"))
                            {
                                if (CellsRecRight.LeftC != "WallAround")
                                {
                                    SolidWall = false;
                                }
                            }
                        }
                    }
                }
                if (CurrentI > 1)
                {
                    if (n == CurrentI)
                    {
                        if (MapArray[n - 1, m] != "0" && MapArray[n - 1, m] != "DeadCell")
                        {
                            if (MapArray[n - 1, m].Split('_')[0].Split('.')[0] != BuildAllModules[0].name)
                            {
                                GameObject DownC = GameObject.Find(MapArray[n - 1, m]);
                                CellsRecognition CellsRecDown = DownC.GetComponent<CellsRecognition>();

                                if (DownC.name.Contains("Entrance"))
                                {
                                    if (CellsRecDown.UpC != "WallAround")
                                    {
                                        SolidWall = false;
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }

    }
}
























