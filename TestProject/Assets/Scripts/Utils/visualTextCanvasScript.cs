using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class visualTextCanvasScript : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler{

    private Text text;
    private SpriteRenderer textAreaBox;
    private Vector3 originalSize;

    void Start()
    {
        //text = this.gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
        textAreaBox = this.gameObject.GetComponentInChildren<SpriteRenderer>();
        originalSize = textAreaBox.transform.localScale;

        //originalColor = textAreaBox.GetComponent<SpriteRenderer>().color;
        //textAreaBox.transform.localScale = new Vector3(0, 0, 0);
    }

    void OnFocus()
    {

        Debug.Log("message received");
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

        Debug.Log("message received");
        textAreaBox.color = Color.blue;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        textAreaBox.color = Color.red;
    }

}
