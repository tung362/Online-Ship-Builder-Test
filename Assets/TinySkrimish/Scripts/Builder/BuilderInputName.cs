using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderInputName : MonoBehaviour
{
    /*Required Components*/
    private BuilderSettings TheBuilderSettings;
    private InputField TheInputField;

    void Start()
    {
        TheBuilderSettings = FindObjectOfType<BuilderSettings>();
        TheInputField = GetComponent<InputField>();

        TheInputField.text = TheBuilderSettings.ShipName;
    }

    public void UpdateShipName()
    {
        TheBuilderSettings.ShipName = TheInputField.text;
    }
}
