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
    [Header("アイテム")]
    [SerializeField] // アイテムコンテンツ
    private RectTransform itemContentTra = null;
    [SerializeField] // アイテムビュー
    private GameObject itemObj = null;
    [Header("武器")]
    [SerializeField] // 武器コンテンツ
    private RectTransform weaponContentTra = null;
    [SerializeField] // 武器トグル
    private List<ToggleUI> weaponToggleList = new List<ToggleUI>();


    private void Awake()
    {
        I = this;
    }

    private void Start()
    {
        // アイテムを消す
        foreach(Transform tra in itemContentTra){
            Destroy(tra.gameObject);
        }
    }

    // 敵のダメージ
    public void InitDamageText(int damage, Transform target)
    {
        GameObject obj = Instantiate(
            damageText.gameObject, damageText.transform.position, Quaternion.identity, damageText.transform.parent);
        obj.GetComponent<TextMeshProUGUI>().text = damage.ToString();
        obj.GetComponent<FollowTransform>().Init(target);
        obj.SetActive(true);
    }

    // プレイヤーのHp
    public void InitHpSlider(int hp)
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }
    public void UpdateHpSlider(int currentHp)
    {
        hpSlider.value = currentHp;
    }

    // 入手アイテム
    public void UpdateItemContent(List<string> nameList)
    {
        foreach(string name in nameList){
            GameObject obj = Instantiate(
                itemObj, itemObj.transform.position, Quaternion.identity, itemContentTra);
            obj.GetComponent<View>().UpdateUI(name);
            Destroy(obj, 3f);
        }
    }

    // 武器リスト
    public void InitWeaponContent(List<Sprite> spriteList)
    {
        foreach(ToggleUI toggle in weaponToggleList){
            toggle.gameObject.SetActive(false);
        }

        for(int i = 0; i < spriteList.Count; i++){
            weaponToggleList[i].gameObject.SetActive(true);
            weaponToggleList[i].GetComponent<View>().UpdateUI(spriteList[i]);
            weaponToggleList[i].group = weaponContentTra.GetComponent<ToggleGroup>();
        }
        weaponToggleList[0].Select();
    }
    public void SelectWeapon(int number)
    {
        weaponToggleList[number].Select();
    }
}
