using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//The binary converted version of the ship
[System.Serializable]
public class ShipData
{
    public string ShipName;
    public ShipBlockData[] Parts;

    public ShipData(ShipBlockData[] Blocks, string RootName)
    {
        ShipName = RootName;

        Parts = new ShipBlockData[Blocks.Length];
        for (int i = 0; i < Parts.Length; i++) Parts[i] = Blocks[i];
    }
}

//The binary converted version of the ship block
[System.Serializable]
public class ShipBlockData
{
    public string Name;
    public float[] Position;
    public float[] Rotation;
    public ShipBlockData[] Connections;

    public ShipBlockData(GameObject ShipBlock)
    {
        Name = ShipBlock.name;
        Position = new float[3];
        Rotation = new float[3];
        Connections = new ShipBlockData[ShipBlock.GetComponent<BuilderAdjacentCheck>().AdjacentBlocks.Count];

        //Position
        Position[0] = ShipBlock.transform.localPosition.x;
        Position[1] = ShipBlock.transform.localPosition.y;
        Position[2] = ShipBlock.transform.localPosition.z;

        //Rotation
        Rotation[0] = ShipBlock.transform.localEulerAngles.x;
        Rotation[1] = ShipBlock.transform.localEulerAngles.y;
        Rotation[2] = ShipBlock.transform.localEulerAngles.z;
    }
}

//Saves and loads ship schematics
public class BuilderSaveLoad : MonoBehaviour
{
    /*Settings*/
    public GameObject ShipRoot;
    public InputField NameText;
    //Seperate from ship parts because it is not spawned in
    public GameObject CommandBlockPrefab;
    public List<GameObject> ShipParts = new List<GameObject>();

    /*Data*/
    public List<FileInfo> AllSaves = new List<FileInfo>();

