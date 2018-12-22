using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderLoaderSlot : MonoBehaviour
{
    /*Data*/
    //Auto assigned by loader tracker script
    //[HideInInspector]
    public string FileName = "";
    public RawImage Slot;
    public GameObject TheSaveShip;
    public Text Cost;
    public Text NameText;

    /*Required Components*/
    private BuilderSaveLoad TheBuilderSaveLoad;

    void Start()
    {
        TheBuilderSaveLoad = FindObjectOfType<BuilderSaveLoad>();
    }

    public void UpdateNameDisplay()
    {
        NameText.text = FileName;
    }

    //Loads saved ship
    public void Load()
    {
        TheBuilderSaveLoad.GetComponent<BuilderSettings>().VerifyLoadName = FileName;
        TheBuilderSaveLoad.GetComponent<BuilderSettings>().LoadVerifyObject.SetActive(true);
        TheBuilderSaveLoad.GetComponent<BuilderSettings>().Loader.SetActive(false);
    }

    //Deletes saved ship
    public void Delete()
    {
        TheBuilderSaveLoad.Delete(FileName);
        Destroy(TheSaveShip);
        Destroy(gameObject);
    }
}
