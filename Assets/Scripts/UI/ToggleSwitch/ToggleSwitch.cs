using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour, IPointerClickHandler
{
    private Slider _slider;

    protected Action onToggleOn;
    protected Action onToggleOff;

    protected virtual void Start()
    {
        _slider = GetComponentInChildren<Slider>();

        if (_slider == null)
        {
            Debug.LogError("No slider found!");
            return;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }

    private void Toggle()
    {
        if (_slider.value > 0.9f)
        {
            _slider.value = 0f;
            onToggleOff?.Invoke();
        }
        else
        {
            _slider.value = 1.0f;
            onToggleOn?.Invoke();
        }
    }

    public bool IsOn()
    {
        if (_slider == null)
            _slider = GetComponentInChildren<Slider>();

        return _slider != null && _slider.value > 0.9f;
    }

    public void SetVisual(bool on)
    {
        if (_slider == null)
            _slider = GetComponentInChildren<Slider>();

        if (_slider == null) return;

        _slider.value = on ? 1.0f : 0f;
    }
}
