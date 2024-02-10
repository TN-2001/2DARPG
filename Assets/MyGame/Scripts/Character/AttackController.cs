using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] // 当たり判定の対象のタグ
    private string tagName = null;
    [SerializeField] // 当たったら消えるタグ
    private List<string> destroyTagNameList = null;
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
    // 当てた敵
    private List<Collider2D> enemyList = new List<Collider2D>();


    public void Initialize(int atk)
    {
        this.atk = atk * atkPercent / 100;
        countTime = 0;
    }

    private void OnEnable()
    {
        enemyList = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == tagName){
            // 1匹の敵に1度だけヒットするように
            bool isHit = false;
            foreach(Collider2D enemy in enemyList){
                if(enemy == other) isHit = true;
            }
            if(!isHit){
                other.GetComponent<PlayerController>()?.OnDamage(atk);
                other.GetComponent<EnemyController>()?.OnDamage(atk);
                enemyList.Add(other);
            }
        }

        // 障害物に当たったら消す
        for(int i = 0; i < destroyTagNameList.Count; i++){
            if(other.gameObject.tag == destroyTagNameList[i]) Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if(isThrow){
            if(countTime > survivalTime) Destroy(gameObject);
            transform.position += transform.up * speed * Time.fixedDeltaTime;
            countTime += Time.fixedDeltaTime;
        }
    }
}
