using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class JoystickEventSystem : EventSystem
{
    #region Injected

    private float _deadZone;

    #endregion

    [Inject]
    private void Construct(float deadZone)
    {
        _deadZone = deadZone;
    }

    protected override void Start()
    {
        base.Start();

        if (firstSelectedGameObject == null)
        {
            firstSelectedGameObject = FindObjectOfType<Button>().gameObject;
        }
    }

    protected override void Update()
    {
        base.Update();

        if (currentSelectedGameObject == null && (
            Input.GetAxis(InputAxes.Horizontal) > _deadZone ||
            Input.GetAxis(InputAxes.Horizontal) < -_deadZone ||
            Input.GetAxis(InputAxes.Vertical) > _deadZone ||
            Input.GetAxis(InputAxes.Vertical) < -_deadZone))
        {
            SetSelectedGameObject(firstSelectedGameObject);
        }
    }
}
