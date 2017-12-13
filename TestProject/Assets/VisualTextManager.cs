using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class VisualTextManager : Singleton<VisualTextManager>
{
    public string dummy = "Hundefutter";
    public GameObject textArea;
    // Use this for initialization
    void Start()
    {
        //visualizeText(dummy);
    }


    // Update is called once per frame
    void Update()
    {

    }

    internal void visualizeText(string someText, Vector2 pixelCoords)
    { // visualText.text = dummyText;
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;
        var CameraToWorld = Camera.main.cameraToWorldMatrix;
        var Projection = Camera.main.projectionMatrix;
        float ImageWidth = Camera.main.pixelWidth;
        float ImageHeight = Camera.main.pixelHeight;

        Vector2 ImagePosZeroToOne = new Vector2(pixelCoords.x / ImageWidth, 1.0f - (pixelCoords.y / ImageHeight));
        Vector2 ImagePosProjected = ((ImagePosZeroToOne * 2.0f) - new Vector2(1, 1)); // -1 to 1 space
        Vector3 CameraSpacePos = UnProjectVector(Projection, new Vector3(ImagePosProjected.x,ImagePosProjected.y, 1));
        Vector3 WorldSpaceRayPoint1 = CameraToWorld.MultiplyVector(new Vector4(0, 0, 0, 1));// camera location in world space
        Vector3 WorldSpaceRayPoint2 = CameraToWorld.MultiplyVector(CameraSpacePos); // ray point in world space
       

        RaycastHit hitInfo;
        if (Physics.Raycast(WorldSpaceRayPoint1, WorldSpaceRayPoint2, out hitInfo))
        {
            Debug.Log("Raycast hit!");
            GameObject newArea = Instantiate(textArea);
            TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
            visualText.text = someText;
            newArea.transform.position = hitInfo.point; //new Vector3(headPosition.x, headPosition.y, headPosition.z + 3);

            Quaternion toQuat = Camera.main.transform.localRotation;
            toQuat.x = 0;
            toQuat.z = 0;
            newArea.transform.rotation = toQuat;
        }

        //GameObject newArea = Instantiate(textArea);
        //TextMesh visualText = newArea.transform.Find("3DTextPrefab").gameObject.GetComponent<TextMesh>();
        //visualText.text = someText;
        //newArea.transform.position = new Vector3(headPosition.x, headPosition.y, headPosition.z + 2);

        //newArea.transform.rotation = Quaternion.LookRotation(gazeDirection);
    }
    public static Vector3 UnProjectVector(Matrix4x4 proj, Vector3 to)
    {
        Vector3 from = new Vector3(0, 0, 0);
        var axsX = proj.GetRow(0);
        var axsY = proj.GetRow(1);
        var axsZ = proj.GetRow(2);
        from.z = to.z / axsZ.z;
        from.y = (to.y - (from.z * axsY.z)) / axsY.y;
        from.x = (to.x - (from.z * axsX.z)) / axsX.x;
        return from;
    }
}



