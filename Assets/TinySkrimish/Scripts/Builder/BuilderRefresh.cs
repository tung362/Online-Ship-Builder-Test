using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderRefresh : MonoBehaviour
{
    /*Settings*/
    public BuilderLoaderTracker TheBuilderLoaderTracker;

    /*Required Components*/
    private BuilderSaveLoad TheBuilderSaveLoad;

    void Start()
    {
        TheBuilderSaveLoad = FindObjectOfType<BuilderSaveLoad>();
    }

    public void RefreshLoader()
    {
        TheBuilderSaveLoad = FindObjectOfType<BuilderSaveLoad>();
        TheBuilderSaveLoad.UpdateFileCount();
        TheBuilderLoaderTracker.UpdateSlots();
    }
}
