using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderMirrorSelect : MonoBehaviour
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
        if (TheBuilderSettings.Mirror) TheImage.enabled = true;
        else TheImage.enabled = false;
    }
}
