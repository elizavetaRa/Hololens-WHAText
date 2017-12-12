using UnityEngine;
using System.Collections;

/// <summary>
/// Storage for camera position, result and text object of last 10 screenshots
/// </summary>
public struct CameraPositionResult
{
    // Matrix that transforms from camera space to world space.
    //Use this to calculate where in the world a specific camera space point is
    public Vector3 cameraPosition;
    public Quaternion cameraRotation;

    public Matrix4x4 projectionMatrix;

    //result of recognition with screenshot
    public OcrResult ocrResult;

    // appropriate text object

}
