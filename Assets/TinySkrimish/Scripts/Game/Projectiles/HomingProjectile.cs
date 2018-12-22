using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HomingProjectile : NetworkBehaviour
{
    /*Settings*/
    public float Speed = 200;
    public float RotationSpeed = 5;
    public float Damage = 50;
    public float SplashRange = 0.73f;
    public float PercentMaintainFromShieldHit = 0.5f;
    public LayerMask Mask;

    /*Data*/
    [HideInInspector]
    public GameObject Target;

    /*Required Components*/
    private Rigidbody TheRigidbody;

    void Start()
    {
        TheRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isServer) return;

        //Slowly rotates towards target
        if (Target != null)
        {
            Vector3 direction = (new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z) - transform.position).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.fixedDeltaTime);
        }
        TheRigidbody.velocity = transform.forward * Speed * Time.fixedDeltaTime;
    }

    void OnTriggerEnter(Collider Other)
    {
        if (!isServer) return;

        if (Other.tag == "Shield" || Other.tag == "Block")
        {
            if (Other.transform.root.GetComponent<ShipSchematic>() != null)
            {
                if (Other.transform.root.GetComponent<UnitID>().ID != GetComponent<ProjectileColorChange>().ID) Explode();
            }
            else Explode();
        }
    }

    void Explode()
    {
        Collider[] others = Physics.OverlapSphere(transform.position, SplashRange, Mask.value);
        for (int i = 0; i < others.Length; i++)
        {
            if (others[i].tag == "Shield")
            {
                if (others[i].transform.root.GetComponent<ShipSchematic>() != null)
                {
                    if (others[i].transform.root.GetComponent<UnitID>().ID != GetComponent<ProjectileColorChange>().ID)
                    {
                        others[i].GetComponent<Shield>().TheBlockStats.ShieldHealth -= Damage * PercentMaintainFromShieldHit;
                        others[i].transform.root.GetComponent<ShipSchematic>().StartRegeneratingTimer = 0;
                    }
                }
            }
            else if (others[i].tag == "Block" || others[i].tag == "Unit" || others[i].tag == "ShipBase")
            {
                bool ProtectedByShield = false;
                RaycastHit[] mouseHits = Physics.RaycastAll(transform.position, (others[i].transform.position - transform.position).normalized, Mask.value);
                for(int j = 0; j < mouseHits.Length; j++)
                {
                    if (mouseHits[j].collider.tag == "Shield")
                    {
                        ProtectedByShield = true;
                        break;
                    }
                }

                if(!ProtectedByShield)
                {
                    if (others[i].transform.root.GetComponent<ShipSchematic>() != null)
                    {
                        if (others[i].transform.root.GetComponent<UnitID>().ID != GetComponent<ProjectileColorChange>().ID)
                        {
                            others[i].GetComponent<BlockStats>().Health -= Damage;
                            others[i].transform.root.GetComponent<ShipSchematic>().StartRegeneratingTimer = 0;
                        }
                    }
                    else others[i].GetComponent<BlockStats>().Health -= Damage;
                }
            }
        }

        Destroy(gameObject);
    }
}
