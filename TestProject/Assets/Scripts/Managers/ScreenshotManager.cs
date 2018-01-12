using UnityEngine;
using HoloToolkit.Unity;
using System;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
using System.Collections.Generic;
#if (!UNITY_EDITOR)
using System.Threading.Tasks;
#endif

///https://docs.unity3d.com/2017.1/Documentation/ScriptReference/VR.WSA.WebCam.PhotoCapture.html

///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class ScreenshotManager : Singleton<ScreenshotManager> {

    /// <summary> object that performs the photo capture </summary>
    PhotoCapture photoCaptureObject;

    Resolution cameraResolution;
    CameraParameters cameraParameters;
    Matrix4x4 cameraToWorldMatrix, projectionMatrix;

    Texture2D imageAsTexture;
    // Renderer quadRenderer;

    bool screenshotsTakeable = false;
    float lastTime = 0;
    int photoCount = 0;

    public event EventHandler ScreenshotTaken;
    public event EventHandler<CameraReadyEventArgs> ScreenshotsTakeable;

    // Use this for initialization
    void Start()
    {
        // Use worst screenshot resolution to reduce CPU time
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);

        imageAsTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
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
            screenshotsTakeable = true;

            // send event if there are subscribers
            CameraReadyEventArgs args = new CameraReadyEventArgs();
            args.cameraReady = true;
            var handler = ScreenshotsTakeable;
            if (handler != null) handler.Invoke(this, args);
        }
        else
        {
            Debug.LogError("Photo Mode couldn't be started");
        }
    }

	// When screenshot is captured to memory
    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
		if (result.success)
        {
            // play photo capture sound
            //Camera.main.GetComponent<AudioSource>().Play();

            // save photograph to texture
            imageAsTexture = new Texture2D(cameraResolution.width, cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(imageAsTexture);

            // position of camera/user at time of capturing screenshot
            photoCaptureFrame.TryGetCameraToWorldMatrix(out cameraToWorldMatrix);
            photoCaptureFrame.TryGetProjectionMatrix(out projectionMatrix);

            if (lastTime == 0)
            {
                lastTime = Time.time;
            }
            if (Time.time - lastTime < 1.0f)
            {
                photoCount++;
            }
            else
            {
                Debug.LogError("Photos per s: " + photoCount);
                lastTime = Time.time;
                photoCount = 0;
            }

            // send event with Bytelist of the captured screenshot
            OnScreenshotTaken();
        }

        this.screenshotsTakeable = true;
        // Deactivate web camera
        //photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void GetLatestPicture(out Texture2D picture, out Matrix4x4 cameraToWorldMatrix, out Matrix4x4 projectionMatrix)
    {
        picture = this.imageAsTexture;
        cameraToWorldMatrix = this.cameraToWorldMatrix;
        projectionMatrix = this.projectionMatrix;
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
    protected virtual void OnScreenshotTaken()
    {
        // send event if there are subscribers
        var handler = ScreenshotTaken;
        if (handler != null) handler.Invoke(this, new EventArgs());
    }


}


public class CameraReadyEventArgs : EventArgs
{
    public bool cameraReady { get; set; }
}

public class QueryPhotoEventArgs : EventArgs
{
    /// <summary>
    /// constructor for the photo capture event parameters
    /// </summary>
    /// <param name="l"> Byte List of the captured screenshot </param>
    public QueryPhotoEventArgs(Matrix4x4 cameraToWorldMatrix, Matrix4x4 projectionMatrix)
    {
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
