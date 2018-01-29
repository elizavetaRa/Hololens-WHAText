using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextCanvasScript : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
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
        //textAreaBox.transform.localScale = new Vector3(0, 0, 0);
    }

    void OnFocus()
    {

        Debug.Log("message received onFocus Canvas");
        //textAreaBox.transform.localScale = originalSize;
        textAreaBox.color= Color.blue;

    }
    void OnDefocus()
    {
        //textAreaBox.transform.localScale = new Vector3(0, 0, 0);

        textAreaBox.color = Color.red;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        //Debug.Log("message received PointerEnter Canvas");
        //textAreaBox.color = Color.blue;
    }
    public void OnPointerEnter()
    {

        Debug.Log("message received onpointerenter no args canvas");
        textAreaBox.color = Color.blue;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        textAreaBox.color = Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("pointerClick received canvas!");
        textAreaBox.color = Color.red;
    }


    public void onClick()
    {
        Debug.Log("onClick received canvas");
        textAreaBox.color = Color.red;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("message received onpointerup");
        textAreaBox.color = Color.blue;
    }
}
