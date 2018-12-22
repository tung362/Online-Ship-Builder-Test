using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ClassOverNetwork : MonoBehaviour
{
    /*Settings*/
    public GameObject ShipSpawner;

    /*Data*/
    public List<FileInfo> AllSaves = new List<FileInfo>();

    /*Required Components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) LoadShip();
    }

    void LoadShip()
    {
        //Creates the new save folder if there isnt one already
        Directory.CreateDirectory(Application.dataPath + "/../" + "/ShipSaves");

        //Looks at the save folder directory
        DirectoryInfo FileInfo = new DirectoryInfo(Application.dataPath + "/../" + "/ShipSaves");
        //Gets all save files
        AllSaves.AddRange(FileInfo.GetFiles());

        //Checks if the file exists
        if (File.Exists(Application.dataPath + "/../" + "/ShipSaves" + "/" + AllSaves[0].Name))
        {
            BinaryFormatter formater = new BinaryFormatter();

            byte[] bytes;
            bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/../" + "/ShipSaves" + "/" + AllSaves[0].Name);

            if (!Tracker.IsFullyReady) return;
            Tracker.ThePlayerControlPanel.TheCommandManager.CmdAddToSpawnQueue(bytes, ShipSpawner, Tracker.ThePlayerControlPanel.ID);
        }
    }
}
