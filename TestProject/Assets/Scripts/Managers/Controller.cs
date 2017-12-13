using HoloToolkit.Unity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

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


    /// <summary> reference to the API manager instance </summary>
    private GesturesManager gesturesManager;
    private VisualTextManager visualTextManager;

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


        //repeating capturing screenshots function starts in 1s every 0.5s
        //InvokeRepeating("TakeScreenshot", 1f, 0.5f);s
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
        if (e.Result == null)
            System.Diagnostics.Debug.WriteLine("No text was found, please reposition yourself and try again");
    }

    public void TakeScreenshot()
    {
        screenshotManager.TakeScreenshot();

    }

    public void displayText()
    {
        visualTextManager.visualizeText("0", new UnityEngine.Vector2(0, 0));
        visualTextManager.visualizeText("1", new UnityEngine.Vector2(100, 0));
        visualTextManager.visualizeText("2", new UnityEngine.Vector2(100, 100));
        visualTextManager.visualizeText("3", new UnityEngine.Vector2(0, 100));
    }


}