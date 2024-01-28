using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDetector : MonoBehaviour
{
    // 当たり判定の対象のタグ
    [SerializeField]
    private string tagName = null;
    protected virtual string TagName => tagName;

    // 引数にColliderを持ったUnityEvent
    public UnityEvent<Collider2D> onTriggerEnter = null;
    public UnityEvent<Collider2D> onTriggerStay = null;
    public UnityEvent<Collider2D> onTriggerExit = null;

    [SerializeField, ReadOnly] // 判定フラグ
    private bool isCollosion = false;
    public bool IsCollosion => isCollosion;


    public void Initialize(string tagName)
    {
        this.tagName = tagName;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {       
        if(other.gameObject.tag == TagName)
        {
            onTriggerEnter?.Invoke(other);
            OnEnter(other);
            isCollosion = true;
        }
    }
    protected virtual void OnEnter(Collider2D other){}

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.tag == TagName)
        {
            onTriggerStay?.Invoke(other);
            OnStay(other);
        }
    }
    protected virtual void OnStay(Collider2D other){}

    private void OnTriggerExit2D(Collider2D other) 
    {
        if(other.gameObject.tag == TagName)
        {
            onTriggerExit?.Invoke(other);
            OnExit(other);
            isCollosion = false;
        }
    }
    protected virtual void OnExit(Collider2D other){}
}
