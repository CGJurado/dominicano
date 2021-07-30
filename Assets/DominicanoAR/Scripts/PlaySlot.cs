using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySlot : MonoBehaviour
{
    private GameObject nextPlaySlot;

    private Vector3 normalNorthPosition = new Vector3(0, 1f, 0);
    private Vector3 normalSouthPosition = new Vector3(0, -1f, 0);
    private Vector3 normalRotation = new Vector3(0, 0, 0);
    private Vector3 normalParentNorthCorner = new Vector3(-1.5f, 0.25f, 0);
    private Vector3 normalParentSouthCorner = new Vector3(1.5f, -0.25f, 0);
    private Vector3 dobleParentNorthPosition = new Vector3(1.5f, 0, 0);
    private Vector3 dobleParentSouthPosition = new Vector3(-1.5f, 0, 0);
    private Vector3 dobleParentRotation = new Vector3(180, 180, 90);
    private Vector3 fixSlotScale = new Vector3(0.5f, 2f, 1f);
    private Vector3 dobleNorthPosition = new Vector3(0, 0.75f, 0);
    private Vector3 dobleSouthPosition = new Vector3(0, -0.75f, 0);
    private Vector3 dobleRotation = new Vector3(180, 180, -90);
    public bool doble = false;
    public bool corner = false;

    public void assignNewParent(GameObject parent)
    {
        this.transform.SetParent(parent.transform);
    }
    public void moveNorthSlot(PlaySlot parent)
    {
        // PlaySlot parent = this.transform.parent.GetComponent<PlaySlot>();
        this.transform.SetParent(parent.transform);
        this.transform.localScale = new Vector3(1, 1, 1);
        print(parent);

        if(parent.doble){
            if(parent.corner){ //Normal slot after doble corner parent
                this.transform.localPosition = normalNorthPosition;
                this.transform.localEulerAngles = normalRotation;
            } else{ //Normal slot after doble Parent
                this.transform.localScale = fixSlotScale;
                this.transform.localPosition = dobleParentNorthPosition;
                this.transform.localEulerAngles = dobleParentRotation;
            }
        }
        else{
            if(parent.corner){ //Normal slot after normal parent
                this.transform.localScale = fixSlotScale;
                this.transform.localPosition = normalParentNorthCorner;
                this.transform.localEulerAngles = dobleRotation;
            } else{ //Normal slot after normal parent
                this.transform.localPosition = normalNorthPosition;
                this.transform.localEulerAngles = normalRotation;
            }
        }

    }

    public void moveSouthSlot(PlaySlot parent)
    {
        // PlaySlot parent = this.transform.parent.GetComponent<PlaySlot>();
        this.transform.SetParent(parent.transform);
        this.transform.localScale = new Vector3(1, 1, 1);

        if(parent.doble){
            if(parent.corner){
                this.transform.localPosition = normalSouthPosition;
                this.transform.localEulerAngles = normalRotation;
            } else{
                this.transform.localScale = fixSlotScale;
                this.transform.localPosition = dobleParentSouthPosition;
                this.transform.localEulerAngles = dobleParentRotation;
            }
        } else{
            if(parent.corner){
                this.transform.localScale = fixSlotScale;
                this.transform.localPosition = normalParentSouthCorner;
                this.transform.localEulerAngles = dobleRotation;
            } else{
                this.transform.localPosition = normalSouthPosition;
                this.transform.localEulerAngles = normalRotation;
            }
        }
        
    }

    public void fixNorthDoblePosition(){
        this.transform.localScale = fixSlotScale;
        this.transform.localPosition = dobleNorthPosition;
        this.transform.localEulerAngles = dobleRotation;
    }

    public void fixSouthDoblePosition(){
        this.transform.localScale = fixSlotScale;
        this.transform.localPosition = dobleSouthPosition;
        this.transform.localEulerAngles = dobleRotation;
    }
}
