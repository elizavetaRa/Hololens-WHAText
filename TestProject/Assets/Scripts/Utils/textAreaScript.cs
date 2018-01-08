using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class textAreaScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    void OnFocused()
    {
        TextMesh textBox = this.gameObject.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
        Debug.Log(textBox.text);
    }
	// Update is called once per frame
	void Update () {
		
	}
}
