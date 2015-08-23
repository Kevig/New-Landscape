using UnityEngine;
using System.Collections;

public class ClickData
{
    public GameObject obj;
    public Vector3 vec3;
	
    public ClickData()
    {
        
    }

    public ClickData(GameObject o, Vector3 v)
    {
        this.obj = o;
        this.vec3 = v;
    }
}
