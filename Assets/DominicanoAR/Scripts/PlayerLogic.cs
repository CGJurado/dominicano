using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public int number;
    private List<FichaScript> fichas = new List<FichaScript>();
    public GameObject northAISlot;
    public GameObject southAISlot;
    public bool myTurn = false;
    public bool done = false;
    private PlaySlotsController northController;
    private PlaySlotsController enterController;
    private PlaySlotsController southController;
    private PlayFichaScript playSLots;
    private FichaScript foundFicha;
    public bool AIPlayer;


    private void Update()
    {
        if (myTurn)
        {
            if (AIPlayer)
            {
                if (foundFicha)
                    this.Play();

            }

            if (done)
            {
                myTurn = false;
                done = false;
                foundFicha = null;
            }
                
        }
    }

    public void addFicha(FichaScript ficha)
    {
        fichas.Add(ficha);
    }

    public void removeFichas()
    {
        fichas = new List<FichaScript>();
        myTurn = false;
    }


    public void Play()
    {
        if (!foundFicha)
        {
            checkFichas();
        }
        else
        {
            if (foundFicha.doneAnimation)
            {
                FindObjectOfType<CanvasManager>().OpenMessagePanel("P" + number + " PlayF: " + foundFicha.values[0] + " : " + foundFicha.values[1]);
                if (foundFicha.transform.position == northAISlot.transform.position || foundFicha.transform.position == southAISlot.transform.position)
                    foundFicha.CheckSlot();
                else if (foundFicha.played)
                    done = true;


            }
        }
    }

    private void checkFichas()
    {
        playSLots = FindObjectOfType<PlayFichaScript>();
        int[] values = playSLots.playValues;
        FichaScript[] myFichas = fichas.ToArray();

        foreach (FichaScript ficha in myFichas)
        {
            if (ficha.played)
                continue;

            int[] northRes = ficha.CompareFichaValues(values[1]);
            if (northRes[0] == 1 || northRes[1] == 1)
            {
                FindObjectOfType<CanvasManager>().OpenMessagePanel("AIFound: " + ficha.values[0] + " : " + ficha.values[1]);
                foundFicha = ficha;
                foundFicha.MoveTo(northAISlot);
                return;
            }
            int[] southRes = ficha.CompareFichaValues(values[0]);
            if (southRes[0] == 1 || southRes[1] == 1)
            {
                FindObjectOfType<CanvasManager>().OpenMessagePanel("AIFound: " + ficha.values[0] + " : " + ficha.values[1]);
                foundFicha = ficha;
                foundFicha.MoveTo(southAISlot);
                return;
            }
        }

        if (!foundFicha)
        {
            FindObjectOfType<CanvasManager>().OpenMessagePanel("AIFound: Pass");
            done = true;
        }
        
    }
}
