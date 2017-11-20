using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (!UNITY_EDITOR)
using System.Diagnostics;
#endif

public class ExecuteApi : MonoBehaviour {

	// Use this for initialization
	void Start () {
        /*
        ApiManager ApiManager = new ApiManager(OcrService.MICROSOFTMEDIAOCR);
        ApiManager.StartRequest();*/
        ApiMicrosoftMediaOcr Api = ApiMicrosoftMediaOcr.Instance;
        System.Diagnostics.Debug.WriteLine(Api);
	}
	
	// Update is called once per frame
	void Update () {
#if (!UNITY_EDITOR)
        System.Diagnostics.Debug.WriteLine("Test");
#endif
    }
}
