using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] // 攻撃％
    private int atkPercent = 100;
    [SerializeField] // 攻撃可能距離
    private float area = 0;
    public float Area => area;
    [SerializeField] // 投げる攻撃
    private bool isThrow = false;
    public bool IsThrow => isThrow;
    [SerializeField] // 移動速度
    private float speed = 0;
    [SerializeField] // 生存時間
    private float survivalTime = 0;

    [SerializeField, ReadOnly] // 攻撃力
    private int atk = 0;
    // 経過時間
    private float countTime = 0;

    public void Initialize(int _atk)
    {
        atk = _atk * atkPercent / 100;
        countTime = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().OnDamage(atk);
        }
        else if(other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyController>().OnDamage(atk);
            GameUI.I.InitializeDamageText(atk, transform);
        }
    }

    private void FixedUpdate()
    {
        if(isThrow)
        {
            if(countTime > survivalTime)
            {
                Destroy(gameObject);
            }
            transform.position += transform.up * speed * Time.fixedDeltaTime;
            countTime += Time.fixedDeltaTime;
        }
    }
}
