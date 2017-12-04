using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class visualTextManager : Singleton<visualTextManager> {
    public string dummy = "was?";
    public GameObject textArea;
	// Use this for initialization
	void Start () {
        visualizeText(dummy);
	}

    private void visualizeText(string dummyText)
    {
        GameObject newArea = Instantiate(textArea);
        TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
        visualText.text = dummyText;
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
