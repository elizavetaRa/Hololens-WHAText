using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using System.Linq;
#if (!UNITY_EDITOR)
using System.Threading.Tasks;
using UnityEngine;
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

    private bool processingScreenshot;
    private CameraPositionResult cameraPositionResultTemp;

    //private Picture screenshot;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    private int currentId;

    // stack of 10 latest camera positions, ocrResults
    //Queue<CameraPositionResult> cameraPositionResultQueue = new Queue<CameraPositionResult>();
    List<CameraPositionResult> regularCameraPositionResultList = new List<CameraPositionResult>();
    List<CameraPositionResult> initiatedCameraPositionResultList = new List<CameraPositionResult>();


    /// <summary>
    /// called when the application is started
    /// </summary>
    void Start()
    {

        // link managers
        apiManager = ApiManager.Instance;
        screenshotManager = ScreenshotManager.Instance;
        visualTextManager = VisualTextManager.Instance;

        // subscribe to events
        screenshotManager.ScreenshotTaken += OnScreenshotTaken;
        apiManager.ImageAnalysed += onImageAnalysed;

        processingScreenshot = false;
        timeCounter = 0;
        timeInterval = 1;

        currentRequestCause = RequestCause.REGULAR;
        nextRequestCause = RequestCause.REGULAR;

        cameraPositionResultTemp = new CameraPositionResult();

        currentId = 0;
    }

    void Update()
    {

    }

    /// <summary>
    /// called whenever a screenshot was taken by the screenshot manager
    /// </summary>
    /// <param name="sender"> the sender of the event </param>
    /// <param name="e"> the photograph event parameters </param>
    private void OnScreenshotTaken(object sender, QueryPhotoEventArgs e)
    {
#if (!UNITY_EDITOR)

        cameraPositionResultTemp = new CameraPositionResult();
        cameraPositionResultTemp.cameraToWorldMatrix = e.CameraToWorldMatrix;
        cameraPositionResultTemp.projectionMatrix = e.ProjectionMatrix;

        //id
        switch (currentRequestCause)
        {
            case RequestCause.REGULAR:

                if (regularCameraPositionResultList.Count > 9)
                {
                    regularCameraPositionResultList.RemoveAt(0);
                }

                cameraPositionResultTemp.id = currentId;
                regularCameraPositionResultList.Insert(regularCameraPositionResultList.Count, cameraPositionResultTemp);

                break;
            case RequestCause.USERINITIATED:

                if (initiatedCameraPositionResultList.Count > 9)
                {
                    initiatedCameraPositionResultList.RemoveAt(0);
                }
                cameraPositionResultTemp.id = currentId;
                initiatedCameraPositionResultList.Insert(regularCameraPositionResultList.Count, cameraPositionResultTemp);


                break;
        }


        //start analyzing image
        switch (currentRequestCause)
        {
            case RequestCause.REGULAR:
                apiManager.AnalyzeImageAsync(RequestType.LOCAL, new Picture(e.ScreenshotAsTexture));
                break;
            case RequestCause.USERINITIATED:
                apiManager.AnalyzeImageAsync(RequestType.REMOTE, new Picture(e.ScreenshotAsTexture));
                break;
        }
#endif
    }

    private void onImageAnalysed(object sender, AnalyseImageEventArgs e)
    {
        if (e.Result == null || e.Result.Text == "")
        {
            if (currentRequestCause == RequestCause.USERINITIATED)
            {
                System.Diagnostics.Debug.WriteLine("No text was found, please reposition yourself and try again");

                initiatedCameraPositionResultList.RemoveAt(initiatedCameraPositionResultList.Count);

            } else
            {
                regularCameraPositionResultList.RemoveAt(initiatedCameraPositionResultList.Count);
            }
        }
        else
        {
            // check for capacity of result list

            switch (currentRequestCause)
            {
                case RequestCause.REGULAR:

                    for (int i = 0; i < regularCameraPositionResultList.Count; i++)
                    {
                        if (regularCameraPositionResultList[i].id == currentId)
                        {
                            regularCameraPositionResultList[i].ocrResult = e.Result;
                            currentId++;
                            break;
                        }
                    }

                    break;
                case RequestCause.USERINITIATED:
                    for (int i = 0; i < initiatedCameraPositionResultList.Count; i++)
                    {
                        if (initiatedCameraPositionResultList[i].id == currentId)
                        {
                            initiatedCameraPositionResultList[i].ocrResult = e.Result;
                             currentId++;
                            break;
                        }                                             
                    }                 
                    break;    
            }

            //currentId++;
            
            displayText();
        }

        //processingScreenshot = false;
        //currentRequestCause = nextRequestCause;
    }


#if (!UNITY_EDITOR)

    public void TakeScreenshot(RequestCause requestCause)
    {
        //processingScreenshot = true;
        this.currentRequestCause = requestCause;
        screenshotManager.TakeScreenshot();
    }


    public void RequestImageProcessing(RequestCause requestCause)
    {
        //nextRequestCause = requestCause;

        TakeScreenshot(requestCause);
    }


#endif

    public void displayText()
    {
        var size = regularCameraPositionResultList.Count;

        visualTextManager.visualizeText(regularCameraPositionResultList.ElementAt(size - 1));

    }


}

public enum RequestCause
{
    REGULAR,
    USERINITIATED,
}

