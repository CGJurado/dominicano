using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomCam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0){
            this.gameObject.GetComponent<Camera>().fieldOfView--;
        }
        if(Input.GetAxis("Mouse ScrollWheel") < 0){
            this.gameObject.GetComponent<Camera>().fieldOfView++;
        }
    }
}
