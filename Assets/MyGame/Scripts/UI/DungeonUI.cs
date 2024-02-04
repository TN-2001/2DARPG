using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DungeonUI : MonoBehaviour
{
    public static DungeonUI I = null;

    [SerializeField] // ダメージテキスト
    private GameObject damageText = null;
    [SerializeField] // hpスライダー
    private Slider hpSlider = null;
    [SerializeField] // hpテキスト
    private TextMeshProUGUI hpText = null;
    [SerializeField] // ダンジョン情報テキスト
    private TextMeshProUGUI dungeonText = null;


    private void Awake()
    {
        I = this;
    }

    public void InitDamageText(int damage, Transform target)
    {
        GameObject obj = Instantiate(
            damageText.gameObject, damageText.transform.position, Quaternion.identity, damageText.transform.parent);
        obj.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        obj.GetComponent<FollowTransform>().Init(target);
        obj.SetActive(true);
    }

    public void InitHpSlider(int hp)
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

    public void UpdateDungeonText(string text)
    {
        dungeonText.text = text;
    }
}
