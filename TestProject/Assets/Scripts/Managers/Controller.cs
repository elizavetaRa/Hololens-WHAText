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

        // subscribe to events
        screenshotManager.ScreenshotTaken += OnScreenshotTaken;

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
        // store new screenshot as byte array
        byte[] screenshotAsByteArray = e.ScreenshotByteList.ToArray();

        // initiate text regognition
        apiManager.AnalyzeImage(RequestType.LOCAL, new Picture(screenshotAsByteArray));
    }

    public void TakeScreenshot()
    {
        screenshotManager.TakeScreenshot();

    }


}