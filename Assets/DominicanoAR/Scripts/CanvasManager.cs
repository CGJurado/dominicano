using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    private GameObject messagePanel;
    public Text textMessage;
    // Start is called before the first frame update
    void Start()
    {
        messagePanel = transform.GetChild(0).gameObject;
    }

    public void OpenMessagePanel(string txt)
    {
        if (!messagePanel.activeInHierarchy)
            messagePanel.SetActive(true);

        textMessage.text = txt;
    }

    public void CloseMessagePanel()
    {
        messagePanel.SetActive(false);
    }
}
