using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    private GameObject messagePanel;
    public Text textMessage;
    public Text playText;
    public Text team1Score;
    public Text team2Score;
    public GameObject passBtn;
    public GameObject playBtn;
    public GameObject menuPanel;
    public GameObject SocketObject;
    public SocketManager sm;

    void Start()
    {
        messagePanel = transform.GetChild(0).gameObject;

        print(Application.platform);
        if(Application.platform == RuntimePlatform.WebGLPlayer){
            GameObject temp = GameObject.Find("/SocketManager");
            Destroy(temp.GetComponent<SocketManager>());
            sm = SocketObject.GetComponent<WebSocketManager>();
        }
        else{
            GameObject temp = GameObject.Find("/SocketManager");
            Destroy(temp.GetComponent<WebSocketManager>());
            sm = SocketObject.GetComponent<SocketManager>();
        }
    
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

    public void togglePassBtn(bool value)
    {
        passBtn.SetActive(value);
    }
    public void showPlayBtn(bool value)
    {
        playBtn.SetActive(value);
    }

    public void reloadScene(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void onlinePass(){
        if(SocketManager.online){
            sm.passTurn();
        }
    }

    public void Host(){
        sm.StartHost();
        menuPanel.SetActive(false);
    }
    public void Join(){
        sm.StartClient();
        menuPanel.SetActive(false);
    }

    public void toggleMenu(){
        menuPanel.SetActive(!menuPanel.activeSelf);
    }
    
}
