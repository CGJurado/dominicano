using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SocketManager : SocketIOComponent
{
    public static bool online {get; set;} = false;
    public static Player mainPlayer;

    public GameLogic game;

    public override void Start()
    {
        mainPlayer = new Player();

    }

    public override void Update()
    {
        if(online)
            base.Update();
    }

    private void setupEvents(){
        On("open", (ev) => {
            print("Connection made to the server");
        });
        On("chat", ev => {
            msgResponse(ev.data.ToString());
        });
        On("joined", (ev) => {
            joinedRoom(ev.data.ToString());
        });
        On("disconnected", ev => {
            playerDisconnected(ev.data.ToString());
        });
        On("spawn", ev => {
            spawnPlayer(ev.data.ToString());
            // string pNumber = RemoveQuotes(ev.data["number"].ToString());
        });
        On("fichaMoved", ev => {
            fichaMoved(ev.data.ToString());
        });
        On("startGame", ev => {
            startGame(ev.data.ToString());
        });
        On("gameEnded", ev => {
            gameEnded(ev.data.ToString());
        });
        On("playerPassed", ev => {
            playerPassed(ev.data.ToString());
        });

    }

    // static send new ficha pos
    public static void sendFichaToObj(FichaMoveResponse fichaRes){
        
        GameObject.FindObjectOfType<SocketManager>().emitFichaToObj(fichaRes);
        print("emitFichaToObj executed!");
    }

    // Common Functions
    public void startGame(string json){
        print("StartGame response!");
        game.startGame();
    }
    public void gameEnded(string json){
        print("StartGame response!");
        Points tempPoints = new Points();
        tempPoints = JsonUtility.FromJson<Points>(json);
        game.endGame();
        game.addPoints(tempPoints);
    }
    public void playerPassed(string json){
        print("a player passed!");

        game.passTurn();
    }
    public void fichaMoved(string json){
        FichaMoveResponse fichaMoveRes = new FichaMoveResponse();
        fichaMoveRes = JsonUtility.FromJson<FichaMoveResponse>(json);

        print("fichaMoveRes!!!!!");
        print(fichaMoveRes.AI);

        FichaScript ficha = game.findFichaByName(fichaMoveRes.fichaName);
        GameObject obj = GameObject.Find(fichaMoveRes.objName);

        ficha.MoveTo(obj);
        GameLogic.playing().recieveFicha(ficha);
        

        // if((obj.name == "North" || obj.name == "South") && fichaMoveRes.AI && mainPlayer.number != 0){
        //     print("inside IF");
        //     StartCoroutine(ExecuteAfterTime(1f));
        // }
    }
    
    private IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        GameLogic.playing().done = true;
    }

    public void msgResponse(string json){
        Player tempPlayer = new Player();
        tempPlayer = JsonUtility.FromJson<Player>(json);
        print("Message::");
        print(tempPlayer.username);
        print(tempPlayer.roomName);
    }
    public void joinedRoom(string json){
        mainPlayer = JsonUtility.FromJson<Player>(json);
        print("I Joined: "+ mainPlayer.roomName);
        
        game.onlineGame(mainPlayer);

    }
    public void spawnPlayer(string json){
        Player tempPlayer = new Player();
        tempPlayer = JsonUtility.FromJson<Player>(json);

        game.addOnlinePlayer(tempPlayer);

        print(tempPlayer.username+" is here");
        game.startGame();
    }
    public void playerDisconnected(string json){
        //TODO: Remove the player model corresponding for this
        
        Player tempPlayer = new Player();
        tempPlayer = JsonUtility.FromJson<Player>(json);

        game.removeOnlinePlayer(tempPlayer);
    }
    
    //Virtual Functions
    public virtual void endGame(int team1Points, int team2Points){
        Points tempPoints = new Points();
        tempPoints.team1Points = team1Points;
        tempPoints.team2Points = team2Points;
        Emit("endGame", new JSONObject(JsonUtility.ToJson(tempPoints)));
    }
    public virtual void emitFichaToObj(FichaMoveResponse pos){
        print("Inside SocketManager emitFichaPos");
        Emit("moveFicha", new JSONObject(JsonUtility.ToJson(pos)));
    }
    public virtual void resetGame(){
        Emit("resetGame", mainPlayer.username);
    }

    public virtual void passTurn(){
        Emit("passTurn", new JSONObject(JsonUtility.ToJson(mainPlayer)));
    }

    public virtual void StartHost(){
        Emit("chat", new JSONObject(JsonUtility.ToJson(mainPlayer)));
    }

    public virtual void StartClient(Player player){
        base.Start();
        setupEvents();
        online = true;
        
        mainPlayer = player;
        Invoke("joinRoom",1);
    }

    public virtual void disconnectClient(){
        online = false;
    }

    private void joinRoom(){
        Emit("join", new JSONObject(JsonUtility.ToJson(mainPlayer)));
    }
    public string RemoveQuotes(string Value) {
        return Value.Replace("\"", "");
    }
}

[System.Serializable]
public class FichaMoveResponse{
    public string fichaName;
    public int playerNumber;
    public bool AI;
    public string objName;
}

[System.Serializable]
public class Points{
    public int team1Points;
    public int team2Points;
}