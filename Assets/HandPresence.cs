using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    private InputDevice targetDevice;
    private GameObject spawnedController; // Modelo del controlador que se generara
    private GameObject spawnedHandModel;  // Modelo de la mano que se generara
    
    public List<GameObject> controllerPrefabs;                   // Lista de modelos de diferentes controladores
    public GameObject handModelPrefab;
    public InputDeviceCharacteristics controllerCharacteristics; // Caracteristica del dispositivo
    public bool showController = false;                          // true = muestra controlador, false = muestra manos

    // Start is called before the first frame update
    void Start()
    {
        // Lista todos los dispositivos de entrada
        List<InputDevice> devices = new List<InputDevice>();
        
        // Caracteristicas del controlador de la mano derecha
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var device in devices)
            Debug.Log(device.name + " " + device.characteristics);


        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            // Es obligatorio que el nombre del prefab coincida con el nombre del dispositivo
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            if (prefab)
                spawnedController = Instantiate(prefab, transform);
            else
            {
                Debug.LogError("Did not find corresponding controller model - [Changing to default controller model]");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Esta variable evita que el programa se cierre inesperadamente si no encuentra el boton en el controlador
        bool valueStatus;

        valueStatus = targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        if (valueStatus && primaryButtonValue)
            Debug.Log("Pressing Primary Button");

        valueStatus = targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if (valueStatus && triggerValue > 0.1f)
            Debug.Log("Trigger pressed - value: " + triggerValue);

        valueStatus = targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 primary2DAxisValue);
        if (valueStatus && primary2DAxisValue != Vector2.zero)
            Debug.Log("Moving Primary Touchpad - value: " + primary2DAxisValue);

        if (showController)
        {
            spawnedController.SetActive(true);
            spawnedHandModel.SetActive(false);
        }
        else
        {
            spawnedController.SetActive(false);
            spawnedHandModel.SetActive(true);
        }
    }
}
