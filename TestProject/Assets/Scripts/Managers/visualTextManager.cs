using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class visualTextManager : Singleton<visualTextManager>
{
    public string dummy = "WAS?";
    public GameObject textArea;
    public GameObject OcrResult;
    public GameObject SpatialMapping;
    // Use this for initialization
    void Start()
    {
        //visualizeText(dummy);
    }

    private void visualizeText(string dummyText)
    {

        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            visualText.text = dummyText;
            newArea.transform.position = headPosition;

            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}



