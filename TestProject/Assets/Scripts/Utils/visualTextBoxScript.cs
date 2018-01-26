using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextBoxScript : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Image textAreaBox;
    private Vector3 originalSize;
    // Use this for initialization
    void Start () {

        textAreaBox = this.gameObject.GetComponent<Image>();
        originalSize = textAreaBox.transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void onThis()
    {

        Debug.Log("message received");
        textAreaBox.color = Color.blue;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("message received");
        textAreaBox.color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textAreaBox.color = Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onThis();
    }
}
