using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderSlotSelect : MonoBehaviour
{
    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    private Image TheImage;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheImage = GetComponent<Image>();
    }

    void Update()
    {
        if (TheBuilderSettings.PlacingNewObject) TheImage.enabled = true;
        else TheImage.enabled = false;
    }
}
