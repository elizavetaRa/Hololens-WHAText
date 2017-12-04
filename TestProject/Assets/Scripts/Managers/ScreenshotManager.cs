using UnityEngine;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;
using System.Threading.Tasks;

///https://docs.unity3d.com/2017.1/Documentation/ScriptReference/VR.WSA.WebCam.PhotoCapture.html

///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class ScreenshotManager: Singleton<ScreenshotManager> {
	
	 /// <summary> object that performs the photo capture </summary>
	PhotoCapture photoCaptureObject = null;

    Resolution cameraResolution;
	
	Texture2D targetTexture = null;
    Renderer quadRenderer = null;

    /// <summary> handles the event when a photograph was taken </summary>
    public event EventHandler<QueryPhotoEventArgs> ScreenshotTaken;



    // Use this for initialization
    void Start()
    {

    }

    public void TakeScreenshot() {
        //First: Last: worst resolution?
        cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();
        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

        // Create a PhotoCapture object
        //Params: Show Holograms=false, onCreatedCallback, wenn PhotoCapture Instance created and ready to be used
        PhotoCapture.CreateAsync(false, delegate(PhotoCapture captureObject) {
                photoCaptureObject = captureObject;
				
				//needed for Calling PhotoCapture.StartPhotoModeAsync
                CameraParameters cameraParameters = new CameraParameters();
                cameraParameters.hologramOpacity = 0.0f;
                cameraParameters.cameraResolutionWidth = cameraResolution.width;
                cameraParameters.cameraResolutionHeight = cameraResolution.height;
                cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

                // Activate the web camera
                photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate(PhotoCapture.PhotoCaptureResult result) {
                    // Take a screenshot
                    photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
                });
            });
    }
	
	
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
            
            
            /*List<byte> imageBufferList = new List<byte>();

            // Convert to Byte List
            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);*/

            // send event with Bytelist of the captured screenshot
            OnScreenshotTaken(new QueryPhotoEventArgs(screenshot));
            
        }

        // Deactivate web camera
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
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
    protected virtual async Task OnScreenshotTaken(QueryPhotoEventArgs e)
    {
        // send event if there are subscribers
        EventHandler<QueryPhotoEventArgs> handler = ScreenshotTaken;
        if (handler != null) handler(this, e);
    }


}


public class QueryPhotoEventArgs : EventArgs
{
    /// <summary>
    /// constructor for the photo capture event parameters
    /// </summary>
    /// <param name="l"> Byte List of the captured screenshot </param>
    public QueryPhotoEventArgs(Texture2D texture)
    {
        ScreenshotAsTexture = texture;
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
}
