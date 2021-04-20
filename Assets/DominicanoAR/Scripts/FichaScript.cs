using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FichaScript : MonoBehaviour
{
    public int[] values = { 0, 0 };
    public bool doble;
    public bool played = false;
    public bool doneAnimation = true;
    private GameObject foundSlot;
    private GameObject lastObject;
    private Vector3 velocity;
    private float smoothTime = 0.25f;
    private Vector3 targetPos;

    private void Start()
    {
        targetPos = this.transform.position;
        string[] strValues = this.gameObject.name.Split(' ')[1].Split('x');
        values[0] = Int32.Parse(strValues[0]);
        values[1] = Int32.Parse(strValues[1]);
    }

    public void Init()
    {
        targetPos = this.transform.position;
    }

    private void Update()
    {
        if (this.transform.position != targetPos)
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPos, ref velocity, smoothTime);
        else
            doneAnimation = true;
    }

    public int[] CompareFichaValues(int otherFichaValue)
    {
        //Return { true/false , value position } Array format
        int[] result = { 0, 0 };
        if(otherFichaValue == values[0])
            result[0] = 1;
        if(otherFichaValue == values[1])
            result[1] = 1;

        return result;
    }

    public void MoveTo(GameObject obj)
    {
        doneAnimation = false;
        velocity = Vector3.zero;
        if(obj.name != "PlaySlot")
            this.transform.rotation = obj.transform.rotation;

        targetPos = obj.transform.position;

        lastObject = obj;
    }

    public void goBack()
    {
        doneAnimation = false;
        velocity = Vector3.zero;
        this.transform.rotation = lastObject.transform.rotation;
        targetPos = lastObject.transform.position;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Slot")
        {
            foundSlot = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foundSlot = null;
    }

    public void CheckSlot()
    {
        if (foundSlot)
        {
            if (foundSlot.transform.parent.name == "PlaySlots")
            {
                bool played = foundSlot.transform.parent.GetComponent<PlayFichaScript>().TryPlayFicha(this.transform.gameObject);
                if (played)
                    this.removeCollition();
                else
                    goBack();
            }
            else
            {
                //TODO: Swap fichas
                MoveTo(foundSlot);
            }
        }
        else
        {
            goBack();
        }
    }

    public void ActiveCollition()
    {
        played = false;
        this.gameObject.GetComponent<Collider>().enabled = true;
    }

    public void removeCollition()
    {
        played = true;
        this.gameObject.GetComponent<Collider>().enabled = false;
    }

}
