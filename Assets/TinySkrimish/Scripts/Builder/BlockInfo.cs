using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stats of each block
public class BlockInfo : MonoBehaviour
{
    /*Settings*/
    public bool IsWeapon = false;
    public float Cost = 0;

    /*Hull Info*/
    [Header("Hull Settings")]
    public float Thrust = 0;
    public float Weight = 0;
    public float Armor = 0;
    public float Shield = 0;

    /*Weapon Info*/
    [Header("Weapon Settings")]
    public float HullDamage = 0;
    public float ShieldDamage = 0;
    public float FireRate = 0;
    public float Velocity = 0;
}
