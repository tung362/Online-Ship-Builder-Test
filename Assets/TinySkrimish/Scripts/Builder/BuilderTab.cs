using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderTab : MonoBehaviour
{
    /*Settings*/
    public GameObject Watch;
    public GameObject Watch3D;

    public int TabID = 1;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
    }

    void Update ()
    {
		if(TheBuilderSettings.CurrentTab == TabID)
        {
            Watch.SetActive(true);
            Watch3D.SetActive(true);
        }
        else
        {
            Watch.SetActive(false);
            Watch3D.SetActive(false);
        }
	}
}
