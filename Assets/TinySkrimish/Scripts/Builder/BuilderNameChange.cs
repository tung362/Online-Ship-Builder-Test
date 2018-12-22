using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderNameChange : MonoBehaviour
{
    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        name = TheBuilderSettings.ShipName;
    }

    void Update ()
    {
        name = TheBuilderSettings.ShipName;
    }
}
