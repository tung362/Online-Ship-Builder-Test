using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipBase : NetworkBehaviour
{
    /*Settings*/
    public float GenMoneyDelay = 1;
    public int GenMoneyRate = 5;
    public float SpawnDelay = 0.5f;
    //Spawnable unit
    public GameObject ShipPrefab;

    /*Data*/
    private float GenMoneyTimer = 0;
    private float SpawnTimer = 0;
    private List<ShipData> SpawnQueue = new List<ShipData>();
    //Npc money if base is currently ran by bandits
    public float Money = 0;

    /*Required components*/
    private ManagerTracker Tracker;

    void Start()
    {
        Tracker = GameObject.FindGameObjectWithTag("ManagerTracker").GetComponent<ManagerTracker>();
    }

    void Update()
    {
        if (!isServer) return;
        if (!Tracker.IsFullyReady) return;

        GenerateMoney();
        SpawnUnits();
    }

    //Generate money overtime
    void GenerateMoney()
    {
        GenMoneyTimer += Time.deltaTime;
        if (GenMoneyTimer >= GenMoneyDelay)
        {
            if(GetComponent<UnitID>().ID != -1) Tracker.ThePlayerControlPanel.TheResourceManager.Moneys[GetComponent<UnitID>().ID] += GenMoneyRate;
            else Money += GenMoneyRate;
            GenMoneyTimer = 0;
        }
    }

    //Spawn units overtime
    void SpawnUnits()
    {
        if (SpawnQueue.Count == 0) return;

        SpawnTimer += Time.deltaTime;
        if (SpawnTimer >= SpawnDelay)
        {
            SpawnUnit(SpawnQueue[0]);
            SpawnTimer = 0;
        }
    }

    Vector3 GeneratePoint(Vector3 Point, float Radius)
    {
        return Point + (Random.insideUnitSphere * Radius);
    }

    //Spawns unit
    [ServerCallback]
    void SpawnUnit(ShipData ShipSchematic)
    {
        //To do: give random move to point
        GameObject spawnedShip = Instantiate(ShipPrefab, new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z), transform.rotation);
        spawnedShip.name = ShipPrefab.name;
        spawnedShip.GetComponent<ShipSchematic>().SetSchematic(ShipSchematic);
        spawnedShip.GetComponent<UnitID>().ID = GetComponent<UnitID>().ID;
        NetworkServer.Spawn(spawnedShip);

        //Spawn ship parts
        //for (int i = 0; i < ShipSchematic.Parts.Length; i++)
        //{
        //    for (int j = 0; j < Tracker.NetworkBlocks.Count; j++)
        //    {
        //        string partName = ShipSchematic.Parts[i].Name;
        //        partName = partName.Replace(Tracker.NetworkBlocks[j].name, "");
        //        if (partName == "Builder")
        //        {
        //            Vector3 spawnPoint = GeneratePoint(spawnedShip.transform.position, 2);
        //            GameObject spawnedBlock = Instantiate(Tracker.NetworkBlocks[j], new Vector3(spawnPoint.x, spawnedShip.transform.position.y, spawnPoint.z), Random.rotation);
        //            spawnedBlock.name = Tracker.NetworkBlocks[j].name;
        //            NetworkServer.Spawn(spawnedBlock);
        //            break;
        //        }
        //    }
        //}

        //Remove from queue
        SpawnQueue.RemoveAt(0);
    }

    //Queues up for spawning
    [ServerCallback]
    public void AddToSpawnQueue(ShipData ShipSchematic, int ID)
    {
        if (ID != GetComponent<UnitID>().ID) return;
        //Calculate cost
        float Cost = 0;
        for(int i = 0; i < ShipSchematic.Parts.Length; i++)
        {
            for(int j = 0; j < Tracker.BuilderBlocks.Count; j++)
            {
                if(ShipSchematic.Parts[i].Name == Tracker.BuilderBlocks[j].name)
                {
                    Cost += Tracker.BuilderBlocks[j].Cost;
                    break;
                }
            }
        }

        //Add to queue
        if (GetComponent<UnitID>().ID != -1)
        {
            if (Tracker.ThePlayerControlPanel.TheResourceManager.Moneys[GetComponent<UnitID>().ID] >= Cost)
            {
                SpawnQueue.Add(ShipSchematic);
                Tracker.ThePlayerControlPanel.TheResourceManager.Moneys[GetComponent<UnitID>().ID] -= Cost;
            }
        }
        else
        {
            if (Money >= Cost)
            {
                SpawnQueue.Add(ShipSchematic);
                Money -= Cost;
            }
        }
    }
}
