using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextBoxScript : MonoBehaviour //, IPointerClickHandler
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

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    //   textAreaBox.color = Color.red;
    //    //string text = this.gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text;
    //    //Controller.Instance.selectedWordsList.Add(text);
    //    Debug.Log("tesstte");
    //}


}
