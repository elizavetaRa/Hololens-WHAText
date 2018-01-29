using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RequestBoxScript : MonoBehaviour {

    // Use this for initialization
    private GameObject requestBox;
    private Text text1;
    private Text text2;
    private Text text3;

    void Start()
    {
        Debug.Log("requestBox made");
        //requestBox = this.gameObject.transform.Find("requestBox").gameObject;
        //text1 = this.gameObject.transform.Find("Text1").gameObject.GetComponent<Text>();
        //text2 = this.gameObject.transform.Find("Text2").gameObject.GetComponent<Text>();
        //text3 = this.gameObject.transform.Find("Text3").gameObject.GetComponent<Text>();
        //requestBox.transform.localScale = new Vector3(0, 0, 0);

    }

    void OnFocus()
    {

        //textAreaBox.transform.localScale = textAreaBoxSize;
        // textAreaBox.GetComponent<SpriteRenderer>().color = Color.blue;

    }

    void SetText(string text)
    {
        text1.text = text;

    }

    void OnDefocus()
    {
        //textAreaBox.transform.localScale = new Vector3(0, 0, 0);

        //textAreaBox.GetComponent<SpriteRenderer>().color = originalColor;
    }

    void OnClick()
    {
        //textAreaBox.GetComponent<SpriteRenderer>().color = Color.green;
        //this.gameObject.SetActive(false);
        //textPrefab.text = ("HI!");
        Debug.Log("requestBox clicked");
    }
    // Update is called once per frame
    void Update()
    {
    }
}
