using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity;

/// <summary> Singleton that is responsible for management of queries, pictures and keywords </summary>
public class ManagerManager : Singleton<ManagerManager>
{

    /// <summary> reference to the screenshot manager instance </summary>
    private ScreenshotManager screenshotManager;

    /// <summary> reference to the API manager instance </summary>
    private ApiManager apiManager;


    //private Picture screenshot;



    /// <summary>
    /// called when the application is started
    /// </summary>
    void Start()
    {

        // link managers
        apiManager = ApiManager.Instance;
        screenshotManager = ScreenshotManager.Instance;

        // subscribe to events
        //screenshotManager.ScreenshotTaken += OnScreenshotTaken;
    }


    /// <summary>
    /// called whenever a screenshot was taken by the screenshot manager
    /// </summary>
    /// <param name="sender"> the sender of the event </param>
    /// <param name="e"> the photograph event parameters </param>
    private void OnScreenshotTaken(object sender, QueryPhotoEventArgs e)
    {
        // create a new picture object and store the photograph

        List<byte> list = new List<byte>();

        //store byte list of the captured screenshot
        list = e.ScreenshotByteList;


        // initiate text regognition
        // apiManager.AnalyzeImage(e.QueryId, image.Id);


    }


}