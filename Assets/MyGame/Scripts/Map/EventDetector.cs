using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventController : MonoBehaviour
{
    // イベント
    public UnityEvent onDo = null;


    public void Do()
    {
        onDo?.Invoke();
    }
}
