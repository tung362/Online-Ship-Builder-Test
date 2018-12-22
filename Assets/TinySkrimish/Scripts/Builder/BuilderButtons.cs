using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class BuilderButtons : MonoBehaviour
{
    /*Settings*/
    public GameObject SaveConfirmed;
    public GameObject SaveVerify;

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
    }

    //Change tabs
    public void SetCurrentTab(int ID)
    {
        TheBuilderSettings.CurrentTab = ID;
        TheBuilderSettings.CurrentSlot = 1;
    }

    //Change slots
    public void SetCurrentSlot(int ID)
    {
        if (TheBuilderSettings.PlacingNewObject)
        {
            if (TheBuilderSettings.CurrentSlot == ID) TheBuilderSettings.PlacingNewObject = false;
        }
        else TheBuilderSettings.PlacingNewObject = true;
        TheBuilderSettings.CurrentSlot = ID;
    }

    public void SetMirror()
    {
        TheBuilderSettings.Mirror = !TheBuilderSettings.Mirror;
    }

    public void Save(bool ForceSave)
    {
        //Overwrite save
        if (ForceSave) TheBuilderSettings.GetComponent<BuilderSaveLoad>().Save();
        else
        {
            //Checks if file exist or not, if it exists bring up on
            if (File.Exists(Application.dataPath + "/../" + "/ShipSaves" + "/" + TheBuilderSettings.ShipName)) SaveVerify.SetActive(true);
            else
            {
                //Save
                TheBuilderSettings.GetComponent<BuilderSaveLoad>().Save();
                SaveConfirmed.SetActive(true);
            }
        }
    }

    public void Load()
    {
        TheBuilderSettings.GetComponent<BuilderSaveLoad>().Load(TheBuilderSettings.VerifyLoadName);
        TheBuilderSettings.VerifyLoad = false;
    }
}
