using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonUI : Singleton<DungeonUI>
{
    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    [SerializeField] // ダメージテキスト
    private GameObject damageText = null;
    [SerializeField] // hpスライダー
    private Slider hpSlider = null;
    [SerializeField] // hpテキスト
    private TextMeshProUGUI hpText = null;
    [SerializeField] // ダンジョン情報テキスト
    private TextMeshProUGUI dungeonText = null;


    public void InitializeDamageText(int damage, Transform target)
    {
        GameObject obj = Instantiate(
            damageText.gameObject, damageText.transform.position, Quaternion.identity, damageText.transform.parent);
        obj.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        obj.GetComponent<FollowTransform>().Initialize(target);
        obj.SetActive(true);
    }

    public void InitializeHpSlider(int hp)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        hpText.text = $"{hp}/{hp}";
    }
    public void UpdateHpSlider(int currentHp)
    {
        hpSlider.value = currentHp;
        hpText.text = $"{currentHp}/{hpSlider.maxValue}";
    }

    public void UpdateDungeonText(string name, int number)
    {
        dungeonText.text = $"{name} {number}F";
    }
}
