using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TableSpawner : MonoBehaviour
{
    private GameObject table;
    private PlacementIndicator placementIndicator;
    private Vector3 currentPointPosition;
    private ARRaycastManager rayManager;
    public bool showIndicator = true;
    private FichaScript[] fichas;
    public GameObject fichasObject;

    private void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        fichas = fichasObject.GetComponentsInChildren<FichaScript>();

        table = transform.GetChild(0).gameObject;
        table.SetActive(false);
    }

    private void Update()
    {
        if(!table.activeInHierarchy && placementIndicator.isActiveAndEnabled && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            //table = Instantiate(tableToSpawn, 
            //    placementIndicator.transform.position, placementIndicator.transform.rotation);

            this.transform.position = placementIndicator.transform.position;
            this.transform.rotation = placementIndicator.transform.rotation;

            table.SetActive(true);
            this.initFichasPos();

            showIndicator = false;
        }
        SpawnTableWithTriggerGesture();
    }


    private void SpawnTableWithTriggerGesture()
    {
        HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
        TrackingInfo trackingInfo = handInfo.tracking_info;
        GestureInfo gestureInfo = handInfo.gesture_info;

        ManoGestureContinuous currentDetectedContinuousGesture = gestureInfo.mano_gesture_continuous;

        if (currentDetectedContinuousGesture == ManoGestureContinuous.POINTER_GESTURE)
        {
            currentPointPosition = Camera.main.ViewportToScreenPoint(trackingInfo.palm_center);

            showIndicator = true;

            //shoot a raycast from the hand position on the screen
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(new Vector2(currentPointPosition.x, currentPointPosition.y), hits, TrackableType.Planes);

            //if we hit an AR plane, update the position and rotation
            if (hits.Count > 0)
            {
                placementIndicator.transform.position = hits[0].pose.position;
                placementIndicator.transform.rotation = hits[0].pose.rotation;
                this.transform.position = hits[0].pose.position;
                this.transform.rotation = hits[0].pose.rotation;
                this.resetFichasPos();
            }

        }
        else if(table.activeInHierarchy)
        {
            showIndicator = false;
        }



    }

    public void resetFichasPos()
    {
        foreach (FichaScript ficha in fichas)
        {
            ficha.goBack();
        }
    }
    public void initFichasPos()
    {
        foreach (FichaScript ficha in fichas)
        {
            ficha.Init();
        }
    }

}
