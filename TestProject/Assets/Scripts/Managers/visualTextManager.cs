using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VisualTextManager : Singleton<VisualTextManager>
{
    public string dummy = "Hundefutter";
    public GameObject textArea;
    public GameObject OcrResult;
    public GameObject SpatialMapping;
    // Use this for initialization
    void Start()
    {
        //visualizeText(dummy);
    }


    // Update is called once per frame
    void Update()
    {

    }

    internal void visualizeText(string someText)
    { // visualText.text = dummyText;
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            visualText.text = someText;
            newArea.transform.position = headPosition;

            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;
        }
    }
}



