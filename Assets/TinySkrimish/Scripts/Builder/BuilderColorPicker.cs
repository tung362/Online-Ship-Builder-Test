using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderColorPicker : MonoBehaviour
{
    /*Settings*/
    public Slider BaseR;
    public Slider BaseG;
    public Slider BaseB;
    public Slider BaseER;
    public Slider BaseEG;
    public Slider BaseEB;

    public Slider TrimR;
    public Slider TrimG;
    public Slider TrimB;
    public Slider TrimER;
    public Slider TrimEG;
    public Slider TrimEB;

    public Slider EngineR;
    public Slider EngineG;
    public Slider EngineB;
    public Slider EngineER;
    public Slider EngineEG;
    public Slider EngineEB;

    public Slider ShieldR;
    public Slider ShieldG;
    public Slider ShieldB;

    //Data
    private bool ColorChanged = false;

    /*Required Components*/
    private BuilderColorTracker TheBuilderColorTracker;

    void Start ()
    {
        TheBuilderColorTracker = GameObject.FindGameObjectWithTag("BuilderColorTracker").GetComponent<BuilderColorTracker>();
        //Set color values to sliders
        SetStartingColors();
    }

    void Update()
    {
        if (ColorChanged) UpdateColors();
    }

    //Prevents spamming
    public void NotifyColorChanged()
    {
        ColorChanged = true;
    }

    void SetStartingColors()
    {
        BaseR.value = TheBuilderColorTracker.BaseBaseColor.r;
        BaseG.value = TheBuilderColorTracker.BaseBaseColor.g;
        BaseB.value = TheBuilderColorTracker.BaseBaseColor.b;
        BaseER.value = TheBuilderColorTracker.BaseEmissionColor.r;
        BaseEG.value = TheBuilderColorTracker.BaseEmissionColor.g;
        BaseEB.value = TheBuilderColorTracker.BaseEmissionColor.b;

        TrimR.value = TheBuilderColorTracker.TrimBaseColor.r;
        TrimG.value = TheBuilderColorTracker.TrimBaseColor.g;
        TrimB.value = TheBuilderColorTracker.TrimBaseColor.b;
        TrimER.value = TheBuilderColorTracker.TrimEmissionColor.r;
        TrimEG.value = TheBuilderColorTracker.TrimEmissionColor.g;
        TrimEB.value = TheBuilderColorTracker.TrimEmissionColor.b;

        EngineR.value = TheBuilderColorTracker.EngineBaseColor.r;
        EngineG.value = TheBuilderColorTracker.EngineBaseColor.g;
        EngineB.value = TheBuilderColorTracker.EngineBaseColor.b;
        EngineER.value = TheBuilderColorTracker.EngineEmissionColor.r;
        EngineEG.value = TheBuilderColorTracker.EngineEmissionColor.g;
        EngineEB.value = TheBuilderColorTracker.EngineEmissionColor.b;

        ShieldR.value = TheBuilderColorTracker.ShieldColor.r;
        ShieldG.value = TheBuilderColorTracker.ShieldColor.g;
        ShieldB.value = TheBuilderColorTracker.ShieldColor.b;

        ColorChanged = true;
    }

    void UpdateColors()
    {
        //Set colors to the values of the slider
        TheBuilderColorTracker.BaseBaseColor = new Color(BaseR.value, BaseG.value, BaseB.value);
        TheBuilderColorTracker.BaseEmissionColor = new Color(BaseER.value, BaseEG.value, BaseEB.value);

        TheBuilderColorTracker.TrimBaseColor = new Color(TrimR.value, TrimG.value, TrimB.value);
        TheBuilderColorTracker.TrimEmissionColor = new Color(TrimER.value, TrimEG.value, TrimEB.value);

        TheBuilderColorTracker.EngineBaseColor = new Color(EngineR.value, EngineG.value, EngineB.value);
        TheBuilderColorTracker.EngineEmissionColor = new Color(EngineER.value, EngineEG.value, EngineEB.value);

        TheBuilderColorTracker.ShieldColor = new Color(ShieldR.value, ShieldG.value, ShieldB.value);

        //Finds every color change script and updates
        BuilderColorChange[] theBuilderColorChanges = FindObjectsOfType<BuilderColorChange>();
        for (int i = 0; i < theBuilderColorChanges.Length; i++) theBuilderColorChanges[i].UpdateColor();

        ColorChanged = false;
    }

    public void SaveColor()
    {
        TheBuilderColorTracker.SaveColor();
    }

    public void LoadColor()
    {
        TheBuilderColorTracker.LoadColor();
        SetStartingColors();
    }
}
