using UnityEngine;
using HoloToolkit.Unity;
using System;
using System.Linq;
using UnityEngine.VR.WSA.WebCam;

///https://docs.unity3d.com/2017.1/Documentation/ScriptReference/VR.WSA.WebCam.PhotoCapture.html

///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class ScreenshotManager : Singleton<ScreenshotManager>
{
    /// <summary> object that performs the photo capture </summary>
    PhotoCapture _photoCaptureObject;

    Resolution _cameraResolution;
    CameraParameters _cameraParameters;

    // temporarily saving data of the latest frame captured
    Matrix4x4 _cameraToWorldMatrixTmp, _projectionMatrixTmp;
    Texture2D _imageAsTextureTmp;

    public bool _screenshotsTakeable;

    // needed for measuring captured frames per second
    float _lastTime;
    int _photoCount;

    public event EventHandler ScreenshotTaken;
    public event EventHandler ScreenshotsTakeable;

    // Use this for initialization
    void Start()
    {
        // Use worst screenshot resolution to reduce CPU time
        _cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).Last();

        _screenshotsTakeable = false;
        _lastTime = 0.0f;
        _photoCount = 0;

        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void Update()
    {
        if (!_screenshotsTakeable) return;

        _screenshotsTakeable = false;
        _photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
    }

    void Stop()
    {
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    private void OnPhotoCaptureCreated(PhotoCapture photoCaptureObject)
    {
        if (photoCaptureObject == null)
        {
            Debug.LogError("Photo Capture could not be created");
        }

        Debug.LogError("Photo Capture created");
        _photoCaptureObject = photoCaptureObject;

        var supportedResolutions = (Resolution[])PhotoCapture.SupportedResolutions;

        // emulator cannot take photos
        if (supportedResolutions.Length == 0)
        {
            Debug.LogError("Photo mode could not be started. Are you using an emulator?");
            _photoCaptureObject.Dispose();
            _photoCaptureObject = null;
            return;
        }

        // needed for starting photo mode
        _cameraParameters = new CameraParameters();
        _cameraParameters.hologramOpacity = 0.0f;
        _cameraParameters.cameraResolutionWidth = _cameraResolution.width;
        _cameraParameters.cameraResolutionHeight = _cameraResolution.height;
        _cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

        // Activate the web camera
        _photoCaptureObject.StartPhotoModeAsync(_cameraParameters, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.LogError("Photo Mode started");
            _screenshotsTakeable = true;

            // send event if there are subscribers
            var handler = ScreenshotsTakeable;
            if (handler != null) handler.Invoke(this, new EventArgs());
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

            // freeing up memory
            Texture.Destroy(_imageAsTextureTmp);

            // save photograph to texture
            _imageAsTextureTmp = new Texture2D(_cameraResolution.width, _cameraResolution.height);
            photoCaptureFrame.UploadImageDataToTexture(_imageAsTextureTmp);

            // position of camera/user at time of capturing screenshot
            photoCaptureFrame.TryGetCameraToWorldMatrix(out _cameraToWorldMatrixTmp);
            photoCaptureFrame.TryGetProjectionMatrix(out _projectionMatrixTmp);

            // measuring captured frames per second
            if (_lastTime == 0)
            {
                _lastTime = Time.time;
            }
            if (Time.time - _lastTime < 1.0f)
            {
                _photoCount++;
            }
            else
            {
                // Debug.LogError("Photos per s: " + _photoCount);
                _lastTime = Time.time;
                _photoCount = 0;
            }

            // send event if there are subscribers
            var handler = ScreenshotTaken;
            if (handler != null) handler.Invoke(this, new EventArgs());
        }

        this._screenshotsTakeable = true;
    }

    public void GetLatestPicture(out Texture2D picture, out Matrix4x4 cameraToWorldMatrix, out Matrix4x4 projectionMatrix)
    {
        picture = _imageAsTextureTmp;
        cameraToWorldMatrix = _cameraToWorldMatrixTmp;
        projectionMatrix = _projectionMatrixTmp;
    }

    // called when photo mode is stopped
    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        // Shutdown photo capture resource
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }
}