using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] private float threshold = 0.1f;
    [SerializeField] private float deadZone = 0.025f;
    [SerializeField] private GameObject menu;

    private bool _isPressed;
    private bool _isEnabledMenu = false;
    private Vector3 _startPos;
    private ConfigurableJoint _joint;

    public UnityEvent onPressed, onReleased;

    #region Public Methods
    void Start()
    {
        _startPos = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();
    }

    void Update()
    {
        if (!_isPressed && GetValue() + threshold >= 1)
            Pressed();
        if (_isPressed && GetValue() - threshold <= 0)
            Released();
    }
    #endregion

    #region Private Methods
    private float GetValue()
    {
        var value = Vector3.Distance(_startPos, transform.localPosition) / _joint.linearLimit.limit;

        if (Mathf.Abs(value) < deadZone)
            value = 0;

        return Mathf.Clamp(value, -1f, 1f);
    }

    private void Pressed()
    {
        _isPressed = true;

        if (_isEnabledMenu)
        {
            _isEnabledMenu = false;
        }
        else
        {
            _isEnabledMenu = true;
        }
            
        menu.SetActive(_isEnabledMenu);
        Debug.Log("Pressed");
    }

    private void Released()
    {
        _isPressed = false;
        Debug.Log("Released");
    }
    #endregion
}
