using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine.UI;
using UnityEngine.VR.WSA.WebCam;

public class VisualTextManager : Singleton<VisualTextManager>
{
    public GameObject textHighlight;
    public GameObject textArea;
    
    public GameObject requestBox;
    public GameObject requestBoxInstance;
    public GameObject visualTextCanvas;
    
    // public GameObject LineRenderer;
    private GazeManager gazeManager;
     private FocusManager focusManager;
    private GameObject lineRendererObject;
    private LineRenderer line1, line2, line3, line4, line5;
    private float imageWidth, imageHeight;
    public event EventHandler<VisualizedTextFocusedEventArgs> VisualizedTextFocused;
    public event EventHandler<EventArgs> VisualizedTextUnfocused;

    // Use this for initialization
    void Start()
    {
        gazeManager = this.gameObject.GetComponentInChildren<GazeManager>();
         focusManager = this.gameObject.GetComponentInChildren<FocusManager>();

        requestBoxInstance = Instantiate(requestBox);
        requestBoxInstance.SetActive(false);

        this.lineRendererObject = this.transform.Find("LineRenderer").gameObject;
        this.line1 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line2 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line3 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line4 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();
        this.line5 = Instantiate(this.lineRendererObject).GetComponent<LineRenderer>();



        requestBoxInstance = Instantiate(requestBox);
        requestBoxInstance.SetActive(false);


        imageWidth = ScreenshotManager.Instance.cameraWidth;
        imageHeight = ScreenshotManager.Instance.cameraHeight;

        //gazeManager.FocusedObjectChanged += new GazeManager.FocusedChangedDelegate(focusChanged);

        focusManager.PointerSpecificFocusChanged += new FocusManager.PointerSpecificFocusChangedMethod(focusChanged);
        

    }


