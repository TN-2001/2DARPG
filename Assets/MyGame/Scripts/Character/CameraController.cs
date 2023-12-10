using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] // ターゲット
    private Transform target = null;


    private void Update()
    {
        if(target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, -10);
        }
    }
}
