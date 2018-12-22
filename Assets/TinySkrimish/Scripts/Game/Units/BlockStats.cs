using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlockStats : NetworkBehaviour
{
    public float Health = 100;
    public float MaxHealth = 100;
    private float HealthRegenDelay = 1;
    private float HealthRegenRate = 5;
    private float HealthTimer = 0;

    [SyncVar]
    public bool ShieldIsDown = true;
    public bool HasShield = false;
    public float ShieldHealth = 0;
    public float MaxShieldHealth = 0;
    private float ShieldRegenDelay = 1;
    private float ShieldRegenRate = 5;
    private float ShieldRegenTimer = 0;
    private float ShieldCooldownDelay = 5;
    private float ShieldCooldownTimer = 0;



    void Update()
    {
        if (!isServer) return;
        if (Health <= 0) Die();
        if (transform.root.GetComponent<ShipSchematic>() == null) return;
        UpdateHealth();
        UpdateShield();
    }

    void UpdateHealth()
    {
        if (Health < MaxHealth)
        {
            HealthTimer += Time.deltaTime;
            if (HealthTimer >= HealthRegenDelay)
            {
                Health += HealthRegenRate;
                HealthTimer = 0;
            }
        }

        if (Health > MaxHealth) Health = MaxHealth; //Limit
        if (Health < 0) Health = 0; //Limit
    }

    void UpdateShield()
    {
        if (!HasShield) return;
        if(ShieldIsDown)
        {
            ShieldCooldownTimer += Time.deltaTime;
            if(ShieldCooldownTimer >= ShieldCooldownDelay)
            {
                ShieldIsDown = false;
                ShieldHealth = 1;
                ShieldCooldownTimer = 0;
            }
        }
        else
        {
            if (ShieldHealth <= 0)
            {
                ShieldIsDown = true;
                return;
            }

            if(ShieldHealth < MaxShieldHealth)
            {
                ShieldRegenTimer += Time.deltaTime;
                if(ShieldRegenTimer >= ShieldRegenDelay)
                {
                    ShieldHealth += ShieldRegenRate;
                    ShieldRegenTimer = 0;
                }
            }

            if (ShieldHealth > MaxShieldHealth) ShieldHealth = MaxShieldHealth; //Limit
            if (ShieldHealth < 0) ShieldHealth = 0; //Limit
        }
    }

    void Die()
    {
        Destroy(gameObject);
        if (transform.root.GetComponent<ShipSchematic>() != null)
        {
            if (gameObject != transform.root.gameObject) transform.root.GetComponent<ShipSchematic>().RequestAdjacentCheck();
            else transform.root.GetComponent<ShipSchematic>().SeperateAllBlocks();
            transform.root.GetComponent<ShipMeshCombine>().RequestCombine();
        }
    }

    void OnDestroy()
    {
        if (isServer) return;
        if(transform.root.GetComponent<ShipMeshCombine>() != null) transform.root.GetComponent<ShipMeshCombine>().RequestCombine();
    }
}
