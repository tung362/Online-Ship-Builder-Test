using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StarBaseSpawner : NetworkBehaviour
{
    /*Data*/
    [HideInInspector]
    public GameObject ExistingStarBase;

    //Which player is capturing the base first
    [SyncVar]
    public int AttackerID = 0;
    //List of attackers within range
    public List<int> Attackers = new List<int>();
    [SyncVar]
    public float CaptureProgress = 0;

    //Spawnable unit
    public GameObject ShipBasePrefab;

    void Start()
    {
        if (isServer) SpawnShipBase();
    }

    void Update()
    {
        if (!isServer) return;
    }

    [ServerCallback]
    void SpawnShipBase()
    {
        GameObject spawnedShipBase = Instantiate(ShipBasePrefab, transform.position, transform.rotation);
        spawnedShipBase.transform.eulerAngles = new Vector3(0, Random.Range(0.0f, 360f), 0);
        //Checks if the file exists
        if (File.Exists(Application.dataPath + "/../" + "/StationData" + "/" + "Station"))
        {
            BinaryFormatter formater = new BinaryFormatter();
            //Open file
            FileStream stream = new FileStream(Application.dataPath + "/../" + "/StationData" + "/" + "Station", FileMode.Open);

            //Obtain the saved data
            ShipData theShipData = formater.Deserialize(stream) as ShipData;
            stream.Close();

            spawnedShipBase.GetComponent<ShipSchematic>().SetSchematic(theShipData);
        }
        NetworkServer.Spawn(spawnedShipBase);
        ShipBasePrefab = spawnedShipBase;
    }
}
