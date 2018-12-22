using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderLoaderTracker : MonoBehaviour
{
    /*Settings*/
    public Scrollbar TheScrollbar;
    public GameObject SaveShipZone;
    public GameObject SlotPrefab;
    public GameObject SaveCamPrefab;
    public GameObject SaveShipPrefab;
    public float SlotOffset = 294;
    public float VisualOffset = 20;

    /*Data*/
    private List<BuilderLoaderSlot> LoaderSlots = new List<BuilderLoaderSlot>();
    private List<Camera> LoaderSaveCams = new List<Camera>();
    private List<BuilderSaveShipRoot> LoaderSaveShips = new List<BuilderSaveShipRoot>();
    private float TotalSlotOffset = 0;
    private Vector3 PreviousPostion;

    /*Required Components*/
    private BuilderSaveLoad TheBuilderSaveLoad;
    private RectTransform TheTransform;

    void Start()
    {
        TheBuilderSaveLoad = FindObjectOfType<BuilderSaveLoad>();
        TheTransform = GetComponent<RectTransform>();
        PreviousPostion = TheTransform.anchoredPosition3D;
    }

    void Update()
    {
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        TheTransform.anchoredPosition3D = Vector3.Lerp(new Vector3(PreviousPostion.x, TheTransform.anchoredPosition3D.y, TheTransform.anchoredPosition3D.z), new Vector3((-TotalSlotOffset + SlotOffset), TheTransform.anchoredPosition3D.y, TheTransform.anchoredPosition3D.z), TheScrollbar.value);
    }

    public void UpdateSlots()
    {
        TheBuilderSaveLoad = FindObjectOfType<BuilderSaveLoad>();

        //Clears out all slots
        for (int i = 0; i < LoaderSlots.Count; i++)
        {
            if (LoaderSlots[i] != null) Destroy(LoaderSlots[i].gameObject);
            if (LoaderSaveCams[i] != null) Destroy(LoaderSaveCams[i].gameObject);
            if (LoaderSaveShips[i] != null) Destroy(LoaderSaveShips[i].gameObject);
        }

        LoaderSlots.Clear();
        LoaderSaveCams.Clear();
        LoaderSaveShips.Clear();

        //Exist Check
        for (int i = 0; i < TheBuilderSaveLoad.AllSaves.Count; i++)
        {
            //Creates new slots
            //Slot
            GameObject theSlot = Instantiate(SlotPrefab, transform.position, SlotPrefab.transform.rotation) as GameObject;
            theSlot.GetComponent<RectTransform>().parent = transform;
            theSlot.GetComponent<RectTransform>().localScale = Vector3.one;
            theSlot.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
            theSlot.GetComponent<BuilderLoaderSlot>().FileName = TheBuilderSaveLoad.AllSaves[i].Name;
            theSlot.GetComponent<BuilderLoaderSlot>().UpdateNameDisplay();
            LoaderSlots.Add(theSlot.GetComponent<BuilderLoaderSlot>());

            //Save cam
            GameObject theCam = Instantiate(SaveCamPrefab, SaveShipZone.transform.position, SaveCamPrefab.transform.rotation) as GameObject;
            theCam.transform.parent = SaveShipZone.transform;
            RenderTexture TheTargetTexture = new RenderTexture(256, 256, 24);
            theCam.GetComponent<Camera>().targetTexture = TheTargetTexture;
            theSlot.GetComponent<BuilderLoaderSlot>().Slot.texture = TheTargetTexture;
            LoaderSaveCams.Add(theCam.GetComponent<Camera>());

            //Save ship
            GameObject theSaveShip = Instantiate(SaveShipPrefab, SaveShipZone.transform.position, SaveShipPrefab.transform.rotation) as GameObject;
            theSaveShip.transform.parent = SaveShipZone.transform;
            TheBuilderSaveLoad.LoadToSaveShip(TheBuilderSaveLoad.AllSaves[i].Name, theSaveShip, theSlot.GetComponent<BuilderLoaderSlot>().Cost);
            theSlot.GetComponent<BuilderLoaderSlot>().TheSaveShip = theSaveShip;
            LoaderSaveShips.Add(theSaveShip.GetComponent<BuilderSaveShipRoot>());
        }

        //Removes junk childs from save ships to prevent lag
        for (int i = 0; i < LoaderSaveShips.Count; i++) LoaderSaveShips[i].ClearAllBlocks();

        //Updates slot positions
        float CurrentSlot = PreviousPostion.x;
        float CurrentVisual = 0;
        for (int i = 0; i < TheBuilderSaveLoad.AllSaves.Count; i++)
        {
            for (int j = 0; j < LoaderSlots.Count; j++)
            {
                if (TheBuilderSaveLoad.AllSaves[i].Name == LoaderSlots[j].FileName)
                {
                    CurrentSlot += SlotOffset;
                    CurrentVisual += VisualOffset;
                    LoaderSaveCams[j].transform.localPosition = new Vector3(CurrentVisual, 4, 0);
                    LoaderSaveShips[j].transform.localPosition = new Vector3(CurrentVisual, 0, 0);
                    LoaderSlots[j].GetComponent<RectTransform>().anchoredPosition = new Vector3(CurrentSlot, 1.6f, 0);
                    break;
                }
            }
        }

        TotalSlotOffset = CurrentSlot;
    }
}
