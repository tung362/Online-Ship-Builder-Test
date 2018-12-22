using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StraightProjectile : NetworkBehaviour
{
    /*Settings*/
    public float Speed = 200;
    public float Damage = 10;
    public float PercentMaintainFromShieldHit = 0.75f;

    /*Required Components*/
    private Rigidbody TheRigidbody;

    void Start()
    {
        TheRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isServer) return;
        TheRigidbody.velocity = transform.forward * Speed * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider Other)
    {
        if (!isServer) return;
        if (Other.tag == "Shield")
        {
            if(Other.transform.root.GetComponent<ShipSchematic>() != null)
            {
                if(Other.transform.root.GetComponent<UnitID>().ID != GetComponent<ProjectileColorChange>().ID)
                {
                    float newDamageValue = Damage - Other.GetComponent<Shield>().TheBlockStats.ShieldHealth;
                    Other.GetComponent<Shield>().TheBlockStats.ShieldHealth -= Damage * PercentMaintainFromShieldHit;
                    Damage = newDamageValue;
                    Other.transform.root.GetComponent<ShipSchematic>().StartRegeneratingTimer = 0;
                }
            }
        }
        else if (Other.tag == "Block" || Other.tag == "Unit" || Other.tag == "ShipBase")
        {
            if (Other.transform.root.GetComponent<ShipSchematic>() != null)
            {
                if (Other.transform.root.GetComponent<UnitID>().ID != GetComponent<ProjectileColorChange>().ID)
                {
                    float newDamageValue = Damage - Other.GetComponent<BlockStats>().Health;
                    Other.GetComponent<BlockStats>().Health -= Damage;
                    Damage = newDamageValue;
                    Other.transform.root.GetComponent<ShipSchematic>().StartRegeneratingTimer = 0;
                }
            }
            else
            {
                float newDamageValue = Damage - Other.GetComponent<BlockStats>().Health;
                Other.GetComponent<BlockStats>().Health -= Damage;
                Damage = newDamageValue;
            }
        }

        if (Damage <= 0) Destroy(gameObject);
    }
}
