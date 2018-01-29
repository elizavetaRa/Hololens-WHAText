using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextBoxScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
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
        Debug.Log("focused");
    }
    public void onThis()
    {

        Debug.Log("message received OnThis Box");
        textAreaBox.color = Color.blue;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("message received internal onPointerEnter Box");
        //textAreaBox.color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("message received internal onPointerExit Box");
        textAreaBox.color = Color.red;
    }
    

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("message received onpointerup received");
        textAreaBox.color = Color.blue;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("message received OnClick Box");
        textAreaBox.color = Color.blue;
    }
}
