using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateScript : MonoBehaviour
{
    public GameObject gear;
    void Update()
    {
        gear.transform.Rotate(0, 0 , 1);
    }
}
