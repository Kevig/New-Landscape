using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CursorInterface : MonoBehaviour {

    private KeyboardInterface keyboardInterface;
    private GameObject markerObj;
    private Marker marker;

    private Module currentModule;
    private bool unlockMarkerY = false;

    private CameraController mainCamera;
    private CameraTargetController camTarget;
    private bool unlockCamRotation = false;

    private SelectType selectType;

    private bool isRightClick = false;

	/// <summary>
    /// 
    /// </summary>
	void Start ()
    {
        keyboardInterface = this.gameObject.GetComponent<KeyboardInterface>();
        mainCamera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        this.selectType = SelectType.NONE;
        this.camTarget = null;
	}
	
    /// <summary>
    /// 
    /// </summary>
    /// <param name="i"></param>
    public void setSelectionState(SelectType i)
    {
        if(this.selectType != i)
        {
            this.selectType = i;
            this.destroyMarker();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="b"></param>
    private void cursorVisible(bool b)
    {
        if(b)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
        if(!b)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

	/// <summary>
    /// 
    /// </summary>
	void Update ()
    {

        if(!unlockCamRotation && !unlockMarkerY && !isRightClick)
        {
            cursorVisible(true);
        }

        if(Input.GetMouseButtonDown(0))
        {
            leftClick();
        }
        if(Input.GetMouseButtonUp(0) && unlockMarkerY)
        {
            unlockMarkerY = false;
        }

        if(Input.GetMouseButtonDown(1))
        {
            isRightClick = true;
        }
        if(Input.GetMouseButtonUp(1))
        {
            isRightClick = false;
        }
        if(Input.GetMouseButtonUp(1) && unlockCamRotation)
        {
            unlockCamRotation = false;
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCamera.modifyZoom(-1);
        }

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCamera.modifyZoom(1);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if(isRightClick)
        {
            rightClick();
            cursorVisible(false);
        }

        if(unlockCamRotation || unlockMarkerY)
        {
            cursorVisible(false);
        }
        if(unlockCamRotation)
        {
            mainCamera.modifyRotation();
        }


        if(unlockMarkerY && this.selectType == SelectType.POINT)
        {
            changeMarkerY();
        }

        if(unlockMarkerY && this.selectType == SelectType.SQUARE)
        {
            changeSquareMarkerY();
        }
    }

    // Left click event handling method
    private void leftClick()
    {
        // Get raycast information as a ClickData object
        ClickData clickData = getClickInfo(get2dCoords());

        // String switch controller
        string n = "";
        
        // If no object hit by raycast:
        // halt method execution to avoid null exception
        if(clickData.obj == null)
        {
            return;    
        }

        // Possible object cases
        if(clickData.obj.name.Contains("Module"))
        {
            n = "Module";
        }
        if(clickData.obj.name.Contains("Marker"))
        {
            n = "Marker";
        }
        
        // Event handling for left click events
        switch(n)
        {

            case "Module":
                this.currentModule = clickData.obj.GetComponent<Module>();
                this.destroyMarker();

                Vector3[] points;

                switch(this.selectType)
                {
                    case SelectType.POINT:
                        points = new Vector3[] { this.currentModule.getSnapPoint(clickData.vec3) };
                        this.markerObj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        this.marker = this.markerObj.AddComponent<Marker>();
                        this.marker.initObj(1, points);
                        break;

                    case SelectType.SQUARE:
                        points = this.currentModule.getSquarePoints(clickData.vec3);
                        this.markerObj = new GameObject();
                        this.marker = this.markerObj.AddComponent<Marker>();
                        this.marker.initObj(2, points);
                        break;
                }
                break;

            case "Marker":
                this.unlockMarkerY = true;
                break;
        }
    }

    // Right click event handling method
    private void rightClick()
    {

        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        switch(this.keyboardInterface.getModifier())
        {
            case Modifier.NONE:
                // Passive actions
                this.unlockCamRotation = true;
                break;

            case Modifier.SHIFT:

                if(this.camTarget == null)
                {
                    this.camTarget = this.mainCamera.getCamTarget();
                    break;
                }

                if(x != 0)
                {
                    this.camTarget.moveX(x);
                }

                if(y != 0)
                {
                    this.camTarget.moveY(y);
                }
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void destroyMarker()
    {
        GameObject m = GameObject.Find("Marker");
        if(m != null)
        {
            Destroy(m);
        }
    }

    // Allows changing of the markers y axis position. modify's the landscape to the
    // same y position as the marker
    private void changeMarkerY()
    {
        if(this.currentModule != null)
        {
            float n = Input.GetAxis("Mouse Y") * Values.yStep;
            Vector3 point = this.marker.getPosition();
            this.marker.setPosition(new Vector3(point.x, point.y + n, point.z));

            foreach(Module m in this.getSearchModules())
            {
                m.modifyVert(point, n); // Modify Mesh
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void changeSquareMarkerY()
    {
        if(this.currentModule != null)
        {
            float n = Input.GetAxis("Mouse Y") * Values.yStep;
            
            //this.marker.setPosition(new Vector3(point.x, point.y + n, point.z));

            foreach(Module m in this.getSearchModules())
            {
                foreach(Vector3 v in this.marker.getMeshVerts())
                {
                    m.modifyVert(v, n); // Modify Mesh
                }
            }
            this.marker.modifyMeshY(n);
        }
    }

    // Build a list of the 8 modules that could be neighbours of the current module
    // to reduce searching. TODO: RETHINK!!! Edges can be calculated to reduce searches...
    // IE. Get array index from current module search, if an edge vert search only required modules
    private List<Module> getSearchModules()
    {
        int i = this.currentModule.getIndex();
        int zStep = Grid.getGridSize();

        List<Module> modules = new List<Module>();
        modules.Add(this.currentModule);
        try{modules.Add(GameObject.Find("Module" + (i - 1)).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i - zStep)).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i - (zStep - 1))).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i - (zStep + 1))).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i + 1)).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i + zStep)).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i + (zStep-1))).GetComponent<Module>());}
        catch(Exception) { }
        try{modules.Add(GameObject.Find("Module" + (i + (zStep+1))).GetComponent<Module>());}
        catch(Exception) { }
        return modules;
    }

    // Get screen x,y cursor coords
    private Vector3 get2dCoords()
    {
        return Input.mousePosition;
    }

    // Get Click information
    private ClickData getClickInfo(Vector3 pos2d)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos2d);
        RaycastHit hit;

        ClickData clickData = new ClickData();

        if(Physics.Raycast(ray, out hit))
        {
            clickData.vec3 = hit.point;
            clickData.obj = hit.collider.gameObject;
        }
        return clickData;
    }
}
