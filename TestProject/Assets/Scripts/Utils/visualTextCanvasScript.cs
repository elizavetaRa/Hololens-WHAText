using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class visualTextCanvasScript : MonoBehaviour {

    private Text text;
    private SpriteRenderer textAreaBox;
    private Vector3 textAreaBoxSize;

    void Start()
    {
        text = this.gameObject.transform.Find("Text").gameObject.GetComponent<Text>();
        textAreaBox = this.gameObject.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
        textAreaBoxSize = textAreaBox.transform.localScale;
        //originalColor = textAreaBox.GetComponent<SpriteRenderer>().color;
        textAreaBox.transform.localScale = new Vector3(0, 0, 0);

    }

    void OnFocus()
    {
        // Debug.Log(textPrefab.text);
        textAreaBox.transform.localScale = textAreaBoxSize;
        // textAreaBox.GetComponent<SpriteRenderer>().color = Color.blue;

    }
    void OnDefocus()
    {
        textAreaBox.transform.localScale = new Vector3(0, 0, 0);

        //textAreaBox.GetComponent<SpriteRenderer>().color = originalColor;
    }
}
