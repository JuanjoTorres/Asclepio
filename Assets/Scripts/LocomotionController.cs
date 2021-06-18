using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController rightTeleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;

    public bool EnableRightTeleport { get; set; } = true;

    // Update is called once per frame
    void Update()
    {
        // Si existe el puntero de la mano derecha...
        if (rightTeleportRay)
            // mira si se ha activado el boton del teleport
            rightTeleportRay.gameObject.SetActive( EnableRightTeleport && CheckIfActivated(rightTeleportRay));
    }

    // Verificar si se ha activado el boton
    public bool CheckIfActivated(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, 
            teleportActivationButton, 
            out bool isActivated, 
            activationThreshold);

        return isActivated;
    }
}
