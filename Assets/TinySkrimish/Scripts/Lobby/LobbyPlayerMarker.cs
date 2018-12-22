using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerMarker : MonoBehaviour
{
    public GameObject FocusTarget;

    void Update()
    {
        transform.LookAt(new Vector3(FocusTarget.transform.position.x, transform.position.y, FocusTarget.transform.position.z));
    }
}
