using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMeshCombine : TungDoesMathForYou
{
    /*Settings*/
    public bool CombineOnStart = false;
    public List<Material> materials = new List<Material>();

    /*Data*/
    private List<GameObject> AllChunks = new List<GameObject>();

    void Start()
    {
        if (CombineOnStart) CombineMeshes();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.V)) CombineMeshes();
    }

    void CombineMeshes()
    {
        //Get all child mesh filters
        MeshFilter[] childFilters = FindAllMeshFilters(transform).ToArray();

        List<Mesh> subMesh = new List<Mesh>();
        List<CombineInstance> combiners = new List<CombineInstance>();

        //Get all mesh with each material
        for (int materialID = 0; materialID < materials.Count; materialID++)
        {
            for (int i = 0; i < childFilters.Length; i++)
            {
                //If no ignore script attached
                if (childFilters[i].GetComponent<IgnoreCombine>() == null)
                {
                    //Disable rendering
                    childFilters[i].GetComponent<MeshRenderer>().enabled = false;

                    for (int j = 0; j < childFilters[i].GetComponent<MeshRenderer>().materials.Length; j++)
                    {
                        if (childFilters[i].GetComponent<MeshRenderer>().materials[j].name.Contains(materials[materialID].name))
                        {
                            CombineInstance combine = new CombineInstance();
                            //foundMaterial = true;
                            combine.mesh = childFilters[i].mesh;
                            combine.subMeshIndex = j;
                            combine.transform = childFilters[i].transform.localToWorldMatrix;
                            combiners.Add(combine);
                            break;
                        }
                    }
                }
            }
        }

        //Combine into a mesh
        for(int i = 0; i < combiners.Count; i++)
        {
            Mesh mesh = new Mesh();
            List<CombineInstance> tempCombiners = new List<CombineInstance>();
            tempCombiners.Add(combiners[i]);
            mesh.CombineMeshes(tempCombiners.ToArray(), true);
            subMesh.Add(mesh);
        }


        ////Final combine
        //List<CombineInstance> finalCombiners = new List<CombineInstance>();
        ////Loop through each sub mesh
        //for (int i = 0; i < subMesh.Count; i++)
        //{
        //    CombineInstance combine = new CombineInstance();
        //    combine.mesh = subMesh[i];
        //    combine.subMeshIndex = 0;
        //    combine.transform = transform.worldToLocalMatrix;
        //    finalCombiners.Add(combine);
        //}

        ////New empty object
        //GameObject finalChunk = new GameObject();
        //finalChunk.AddComponent<IgnoreCombine>();
        //finalChunk.AddComponent<MeshFilter>();
        //finalChunk.AddComponent<MeshRenderer>();
        //finalChunk.transform.parent = transform;
        //finalChunk.transform.localPosition = Vector3.zero;
        //finalChunk.transform.localEulerAngles = Vector3.zero;

        ////Apply mesh
        //Mesh finalSplitedMesh = new Mesh();
        //finalSplitedMesh.CombineMeshes(finalCombiners.ToArray(), false);
        //finalChunk.GetComponent<MeshFilter>().mesh = finalSplitedMesh;
        ////finalChunk.GetComponent<MeshRenderer>().materials = finalTempMaterialList.ToArray();
        //AllChunks.Add(finalChunk);
    }
}
