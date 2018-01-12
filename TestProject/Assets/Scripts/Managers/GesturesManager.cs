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

    // Use this for initialization
    void Start()
    {



        // create an new gesture recognizer to detect when user taps to shoot a screenshot
        gestureRecognizer = new GestureRecognizer();
        gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
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

        // send log message and set waiting mode
        System.Diagnostics.Debug.WriteLine("Tap was recognized.");

        // send event to Controller
#if (!UNITY_EDITOR)
        TapEventArgs args = new TapEventArgs();
        args.RequestCause = RequestCause.USERINITIATED;
        var handler = Tapped;
        if (handler != null) handler.Invoke(this, args);
#endif

    }

}

public class TapEventArgs : EventArgs
{
    public RequestCause RequestCause { get; set; }
}
