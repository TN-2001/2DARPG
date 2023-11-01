using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : Singleton<GameUI>
{
    [SerializeField] // ダメージテキスト
    private GameObject damageText = null;

    // シングルトンのタイプ
    protected override Type type => Type.Destroy;


    public void InitializeDamageText(int damage, Transform target)
    {
        GameObject obj = Instantiate(damageText.gameObject, damageText.transform.parent);
        obj.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        obj.GetComponent<FollowTransform>().Initialize(target);
    }
}
