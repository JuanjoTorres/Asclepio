using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController rightTeleportRay;
    public InputHelpers.Button teleportActivationButton;
    public float activationThreshold = 0.1f;
    public XRRayInteractor rightInteractorRay;

    public bool EnableRightTeleport { get; set; } = true;

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        Vector3 pos = new Vector3();
        Vector3 norm = new Vector3();
        int index = 0;
        bool validTarget = false;

        // Si existe el puntero de la mano derecha...
        if (rightTeleportRay)
        {
            bool isRightInteractorHovering = rightInteractorRay.TryGetHitInfo(ref pos, ref norm, ref index, ref validTarget);
            // mira si se ha activado el boton del teleport
            rightTeleportRay.gameObject.SetActive(EnableRightTeleport && CheckIfActivated(rightTeleportRay) && !isRightInteractorHovering);
        }
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
