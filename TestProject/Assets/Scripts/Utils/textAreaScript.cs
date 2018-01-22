using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAreaScript : MonoBehaviour {

	// Use this for initialization
    private TextMesh textPrefab;
    private GameObject textAreaBox;
    private Vector3 textAreaBoxSize;
    private Color originalColor;
	void Start () {
        textPrefab = this.gameObject.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
        textAreaBox = this.gameObject.transform.Find("textAreaBox").gameObject;
        textAreaBoxSize = textAreaBox.transform.localScale;
        //originalColor = textAreaBox.GetComponent<SpriteRenderer>().color;
        // textAreaBox.transform.localScale = new Vector3(0, 0, 0);
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
        textAreaBox.transform.localScale = new Vector3(0,0,0);

        //textAreaBox.GetComponent<SpriteRenderer>().color = originalColor;
    }

    void OnClick()
    {
        //textAreaBox.GetComponent<SpriteRenderer>().color = Color.green;
        this.gameObject.SetActive(false);
        textPrefab.text = ("HI!");
        //Debug.Log(textPrefab.text);
    }
	// Update is called once per frame
	void Update () {
	}
}
