using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LaserTurret : NetworkBehaviour
{
    public LineRenderer Laser;
    public GameObject Flare;
    public GameObject FireSpot;

    public float Range = 12;
    public float FireDelay = 0.1f;
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
                Laser.SetPosition(1, FireSpot.transform.position);
                Laser.SetPosition(2, FireSpot.transform.position);
                Flare.SetActive(false);
                float ClosestDistance = int.MaxValue;
                RaycastHit[] hits = Physics.RaycastAll(transform.position, (transform.root.GetComponent<UnitID>().Target.transform.position - transform.position).normalized, Range);
                for(int i = 0; i < hits.Length; i++)
                {
                    if(hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Attached") || hits[i].transform.gameObject.layer == LayerMask.NameToLayer("Unattached"))
                    {

                    }
                }

                //Fire
                FireTimer += Time.deltaTime;
                if (FireTimer >= FireDelay)
                {
                    FireTimer = 0;
                }
            }
        }
        else
        {
            Laser.SetPosition(1, Vector3.zero);
            Laser.SetPosition(2, Vector3.zero);
            Flare.SetActive(false);
            if (FireTimer < FireDelay) FireTimer += Time.deltaTime;
        }
    }
}