    // Update is called once per frame
    void Update()
    {

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

    public void  visualizeSelectedWords(List<String> selectedWordList)
    {
        // hologramm mit selektierten woertern

        //requestBoxInstance.SetActive(false);

        if (!requestBoxInstance.active)
        {
            requestBoxInstance.SetActive(true);
        }
                //GameObject requestBoxInstance = Instantiate(requestBox);

        if (selectedWordList.Count > 0)
        {
            foreach (String w in selectedWordList)
            {
                if (requestBoxInstance.transform.Find("Text1").gameObject.GetComponent<Text>().text == "")
                {
                    requestBoxInstance.transform.Find("Text1").gameObject.GetComponent<Text>().text = w;
                } else if (requestBoxInstance.transform.Find("Text2").gameObject.GetComponent<Text>().text == "")
                {
                    requestBoxInstance.transform.Find("Text2").gameObject.GetComponent<Text>().text = w;
                }
                else if (requestBoxInstance.transform.Find("Text3").gameObject.GetComponent<Text>().text == "")
                {
                    requestBoxInstance.transform.Find("Text3").gameObject.GetComponent<Text>().text = w;
                }                          

            }

            requestBoxInstance.transform.position = new Vector3(1, -2f, 0.2f);  
        }


    }

    void focusChanged(IPointingSource pointer, GameObject oldObject, GameObject newObject)
    {
        if (oldObject != null)
        {
            oldObject.SendMessageUpwards("OnDefocus", SendMessageOptions.DontRequireReceiver);

            var handler = VisualizedTextUnfocused;
            if (handler != null) handler.Invoke(this, new EventArgs());
        }
        if (newObject != null)
        {
            newObject.SendMessageUpwards("OnFocus", SendMessageOptions.DontRequireReceiver);
            Debug.Log("newObject: " + newObject.name);
            if (newObject.tag != null && newObject.tag == "visualTextCanvas")
            {
                Text visualText = newObject.transform.Find("Text").gameObject.GetComponent<Text>();
                string focusedtext = visualText.text;

                VisualizedTextFocusedEventArgs args = new VisualizedTextFocusedEventArgs();
                args.visualizedText = focusedtext;

                var handler = VisualizedTextFocused;
                if (handler != null) handler.Invoke(this, args);
            }
        }
    }
    




    internal void visualizeText(List<CameraPositionResult> initiatedCameraPositionResultList, CameraPositionResult cameraPositionResult)
    {
        float ImageWidth = 896;// Camera.main.pixelWidth;
        float ImageHeight = 504;// Camera.main.pixelHeight;

        var ocrResult = cameraPositionResult.ocrResult; //new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));
        var headPosition = Camera.main.transform.position;
        float textX = ocrResult.BoundingBox.x;
        float textY = ocrResult.BoundingBox.y;
        float textWidth = ocrResult.BoundingBox.width;
        float textHeight =ocrResult.BoundingBox.height;
        var gazeDirection = Camera.main.transform.forward;

        Debug.Log(ocrResult.BoundingBox);
       // Debug.Log("textWidth: " + textWidth + "; textHeight: " + textHeight + "; camHeight: " + ImageHeight + "; camWidtht: " + ImageWidth);
        Vector3[] WorldSpaceCenter = convert2DtoWorld(textX + (textWidth / 2), textY + (textHeight / 2), ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);

        Vector3[] WorldSpaceTopLeft = convert2DtoWorld(textX, textY, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceTopRight = convert2DtoWorld(textX + textWidth, textY, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotLeft = convert2DtoWorld(textX, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        Vector3[] WorldSpaceBotRight = convert2DtoWorld(textX + textWidth, textY + textHeight, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);
        var CameraToWorld = cameraPositionResult.cameraToWorldMatrix;
        var Projection = cameraPositionResult.projectionMatrix;
        

        RaycastHit hitCenter, hitTopLeft, hitTopRight, hitBotLeft, hitBotRight, hitGaze;
        if (Physics.Raycast(WorldSpaceCenter[0], WorldSpaceCenter[1], out hitCenter) && Physics.Raycast(WorldSpaceTopLeft[0], WorldSpaceTopLeft[1], out hitTopLeft) && Physics.Raycast(WorldSpaceBotLeft[0], WorldSpaceTopRight[1], out hitTopRight) && Physics.Raycast(WorldSpaceBotLeft[0], WorldSpaceBotLeft[1], out hitBotLeft) && Physics.Raycast(WorldSpaceBotRight[0], WorldSpaceBotRight[1], out hitBotRight) && Physics.Raycast(headPosition, gazeDirection, out hitGaze))
           // if (Physics.Raycast(headPosition, WorldSpaceCenter[1], out hitCenter) && Physics.Raycast(headPosition, gazeDirection, out hitGaze))
        {
            //Debug.Log("Raycasts hit!");
            


            // Lisa:
            for (int i = 0; i < initiatedCameraPositionResultList.Count; i++)
            {
                if (initiatedCameraPositionResultList[i].visualizedTextObject)
                {
                    float vdistance = (hitCenter.point - initiatedCameraPositionResultList[i].visualizedTextObject.transform.position).magnitude;

                    if (vdistance <= 0.4)
                    {
                        Destroy(initiatedCameraPositionResultList[i].visualizedTextObject);
                        initiatedCameraPositionResultList.RemoveAt(i);                      
                    }

                }

            }


            GameObject newArea = Instantiate(visualTextCanvas);

            // saving ref to created game object for later
            cameraPositionResult.visualizedTextObject = newArea;


           // Text visualText = newArea.transform.Find("Text").gameObject.GetComponent<Text>();

            //set Text
            newArea.transform.Find("Text").gameObject.GetComponent<Text>().text = ocrResult.Text;

            //set Position
            newArea.transform.position = hitCenter.point;

            //opposite direction
            Vector3 opposite = (WorldSpaceCenter[1] * -0.5f).normalized;
            //newArea.transform.Translate(opposite);

            //set Rotation
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;

            //set Size

            //desired size
            float distance = (hitCenter.point - headPosition).magnitude; ;
            float targetWidth = (hitTopLeft.point - hitTopRight.point).magnitude;
            float targetHeight = (hitTopLeft.point - hitBotLeft.point).magnitude;

            //current size

            Vector3 oldScale = newArea.transform.localScale;
            RectTransform textBox = newArea.transform.GetComponent<RectTransform>();
            float currentWidth = oldScale.x*textBox.rect.width;
            float currentHeight = oldScale.y*textBox.rect.height;
            
            float scaleWidth = targetWidth / currentWidth;
            float scaleHeight = targetHeight / currentHeight;

            Vector3 newScale = new Vector3(scaleWidth *oldScale.x, scaleHeight*oldScale.y, oldScale.z);


           newArea.transform.localScale = newScale ;

        }
    }

    internal void hightlightTextLocation(List<CameraPositionResult> regularCameraPositionResultList, CameraPositionResult cameraPositionResult)
    {
        float ImageWidth = 896;// Camera.main.pixelWidth;
        float ImageHeight = 504;// Camera.main.pixelHeight;

        
        var ocrResult = cameraPositionResult.ocrResult; //new OcrResult("hi", new Rect(ImageWidth / 2, ImageHeight / 2, 0, 0));
        var headPosition = Camera.main.transform.position;
        float textX = ocrResult.BoundingBox.x;
        float textY = ocrResult.BoundingBox.y;
        float textWidth = ocrResult.BoundingBox.width;
        float textHeight = ocrResult.BoundingBox.height;
        var gazeDirection = Camera.main.transform.forward;

        //Debug.Log(ocrResult.BoundingBox);
        //Debug.Log("textWidth: " + textWidth + "; textHeight: " + textHeight + "; camHeight: " + ImageHeight + "; camWidtht: " + ImageWidth);
        Vector3[] WorldSpaceCenter = convert2DtoWorld(textX , textY, ImageWidth, ImageHeight, cameraPositionResult.cameraToWorldMatrix, cameraPositionResult.projectionMatrix);

        


        RaycastHit hitCenter;
        if (Physics.Raycast(WorldSpaceCenter[0], WorldSpaceCenter[1], out hitCenter))
        {
            // Debug.Log("Raycasts hit!");

            // Lisa:
            for (int i = 0; i < regularCameraPositionResultList.Count; i++)
            {
                if (regularCameraPositionResultList[i].textHighlightObject)
                {
                    float vdistance = (hitCenter.point - regularCameraPositionResultList[i].textHighlightObject.transform.position).magnitude;

                    if (vdistance <= 0.4)
                    {
                        //Destroy(regularCameraPositionResultList[i].textHighlightObject);
                        //regularCameraPositionResultList.RemoveAt(i);       

                        //Destroy(regularCameraPositionResultList[i].textHighlightObject);
                        regularCameraPositionResultList.RemoveAt(regularCameraPositionResultList.Count-1);
                        return;             
                    }

                }

            }

            //Debug.Log("parent of dot" + textHighlight.);
            GameObject newHighlight = Instantiate(textHighlight);

            // saving ref to text highlight object for later
            cameraPositionResult.textHighlightObject = newHighlight;



            //set Position
            newHighlight.transform.position = hitCenter.point;

            //set Rotation
            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newHighlight.transform.rotation = toQuat;

        }

    }



    public Vector3[] convert2DtoWorld(float x, float y, float imageWidth, float imageHeight, Matrix4x4 CameraToWorld, Matrix4x4 Projection)
    {

        Vector3[] result = new Vector3[2];

        Vector2 ImagePosZeroToOne = new Vector2(x / imageWidth, 1f - (y / imageHeight));

        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1f, 1f)); // -1 to 1 space

        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x, ImagePosProjected.y, 1));
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyPoint3x4(new Vector4(0f, 0f, 0f, 1f)); // camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
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

public class VisualizedTextFocusedEventArgs : EventArgs {

    public string visualizedText { get; set; }

}