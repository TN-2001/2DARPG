using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : CollisionDetector
{
    [SerializeField] // 攻撃力
    private int atk = 0;
    [SerializeField] // 攻撃データ
    private AttackData data = null;
    // 経過時間
    private float countTime = 0;

    public void Initialize(int _atk, AttackData _data)
    {
        atk = _atk;
        data = _data;
        countTime = 0;
        onTriggerEnter.RemoveAllListeners();
        onTriggerEnter.AddListener(OnHit);
        gameObject.SetActive(true);
    }

    private void OnHit(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().OnDamage(atk);
        }
        else if(other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyController>().OnDamage(atk);
        }
    }

    private void FixedUpdate()
    {
        if(data._Type == AttackData.Type.Throw)
        {
            if(countTime > data.SurvivalTime)
            {
                Destroy(gameObject);
            }
            transform.position += transform.up * data.Speed * Time.fixedDeltaTime;
            countTime += Time.fixedDeltaTime;
        }
    }
}
