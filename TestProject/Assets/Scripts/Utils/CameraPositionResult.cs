using UnityEngine;
using System.Collections;

/// <summary>
/// Storage for camera position, result and text object of last 10 screenshots
/// </summary>
public class CameraPositionResult
{
    public int id { get; set; }

    public Matrix4x4 projectionMatrix { get; set; }
    public Matrix4x4 cameraToWorldMatrix { get; set; }

    //result of recognition with screenshot
    public OcrResult ocrResult { get; set; }

    // appropriate text object

}
