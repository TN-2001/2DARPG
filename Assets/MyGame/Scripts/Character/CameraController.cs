using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] // ターゲット
    private Transform target = null;
    // ターゲットとの初期距離
    private Vector3 distance = Vector3.zero;

    private void Start()
    {
        distance = transform.position - target.position;
    }

    private void Update()
    {
        transform.position = target.position + distance;
    }
}
