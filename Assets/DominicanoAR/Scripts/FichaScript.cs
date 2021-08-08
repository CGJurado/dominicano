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
    private GameObject targetObject;
    private PlayFichaScript playSlots;

    private void Start()
    {
        targetObject = this.gameObject;
        lastObject = this.gameObject;
        string[] strValues = this.gameObject.name.Split(' ')[1].Split('x');
        values[0] = Int32.Parse(strValues[0]);
        values[1] = Int32.Parse(strValues[1]);
        playSlots = FindObjectOfType<PlayFichaScript>();
    }

    public void Init()
    {
        targetObject = this.gameObject;
        lastObject = this.gameObject;
    }

    private void Update()
    {
        if (this.transform.position != targetObject.transform.position)
            this.transform.position = Vector3.SmoothDamp(this.transform.position, targetObject.transform.position, ref velocity, smoothTime);
        else
        {
            doneAnimation = true;
            if(lastObject != targetObject)
                CheckSlot();
        }
    }

    public bool canBePlayed(){
        if(playSlots.enterSlot.activeInHierarchy){
            return true;
        }
        int[] values = playSlots.playValues;
        int[] northRes = this.CompareFichaValues(values[1]);
        if (northRes[0] == 1 || northRes[1] == 1){
            return true;
        }
        int[] southRes = this.CompareFichaValues(values[0]);
        if (southRes[0] == 1 || southRes[1] == 1){
            return true;
        }

        return false;
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
        lastObject = targetObject;
        doneAnimation = false;
        
        if(SocketManager.online && (GameLogic.myTurn || (GameLogic.playing().AIPlayer && SocketManager.mainPlayer.number == 1 )) 
        && obj.name != "PlaySlot(Clone) (PlaySlot)" && obj.name != "PlaySlot(Clone)"){
            FichaMoveResponse tempFichaRes = new FichaMoveResponse();
            tempFichaRes.objName = obj.transform.parent.name+ "/"+ obj.name;
            tempFichaRes.playerNumber = SocketManager.mainPlayer.number;
            tempFichaRes.fichaName = this.gameObject.name;
            tempFichaRes.AI = GameLogic.playing().AIPlayer;

            SocketManager.sendFichaToObj(tempFichaRes);
        }

        velocity = Vector3.zero;
        if(obj.name != "PlaySlot")
            this.transform.rotation = obj.transform.rotation;

        targetObject = obj;

    }

    public void goBack()
    {
        doneAnimation = false;
        velocity = Vector3.zero;
        if(lastObject)
            this.transform.rotation = lastObject.transform.rotation;
        else
            print("lastObject doesn't exist!");
        targetObject = lastObject;
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

    public string getFoundSlotName(){
        
        if (foundSlot)
        {
            if (foundSlot.name == "NorthSlot")
            {
                return "AISlots/North";

            } else if(foundSlot.name == "SouthSlot"  || foundSlot.name == "EnterSlot"){
                return "AISlots/South";
            }
        }
        return "notFound";
    }

    public void CheckSlot()
    {
        if (foundSlot)
        {
            if (foundSlot.transform.parent.name == "PlaySlots")
            {
                bool played = foundSlot.transform.parent.GetComponent<PlayFichaScript>().TryPlayFicha(this.transform.gameObject);
                if (played)
                {
                    this.removeCollition();
                    lastObject = targetObject;
                }
                else
                    goBack();
                
                // print(this.transform.name);
                // print(this.played);
            }
            else
            {
                //TODO: Swap fichas
                MoveTo(foundSlot);
                lastObject = targetObject;
            }
        }
        else if(!played)
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
