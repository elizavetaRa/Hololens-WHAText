using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAreaScript : MonoBehaviour {

	// Use this for initialization
    private TextMesh textPrefab;
    private GameObject textAreaBox;
    private Vector3 textAreaBoxSize;
    private 
	void Start () {
        textPrefab = this.gameObject.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
        textAreaBox = this.gameObject.transform.Find("textAreaBox").gameObject;
        textAreaBoxSize = textAreaBox.transform.localScale;
       // textAreaBox.transform.localScale = new Vector3(0, 0, 0);

    }

    void OnFocused()
    {
       // Debug.Log(textPrefab.text);
        //textAreaBox.transform.localScale = textAreaBoxSize;
        textAreaBox.GetComponent<SpriteRenderer>().color = Color.blue;
    }
    void OnDefocused()
    {
        //textAreaBox.transform.localScale = new Vector3(0,0,0);
       
        textAreaBox.GetComponent<SpriteRenderer>().color = Color.red;
    }

    void onClicked()
    {
        Debug.Log(textPrefab.text);
    }
	// Update is called once per frame
	void Update () {
	}
}
