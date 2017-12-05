using HoloToolkit.Unity;

/// <summary> Singleton that is responsible for management of queries, pictures and keywords </summary>
public class Controller : Singleton<Controller>
{

    /// <summary> reference to the screenshot manager instance </summary>
    private ScreenshotManager screenshotManager;

    /// <summary> reference to the API manager instance </summary>
    private ApiManager apiManager;


    /// <summary> reference to the API manager instance </summary>
    private GesturesManager gesturesManager;


    //private Picture screenshot;



    /// <summary>
    /// called when the application is started
    /// </summary>
    void Start()
    {

        // link managers
        apiManager = ApiManager.Instance;
        screenshotManager = ScreenshotManager.Instance;
        gesturesManager = GesturesManager.Instance;

        // subscribe to events
        screenshotManager.ScreenshotTaken += OnScreenshotTaken;
        apiManager.ImageAnalysed += onImageAnalysed;

        //repeating capturing screenshots function starts in 1s every 0.5s
        //InvokeRepeating("TakeScreenshot", 1f, 0.5f);
    }


    /// <summary>
    /// called whenever a screenshot was taken by the screenshot manager
    /// </summary>
    /// <param name="sender"> the sender of the event </param>
    /// <param name="e"> the photograph event parameters </param>
    private void OnScreenshotTaken(object sender, QueryPhotoEventArgs e)
    {
        // initiate text regognition
#if (!UNITY_EDITOR)
        apiManager.AnalyzeImageAsync(RequestType.REMOTE, new Picture(e.ScreenshotAsTexture));
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


}