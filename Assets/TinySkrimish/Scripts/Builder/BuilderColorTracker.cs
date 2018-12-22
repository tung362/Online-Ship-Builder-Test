using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Tracks colors to use when building ships offline, also manage save and load of color
public class BuilderColorTracker : MonoBehaviour
{
    /*Color change (offline only)*/
    [Header("Player Colors")]
    public Color BaseBaseColor;
    public Color BaseEmissionColor;
    public Color TrimBaseColor;
    public Color TrimEmissionColor;
    public Color EngineBaseColor;
    public Color EngineEmissionColor;
    public Color ShieldColor;

    [Header("Default Colors")]
    public Color DefaultBaseBaseColor;
    public Color DefaultBaseEmissionColor;
    public Color DefaultTrimBaseColor;
    public Color DefaultTrimEmissionColor;
    public Color DefaultEngineBaseColor;
    public Color DefaultEngineEmissionColor;
    public Color DefaultShieldColor;

    [Header("Neutral Colors")]
    public Color NeutralBaseBaseColor;
    public Color NeutralBaseEmissionColor;
    public Color NeutralTrimBaseColor;
    public Color NeutralTrimEmissionColor;
    public Color NeutralEngineBaseColor;
    public Color NeutralEngineEmissionColor;
    public Color NeutralShieldColor;

    void Awake()
    {
        //If theres a builder setting that already exists, destroy it
        BuilderColorTracker[] managers = FindObjectsOfType<BuilderColorTracker>();
        for (int i = 0; i < managers.Length; i++)
        {
            if (managers[i].gameObject != gameObject) DestroyImmediate(managers[i].gameObject);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //Creates the new save folder if there isnt one already
        Directory.CreateDirectory(Application.dataPath + "/../" + "/ColorPreference");
        Directory.CreateDirectory(Application.dataPath + "/../" + "/StationData");
        //Loads a color if theres a prefernce
        LoadColor();
    }

    public void SaveColor()
    {
        BinaryFormatter formater = new BinaryFormatter();
        //Create file
        FileStream stream = new FileStream(Application.dataPath + "/../" + "/ColorPreference" + "/ColorPreference", FileMode.Create);

        //The whole ship's information
        ColorTrackerData theColorTrackerData = new ColorTrackerData(this);

        //Write to file
        formater.Serialize(stream, theColorTrackerData);
        stream.Close();
    }

    public void LoadColor()
    {
        //Checks if the file exists
        if (File.Exists(Application.dataPath + "/../" + "/ColorPreference" + "/ColorPreference"))
        {
            BinaryFormatter formater = new BinaryFormatter();
            //Open file
            FileStream stream = new FileStream(Application.dataPath + "/../" + "/ColorPreference" + "/ColorPreference", FileMode.Open);

            //Obtain the saved data
            ColorTrackerData theColorTrackerData = formater.Deserialize(stream) as ColorTrackerData;
            stream.Close();

            BaseBaseColor = new Color(theColorTrackerData.BaseBaseColor[0], theColorTrackerData.BaseBaseColor[1], theColorTrackerData.BaseBaseColor[2], theColorTrackerData.BaseBaseColor[3]);
            BaseEmissionColor = new Color(theColorTrackerData.BaseEmissionColor[0], theColorTrackerData.BaseEmissionColor[1], theColorTrackerData.BaseEmissionColor[2], theColorTrackerData.BaseEmissionColor[3]);

            TrimBaseColor = new Color(theColorTrackerData.TrimBaseColor[0], theColorTrackerData.TrimBaseColor[1], theColorTrackerData.TrimBaseColor[2], theColorTrackerData.TrimBaseColor[3]);
            TrimEmissionColor = new Color(theColorTrackerData.TrimEmissionColor[0], theColorTrackerData.TrimEmissionColor[1], theColorTrackerData.TrimEmissionColor[2], theColorTrackerData.TrimEmissionColor[3]);

            EngineBaseColor = new Color(theColorTrackerData.EngineBaseColor[0], theColorTrackerData.EngineBaseColor[1], theColorTrackerData.EngineBaseColor[2], theColorTrackerData.EngineBaseColor[3]);
            EngineEmissionColor = new Color(theColorTrackerData.EngineEmissionColor[0], theColorTrackerData.EngineEmissionColor[1], theColorTrackerData.EngineEmissionColor[2], theColorTrackerData.EngineEmissionColor[3]);

            ShieldColor = new Color(theColorTrackerData.ShieldColor[0], theColorTrackerData.ShieldColor[1], theColorTrackerData.ShieldColor[2], theColorTrackerData.ShieldColor[3]);
        }
    }

    //The binary converted version of the color tracker
    [System.Serializable]
    public class ColorTrackerData
    {
        public float[] BaseBaseColor;
        public float[] BaseEmissionColor;
        public float[] TrimBaseColor;
        public float[] TrimEmissionColor;
        public float[] EngineBaseColor;
        public float[] EngineEmissionColor;
        public float[] ShieldColor;

        public ColorTrackerData(BuilderColorTracker TheTracker)
        {
            BaseBaseColor = new float[4];
            BaseEmissionColor = new float[4];
            TrimBaseColor = new float[4];
            TrimEmissionColor = new float[4];
            EngineBaseColor = new float[4];
            EngineEmissionColor = new float[4];
            ShieldColor = new float[4];

            BaseBaseColor[0] = TheTracker.BaseBaseColor.r;
            BaseBaseColor[1] = TheTracker.BaseBaseColor.g;
            BaseBaseColor[2] = TheTracker.BaseBaseColor.b;
            BaseBaseColor[3] = TheTracker.BaseBaseColor.a;

            BaseEmissionColor[0] = TheTracker.BaseEmissionColor.r;
            BaseEmissionColor[1] = TheTracker.BaseEmissionColor.g;
            BaseEmissionColor[2] = TheTracker.BaseEmissionColor.b;
            BaseEmissionColor[3] = TheTracker.BaseEmissionColor.a;

            TrimBaseColor[0] = TheTracker.TrimBaseColor.r;
            TrimBaseColor[1] = TheTracker.TrimBaseColor.g;
            TrimBaseColor[2] = TheTracker.TrimBaseColor.b;
            TrimBaseColor[3] = TheTracker.TrimBaseColor.a;

            TrimEmissionColor[0] = TheTracker.TrimEmissionColor.r;
            TrimEmissionColor[1] = TheTracker.TrimEmissionColor.g;
            TrimEmissionColor[2] = TheTracker.TrimEmissionColor.b;
            TrimEmissionColor[3] = TheTracker.TrimEmissionColor.a;

            EngineBaseColor[0] = TheTracker.EngineBaseColor.r;
            EngineBaseColor[1] = TheTracker.EngineBaseColor.g;
            EngineBaseColor[2] = TheTracker.EngineBaseColor.b;
            EngineBaseColor[3] = TheTracker.EngineBaseColor.a;

            EngineEmissionColor[0] = TheTracker.EngineEmissionColor.r;
            EngineEmissionColor[1] = TheTracker.EngineEmissionColor.g;
            EngineEmissionColor[2] = TheTracker.EngineEmissionColor.b;
            EngineEmissionColor[3] = TheTracker.EngineEmissionColor.a;

            ShieldColor[0] = TheTracker.ShieldColor.r;
            ShieldColor[1] = TheTracker.ShieldColor.g;
            ShieldColor[2] = TheTracker.ShieldColor.b;
            ShieldColor[3] = TheTracker.ShieldColor.a;
        }
    }
}
