using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUITextField : MonoBehaviour
{
    public void setText(string text)
    {
        this.GetComponent<Text>().text = text;
    }
}
