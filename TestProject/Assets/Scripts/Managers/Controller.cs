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

    //private Picture screenshot;
    private Vector3 cameraPosition;
    private Quaternion cameraRotation;

    // stack of 10 latest camera positions, ocrResults
    Queue<CameraPositionResult> cameraPositionResultQueue = new Queue<CameraPositionResult>();



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

        //repeating capturing screenshots function starts in 1s every 0.5s
        //timer = new System.Threading.Timer(IsImageProcessed, "Timer", TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0));
    }

    void Update()
    {
        //        // process images every timeInterval seconds
        //#if (!UNITY_EDITOR)
        //        timeCounter += Time.deltaTime;
        //#endif
        //        if (timeCounter >= timeInterval)
        //        {
        //            timeCounter = 0;
        //            if (!processingScreenshot)
        //            {
        //                timeInterval = longTime;
        //#if (!UNITY_EDITOR)
        //                TakeScreenshot(nextRequestCause);
        //#endif
        //                // by default, request cause should always be regular processing
        //                nextRequestCause = RequestCause.REGULAR;
        //            }
        //            // check for state 'image processing finished' more often to reduce waiting time
        //            else
        //            {
        //                if (timeInterval != shortTime) timeInterval = shortTime;
        //            }
        //        }

    }

    /// <summary>
    /// called whenever a screenshot was taken by the screenshot manager
    /// </summary>
    /// <param name="sender"> the sender of the event </param>
    /// <param name="e"> the photograph event parameters </param>
    private void OnScreenshotTaken(object sender, QueryPhotoEventArgs e)
    {
#if (!UNITY_EDITOR)

        //recalculate Camera to World Matrix to position and rotation
        //cameraPosition = e.CameraToWorldMatrix.MultiplyPoint3x4(new Vector3(0, 0, -1));
        //cameraRotation = Quaternion.LookRotation(-e.CameraToWorldMatrix.GetColumn(2), e.CameraToWorldMatrix.GetColumn(1));
        //System.Diagnostics.Debug.WriteLine(" camera position, rotation " + cameraPosition + cameraRotation);

        // store last 10 camera positions to queue of cemera position results
        CameraPositionResult cameraPositionResult = new CameraPositionResult();


        //cameraPositionResult.cameraPosition = cameraPosition;
        //cameraPositionResult.cameraRotation = cameraRotation;
        cameraPositionResult.cameraToWorldMatrix = e.CameraToWorldMatrix;
        cameraPositionResult.projectionMatrix = e.ProjectionMatrix;

        if (cameraPositionResultQueue.Count > 9)
        {
            cameraPositionResultQueue.Dequeue();
        }

        cameraPositionResultQueue.Enqueue(cameraPositionResult);


            this.displayText();
        

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
        if ((e.Result == null || e.Result.Text == "") && currentRequestCause == RequestCause.USERINITIATED)
            System.Diagnostics.Debug.WriteLine("No text was found, please reposition yourself and try again");

        processingScreenshot = false;
        currentRequestCause = nextRequestCause;
    }


#if (!UNITY_EDITOR)

    public async Task TakeScreenshot(RequestCause requestCause)
    {
        //processingScreenshot = true;
        //this.currentRequestCause = requestCause;
        screenshotManager.TakeScreenshot();
    }


#endif

    public void RequestImageProcessing(RequestCause requestCause)
    {
        nextRequestCause = requestCause;
    }

    public void displayText()
    {
        var size = cameraPositionResultQueue.Count;

        visualTextManager.visualizeText(cameraPositionResultQueue.ElementAt(size - 1));

    }


}

public enum RequestCause
{
    REGULAR,
    USERINITIATED,
}
