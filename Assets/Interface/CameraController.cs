using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

    private GameObject target;
    private GameObject posTarget;

    private float x;
    private float y;


    // Use this for initialization
    void Start()
    {        
        this.initTarget();
        this.initCamera();
    }
    
    // Initialise target
    private void initTarget()
    {
        this.target = new GameObject();
        this.target.name = "CameraTarget";
        this.target.transform.position = new Vector3(this.target.transform.position.x,
                                                     this.getHeight() + Values.camTargetHeight,
                                                     this.target.transform.position.z);

        this.posTarget = new GameObject();
        this.posTarget.name = "CameraPosTarget";
        this.posTarget.transform.position = new Vector3(this.target.transform.position.x,
                                                     this.getHeight() + Values.camTargetHeight,
                                                     this.target.transform.position.z);

        this.posTarget.AddComponent<CameraTargetController>();
        this.target.transform.parent = this.posTarget.transform;
    }

    // Initialise camera relative to target
    private void initCamera()
    {
        this.transform.position = new Vector3(target.transform.position.x,
                                              target.transform.position.y + 10f,
                                              target.transform.position.z - this.getDistance());

        this.transform.LookAt(this.target.transform);
        this.transform.parent = this.target.transform;
    }

    public CameraTargetController getCamTarget()
    {
        return this.posTarget.GetComponent<CameraTargetController>();
    }

    // Pre-condtion: requires value -1 or 1
    // Reduce / Increase distance between camera and target
    public void modifyZoom(int n)
    {
        float dist = this.getDistance();
        if(n < 0 && dist >= Values.maxCamDist || n > 0 && dist <= Values.minCamDist)
        {
            n = 0;
        }
        this.transform.position = Vector3.MoveTowards(this.transform.position, 
                                                      this.target.transform.position, 
                                                      (n * Values.camZoomStep));
    }

    // Modify camera's rotation
    public void modifyRotation()
    {
        this.x += Input.GetAxis("Mouse X") * Values.camSensitivity;
        this.y -= Input.GetAxis("Mouse Y") * Values.camSensitivity;

        this.posTarget.transform.rotation = Quaternion.Euler(0.0f, this.x, 0.0f);
        this.target.transform.rotation = Quaternion.Euler(this.y, this.x, 0.0f);
    }

    // Raycast -y axis from target to landscape collider to keep target objects height above the landscape
    private float getHeight()
    {
        RaycastHit hit;
        Vector3 hitPoint = new Vector3();

        if(Physics.Raycast(this.target.transform.position, Vector3.down, out hit))
        {
            hitPoint = hit.point;
        }
        return hitPoint.y;
    }

    // Returns the distance between the Vector3 world coordinates of this object and the target object
    private float getDistance()
    {
        return Vector3.Distance(this.transform.position, this.target.transform.position);
    }

}
