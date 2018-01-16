
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;

public class VisualTextManager : Singleton<VisualTextManager>
{

    public GameObject textArea;
   // public GameObject LineRenderer;
    private GazeManager gazeManager;
    private GameObject currentlyFocused;
    private GameObject dummy;
    private FocusManager focusManager;
    private LineRenderer lineRenderer;
    // Use this for initialization
    void Start()
    {
        gazeManager = this.gameObject.GetComponentInChildren<GazeManager>();
        focusManager = this.gameObject.GetComponentInChildren<FocusManager>();
        dummy = new GameObject();
        this.lineRenderer = this.gameObject.GetComponent<LineRenderer>();
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
        Debug.Log(ocrResult.BoundingBox);
        //Debug.Log("textWidth: " + textWidth + "; textHeight: " + textHeight);
        Vector3[] WorldSpaceCenter = convert2DtoWorld(textX + (textWidth / 2), textY + (textHeight / 2), ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopLeft = convert2DtoWorld(textX , textY , ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopRight = convert2DtoWorld(textX + textWidth, textY, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotLeft = convert2DtoWorld(textX, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotRight = convert2DtoWorld(textX + textWidth, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        var Projection = cameraPositionResult.projectionMatrix;


        Vector2 ImagePosZeroToOne = new Vector2((ocrResult.BoundingBox.x + textWidth) / ImageWidth, 1.0f - ((ocrResult.BoundingBox.y+textHeight) / ImageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space



        RaycastHit hitCenter, hitTopLeft, hitTopRight, hitBotLeft, hitBotRight;
        if (Physics.Raycast(WorldSpaceRayPoint1, WorldSpaceRayPoint2, out hitCenter) && Physics.Raycast(headPosition, WorldSpaceTopLeft[1], out hitTopLeft) && Physics.Raycast(headPosition, WorldSpaceTopRight[1], out hitTopRight) && Physics.Raycast(headPosition, WorldSpaceBotLeft[1], out hitBotLeft) && Physics.Raycast(headPosition, WorldSpaceBotRight[1], out hitBotRight))
        {
            Debug.Log("Raycasts hit!");

            var drawer = this.lineRenderer;
            drawer.SetPositions(new[] { WorldSpaceRayPoint1, hitCenter.point });
            //drawer.SetPositions(new[] { headPosition, hitTopLeft.point });
            //drawer.SetPositions(new[] { headPosition, hitTopRight.point });
            //drawer.SetPositions(new[] { headPosition, hitBotLeft.point });

            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            //Debug.Log("center: " + hitCenter.point + "; TopLeft: " + hitTopLeft.point + "; TopRight: " + hitTopRight.point + "; BotLeft: " + hitBotLeft.point);
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
            //Debug.Log("targetwidth " + targetWidth + " ; targetHeight " + targetHeight);
            //Debug.Log("currentWidth " + currentWidth + " ; currentHeight " + currentHeight);
            //Debug.Log("scaleWidth " + scaleWidth + " ; scaleHeight " + scaleHeight);
            //Debug.Log("scale.x " + scale.x + " ; scale.y " + scale.y);
            scale.x = scaleWidth * scale.x;
            scale.y = scaleHeight * scale.y;
            newArea.transform.localScale = scale;

            //Debug.Log("scale.x after: " + scale.x + " ; scale.y after: " + scale.y);
        }
    }


    public Vector3[] convert2DtoWorld(float x, float y, float imageWidth, float imageHeight, Matrix4x4 CameraToWorld, Matrix4x4 Projection )
    {
        //var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        //var Projection = cameraPositionResult.projectionMatrix;


        //Vector2 ImagePosZeroToOne = new Vector2(ocrResult.BoundingBox.x / ImageWidth, 1.0f - (ocrResult.BoundingBox.y / ImageHeight));
        //Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        //Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));

        ////Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector3(0, 0, -1)); // camera location in world space
        //Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space

        Vector3[] result = new Vector3[2];

        Vector2 ImagePosZeroToOne = new Vector2(x / imageWidth, 1.0f - (y / imageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));
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



