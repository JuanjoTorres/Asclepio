using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SliceListener : MonoBehaviour
{
    public Slicer slicer;
    public GameObject slicerTool;
    public GameObject slicerToolUI;
    public InputDevice targetDevice;

    private void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftDeviceCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftDeviceCharacteristics, devices);

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
        }

        slicerTool.SetActive(false);
        slicerToolUI.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        slicer.isTouched = true;
    }

    private void FixedUpdate()
    {
        slicer.isPress = IsPressPrimaryButton();

        RotateSlicer();
        RefreshDegreesInUI();
    }

    private bool IsPressPrimaryButton()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue);
        return primaryButtonValue;
    }
    private void RefreshDegreesInUI()
    {
        slicer.degreesOfCut.text = slicer.transform.rotation.eulerAngles.z.ToString("0.0");
    }

    public void RotateSlicer()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 coord);

        if (coord.x < 0.0f || coord.x > 0.0f)
        {
            slicer.cutPlane.transform.Rotate(Vector3.forward * coord.x);
        }
    }

    public void IsEnabledTool()
    {
        slicerTool.SetActive(true);
        slicerToolUI.SetActive(true);

        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.isTrigger = true;
        }
    }

    public void IsDisabledTool()
    {
        slicerTool.SetActive(false);
        slicerToolUI.SetActive(false);

        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.isTrigger = false;
        }
    }
}