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
    private float moveDistance = 0.036f;
    public int[] playValues;

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
        enterSlot.GetComponent<PlaySlotsController>().AddPlaySlot(enterPlaySlot);

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
            this.AddPlaySlot(fichaComponent);

            return true;
        }


    }

    private void AddPlaySlot(FichaScript fichaComponent)
    {
        if (playSlotName == "NorthSlot")
        {
            if (fichaComponent.doble)
            {
                moveDistance = 0.036f;
                playSlot.transform.position = new Vector3(playSlot.transform.position.x,
                    playSlot.transform.position.y, playSlot.transform.position.z - 0.017f);
            }
            else
                moveDistance = 0.05f;

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
                moveDistance = 0.036f;
                playSlot.transform.position = new Vector3(playSlot.transform.position.x,
                    playSlot.transform.position.y, playSlot.transform.position.z + 0.017f);
            }
            else
                moveDistance = 0.05f;

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
                moveDistance = 0.036f;
            }
            else
                moveDistance = 0.05f;

            fichaComponent.MoveTo(playSlot);

            playValues[0] = fichaComponent.values[0];
            playValues[1] = fichaComponent.values[1];
            this.ActivatePlaySlots();
        }

        // If player is not AI then his turn is done
        if (!game.playing().AIPlayer)
            game.playing().done = true;
    }

    private void AddNorthPlaySlot()
    {
        //northSlot.GetComponent<MeshRenderer>().enabled = false;
        playSlot.GetComponent<MeshRenderer>().enabled = false;

        GameObject northPlaySlot = Instantiate(playSlotPrefab, this.transform);
        northPlaySlot.transform.position = new Vector3(playSlot.transform.position.x,
            playSlot.transform.position.y, playSlot.transform.position.z + moveDistance);

        //FindObjectOfType<CanvasManager>().OpenMessagePanel("NorthX: " + northPlaySlot.transform.position.x);

        northSlot.GetComponent<PlaySlotsController>().AddPlaySlot(northPlaySlot);
    }
    private void AddSouthPlaySlot()
    {
        //southSlot.GetComponent<MeshRenderer>().enabled = false;
        playSlot.GetComponent<MeshRenderer>().enabled = false;

        GameObject southPlaySlot = Instantiate(playSlotPrefab, this.transform);
        southPlaySlot.transform.position = new Vector3(playSlot.transform.position.x,
            playSlot.transform.position.y, playSlot.transform.position.z - moveDistance);

        //FindObjectOfType<CanvasManager>().OpenMessagePanel("SouthX: " + southSlot.transform.position.x);

        southSlot.GetComponent<PlaySlotsController>().AddPlaySlot(southPlaySlot);
    }
}