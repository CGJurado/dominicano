using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class WebSocketManager : SocketManager
{
    #if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void initSocket();

    [DllImport("__Internal")]
    private static extern void stopConnection();

    [DllImport("__Internal")]
    private static extern void initListeners();

    [DllImport("__Internal")]
    private static extern void goRoom(string data);

    [DllImport("__Internal")]
    private static extern void sendMsg(string data);

    [DllImport("__Internal")]
    private static extern void sendFicha(string data);

    [DllImport("__Internal")]
    private static extern void restartGame(string data);

    [DllImport("__Internal")]
    private static extern void finishGame(string data);

    [DllImport("__Internal")]
    private static extern void emitPass(string data);
    #else
    [DllImport("comunication.jslib")]
    private static extern void initSocket();

    [DllImport("comunication.jslib")]
    private static extern void stopConnection();

    [DllImport("comunication.jslib")]
    private static extern void initListeners();

    [DllImport("comunication.jslib")]
    private static extern void goRoom(string data);

    [DllImport("comunication.jslib")]
    private static extern void sendMsg(string data);

    [DllImport("comunication.jslib")]
    private static extern void sendFicha(string data);

    [DllImport("comunication.jslib")]
    private static extern void restartGame(string data);

    [DllImport("comunication.jslib")]
    private static extern void finishGame(string data);

    [DllImport("comunication.jslib")]
    private static extern void emitPass(string data);
    #endif

    public override void Start()
    {
        mainPlayer = new Player();
        
        print("Inside webSocket Start");
    }
    public override void Update(){

    }

    public override void endGame(int team1Points, int team2Points){
        print("Inside webSocket emitFichaPos");
        Points tempPoints = new Points();
        tempPoints.team1Points = team1Points;
        tempPoints.team2Points = team2Points;
        string data = JsonUtility.ToJson(tempPoints);
        
        finishGame(data);
    }
    public override void emitFichaToObj(FichaMoveResponse pos){
        print("Inside webSocket emitFichaPos");
        
        string data = JsonUtility.ToJson(pos);
        sendFicha(data);
    }
    public override void resetGame(){
        restartGame(mainPlayer.username);
    }

    public override void passTurn(){
        print("passTurn");
        
        string data = JsonUtility.ToJson(mainPlayer);
        print("passTurn after");
        emitPass(data);
    }

    public override void StartHost(){
        print("Inside webSocket StartHost");
        
        string data = JsonUtility.ToJson(mainPlayer);
        sendMsg(data);
    }

    public override void StartClient(Player player){
        initSocket();
        initListeners();
        online = true;

        mainPlayer = player;
        Invoke("joinRoom",1);
    }

    public override void disconnectClient(){
        online = false;

        stopConnection();

    }

    private void joinRoom(){
        print("Inside webSocket StartClient");
        string data = JsonUtility.ToJson(mainPlayer);
        goRoom(data);
    }
}
