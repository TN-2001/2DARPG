using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ToggleUI : Toggle
{
    public UnityEvent onSelect = null;

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        onSelect?.Invoke();
        isOn = true;
    }
}
