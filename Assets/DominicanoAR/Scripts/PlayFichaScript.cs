using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFichaScript : MonoBehaviour
{

    public GameObject enterSlot;
    public GameObject northSlot;
    public GameObject southSlot;

    [SerializeField]
    private GameObject playSlotPrefab;

    private GameObject playSlot;
    private GameLogic game;
    private string playSlotName;
    private int fichaPosition;
    public int[] playValues;
    public int southSlotCount = 0;
    public int northSlotCount = 0;
    private int verticalLimit = 6;
    private int horizontalLimit = 10;

    private void Start()
    {
        game = FindObjectOfType<GameLogic>();
    }

    public void ActivatePlaySlots()
    {
        if (!northSlot.activeInHierarchy)
            northSlot.SetActive(true);

        this.AddNorthPlaySlot();

        if (!southSlot.activeInHierarchy)
            southSlot.SetActive(true);

        this.AddSouthPlaySlot();

        enterSlot.SetActive(false);
    }

    public void ActivateEnterSlot()
    {
        if (!enterSlot.activeInHierarchy)
            enterSlot.SetActive(true);

        northSlot.SetActive(false);
        southSlot.SetActive(false);

        GameObject enterPlaySlot = Instantiate(playSlotPrefab, this.transform);
        enterSlot.GetComponent<PlaySlotsController>().savePlaySlot(enterPlaySlot);

        //Initialize playSlot for the first time to the center of the table
        playSlot = enterPlaySlot;

    }

    public void UpdatePlaySlot(GameObject nextPlaySlot, string nextSlotName, int nextFichaPosition)
    {
        playSlot = nextPlaySlot;
        playSlotName = nextSlotName;
        fichaPosition = nextFichaPosition;
    }

    public bool TryPlayFicha(GameObject ficha)
    {
        FindObjectOfType<CanvasManager>().CloseMessagePanel();

        FichaScript fichaComponent = ficha.GetComponent<FichaScript>();

        if (playSlotName == "Invalid")
            return false;
        else
        {
            //ficha.transform.position = playSlot.transform.position;
            ficha.transform.rotation = playSlot.transform.rotation;

            // This func also MOVES the FICHA
            // Before Adding the new playslot, this func is going to move the FICHA to the actual slot
            this.addPlaySlot(fichaComponent);

            return true;
        }

    }

    private void addPlaySlot(FichaScript fichaComponent)
    {
        if (playSlotName == "NorthSlot")
        {
            if (fichaComponent.doble)
            {
                playSlot.GetComponent<PlaySlot>().doble = true;
                if(playSlot.transform.parent.GetComponent<PlaySlot>().corner){
                    northSlotCount--;
                    playSlot.GetComponent<PlaySlot>().corner = true;
                }
                playSlot.GetComponent<PlaySlot>().fixNorthDoblePosition();
            }

            fichaComponent.MoveTo(playSlot);
            if (fichaPosition == 0)
                fichaComponent.gameObject.transform.Rotate(0f, 0f, 180f);

            this.playValues[1] = fichaComponent.values[fichaPosition];
            this.AddNorthPlaySlot();
        }
        else if (playSlotName == "SouthSlot")
        {
            if (fichaComponent.doble)
            {
                playSlot.GetComponent<PlaySlot>().doble = true;
                if(playSlot.transform.parent.GetComponent<PlaySlot>().corner){
                    southSlotCount--;
                    playSlot.GetComponent<PlaySlot>().corner = true;
                }
                playSlot.GetComponent<PlaySlot>().fixSouthDoblePosition();
            }

            fichaComponent.MoveTo(playSlot);
            if (fichaPosition == 1)
                fichaComponent.gameObject.transform.Rotate(0f, 0f, 180f);

            this.playValues[0] = fichaComponent.values[fichaPosition];
            this.AddSouthPlaySlot();
        }
        else if (playSlotName == "EnterSlot")
        {
            if (fichaComponent.doble)
            {
                playSlot.GetComponent<PlaySlot>().doble = true;
            }

            fichaComponent.MoveTo(playSlot);

            playValues[0] = fichaComponent.values[0];
            playValues[1] = fichaComponent.values[1];
            this.ActivatePlaySlots();
        }

        // If player is not AI then his turn is done
        if (!GameLogic.playing().AIPlayer)
            GameLogic.playing().done = true;
    }

    private void AddNorthPlaySlot()
    {
        //northSlot.GetComponent<MeshRenderer>().enabled = false;
        playSlot.GetComponent<MeshRenderer>().enabled = false;

        northSlotCount++;
        GameObject northPlaySlot = Instantiate(playSlotPrefab, this.transform);
        // northPlaySlot.transform.parent = playSlot.transform;
        northPlaySlot.GetComponent<PlaySlot>().moveNorthSlot(playSlot.GetComponent<PlaySlot>());
        if(northSlotCount == verticalLimit || northSlotCount == horizontalLimit){
            northPlaySlot.GetComponent<PlaySlot>().corner = true;
        }

        northSlot.GetComponent<PlaySlotsController>().savePlaySlot(northPlaySlot);
    }
    private void AddSouthPlaySlot()
    {
        //southSlot.GetComponent<MeshRenderer>().enabled = false;
        playSlot.GetComponent<MeshRenderer>().enabled = false;

        southSlotCount++;
        GameObject southPlaySlot = Instantiate(playSlotPrefab, this.transform);

        southPlaySlot.GetComponent<PlaySlot>().moveSouthSlot(playSlot.GetComponent<PlaySlot>());
        if(southSlotCount == verticalLimit || southSlotCount == horizontalLimit){
            southPlaySlot.GetComponent<PlaySlot>().corner = true;
        }

        southSlot.GetComponent<PlaySlotsController>().savePlaySlot(southPlaySlot);
    }
}