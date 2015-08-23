using UnityEngine;
using System.Collections;

public class Landscape : MonoBehaviour {

    private GameObject[] modules;

    void Awake()
    {
        Grid.load = true;
    }

    // Use this for initialization
    void Start ()
    {
        int mods = Grid.getGridSize();
        modules = new GameObject[mods*mods];

        for(int i = 0; i < mods*mods; i++)
        {
            this.modules[i] = new GameObject();
            this.modules[i].name = "Module" + (i+1);
            this.modules[i].AddComponent<Module>();
        }    
	}
	
    public GameObject[] getModules()
    {
        return this.modules;
    }

	// Update is called once per frame
	void Update ()
    {
	
	}
}
