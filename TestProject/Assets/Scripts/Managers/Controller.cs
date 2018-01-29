using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if (!UNITY_EDITOR)
#endif

/// <summary> Singleton that is responsible for management of queries, pictures and keywords </summary>
public class Controller : Singleton<Controller>
{

    /// <summary> reference to the screenshot manager instance </summary>
    private ScreenshotManager screenshotManager;

    /// <summary> reference to the API manager instance </summary>
    private ApiManager apiManager;

    private VisualTextManager visualTextManager;

    /// <summary> reference to the API manager instance </summary>
    private GesturesManager gesturesManager;

    private float timeCounter;
    /// <summary> Interval in which images are being process regularly</summary>
    private float timeInterval;
    private const float longTime = .7F;
    private const float shortTime = .1F;

    private RequestCause currentRequestCause, nextRequestCause;

    private bool analysingScreenshot;
    private CameraPositionResult cameraPositionResultTmp;
    private bool screenshotsTakeable;

    //private Picture screenshot;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    // temporarily saving data from the latest frame captured
    private Matrix4x4 cameraToWorldMatrixTmp, projectionMatrixTmp;
    private Texture2D imageAsTextureTmp;

    private int currentId;

    // stack of 10 latest camera positions, ocrResults
    private List<CameraPositionResult> regularCameraPositionResultList = new List<CameraPositionResult>();
    private List<CameraPositionResult> initiatedCameraPositionResultList = new List<CameraPositionResult>();

    // list with selected words to build request
    public List<String> selectedWordsList = new List<String>();

    // temporarily save focus/unfocus event data
    private bool visualizedTextFocused;
    private string focusedVisualizedTextTmp;

    /// <summary>
    /// called when the application is started
    /// </summary>
    void Start()
    {
        // link managers
        apiManager = ApiManager.Instance;
        screenshotManager = ScreenshotManager.Instance;
        visualTextManager = VisualTextManager.Instance;
        gesturesManager = GesturesManager.Instance;

        // subscribe to events
        screenshotManager.ScreenshotTaken += OnScreenshotTaken;
        apiManager.ImageAnalysed += onImageAnalysed;
        gesturesManager.Tapped += onTapped;
        gesturesManager.DoubleTapped += onDoubleTapped;
        visualTextManager.VisualizedTextFocused += OnVisualizedTextFocused;
        visualTextManager.VisualizedTextUnfocused += OnVisualizedTextUnfocused;

        analysingScreenshot = false;
        timeCounter = 0;
        timeInterval = 1;

        currentRequestCause = RequestCause.REGULAR;
        nextRequestCause = RequestCause.REGULAR;

        screenshotsTakeable = true;


        cameraPositionResultTmp = new CameraPositionResult();

        currentId = 0;

        visualizedTextFocused = false;
        focusedVisualizedTextTmp = "";
    }

    void Update()
    {
    }

    /// <summary>
    /// called whenever a screenshot was taken by the screenshot manager
    /// </summary>
    /// <param name="sender"> the sender of the event </param>
    /// <param name="e"> the photograph event parameters </param>
    private void OnScreenshotTaken(object sender, EventArgs e)
    {
        // Save ref for latest picture taken
        screenshotManager.GetLatestPicture(out imageAsTextureTmp, out cameraToWorldMatrixTmp, out projectionMatrixTmp);

        cameraPositionResultTmp = new CameraPositionResult();
        cameraPositionResultTmp.cameraToWorldMatrix = cameraToWorldMatrixTmp;
        cameraPositionResultTmp.projectionMatrix = projectionMatrixTmp;

        //start analyzing image
        analysingScreenshot = true;

#if (!UNITY_EDITOR)
        if (screenshotsTakeable)
        {
            //id
            switch (currentRequestCause)
            {
                case RequestCause.REGULAR:

                    if (regularCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(regularCameraPositionResultList[0].textHighlightObject);

                        regularCameraPositionResultList.RemoveAt(0);
                    }

                    cameraPositionResultTmp.id = currentId;
                    regularCameraPositionResultList.Insert(regularCameraPositionResultList.Count, cameraPositionResultTmp);
                    // Debug.Log("!!!!!RegularLIST Count" + regularCameraPositionResultList.Count);
                    apiManager.AnalyzeImageAsync(RequestType.LOCAL, new Picture(imageAsTextureTmp));
                    break;
                case RequestCause.USERINITIATED:

                    screenshotsTakeable = false;
                    this.screenshotManager._screenshotsTakeable = false;

                    if (initiatedCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(initiatedCameraPositionResultList[0].visualizedTextObject);

                        initiatedCameraPositionResultList.RemoveAt(0);
                    }
                    cameraPositionResultTmp.id = currentId;
                    initiatedCameraPositionResultList.Insert(initiatedCameraPositionResultList.Count, cameraPositionResultTmp);
                    // Debug.Log("!!!!!InitiatedLIST Count" + initiatedCameraPositionResultList.Count);
                    apiManager.AnalyzeImageAsync(RequestType.REMOTE, new Picture(imageAsTextureTmp));
                    break;
            }
            
        }
#endif

    }

    private void onDoubleTapped(object sender, DoubleTapEventArgs e)
    {
        if (screenshotsTakeable)
        {
            this.currentRequestCause= e.RequestCause;
        }
    }

