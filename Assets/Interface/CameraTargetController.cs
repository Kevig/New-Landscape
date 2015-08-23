using UnityEngine;
using System.Collections;

public class CameraTargetController : MonoBehaviour {

    public void moveX(float x)
    {
        transform.position += (transform.right * x) * Values.camMoveStep;
    }

    public void moveY(float y)
    {
        transform.position += (transform.forward * y) * Values.camMoveStep;
    }
}
