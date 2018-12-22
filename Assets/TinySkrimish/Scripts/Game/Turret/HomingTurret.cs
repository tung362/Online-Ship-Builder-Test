﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HomingTurret : NetworkBehaviour
{
    public GameObject ProjectilePrefab;
    public GameObject FireSpot;
    public float Range = 14;
    public float FireDelay = 0.7f;
    private float FireTimer = 0;

    /*Data*/
    private bool RunOnce = true;

    void Start()
    {
        FireTimer = FireDelay;
    }

    void Update()
    {
        UpdateTurret();
    }

    void UpdateTurret()
    {
        if (!isServer) return;
        if (transform.root.GetComponent<ShipSchematic>() == null) return;


        if (transform.root.GetComponent<UnitID>().Target != null)
        {
            //Distance check
            if (Vector3.Distance(transform.position, transform.root.GetComponent<UnitID>().Target.transform.position) <= Range)
            {
                //Fire
                FireTimer += Time.deltaTime;
                if (FireTimer >= FireDelay)
                {
                    GameObject spawnedUnit = Instantiate(ProjectilePrefab, FireSpot.transform.position, FireSpot.transform.rotation) as GameObject;
                    spawnedUnit.GetComponent<HomingProjectile>().Target = transform.root.GetComponent<UnitID>().Target;
                    spawnedUnit.GetComponent<ProjectileColorChange>().ID = transform.root.GetComponent<UnitID>().ID;
                    NetworkServer.Spawn(spawnedUnit);
                    FireTimer = 0;
                }
            }
        }
        else
        {
            if (FireTimer < FireDelay) FireTimer += Time.deltaTime;
        }
    }
}
