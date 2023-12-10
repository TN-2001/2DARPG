using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] // 追従する対象
    private Transform target = null;
    [SerializeField] // 追従するか否か
    private bool isFollow = false;
    [SerializeField] // 消すか否か
    private bool isDestroy = false;
    [SerializeField] // 生存時間
    private float survivalTime = 0;

    // 追従する対象の最初の位置
    private Vector3 firstPosition = Vector3.zero;


    public void Initialize(Transform target)
    {
        this.target = target;
        firstPosition = target.position;
        transform.position = Camera.main.WorldToScreenPoint(target.position);
        gameObject.SetActive(true);

        if(isDestroy)
        {
            Destroy(gameObject, survivalTime);
        }
    }

    private void LateUpdate()
    {
        if(isFollow)
        {
            if(target)
            {
                transform.position = Camera.main.WorldToScreenPoint(target.position);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Camera.main.WorldToScreenPoint(firstPosition);
        }
    }
}
