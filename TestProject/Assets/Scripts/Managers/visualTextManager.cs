using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VisualTextManager : Singleton<VisualTextManager>
{
    
    public GameObject textArea;
    //public GameObject LineRenderer;
    // Use this for initialization
    void Start()
    {
        //visualizeText(dummy);
    }


    // Update is called once per frame
    void Update()
    {

    }


    //internal void visualizeText(OcrResult ocrResult)
    //{ // visualText.text = dummyText;
    //    var headPosition = Camera.main.transform.position;
    //    var gazeDirection = Camera.main.transform.forward;
    //    var CameraToWorld = Camera.main.cameraToWorldMatrix;
    //    var Projection = Camera.main.projectionMatrix;
    //    float ImageWidth = Camera.main.pixelWidth;
    //    float ImageHeight = Camera.main.pixelHeight;

    //    Vector2 ImagePosZeroToOne = new Vector2(ocrResult.BoundingBox.x / ImageWidth, 1.0f - (ocrResult.BoundingBox.y / ImageHeight));
    //    Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space
    //    Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));
    //    Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1));// camera location in world space
    //    Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space


    //    RaycastHit hitInfo;
    //    if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
    //    {
    //        Debug.Log("Raycast hit!");
    //        GameObject newArea = Instantiate(textArea);
    //        TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
    //        visualText.text = ocrResult.Text;
    //        newArea.transform.position = hitInfo.point; //new Vector3(headPosition.x, headPosition.y, headPosition.z + 3);

    //        Quaternion toQuat = Camera.main.transform.localRotation;
    //        toQuat.x = 0;
    //        toQuat.z = 0;
    //        newArea.transform.rotation = toQuat;
    //    }
    //}


    internal void visualizeText(CameraPositionResult cameraPositionResult)
    {
        float ImageWidth = Camera.main.pixelWidth;
        float ImageHeight = Camera.main.pixelHeight;
        var ocrResult = new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        var Projection = cameraPositionResult.projectionMatrix;


        Vector2 ImagePosZeroToOne = new Vector2(ocrResult.BoundingBox.x / ImageWidth, 1.0f - (ocrResult.BoundingBox.y / ImageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space
        
        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, WorldSpaceRayPoint2, out hitInfo))
        {
            Debug.Log("Raycast hit!");

            // var drawer = this.LineRenderer.GetComponent<LineRenderer>();
            //drawer.SetPositions(new[] { headPosition, hitInfo.point});

            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();

            //set Text
            visualText.text = ocrResult.Text;

            //set Position
            newArea.transform.position = hitInfo.point;
            
            //set Rotation
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;

            //set Size
            Bounds textAreaBox = newArea.transform.Find("textAreaBox").gameObject.GetComponent<SpriteRenderer>().bounds;
            float targetWidth = ocrResult.BoundingBox.width;
            float targetHeight = ocrResult.BoundingBox.height;
            float currentWidth = textAreaBox.size.x;
            float currentHeight = textAreaBox.size.y;
            Vector3 scale = newArea.transform.localScale;
            scale.x = targetWidth * scale.x / currentWidth;
            scale.y = targetHeight * scale.y / currentHeight;
            newArea.transform.localScale = scale;
        }
    }
    public static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
    {
        Vector3 from = new Vector3(0, 0, 0);
        var axsX = proj.GetRow(0);
        var axsY = proj.GetRow(1);
        var axsZ = proj.GetRow(2);
        from.z = to.z / axsZ.z;
        from.y = (to.y - (from.z * axsY.z)) / axsY.y;
        from.x = (to.x - (from.z * axsX.z)) / axsX.x;
        return from;
    }
}



