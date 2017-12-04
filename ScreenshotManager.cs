using UnityEngine;
using System;
using System.Collections;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;

///https://docs.unity3d.com/2017.1/Documentation/ScriptReference/VR.WSA.WebCam.PhotoCapture.html

///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class ScreenshotManager: Singleton<ScreenshotManager> {
	
	 /// <summary> object that performs the photo capture </summary>
	PhotoCapture photoCaptureObject = null;
	
	
	//Texture2D targetTexture = null;
    Renderer quadRenderer = null;	
	
	

    // Use this for initialization
    void Start()
    {
		//First: Last: worst resolution?
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();
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
            Camera.main.GetComponent<AudioSource>().Play();
		
			// Copy the raw image data into our target texture
			//photoCaptureFrame.UploadImageDataToTexture(targetTexture);

			//send to OCR!!! 
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
}



