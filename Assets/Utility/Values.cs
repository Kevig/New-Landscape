using UnityEngine;
using System.Collections;

public static class Values
{
    // Mesh Values
    public static float yStep = 0.2f;
    public static float absoluteFloor = -(yStep * 100);

    // Camera Values
    public static float camSensitivity = 2f;
    public static float camMoveStep = 0.2f;
    public static float camZoomStep = 1f;
    public static float camTargetHeight = 1f;
    public static float minCamDist = 1f;
    public static float maxCamDist = 100f;
    
}
