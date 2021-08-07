using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GrabScript : MonoBehaviour
{
    bool isSessionQualityOK;
    private PlacementIndicator placementIndicator;
    private GameObject visual;
    private Vector3 currentPOIPosition;
    private Vector3 currentPOIWorldPosition;
    private Vector3 currentHandPosition;
    private Vector3 currentHandWorldPosition;
    private GameObject foundItem;
    private TableSpawner tableSpawner;

    private ARRaycastManager rayManager;
    // Start is called before the first frame update
    void Start()
    {
        ARSession.stateChanged += HandleStateChanged;
        placementIndicator = FindObjectOfType<PlacementIndicator>();
        rayManager = FindObjectOfType<ARRaycastManager>();
        tableSpawner = FindObjectOfType<TableSpawner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isSessionQualityOK)
        {
            //SpawnCubeWithClickTriggerGesture();
            GrabObject();
        }
        
    }

    private void HandleStateChanged(ARSessionStateChangedEventArgs stateEventArguments)
    {
        isSessionQualityOK = stateEventArguments.state == ARSessionState.SessionTracking;
    }

    public GameObject itemPrefab;

    private void SpawnCubeWithClickTriggerGesture()
    {
        HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
        TrackingInfo trackingInfo = handInfo.tracking_info;
        GestureInfo gestureInfo = handInfo.gesture_info;
        
        ManoGestureTrigger currentDetectedTriggerGesture = gestureInfo.mano_gesture_trigger;

        if (currentDetectedTriggerGesture == ManoGestureTrigger.CLICK)
        {

            currentPOIPosition = Camera.main.ViewportToScreenPoint(trackingInfo.poi);
            //shoot a raycast from the center of the screen
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            rayManager.Raycast(new Vector2(currentPOIPosition.x, currentPOIPosition.y), hits, TrackableType.Planes);
            //if we hit an AR plane, update the position and rotation
            if (hits.Count > 0)
            {
                //placementIndicator.transform.position = hits[0].pose.position;
                //placementIndicator.transform.rotation = hits[0].pose.rotation;
                //if (!visual.activeInHierarchy)
                //    visual.SetActive(true);

                GameObject newItem = Instantiate(itemPrefab, 
                    new Vector3(hits[0].pose.position.x, hits[0].pose.position.y + 1, hits[0].pose.position.z),
                        hits[0].pose.rotation);

                // Handheld.Vibrate();
            }
        }


    }

    private void GrabObject()
    {
        HandInfo handInfo = ManomotionManager.Instance.Hand_infos[0].hand_info;
        TrackingInfo trackingInfo = handInfo.tracking_info;
        GestureInfo gestureInfo = handInfo.gesture_info;

        ManoGestureTrigger currentDetectedTriggerGesture = gestureInfo.mano_gesture_trigger;
        ManoGestureContinuous currentDetectedContinuousGesture = gestureInfo.mano_gesture_continuous;

        // ############################## Find a "Ficha" and move it to Hand POI ###############################
        if (currentDetectedTriggerGesture == ManoGestureTrigger.PICK)
        {
            currentPOIPosition = Camera.main.ViewportToScreenPoint(trackingInfo.poi);
            //shoot a raycast from the grabGesture position on the screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(currentPOIPosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Ficha")
                {
                    currentPOIWorldPosition = Camera.main.ViewportToWorldPoint(
                        new Vector3(trackingInfo.poi.x, trackingInfo.poi.y, trackingInfo.depth_estimation));

                    foundItem = hit.collider.gameObject;
                    foundItem.transform.position = currentPOIWorldPosition;

                    // Handheld.Vibrate();
                }
            }
            else
            {
                foundItem = null;
            }

        }
        // Hold the "Ficha" as long as HOLD_GESTURE last, and make it follow the Hand POI
        else if (foundItem && currentDetectedContinuousGesture == ManoGestureContinuous.HOLD_GESTURE)
        {
            currentPOIWorldPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(trackingInfo.poi.x, trackingInfo.poi.y, trackingInfo.depth_estimation));

            foundItem.transform.position = currentPOIWorldPosition;
            foundItem.transform.rotation = Camera.main.transform.rotation;
            foundItem.transform.Rotate(0f, 180f, 0f);
            
        }
        // Release the "Ficha"
        else if (foundItem && currentDetectedContinuousGesture == ManoGestureContinuous.OPEN_PINCH_GESTURE)
        {
            GameObject tempObj = new GameObject("temp");
            tempObj.transform.position = currentPOIWorldPosition;
            //Check if 'Ficha' ended up inside an available Slot
            // foundItem.GetComponent<FichaScript>().CheckSlot();
            foundItem.GetComponent<FichaScript>().MoveTo(tempObj);
            foundItem = null;
        }
        // ######################################## Find the Table ##############################################
        else if (currentDetectedTriggerGesture == ManoGestureTrigger.GRAB_GESTURE)
        {
            currentHandPosition = Camera.main.ViewportToScreenPoint(trackingInfo.palm_center);
            //shoot a raycast from the center of the screen
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(currentHandPosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "WholeTable")
                {
                    foundItem = hit.collider.gameObject;
                    //foundItem.transform.position = currentHandWorldPosition;
                    // Handheld.Vibrate();
                }
            }
            else
            {
                foundItem = null;
            }
        }
        // Hold the Table and move it relative to Y axis
        else if (foundItem && currentDetectedContinuousGesture == ManoGestureContinuous.CLOSED_HAND_GESTURE)
        {
            currentHandWorldPosition = Camera.main.ViewportToWorldPoint(
                new Vector3(trackingInfo.palm_center.x, trackingInfo.palm_center.y, trackingInfo.depth_estimation + 0.5f));

            foundItem.transform.position = new Vector3(foundItem.transform.position.x, 
                currentHandWorldPosition.y - 0.5f, foundItem.transform.position.z);
        }
        // Release the Table
        else if (foundItem && currentDetectedContinuousGesture == ManoGestureContinuous.OPEN_HAND_GESTURE)
        {
            tableSpawner.resetFichasPos();
            foundItem = null;
        }
    }
}
