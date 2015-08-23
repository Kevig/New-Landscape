using UnityEngine;

public class CameraTargetController : MonoBehaviour {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    public void moveX(float x)
    {
        transform.position += (transform.right * x) * Values.camMoveStep;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="y"></param>
    public void moveY(float y)
    {
        transform.position += (transform.forward * y) * Values.camMoveStep;
    }
}
