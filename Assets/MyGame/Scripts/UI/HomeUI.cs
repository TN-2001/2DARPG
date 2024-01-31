using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeUI : MonoBehaviour
{
    [Header("選択")]
    [SerializeField] // 選択コンテンツ
    private RectTransform choiceContentTra = null;
    [SerializeField] // 選択オブジェクト
    private GameObject choiceBtnObj = null;
    [Header("スクロールヘッダー")]
    [SerializeField] // スクロールヘッダー
    private GameObject scrollHeader = null;
    [Header("長方形スクロール")]
    [SerializeField] // 長方形スクロール
    private GameObject rectScroll = null;
    [SerializeField] // 長方形コンテンツ
    private RectTransform rectContentTra = null;
    [SerializeField] // 長方形トグル
    private GameObject rectToggle = null;
    [Header("正方形スクロール")]
    [SerializeField] // 正方形スクロール
    private GameObject squareScroll = null;
    [SerializeField] // 正方形コンテンツ
    private RectTransform squareContentTra = null;
    [SerializeField] // 正方形トグル
    private GameObject squareToggle = null;
    [Header("装備ウィンドウ")]
    [SerializeField] // 装備ウィンドウ
    private GameObject equipWindowObj = null;
    [SerializeField] // 装備トグル
    private List<ToggleUI> equipToggleList = new List<ToggleUI>(5);
    [Header("右ウィンドウ")]
    [SerializeField] // 右ビュー
    private View rightView = null;
    [Header("コマンド")]
    [SerializeField] // コマンドビュー
    private List<Button> commandList = new List<Button>();

    [SerializeField, ReadOnly] // 番号
    private int number = 0;


    private void Start()
    {
        FadeUI.I.FadeOut(null);
    }

    // 選択
    private void OnChoiceView(List<(string name, UnityAction method)> nameList)
    {
        OffChoiceView();

        // 選択ボタンを生成
        for(int i = 0;  i < nameList.Count; i++)
        {
            GameObject obj = Instantiate(
                choiceBtnObj, choiceBtnObj.transform.position, Quaternion.identity, choiceContentTra);
            obj.GetComponent<View>().UpdateUI(nameList[i].name);
            obj.GetComponent<Button>().onClick.AddListener(nameList[i].method);

            if(i == 0) obj.GetComponent<Button>().Select();
        }

        choiceContentTra.gameObject.SetActive(true);
    }

    private void OffChoiceView()
    {
        // 全ての選択ボタンを削除
        foreach(RectTransform tra in choiceContentTra)
        {
            Destroy(tra.gameObject);
        }

        choiceContentTra.gameObject.SetActive(false);
    }

    //ボックスUI
    public void OnBoxView()
    {
        PlayerController.I.isIdle = true;
        OnEquipView();
    }

    private void OnEquipView()
    {
        Player player = GameManager.I.Data.Player;
        // トグルの編集
        equipToggleList[0].GetComponent<View>().UpdateUI(player.Weapon.Data.Name);
        equipToggleList[0].onSelect.RemoveAllListeners();
        string equipInfo = $"レベル：{player.Weapon.Lev}\n攻撃力：{player.Weapon.Atk}\n\n{player.Weapon.Data.Info}";
        equipToggleList[0].onSelect.AddListener(delegate{
            rightView.UpdateUI(new List<string>(){player.Weapon.Data.Name, equipInfo});
            number = 0;
            // コマンドボタンを初期化
            if(GameManager.I.Data.WeaponList.Count > 0){
                OnCommandView(new List<(string name, UnityAction method)>(){
                    ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}),
                    ("装備変更", delegate{OffView(); OnWeaponView();})
                });
            }
            else{
                OnCommandView(new List<(string name, UnityAction method)>(){
                    ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;})
                });
            }
        });
        for(int i = 0; i < 4; i++)
        {
            ToggleUI toggle = equipToggleList[i+1];
            toggle.onSelect.RemoveAllListeners();
            List<Armor> armorList =  GameManager.I.Data.ArmorList;
            if(i == 0) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Head);
            else if(i == 1) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Chest);
            else if(i == 2) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Arm);
            else armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Leg);
            if(player.ArmorList[i].Data != null){
                Armor armor = player.ArmorList[i];
                toggle.GetComponent<View>().UpdateUI(armor.Data.Name);
                toggle.onSelect.AddListener(delegate{
                    string info = $"体力　：{armor.Data.Hp}\n\n{armor.Data.Info}";
                    rightView.UpdateUI(new List<string>(){armor.Data.Name, info});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(armorList.Count > 0){
                        OnCommandView(new List<(string name, UnityAction method)>(){
                            ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}),
                            ("装備変更", delegate{OffView(); OnArmorView();})
                        });
                    }
                    else{
                        OnCommandView(new List<(string name, UnityAction method)>(){
                            ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;})
                        });
                    }
                });
            }
            else{
                toggle.GetComponent<View>().UpdateUI("なし");
                toggle.onSelect.AddListener(delegate{
                    rightView.UpdateUI(new List<string>(){"なし"});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(armorList.Count > 0){
                        OnCommandView(new List<(string name, UnityAction method)>(){
                            ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}),
                            ("装備変更", delegate{OffView(); OnArmorView();})
                        });
                    }
                    else{
                        OnCommandView(new List<(string name, UnityAction method)>(){
                            ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;})
                        });
                    }
                });
            }
        }

        // 初期化
        equipWindowObj.SetActive(true);
        rightView.gameObject.SetActive(true);
        equipToggleList[0].Select();
    }

    private void OnWeaponView()
    {
        // コンテンツ内をからに
        foreach(Transform child in squareContentTra) Destroy(child.gameObject);
        // コンテンツ内に必要なものを生成
        List<Weapon> weaponList = GameManager.I.Data.WeaponList;
        ToggleUI firstToggle = null;
        for(int i = 0; i < weaponList.Count; i++)
        {
            GameObject obj = Instantiate(
                squareToggle, squareToggle.transform.position, Quaternion.identity, squareContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(weaponList[i].Data.Name);
            ToggleUI toggle = obj.GetComponent<ToggleUI>();
            toggle.onSelect.AddListener(delegate{
                number = int.Parse(obj.name);
                Weapon data = weaponList[number];
                string info = $"レベル：{data.Lev}\n攻撃力：{data.Atk}\n\n{data.Data.Info}";
                rightView.UpdateUI(new List<string>(){data.Data.Name, info});
            });
            toggle.group = squareContentTra.GetComponent<ToggleGroup>();

            if(i == 0) firstToggle = toggle;
        }

        // 初期化
        squareScroll.SetActive(true);
        rightView.gameObject.SetActive(true);
        firstToggle?.Select();

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>(){
            ("戻る", delegate{OffView(); OnEquipView();}), 
            ("装備", delegate{GameManager.I.Data.ChengeWeapon(number); OffView(); OnEquipView();})
        });
    }

    private void OnArmorView()
    {

    }

    // ダンジョンUI
    public void OnDungeonView()
    {
        PlayerController.I.isIdle = true;

        // コンテンツ内をからに
        foreach(Transform child in rectContentTra) Destroy(child.gameObject);
        // コンテンツ内に必要なものを生成
        List<DungeonData> dungeonDataList = GameManager.I.Data.DungeonDataList;
        ToggleUI firstToggle = null;
        for(int i = 0; i < dungeonDataList.Count; i++)
        {
            GameObject obj = Instantiate(
                rectToggle, rectToggle.transform.position, Quaternion.identity, rectContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(dungeonDataList[i].Name);
            ToggleUI toggle = obj.GetComponent<ToggleUI>();
            toggle.onSelect.AddListener(delegate{
                number = int.Parse(obj.name);
                DungeonData data = dungeonDataList[number];
                string info = $"階層数：{data.FloorNumber}階";
                for(int j = 0; j < data.EnemyDataList.Count; j++){
                    if(j == 0) info = $"{info}\n敵　　：{data.EnemyDataList[j].Name}";
                    else  info = $"{info}\n　　　：{data.EnemyDataList[j].Name}";
                }
                info = $"{info}\nボス敵：{data.BossEnemyData.Name}";
                rightView.UpdateUI(new List<string>(){data.Name, info});
            });
            toggle.group = rectContentTra.GetComponent<ToggleGroup>();

            if(i == 0) firstToggle = toggle;
        }

        // 初期化
        rectScroll.SetActive(true);
        rightView.gameObject.SetActive(true);
        firstToggle.Select();

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>()
            {("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}), ("行く", GoDungeon)});
    }

    private void GoDungeon()
    {
        GameManager.I.InitDungeon(number);
        FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Dungeon");});
    }

    // コマンド
    private void OnCommandView(List<(string name, UnityAction method)> nameList)
    {
        // 全てのコマンドボタンをoff
        foreach(Button btn in commandList)
        {
            btn.gameObject.SetActive(false);
            btn.onClick.RemoveAllListeners();
        }

        // コマンドボタンを編集
        for(int i = 0;  i < nameList.Count; i++)
        {
            Button btn = commandList[i];
            btn.onClick.AddListener(nameList[i].method);
            btn.GetComponent<View>().UpdateUI(nameList[i].name);
            btn.gameObject.SetActive(true);
        }
    }

    // オフ
    private void OffView()
    {
        // 全てのウィンドウをオフ
        rectScroll.SetActive(false);
        squareScroll.SetActive(false);
        equipWindowObj.SetActive(false);
        rightView.gameObject.SetActive(false);

        // 全てのコマンドボタンをoff
        foreach(Button btn in commandList)
        {
            btn.gameObject.SetActive(false);
        }
    }
}
