using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour {

    public Text notificationText;
    public Image iconImage;
    public GameObject dialogPanelObject;
    public int displayTime;

    private float initialSpeedOpeningUpPanel, speedOpeningUpPanel;
    private static DialogPanel dialogPanel;
    private bool openingUpPanel;
    private Vector3 initialAnchoredPosition;
    private List<string> enqueuedNotifications;


    public static DialogPanel Instance()
    {
        if (!dialogPanel)
        {
            dialogPanel = FindObjectOfType(typeof(DialogPanel)) as DialogPanel;
            if (!dialogPanel)
                Debug.LogError("There needs to be one active DialogPanel script on a GameObject in your scene");
        }

        return dialogPanel;
    }

    void Awake()
    {
        initialAnchoredPosition = dialogPanelObject.GetComponent<RectTransform>().anchoredPosition;
        initialSpeedOpeningUpPanel = 0.1f;
        speedOpeningUpPanel = initialSpeedOpeningUpPanel;
        enqueuedNotifications = new List<string>();
        openingUpPanel = false;
    }

    void Update()
    {
        if (this.enqueuedNotifications.Count > 0 && !this.dialogPanelObject.activeSelf)
            this.OpenUpPanel(this.enqueuedNotifications[0]);

        // animating panel popup over time
        if (openingUpPanel)
        {
            if (dialogPanelObject.GetComponent<RectTransform>().anchoredPosition.y < 10)
                speedOpeningUpPanel = initialSpeedOpeningUpPanel - (initialSpeedOpeningUpPanel / 3f);
            if (dialogPanelObject.GetComponent<RectTransform>().anchoredPosition.y < 0)
            {
                openingUpPanel = false;
                return;
            }
            dialogPanelObject.transform.Translate(0, -Time.deltaTime*speedOpeningUpPanel, 0);
        }
    }

    public void enqueueNotification(string notificationText)
    {
        this.enqueuedNotifications.Add(notificationText);
    }

    public void OpenUpPanel(string notificationText)
    {
        // remove currently processed notification
        this.enqueuedNotifications.RemoveAt(0);

        this.openingUpPanel = true;
        dialogPanelObject.SetActive(true);

        this.notificationText.text = notificationText;

        StartCoroutine(WaitAndExecute(displayTime, ClosePanel));

    }

    IEnumerator WaitAndExecute(float timer, Action cb = null)
    {
        yield return new WaitForSeconds(timer);
        if (cb != null) cb();
    }

    void ClosePanel()
    {
        dialogPanelObject.SetActive(false);
        dialogPanelObject.GetComponent<RectTransform>().anchoredPosition = initialAnchoredPosition;
        speedOpeningUpPanel = initialSpeedOpeningUpPanel;
    } 
	
}
