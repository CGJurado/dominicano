using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : MonoBehaviour
{

    public int number;
    private List<FichaScript> fichas = new List<FichaScript>();
    public GameObject northAISlot;
    public GameObject southAISlot;
    public bool turn = false;
    public bool done = false;
    public bool passed = false;
    public PlaySlotsController northController;
    public PlaySlotsController enterController;
    public PlaySlotsController southController;
    private PlayFichaScript playSlots;
    private FichaScript foundFicha;
    public PlayerBody body;
    public string team = "team1";
    public bool AIPlayer;
    private bool fichaGotClicked = false;

    private void Start() {
        if(number == 2 || number == 4)
            team = "team2";
    }
    // Future scalable function if we ever use NetworkManager in the future
    // private void Init() 
    // {
    //     GameLogic game = FindObjectOfType<GameLogic>();
    //     this.transform.SetParent(game.transform);

    //     northController = game.playSlots.northSlot.GetComponent<PlaySlotsController>();
    //     enterController = game.playSlots.enterSlot.GetComponent<PlaySlotsController>();
    //     southController = game.playSlots.southSlot.GetComponent<PlaySlotsController>();

    //     northAISlot = FindObjectOfType<NorthAISlot>().gameObject;
    //     southAISlot = FindObjectOfType<SouthAISlot>().gameObject;
    // }

    private void Update()
    {
        if (turn)
        {
            if (AIPlayer || SocketManager.online)
            {
                this.transform.LookAt(Camera.main.transform);
                
                if (foundFicha)
                    this.Play();

            }

            if (done)
            {
                turn = false;
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

            if(Input.GetMouseButtonDown(0) && GameLogic.myTurn)
            {
                Camera[] allCameras = new Camera[2];
                Camera.GetAllCameras(allCameras);
                if(allCameras[1])
                {
                    Ray ray = allCameras[1].ScreenPointToRay(Input.mousePosition);
                    clickPlay(ray);
                }

            }
            
            // TODO: switch camera so player can select where to play from above
            // if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            // {
            //     Ray ray = Camera.main.ScreenPointToRay(Input.touches[0].position);
            //     clickPlay(ray);
            // }

            if(fichaGotClicked && GameLogic.myTurn)
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

                    // Handheld.Vibrate();
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
        turn = false;
        fichas = new List<FichaScript>();
    }


    public void Play()
    {
            
        if (!foundFicha)
        {
            if(SocketManager.online && !(SocketManager.mainPlayer.number == 1))
                return;

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

    public void recieveFicha(FichaScript ficha){
        foundFicha = ficha;
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
            CanvasManager tempCanv = FindObjectOfType<CanvasManager>();
            tempCanv.OpenMessagePanel("AIFound: Pass");
            tempCanv.onlinePass();
            this.transform.parent.GetComponent<GameLogic>().passTurn();
            // done = true;
        }
        
    }

    public int beforeMe(){
        int arrPos = number - 1;

        if(arrPos == 0)
            return 3;
        
        return (arrPos - 1);
    }
    public int afterMe(){
        int arrPos = number - 1;

        if(arrPos == 3)
            return 0;
        
        return (arrPos + 1);
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
