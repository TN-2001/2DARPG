using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] // ターゲット
    private Transform target = null;


    public void Initialize(Transform target)
    {
        this.target = target;
        
        if(target)
        {
            transform.position = new Vector3(target.position.x, target.position.y, -10);
        }
        else
        {
            transform.position = new Vector3(0, 0, -10);
        }
    }

    private void Update()
    {
        if(target != null)
        {
            transform.position = new Vector3(target.position.x, target.position.y, -10);
        }
        else if(transform.position != new Vector3(0, 0, -10))
        {
            transform.position = new Vector3(0, 0, -10);
        }
    }
}