    /*Required Components*/
    private BuilderSettings TheBuilderSettings;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        //Creates the new save folder if there isnt one already
        Directory.CreateDirectory(Application.dataPath + "/../" + "/ShipSaves");
    }

    //Updates how many files there are in the save folder
    public void UpdateFileCount()
    {
        AllSaves.Clear();
        //Looks at the save folder directory
        DirectoryInfo FileInfo = new DirectoryInfo(Application.dataPath + "/../" + "/ShipSaves");
        //Gets all save files
        AllSaves.AddRange(FileInfo.GetFiles());
    }

    //Save ship to binary file
    public void Save()
    {
        BinaryFormatter formater = new BinaryFormatter();
        //Create file
        FileStream stream = new FileStream(Application.dataPath + "/../" + "/ShipSaves" + "/" + TheBuilderSettings.ShipName, FileMode.Create);

        //Get all blocks
        BuilderRoot theBuilderRoot = ShipRoot.GetComponent<BuilderRoot>();
        theBuilderRoot.AdjacentCheck();
        theBuilderRoot.ClearAllRedBlocks();
        List<ShipBlockData> blocks = new List<ShipBlockData>();
        for(int i = 0; i < theBuilderRoot.AllChilds.Count; i++) blocks.Add(new ShipBlockData(theBuilderRoot.AllChilds[i]));
        //Get all block connections
        //Each child
        for (int i = 0; i < theBuilderRoot.AllChilds.Count; i++)
        {
            BuilderAdjacentCheck theBuilderAdjacentCheck = theBuilderRoot.AllChilds[i].GetComponent<BuilderAdjacentCheck>();
            //Each child's adjacent block
            for(int j = 0; j < theBuilderAdjacentCheck.AdjacentBlocks.Count; j++)
            {
                //Loop through each ship child
                for(int k = 0; k < theBuilderRoot.AllChilds.Count; k++)
                {
                    //If there was a match for one of the adjacent blocks
                    if(theBuilderAdjacentCheck.AdjacentBlocks[j] == theBuilderRoot.AllChilds[k]) blocks[i].Connections[j] = blocks[k];
                }
            }
        }

        //The whole ship's information
        ShipData theShipData = new ShipData(blocks.ToArray(), TheBuilderSettings.ShipName);

        //Write to file
        formater.Serialize(stream, theShipData);
        stream.Close();
    }

    //Load ship from a binary file
    public void Load(string FileName)
    {
        //Checks if the file exists
        if(File.Exists(Application.dataPath + "/../" + "/ShipSaves" + "/" + FileName))
        {
            BinaryFormatter formater = new BinaryFormatter();
            //Open file
            FileStream stream = new FileStream(Application.dataPath + "/../" + "/ShipSaves" + "/" + FileName, FileMode.Open);

            //Obtain the saved data
            ShipData theShipData = formater.Deserialize(stream) as ShipData;
            stream.Close();

            //Load
            NameText.text = FileName;
            BuilderRoot theBuilderRoot = ShipRoot.GetComponent<BuilderRoot>();
            //Clear existing ship
            theBuilderRoot.ClearAllBlocks();

            //Create new ship using the saved data
            for(int i = 0; i < theShipData.Parts.Length; i++)
            {
                for(int j = 0; j < ShipParts.Count; j++)
                {
                    if (theShipData.Parts[i].Name == ShipParts[j].name)
                    {
                        GameObject spawnedShipPart = Instantiate(ShipParts[j], Vector3.zero, Quaternion.identity);
                        spawnedShipPart.name = ShipParts[j].name;
                        spawnedShipPart.transform.parent = ShipRoot.transform;
                        spawnedShipPart.transform.localPosition = new Vector3(theShipData.Parts[i].Position[0], theShipData.Parts[i].Position[1], theShipData.Parts[i].Position[2]);
                        spawnedShipPart.transform.localEulerAngles = new Vector3(theShipData.Parts[i].Rotation[0], theShipData.Parts[i].Rotation[1], theShipData.Parts[i].Rotation[2]);
                        theBuilderRoot.AllChilds.Add(spawnedShipPart);
                    }
                }
            }
        }
    }

    //Load ship from a binary file to a save ship and combine
    public void LoadToSaveShip(string FileName, GameObject TheRoot, Text CostText)
    {
        //Checks if the file exists
        if (File.Exists(Application.dataPath + "/../" + "/ShipSaves" + "/" + FileName))
        {
            BinaryFormatter formater = new BinaryFormatter();
            //Open file
            FileStream stream = new FileStream(Application.dataPath + "/../" + "/ShipSaves" + "/" + FileName, FileMode.Open);

            //Obtain the saved data
            ShipData theShipData = formater.Deserialize(stream) as ShipData;
            stream.Close();

            //Load
            BuilderSaveShipRoot theBuilderRoot = TheRoot.GetComponent<BuilderSaveShipRoot>();

            float totalCostPrediction = 0;
            //Create new ship using the saved data
            for (int i = 0; i < theShipData.Parts.Length; i++)
            {
                for (int j = 0; j < ShipParts.Count; j++)
                {
                    if (theShipData.Parts[i].Name == ShipParts[j].name)
                    {
                        GameObject spawnedShipPart = Instantiate(ShipParts[j], Vector3.zero, Quaternion.identity);
                        spawnedShipPart.name = ShipParts[j].name;
                        spawnedShipPart.transform.parent = TheRoot.transform;
                        spawnedShipPart.transform.localPosition = new Vector3(theShipData.Parts[i].Position[0], theShipData.Parts[i].Position[1], theShipData.Parts[i].Position[2]);
                        spawnedShipPart.transform.localEulerAngles = new Vector3(theShipData.Parts[i].Rotation[0], theShipData.Parts[i].Rotation[1], theShipData.Parts[i].Rotation[2]);
                        theBuilderRoot.AllChilds.Add(spawnedShipPart);
                        totalCostPrediction += ShipParts[j].GetComponent<BlockInfo>().Cost;
                    }
                }
            }
            totalCostPrediction += CommandBlockPrefab.GetComponent<BlockInfo>().Cost;

            CostText.text = totalCostPrediction.ToString();
        }
    }

    //Deletes a save file
    public void Delete(string FileName)
    {
        for(int i = 0; i < AllSaves.Count; i++)
        {
            if(AllSaves[i].Name == FileName)
            {
                AllSaves[i].Delete();
                AllSaves.RemoveAt(i);
                break;
            }
        }
    }
}
