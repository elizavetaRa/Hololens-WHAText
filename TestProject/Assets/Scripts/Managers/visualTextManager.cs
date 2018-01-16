
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;

public class VisualTextManager : Singleton<VisualTextManager>
{

    public GameObject textArea;
    public GameObject LineRenderer;
    private GazeManager gazeManager;
    private GameObject currentlyFocused;
    private GameObject dummy;
    private FocusManager focusManager;
    // Use this for initialization
    void Start()
    {
        gazeManager = this.gameObject.GetComponentInChildren<GazeManager>();
        focusManager = this.gameObject.GetComponentInChildren<FocusManager>();
        dummy = new GameObject();
        currentlyFocused = dummy;
        //gazeManager.FocusedObjectChanged += new GazeManager.FocusedChangedDelegate(focusChanged);
        focusManager.PointerSpecificFocusChanged += new FocusManager.PointerSpecificFocusChangedMethod(focusChanged);
        //visualizeText(dummy);
    }


    // Update is called once per frame
    void Update()
    {
        //GameObject focusedObject = gazeManager.HitObject;
        //if (focusedObject != null && focusedObject.tag != null && focusedObject.tag == "textArea" && focusedObject != currentlyFocused)
        //{
        //   // Debug.Log("found!");
        //    focusTextArea(focusedObject);
        //}

    }

    //internal void focusTextArea(GameObject focusedObject)
    //{
    //    currentlyFocused = focusedObject;
    //    focusedObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
    //}
    //internal void deFocusTextArea(GameObject deFocusedObject)
    //{
    //    //Debug.Log("call");
    //    currentlyFocused = dummy;
    //    deFocusedObject.SendMessageUpwards("OnDefocus", SendMessageOptions.DontRequireReceiver);
    //}

    static void focusChanged(IPointingSource pointer, GameObject oldObject, GameObject newObject)
    {
        if(oldObject != null)
        {

            oldObject.SendMessageUpwards("OnDefocus", SendMessageOptions.DontRequireReceiver);
        }
        if (newObject != null)
        {
            newObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
        }
        //newObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
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
        var ocrResult = cameraPositionResult.ocrResult; //new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));
        var headPosition = Camera.main.transform.position;
        var textX = ocrResult.BoundingBox.x;
        var textY = ocrResult.BoundingBox.y;
        var textWidth = ocrResult.BoundingBox.width;
        var textHeight = ocrResult.BoundingBox.height;
        //var gazeDirection = Camera.main.transform.forward;

        Vector3[] WorldSpaceCenter = convert2DtoWorld(textX + (textWidth / 2), textY + (textHeight / 2), ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopLeft = convert2DtoWorld(textX , textY , ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopRight = convert2DtoWorld(textX + textWidth, textY + (textHeight / 2), ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotLeft = convert2DtoWorld(textX , textY + textHeight , ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);

        //var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        //var Projection = cameraPositionResult.projectionMatrix;


        //Vector2 ImagePosZeroToOne = new Vector2(ocrResult.BoundingBox.x / ImageWidth, 1.0f - (ocrResult.BoundingBox.y / ImageHeight));
        //Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        //Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        ////Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space



        RaycastHit hitCenter, hitTopLeft, hitTopRight, hitBotLeft;
        if (Physics.Raycast(headPosition, WorldSpaceCenter[1], out hitCenter) && Physics.Raycast(headPosition, WorldSpaceTopLeft[1], out hitTopLeft) && Physics.Raycast(headPosition, WorldSpaceTopRight[1], out hitTopRight) && Physics.Raycast(headPosition, WorldSpaceBotLeft[1], out hitBotLeft))
        {
            Debug.Log("Raycasts hit!");

            var drawer = this.LineRenderer.GetComponent<LineRenderer>();
            drawer.SetPositions(new[] { headPosition, hitCenter.point });
            drawer.SetPositions(new[] { headPosition, hitTopLeft.point });
            drawer.SetPositions(new[] { headPosition, hitTopRight.point });
            drawer.SetPositions(new[] { headPosition, hitBotLeft.point });

            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            Debug.Log("center: " + hitCenter.point + "; TopLeft: " + hitTopLeft.point + "; TopRight: " + hitTopRight.point + "; BotLeft: " + hitBotLeft.point);
            //set Text
            visualText.text = ocrResult.Text;

            //set Position
            newArea.transform.position = hitCenter.point;

            //set Rotation
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;
            //Debug.Log(ocrResult.BoundingBox);

            //set Size
            Bounds textAreaBox = newArea.GetComponent<BoxCollider>().bounds;   //.transform.Find("textAreaBox").gameObject.GetComponent<SpriteRenderer>().bounds;
     
            //theSprite.OverrideGeometry();
            float targetWidth = (hitTopLeft.point - hitTopRight.point).magnitude;
            float targetHeight = (hitTopLeft.point - hitBotLeft.point).magnitude;
            float currentWidth = textAreaBox.size.x;
            float currentHeight = textAreaBox.size.y;
            float scaleWidth = targetWidth / currentWidth;
            float scaleHeight = targetHeight / currentHeight;
            Vector3 scale = newArea.transform.localScale;
            Debug.Log("targetwidth " + targetWidth + " ; targetHeight " + targetHeight);
            Debug.Log("currentWidth " + currentWidth + " ; currentHeight " + currentHeight);
            Debug.Log("scaleWidth " + scaleWidth + " ; scaleHeight " + scaleHeight);
            //Debug.Log("scale.x " + scale.x + " ; scale.y " + scale.y);
            scale.x = scaleWidth * scale.x;
            scale.y = scaleHeight * scale.y;
            //newArea.transform.localScale = scale;

            //Debug.Log("scale.x after: " + scale.x + " ; scale.y after: " + scale.y);
        }
    }


    public Vector3[] convert2DtoWorld(float x, float y, float imageWidth, float imageHeight, Matrix4x4 CameraToWorld, Matrix4x4 Projection )
    {
        Vector3[] result = new Vector3[2];

        //float ImageWidth = Camera.main.pixelWidth;
        //float ImageHeight = Camera.main.pixelHeight;
       // var ocrResult = cameraPositionResult.ocrResult; //new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));
        //var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        //var Projection = cameraPositionResult.projectionMatrix;

        Vector2 ImagePosZeroToOne = new Vector2(x / imageWidth, 1.0f - (y / imageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space
        result[0] = WorldSpaceRayPoint1;
        result[1] = WorldSpaceRayPoint2;

        return result;
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



