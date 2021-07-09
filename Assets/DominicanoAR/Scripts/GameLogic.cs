using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public CanvasManager canvas;
    public GameObject fichasObject;
    private FichaScript[] fichas;
    public GameObject P1Object;
    public GameObject P2Object;
    public GameObject P3Object;
    public GameObject P4Object;
    private SlotsController[] P1Slots;
    private SlotsController[] P2Slots;
    private SlotsController[] P3Slots;
    private SlotsController[] P4Slots;
    private PlayerLogic[] players;
    private int team1Points;
    private int team2Points;
    private int playerTurn = 0;
    public PlayFichaScript playSlots;
    public bool start;

    // Start is called before the first frame update
    void Start()
    {
        team1Points = 0;
        team2Points = 0;
        canvas = FindObjectOfType<CanvasManager>();
        fichas = fichasObject.GetComponentsInChildren<FichaScript>();
        P1Slots = P1Object.GetComponentsInChildren<SlotsController>();
        P2Slots = P2Object.GetComponentsInChildren<SlotsController>();
        P3Slots = P3Object.GetComponentsInChildren<SlotsController>();
        P4Slots = P4Object.GetComponentsInChildren<SlotsController>();
        players = this.gameObject.GetComponentsInChildren<PlayerLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if(playing().myTurn == false)
            {
                if(checkGameEnded())
                {
                    endGame();
                }
                else
                {
                    playerTurn++;
                    if (playerTurn > 3)
                        playerTurn = 0;

                    players[playerTurn].myTurn = true;

                    canvas.OpenMessagePanel("Player " + (playerTurn + 1) + " turn");

                    canvas.togglePassBtn(true);
                    if (players[playerTurn].AIPlayer)
                    {
                        canvas.togglePassBtn(false);
                        StartCoroutine(ExecuteAfterTime(1f));
                    }
                }
                
            }
        }
    }

    public void startGame()
    {
        canvas.OpenMessagePanel("Player " + (playerTurn+1) + " turn");
        this.RepartirFichas();
        playerTurn = 0;
        players[playerTurn].myTurn = true;
        start = true;
        canvas.togglePassBtn(true);
    }

    public void endGame()
    {
        this.start = false;
        canvas.OpenMessagePanel("Game Ended!");
    }

    private void addPointsToTeam(ref int winnerTeamPoints)
    {
        int points = 0;

        List<FichaScript> fichasLeft = getFichasLeft();
        foreach (FichaScript ficha in fichasLeft)
        {
            points += ficha.values[0];
            points += ficha.values[1];
        }

        winnerTeamPoints += points;
        canvas.team1Score.text = team1Points.ToString();
        canvas.team1Score.text = team2Points.ToString();

    }

    public void passTurn(){
        playing().myTurn = false;
    }

    public PlayerLogic playing()
    {
        return players[playerTurn];
    }

    private bool checkGameEnded()
    {
        ref int points = ref team2Points;
        if(playing().number == 1 || playing().number == 3)
            points = ref team1Points;

        if(checkGameBlockade())
        {
            List<FichaScript> blockerFichas = playing().fichasLeft();
            List<FichaScript> blockedFichas = players[playerTurn+1].fichasLeft();

            int blockerCounter = 0;
            int blockedCounter = 0;

            foreach (FichaScript ficha in blockerFichas)
            {
                blockerCounter += ficha.values[0];
                blockerCounter += ficha.values[1];
            }
            foreach (FichaScript ficha in blockedFichas)
            {
                blockedCounter += ficha.values[0];
                blockedCounter += ficha.values[1];
            }

            if(blockerCounter >= blockedCounter)
                addPointsToTeam(ref team1Points);
            else
                addPointsToTeam(ref team2Points);

            return true;
        }
        else if(!checkPlayerHasFichas()){

            addPointsToTeam(ref points);
            return true;
        }
        return false;
    }

    private bool checkPlayerHasFichas()
    {

        if(playing().fichasLeft().ToArray().Length > 0)
            return true;

        return false;
    }

    private List<FichaScript> getFichasLeft()
    {
        List<FichaScript> fichasLeft = new List<FichaScript>();
        foreach (FichaScript ficha in fichas)
        {
            if (ficha.played)
                continue;

            fichasLeft.Add(ficha);
        }
        return fichasLeft;
    }
    private bool checkGameBlockade()
    {
        List<FichaScript> fichasLeft = getFichasLeft();
        
        foreach (FichaScript ficha in fichasLeft)
        {
            int[] northRes = ficha.CompareFichaValues(playSlots.playValues[1]);
            if (northRes[0] == 1 || northRes[1] == 1)
            {
                return false;
            }
            
            int[] southRes = ficha.CompareFichaValues(playSlots.playValues[0]);
            if (southRes[0] == 1 || southRes[1] == 1)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Code to execute after the delay
        players[playerTurn].Play();
    }

    private void RepartirFichas()
    {
        players[0].removeFichas();
        players[1].removeFichas();
        players[2].removeFichas();
        players[3].removeFichas();

        // re-Activate fichas collition for grabbing after game reset
        foreach (FichaScript ficha in fichas)
        {
            ficha.ActiveCollition();
        }

        // Shuffle fichas and re-activate its collision
        for (int i = fichas.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            FichaScript temp = fichas[i];
            fichas[i] = fichas[randomIndex];
            fichas[randomIndex] = temp;
        }

        //Delete this on production
        for (int i = fichas.Length - 1; i > 0; i--)
        {
            int[] arr = fichas[i].CompareFichaValues(6);
            if (arr[0] == 1 && arr[1] == 1)
            {
                FichaScript temp = fichas[0];
                fichas[0] = fichas[i];
                fichas[i] = temp;
                break;
            }
        }

        // 7 Fichas for each player, "J" reprecents each player and "I" represents each slot.
        for (int i = 0, j = 0; i < 7; i++, j += 4)
        {
            //Position each ficha
            fichas[j].MoveTo(P1Slots[i].gameObject);
            fichas[j + 1].MoveTo(P2Slots[i].gameObject);
            fichas[j + 2].MoveTo(P3Slots[i].gameObject);
            fichas[j + 3].MoveTo(P4Slots[i].gameObject);

            //Give fichas to each player
            players[0].addFicha(fichas[j]);
            players[1].addFicha(fichas[j+1]);
            players[2].addFicha(fichas[j+2]);
            players[3].addFicha(fichas[j+3]);

        }

        // Remove all played slots that were been used on the prev game
        PlaySlot[] allPlayslots = FindObjectsOfType<PlaySlot>();
        foreach (PlaySlot slot in allPlayslots)
        {
            Destroy(slot.gameObject);
        }

        playSlots.ActivateEnterSlot();
    }

    private void ShowGame()
    {

        HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
        HandSide handside = handInfo.gesture_info.hand_side;

        //TODO: Mostrar las fichas jugadas frente a la mano del jugador.
        if (handside == HandSide.Palmside)
        {

        }
    }
}
