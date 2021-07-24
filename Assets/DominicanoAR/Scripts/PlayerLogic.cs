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
    public PlaySlotsController northController;
    public PlaySlotsController enterController;
    public PlaySlotsController southController;
    private PlayFichaScript playSlots;
    private FichaScript foundFicha;
    public bool AIPlayer;
    private bool fichaGotClicked = false;


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
                fichaGotClicked = false;
                foundFicha = null;
                northController.unRender();
                enterController.unRender();
                southController.unRender();
                northController.gotClicked = false;
                southController.gotClicked = false;
                enterController.gotClicked = false;
            }

            if(Input.GetMouseButtonDown(0))
            {
                Camera[] allCameras = new Camera[2];
                Camera.GetAllCameras(allCameras);
                if(allCameras[1])
                {
                    Ray ray = allCameras[1].ScreenPointToRay(Input.mousePosition);
                    clickPlay(ray);
                }

            }
            
            // TODO: switch camera to player can select where to play from above
            // if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            //     clickPlay(ray);
            // }

            if(fichaGotClicked)
            {
                if(northController.gotClicked){
                    foundFicha.MoveTo(northAISlot);
                    fichaGotClicked = false;
                    northController.gotClicked = false;
                }
                else if(southController.gotClicked)
                {
                    foundFicha.MoveTo(southAISlot);
                    fichaGotClicked = false;
                    southController.gotClicked = false;
                }
                else if(enterController.gotClicked)
                {
                    foundFicha.MoveTo(southAISlot);
                    fichaGotClicked = false;
                    enterController.gotClicked = false;
                }

                this.Play();
            }
                
        }
    }

    private void clickPlay(Ray ray){
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            if (hit.transform != null && hit.collider.gameObject.tag == "Ficha")
            {
                foundFicha = hit.transform.gameObject.GetComponent<FichaScript>();
                if(foundFicha.canBePlayed()){

                    fichaGotClicked = true;
                    northController.render();
                    enterController.render();
                    southController.render();

                    Handheld.Vibrate();
                }
                else{
                    foundFicha = null;
                }

            }
        }
    }

    public void addFicha(FichaScript ficha)
    {
        fichas.Add(ficha);
    }

    public void removeFichas()
    {
        myTurn = false;
        fichas = new List<FichaScript>();
    }


    public void Play()
    {
        if (!foundFicha)
        {
            checkFichas();
        }
        else if (foundFicha.doneAnimation && foundFicha.played)
        {
            
            // if (foundFicha.transform.position == northAISlot.transform.position || foundFicha.transform.position == southAISlot.transform.position)
            //     foundFicha.CheckSlot();
            // else if (foundFicha.played)
                done = true;


        }
        // FindObjectOfType<CanvasManager>().OpenMessagePanel("P" + number + " PlayF: " + foundFicha.values[0] + " : " + foundFicha.values[1]);
    }

    private void checkFichas()
    {
        playSlots = FindObjectOfType<PlayFichaScript>();
        int[] values = playSlots.playValues;
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

    public List<FichaScript> fichasLeft(){

        List<FichaScript> tempFichas = new List<FichaScript>();
        foreach (FichaScript ficha in fichas)
        {
            if (ficha.played)
                continue;

            tempFichas.Add(ficha);
        }

        return tempFichas;
    }


}
