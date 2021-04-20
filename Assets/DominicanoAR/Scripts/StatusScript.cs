using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class StatusScript: MonoBehaviour
{
    private TextMeshProUGUI sessionStatus;
    // Start is called before the first frame update
    void Start()
    {
        ARSession.stateChanged += HaddleStateChanged;
        sessionStatus = this.GetComponent<TextMeshProUGUI>();
    }

    private void HaddleStateChanged(ARSessionStateChangedEventArgs stateEventArgs)
    {
        switch (stateEventArgs.state)
        {
            case ARSessionState.None:
                sessionStatus.text = "Status: Unknown";
                break;

            case ARSessionState.Unsupported:
                sessionStatus.text = "Status: ARFoundation not supported";
                break;

            case ARSessionState.CheckingAvailability:
                sessionStatus.text = "Checking...";
                break;

            case ARSessionState.NeedsInstall:
                sessionStatus.text = "Needs Install";
                break;

            case ARSessionState.Installing:
                sessionStatus.text = "Status: ";
                break;

            case ARSessionState.Ready:
                sessionStatus.text = "Ready";
                break;

            case ARSessionState.SessionInitializing:
                sessionStatus.text = "Poor SLAM Quality";
                break;

            case ARSessionState.SessionTracking:
                sessionStatus.text = "Tracking Quality is good";
                break;

            default:
                sessionStatus.text = "Session Status: Unknown";
                break;
        }
    }
}
