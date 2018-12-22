using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Determine which side this unit is on
public class UnitID : NetworkBehaviour
{
    [SyncVar]
    public int ID = -1;

    [SyncVar]
    public GameObject Target;
}
