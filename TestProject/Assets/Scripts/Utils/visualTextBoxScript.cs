using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextBoxScript : MonoBehaviour
{

    private Image textAreaBox;
    private Text textObject;
    private Vector3 originalSize;
    // Use this for initialization
    void Start () {
        textAreaBox = this.gameObject.GetComponent<Image>();
        originalSize = textAreaBox.transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnFocus()
    {

    }


}
