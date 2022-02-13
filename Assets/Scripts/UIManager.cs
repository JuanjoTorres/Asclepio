using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    private GameObject _canvasMenu;
    private bool _enabled;

    void Start()
    {
        _enabled = false;
        _canvasMenu = gameObject;
        _canvasMenu.SetActive(_enabled);
    }
}
