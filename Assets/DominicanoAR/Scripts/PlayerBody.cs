using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMesh playerName;

    public void showBody(bool value){
        this.gameObject.SetActive(value);
    }

    public void setName(string value){
        this.playerName.text = value;
    }
}
