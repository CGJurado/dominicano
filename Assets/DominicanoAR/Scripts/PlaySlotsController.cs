using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySlotsController : MonoBehaviour
{
    public int value;
    public bool gotClicked = false;
    private CanvasManager canvas;
    private GameObject nextPlaySlot;
    private GameObject tempPlaySlot;
    private Quaternion tempRotation;

    // Start is called before the first frame update
    void Start()
    {
        canvas = FindObjectOfType<CanvasManager>();
    }

    public void AddPlaySlot(GameObject playSlot)
    {
        nextPlaySlot = playSlot;
        tempPlaySlot = playSlot;
        tempRotation = playSlot.transform.rotation;
    }

    private int GetId()
    {
        if (this.gameObject.name == "NorthSlot")
            return 1;
        else
            return 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ficha")
        {
            FichaScript ficha = other.gameObject.GetComponent<FichaScript>();
            PlayFichaScript parent = this.transform.parent.GetComponent<PlayFichaScript>();

            if(this.gameObject.name == "EnterSlot")
            {
                parent.playValues[0] = ficha.values[0];
                parent.playValues[1] = ficha.values[1];
            }

            int[] comparisonResult = ficha.CompareFichaValues(parent.playValues[this.GetId()]);

            if (comparisonResult[0] == 1 || comparisonResult[1] == 1)
            {
                canvas.OpenMessagePanel("Jugar ficha: " + ficha.values[0] + " : " + ficha.values[1]);
                if (ficha.doble)
                    nextPlaySlot.transform.Rotate(0f, 0f, 90f);

                //When ficha value equals north or south slot value, send the oposite value position
                if (comparisonResult[1] == 1)
                    parent.UpdatePlaySlot(nextPlaySlot, this.name, 0);
                else
                    parent.UpdatePlaySlot(nextPlaySlot, this.name, 1);
            }
            else
            {
                canvas.OpenMessagePanel("Acción invalida");
                parent.UpdatePlaySlot(nextPlaySlot, "Invalid", 1);
            }

            // this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            nextPlaySlot.GetComponent<MeshRenderer>().enabled = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        // this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.nextPlaySlot.GetComponent<MeshRenderer>().enabled = false;
        nextPlaySlot.transform.rotation = tempRotation;

        canvas.CloseMessagePanel();
    }

    private void OnMouseOver() 
    {
        nextPlaySlot.GetComponent<MeshRenderer>().enabled = true;
    }
    private void OnMouseExit() {
        nextPlaySlot.GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnMouseUpAsButton() {
        this.gotClicked = true;
    }

    public void render()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = true;
    }
    public void unRender()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
