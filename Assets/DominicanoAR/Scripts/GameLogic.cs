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
    private int playerTurn = 0;
    public PlayFichaScript playSlots;
    public bool start;

    // Start is called before the first frame update
    void Start()
    {
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
            if(players[playerTurn].myTurn == false)
            {
                playerTurn++;
                if (playerTurn > 3)
                    playerTurn = 0;

                players[playerTurn].myTurn = true;

                canvas.OpenMessagePanel("Player " + (playerTurn + 1) + " turn");

                if (players[playerTurn].AIPlayer)
                    StartCoroutine(ExecuteAfterTime(1f));
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
    }

    public PlayerLogic playing()
    {
        return players[playerTurn];
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
