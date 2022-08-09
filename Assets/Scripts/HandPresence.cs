using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    private InputDevice targetDevice;
    private GameObject spawnedController; // Modelo del controlador que se generara.
    private GameObject spawnedHandModel;  // Modelo de la mano que se generara.
    private Animator handAnimator;
    
    public List<GameObject> controllerPrefabs;                   // Lista de modelos de diferentes controladores.
    public GameObject handModelPrefab;
    public InputDeviceCharacteristics controllerCharacteristics; // Caracteristica del dispositivo.
    public bool showController = false;                          // true = muestra controlador, false = muestra manos

    // Start is called before the first frame update.
    void Start()
    {
        TryInitialize();
    }

    // Se encarga de actualizar el valor de movimiento de las animaciones en las manos.
    void UpdateHandAnimation()
    {
        // Esta variable evita que el programa se cierre inesperadamente si no encuentra el boton en el controlador.
        bool valueStatus;

        // Detecta cuando se pulsa el boton trigger.
        valueStatus = targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue);
        if (valueStatus)
            handAnimator.SetFloat("Trigger", triggerValue);
        else
            handAnimator.SetFloat("Trigger", 0);

        // Detecta cuando se pulsa el boton grip.
        valueStatus = targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue);
        if (valueStatus)
            handAnimator.SetFloat("Grip", gripValue);
        else
            handAnimator.SetFloat("Grip", 0);
    }

    // Inicializa todos los modelos
    void TryInitialize()
    {
        // Lista todos los dispositivos de entrada.
        List<InputDevice> devices = new List<InputDevice>();

        // Caracteristicas del controlador de la mano derecha.
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            // Es obligatorio que el nombre del prefab coincida con el nombre del dispositivo.
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            // Si encuentra una coincidencia instancia su respectivo modelo.
            if (prefab)
                spawnedController = Instantiate(prefab, transform);
            // Sino, carga los modelos por defecto y muestro un aviso de error por consola.
            else
            {
                Debug.LogError("Did not find corresponding controller model - [Changing to default controller model]");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }

            // Carga el modelo de las manos y le asigna su componente de animacion.
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
        }
    }

    void Update()
    {
        // Esta condicion resuelve el problema de si no se encuentran controladores al inicio de ejecutar la app.
        // Vuelve a inicializar si se dejan de detectar.
        if (!targetDevice.isValid)
            TryInitialize();
        else
        {
            // Se muestran los mandos y se desactivan las manos.
            if (showController)
            {
                spawnedController.SetActive(true);
                spawnedHandModel.SetActive(false);
            }
            // Se muestran las manos y se desactivan los mandos.
            else
            {
                spawnedController.SetActive(false);
                spawnedHandModel.SetActive(true);

                UpdateHandAnimation();
            }
        }
    }
}
