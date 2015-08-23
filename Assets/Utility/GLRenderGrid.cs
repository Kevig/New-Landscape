using System.Threading;
using UnityEngine;

public class GLRenderGrid : MonoBehaviour
{
    public delegate void updateGridLines();
    public static event updateGridLines OnRenderLines;

    KeyboardInterface keyInterface;

    void Start()
    {
        keyInterface = GameObject.Find("Core").GetComponent<KeyboardInterface>();
    }

    void OnPostRender()
    {
        if(keyInterface.renderGrid)
        {
            OnRenderLines();
        }
    }
}
