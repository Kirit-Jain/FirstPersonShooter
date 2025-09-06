using UnityEngine;
using StarterAssets; // Make sure you have this line

public class InputDebugger : MonoBehaviour
{
    // A slot for our input script
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;

    // We use LateUpdate to make sure the input for the frame has been processed
    void LateUpdate()
    {
        // Check if the slot has been filled in the Inspector
        if (starterAssetsInputs != null)
        {
            // Print the current state of the 'charge' boolean to the console
            Debug.Log("DEBUGGER --- Charge Input State is: " + starterAssetsInputs.charge);
        }
        else
        {
            Debug.LogWarning("DEBUGGER --- StarterAssetsInputs has not been assigned in the Inspector!");
        }
    }
}