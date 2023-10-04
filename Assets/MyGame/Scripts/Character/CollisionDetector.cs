using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    // 当たり判定の対象のタグ
    [SerializeField]
    private string tagName = null;

    // フラグ
    public bool isCollision { get; private set; } = false;
    // 衝突中のオブジェクト
    public GameObject collisionObject { get; private set; } = null;

    // 引数にColliderを持ったUnityEvent
    public UnityEvent<Collider2D> onTriggerEnter = null;
    public UnityEvent<Collider2D> onTriggerStay = null;
    public UnityEvent<Collider2D> onTriggerExit = null;

    private void OnTriggerEnter2D(Collider2D other)
    {       
        if(other.gameObject.tag == tagName)
        {
            isCollision = true;
            collisionObject = other.gameObject;
            onTriggerEnter?.Invoke(other);        
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == tagName)
        {
            onTriggerStay?.Invoke(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == tagName)
        {
            isCollision = false;
            collisionObject = null;
            onTriggerExit?.Invoke(other);
        }
    }
}
