using UnityEngine;
using HoloToolkit.Unity;
using System;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
#if (!UNITY_EDITOR)
using System.Threading.Tasks;
#endif

///https://docs.unity3d.com/2017.1/Documentation/ScriptReference/VR.WSA.WebCam.PhotoCapture.html

///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class ScreenshotManager: Singleton<ScreenshotManager> {

    /// <summary> object that performs the photo capture </summary>
    PhotoCapture photoCaptureObject;

    Resolution cameraResolution;
    CameraParameters cameraParameters;

    Texture2D targetTexture;
    // Renderer quadRenderer;

    bool screenshotsTakeable = false;

    public event EventHandler<QueryPhotoEventArgs> ScreenshotTaken;
    public event EventHandler<bool> ScreenshotsTakeable;

    // Use this for initialization
    void Start()
    {
        // Use worst screenshot resolution to reduce CPU time
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
    }

    void Update()
    {
        if (!screenshotsTakeable)
        {
            return;
        }
        screenshotsTakeable = false;
        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }

    void Stop()
    {
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    private void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        if (captureObject == null)
        {
            Debug.LogError("Photo Capture could not be created");
        }

        Debug.LogError("Photo Capture created");
        this.photoCaptureObject = captureObject;

        var supportedResolutions = (Resolution[])PhotoCapture.SupportedResolutions;

        // emulator cannot take photos
        if (supportedResolutions.Length == 0)
        {
            Debug.LogError("Photo mode could not be started. Are you using an emulator?");
            this.photoCaptureObject.Dispose();
            this.photoCaptureObject = null;
            return;
        }

        //needed for starting photo mode
        cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        // Activate the web camera
        photoCaptureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Photo Mode started");
            ScreenshotsTakeable?.Invoke(this, result.success);
        }
        else
        {
            Debug.LogError("Photo Mode couldn't be started");
        }
    }

    //internal void TakeScreenshot()
    //{
        

    //    // Create a PhotoCapture object
    //    //Params: Show Holograms=false, onCreatedCallback, wenn PhotoCapture Instance created and ready to be used
    //    PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
    //    {
           
    //        // Activate the web camera
    //        photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
    //        {
    //            if (result.success)
    //            {
    //                // Take a screenshot
    //                photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    //            }
    //            else
    //            {
    //                // try to capture a photograph if photo mode was already started before
    //                if (WebCam.Mode == WebCamMode.PhotoMode)
    //                {
    //                    try
    //                    {
    //                        photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    //                    }
    //                    catch
    //                    {
    //                        Debug.LogError("Photo mode was already started but capturing did not succeed.");
    //                    }

    //                }
    //                else
    //                {
    //                    // use standard photograph if using photo mode did not succeed
    //                    Debug.LogError("Unable to start photo mode!");
    //                }
    //            }
    //        });
    //    });

    //}

	//wenn screenshot is captured to memory
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        
		if (result.success)
        {
            // play photo capture sound
            //Camera.main.GetComponent<AudioSource>().Play();

            // save photograph to texture
            Texture2D screenshot = new Texture2D(cameraResolution.width, cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(screenshot);

            // position of camera/user at time of capturing screenshot
            var cameraToWorldMatrix = new Matrix4x4();
            var projectionMatrix = new Matrix4x4();

            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            // send event with Bytelist of the captured screenshot
            OnScreenshotTaken(new QueryPhotoEventArgs(screenshot, cameraToWorldMatrix, projectionMatrix));
        }

        this.screenshotsTakeable = true;
        // Deactivate web camera
        //photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

	
	
	
	/// called when photo mode is stopped
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown our photo capture resource
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }


    /// <summary>
    /// called whenever a screenshot has been taken
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnScreenshotTaken(QueryPhotoEventArgs e)
    {
        // send event if there are subscribers
        ScreenshotTaken?.Invoke(this, e);
    }


}


public class QueryPhotoEventArgs : EventArgs
{
    /// <summary>
    /// constructor for the photo capture event parameters
    /// </summary>
    /// <param name="l"> Byte List of the captured screenshot </param>
    public QueryPhotoEventArgs(Texture2D texture, Matrix4x4 cameraToWorldMatrix, Matrix4x4 projectionMatrix)
    {
        ScreenshotAsTexture = texture;
        CameraToWorldMatrix = cameraToWorldMatrix;
        ProjectionMatrix = projectionMatrix;
       
    }

    
    /// <summary>
    /// Bytelist of the captured screenshot
    /// </summary>
    private Texture2D screenshot;


    /// <summary>
    /// Bytelist of the captured screenshot
    /// </summary>
    public Texture2D ScreenshotAsTexture
    {
        get; private set;
    }


    /// <summary>
    /// cameraToWorld matrix
    /// </summary>
    private Matrix4x4 cameraToWorldMatrix;


    /// <summary>
    /// cameraToWorld matrix
    /// </summary>
    public Matrix4x4 CameraToWorldMatrix
    {
        get; private set;
    }

    /// <summary>
    /// projection matrix
    /// </summary>
    private Matrix4x4 projectionMatrix;


    /// <summary>
    /// cameraToWorld matrix
    /// </summary>
    public Matrix4x4 ProjectionMatrix
    {
        get; private set;
    }
}
