using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChanger : MonoBehaviour
{
    public void PointeRed()
    {
        GetComponent<Image>().color = Color.red;
    }

    public void PainteGreen()
    {
        GetComponent<Image>().color = Color.green;
    }

    public void PaintOnBool(bool value)
    {
        if (value)
        {
            PainteGreen();
        }
        else
        {
            PointeRed();
        }
    }
}
