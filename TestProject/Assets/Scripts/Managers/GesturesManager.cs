using HoloToolkit.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.WSA.Input;



///Demonstrates how to take a photo using the PhotoCapture functionality and display it on a Unity GameObject.
/// <summary> Class responsible for taking screenshots </summary>
public class GesturesManager: Singleton<GesturesManager> {


    /// <summary>
    /// recognizes user's air tap
    /// </summary>
    private GestureRecognizer gestureRecognizer;
    public event EventHandler<TapEventArgs> Tapped;
    public event EventHandler<DoubleTapEventArgs> DoubleTapped;
    const float DELAY = .5f;

    public AudioClip singleTapSound, doubleTapSound;
    private AudioSource source;

    // Use this for initialization
    void Start()
    {
        // init audio source
        source = GetComponent<AudioSource>();

        // create an new gesture recognizer to detect when user taps to shoot a screenshot
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap | GestureSettings.DoubleTap);
        gestureRecognizer.TappedEvent += OnTap;
        gestureRecognizer.StartCapturingGestures();


    }


    /// <summary>
    /// called whenever the user performs an air tap gesture
    /// </summary>
    /// <param name="source"> source of the interaction </param>
    /// <param name="tapCount"> count of the tap </param>
    /// <param name="headRay"> ray of the head </param>
    private void OnTap(InteractionSourceKind source, int tapCount, Ray headRay)
    {

        if (tapCount == 1)
            Invoke("SingleTap", DELAY);
        else if (tapCount == 2)
        {
            CancelInvoke("SingleTap");
            DoubleTap();
        }

        
    }

    void SingleTap()
    {
        Debug.LogError("SingleTap was recognized.");
       // source.PlayOneShot(singleTapSound);
        // select words
#if (!UNITY_EDITOR)
        // send event to Controller
        TapEventArgs args = new TapEventArgs();

        args.Word = "garlic";
        var handler = Tapped;
        if (handler != null) handler.Invoke(this, args);
#endif
    }

    void DoubleTap()
    {
        Debug.LogError("DoubleTap was recognized.");
        //source.PlayOneShot(doubleTapSound);
#if (!UNITY_EDITOR)
        // send event to Controller
        DoubleTapEventArgs args = new DoubleTapEventArgs();
        args.RequestCause = RequestCause.USERINITIATED;
        var handler = DoubleTapped;
        if (handler != null) handler.Invoke(this, args);
#endif
    }

}

public class TapEventArgs : EventArgs
{
    public String Word { get; set; }
}

public class DoubleTapEventArgs : EventArgs
{
    public RequestCause RequestCause { get; set; }
}