    private void onTapped(object sender, TapEventArgs e)
    {
        if ((focusedVisualizedTextTmp != "" && focusedVisualizedTextTmp != null)
            && (visualizedTextFocused))
        {
            //Test
            selectedWordsList.Add("Broccoli");
            selectedWordsList.Add("Tomatoes");
            //selectedWordsList.Add(focusedVisualizedTextTmp);
            visualTextManager.visualizeSelectedWords(selectedWordsList);
        } else {//Test
            selectedWordsList.Add("Broccoli");
        selectedWordsList.Add("Tomatoes");
        //selectedWordsList.Add(focusedVisualizedTextTmp);
        visualTextManager.visualizeSelectedWords(selectedWordsList);
         }
            

        //        selectedWordsList.Insert(selectedWordsList.Count, e.Word);
        //#if (!UNITY_EDITOR)
        //        ApiYummlyRecipes.Instance.HttpGetRecipesByIngredients(new string[] { e.Word });
        //#endif
    }

    private void onImageAnalysed(object sender, AnalyseImageEventArgs e)
    {
        if (!screenshotsTakeable && currentRequestCause == RequestCause.USERINITIATED && e.Result.OcrService == OcrService.MICROSOFTMEDIAOCR)
        {
            Destroy(regularCameraPositionResultList[regularCameraPositionResultList.Count - 1].visualizedTextObject);
            regularCameraPositionResultList.RemoveAt(regularCameraPositionResultList.Count-1);
            return;
        }
        else
        {
            if (e.Result.OcrService == OcrService.MICROSOFTAZUREOCR)
            {
                // Debug.LogError("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + e.Result.OcrService + ": " + e.Result.Text + "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\n!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            }
            else
            {
                //Debug.LogError(e.Result.OcrService + ": " + e.Result.Text);
            }
        }


        if (e.Result == null || e.Result.Text == "")
        {
            switch (currentRequestCause)
            {
                case RequestCause.USERINITIATED:

                    if (initiatedCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(initiatedCameraPositionResultList[0].visualizedTextObject);

                        initiatedCameraPositionResultList.RemoveAt(0);
                    }
                    Debug.LogError("No text was found, please reposition yourself and try again");


                    analysingScreenshot = false;

                    Destroy(initiatedCameraPositionResultList[initiatedCameraPositionResultList.Count - 1].visualizedTextObject);
                    initiatedCameraPositionResultList.RemoveAt(initiatedCameraPositionResultList.Count-1);
                    break;

                case RequestCause.REGULAR:

                    if (regularCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(regularCameraPositionResultList[0].textHighlightObject);
                        regularCameraPositionResultList.RemoveAt(0);
                    }

                    Destroy(regularCameraPositionResultList[regularCameraPositionResultList.Count - 1].textHighlightObject);
                    regularCameraPositionResultList.RemoveAt(regularCameraPositionResultList.Count-1);
                    break;
            }
        }
        else
        {
            // check for capacity of result list
            switch (currentRequestCause)
            {
                case RequestCause.REGULAR:

                    if (regularCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(regularCameraPositionResultList[0].textHighlightObject);
                        regularCameraPositionResultList.RemoveAt(0);
                    }

                    for (int i = 0; i < regularCameraPositionResultList.Count; i++)
                    {
                        if (regularCameraPositionResultList[i].id == currentId)
                        {
                            regularCameraPositionResultList[i].ocrResult = e.Result;
                            // visualize POI
                            visualTextManager.hightlightTextLocation(regularCameraPositionResultList, regularCameraPositionResultList[i]);
                            currentId++;
                            break;
                        }
                    }

                    break;
                case RequestCause.USERINITIATED:

                    if (initiatedCameraPositionResultList.Count > 9)
                    {
                        // removing game object
                        Destroy(initiatedCameraPositionResultList[0].visualizedTextObject);

                        initiatedCameraPositionResultList.RemoveAt(0);
                    }

                    for (int i = 0; i < initiatedCameraPositionResultList.Count; i++)
                    {
                        if (initiatedCameraPositionResultList[i].id == currentId)
                        {
                            initiatedCameraPositionResultList[i].ocrResult = e.Result;
                            // visualize text
                            visualTextManager.visualizeText(initiatedCameraPositionResultList, initiatedCameraPositionResultList[i]);
                            currentId++;
                            break;
                        }                                             
                    }                 
                    break;    
            }
        }


        if (!screenshotsTakeable && currentRequestCause == RequestCause.USERINITIATED)
        {
            screenshotsTakeable = true;
            this.currentRequestCause = RequestCause.REGULAR;
            // this.nextRequestCause = RequestCause.REGULAR;
            this.screenshotManager._screenshotsTakeable = true;
            return;
        }

        
    }

    public void OnVisualizedTextFocused(object sender, VisualizedTextFocusedEventArgs e)
    {
        visualizedTextFocused = true;
        focusedVisualizedTextTmp = e.visualizedText;
    }

    public void OnVisualizedTextUnfocused(object sender, EventArgs e)
    {
        visualizedTextFocused = false;
        focusedVisualizedTextTmp = "";
    }

    public void displayText()
    {
        var size = regularCameraPositionResultList.Count;
        visualTextManager.hightlightTextLocation(regularCameraPositionResultList, regularCameraPositionResultList.ElementAt(size - 1));
    }


}

public enum RequestCause
{
    REGULAR,
    USERINITIATED,
}


