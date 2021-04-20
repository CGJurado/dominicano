using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectTriggerCollision : MonoBehaviour
{

    private CanvasManager canvas;
    // Use this for initialization
    private void Start()
    {

        canvas = FindObjectOfType<CanvasManager>();
    }

    Color lastColor;


    void OnTriggerEnter(Collider other)
    {
        //lastColor = other.gameObject.GetComponent<Renderer>().material.color;
        // trigCol.gameObject.GetComponent<Renderer>().material.color = Color.green;

        gameObject.GetComponent<Renderer>().material.color = Color.cyan;
        canvas.OpenMessagePanel("we did it bitch");

    }

    void OnTriggerStay(Collider other)
    {
        //other.gameObject.GetComponent<Renderer>().material.color = Color.yellow;

        gameObject.GetComponent<Renderer>().material.color = Color.yellow;

    }

    void OnTriggerExit(Collider other)
    {
        //other.gameObject.GetComponent<Renderer>().material.color = gameObject.GetComponent<Renderer>().material.color;
        gameObject.GetComponent<Renderer>().material.color = lastColor;

        canvas.CloseMessagePanel();

    }
}
