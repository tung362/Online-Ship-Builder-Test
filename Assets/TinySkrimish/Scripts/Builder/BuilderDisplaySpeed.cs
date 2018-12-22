using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderDisplaySpeed : MonoBehaviour
{
    /*Settings*/
    public GameObject TheShipRoot;
    public Color PositiveColor;
    public Color NegativeColor;

    /*Required Components*/
    private Text TheText;

    void Start()
    {
        TheText = GetComponent<Text>();
    }

    void Update ()
    {
        if (TheShipRoot.GetComponent<BuilderRoot>().Speed < 0) TheText.color = NegativeColor;
        else TheText.color = PositiveColor;

        TheText.text = TheShipRoot.GetComponent<BuilderRoot>().Speed.ToString();
    }
}
