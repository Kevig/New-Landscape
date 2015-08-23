using UnityEngine;

/// <summary>
/// Camera Control Script
/// A Camera consists of a target Object, used as a rotation pivot point and
/// a posTarget for movement around the landscape
/// </summary>
public class CameraController : MonoBehaviour
{
    private GameObject target;
    private GameObject posTarget;

    private float x;
    private float y;

    /// <summary>
    /// Unity initialisation method
    /// </summary>
    void Start()
    {        
        this.initTarget();
        this.initCamera();
    }
    
    /// <summary>
    /// Creates pivot and movement objects
    /// Sets pivot as child of movement object
    /// </summary>
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

    /// <summary>
    /// Sets this objects position and as a child of the pivot object
    /// </summary>
    private void initCamera()
    {
        this.transform.position = new Vector3(target.transform.position.x,
                                              target.transform.position.y + 10f,
                                              target.transform.position.z - this.getDistance());

        this.transform.LookAt(this.target.transform);
        this.transform.parent = this.target.transform;
    }

    /// <summary>
    /// Getter for position object's attached CameraTargetController script
    /// </summary>
    /// <returns></returns>
    public CameraTargetController getCamTarget()
    {
        return this.posTarget.GetComponent<CameraTargetController>();
    }

    /// <summary>
    /// Zoom function
    /// Changes this objects distance between this and its pivot object
    /// </summary>
    /// <param name="n">+1 for zoom in, -1 for zoom out</param>
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

    /// <summary>
    /// Modiy Camera's rotation relative to pivot object
    /// </summary>
    public void modifyRotation()
    {
        this.x += Input.GetAxis("Mouse X") * Values.camSensitivity;
        this.y -= Input.GetAxis("Mouse Y") * Values.camSensitivity;

        this.posTarget.transform.rotation = Quaternion.Euler(0.0f, this.x, 0.0f);
        this.target.transform.rotation = Quaternion.Euler(this.y, this.x, 0.0f);
    }

    /// <summary>
    /// Height check function - TODO: Move to Movement object controller
    /// Raycast -y axis from target to landscape collider to keep target objects 
    /// height above the landscape
    /// </summary>
    /// <returns>A float value of the raycast hit position's y axis coordinate</returns>
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

    /// <summary>
    /// Distance check - TODO: Make generic / utlity
    /// </summary>
    /// <returns>A float value of the distance between this position and targets position</returns>
    private float getDistance()
    {
        return Vector3.Distance(this.transform.position, this.target.transform.position);
    }

}
