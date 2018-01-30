using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextCanvasScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{ 
    private Text text;
    private Image textAreaBox;
    private Vector3 originalSize;

    void Start()
    {
        //text = this.gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
        textAreaBox = this.gameObject.GetComponentInChildren<Image>();
        originalSize = textAreaBox.transform.localScale;
        //Debug.Log("started");
        //originalColor = textAreaBox.GetComponent<SpriteRenderer>().color;
        textAreaBox.transform.localScale = new Vector3(0, 0, 0);
    }

    void OnFocus()
    {

    }
    void OnDefocus()
    {
        //textAreaBox.transform.localScale = new Vector3(0, 0, 0);

        //textAreaBox.color = Color.red;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //Debug.Log("message received PointerEnter Canvas");
        //textAreaBox.color = Color.blue;
        textAreaBox.transform.localScale = originalSize;
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        textAreaBox.transform.localScale = new Vector3(0, 0, 0);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    //   textAreaBox.color = Color.red;
    

    }

}
