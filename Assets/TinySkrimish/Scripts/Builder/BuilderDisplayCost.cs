using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuilderDisplayCost : MonoBehaviour
{
    /*Settings*/
    public GameObject TheShipRoot;

    /*Required Components*/
    private Text TheText;

    void Start ()
    {
        TheText = GetComponent<Text>();
    }
	
	void Update ()
    {
        TheText.text = TheShipRoot.GetComponent<BuilderRoot>().Cost.ToString();
    }
}
