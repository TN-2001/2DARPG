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
    [SerializeField] // プレイヤーステータステキスト
    private TextMeshProUGUI playerStatusText = null;
    [Header("右ウィンドウ")]
    [SerializeField] // 右ビュー
    private View rightView = null;
    [Header("コマンド")]
    [SerializeField] // コマンドビュー
    private List<Button> commandList = new List<Button>();

    [SerializeField, ReadOnly] // 番号
    private int number = 0;
    [SerializeField, ReadOnly] // 前の番号
    private int preNumber = 0;


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
        preNumber = 0;
        OnEquipView();
    }

    private void OnEquipView()
    {
        Player player = GameManager.I.Data.Player;
        // トグルの編集
        for(int i = 0; i < 4; i++)
        {
            ToggleUI toggle = equipToggleList[i];
            toggle.onSelect.RemoveAllListeners();
            List<Weapon> weaponList =  GameManager.I.Data.WeaponList;
            if(player.WeaponList[i].Data){
                Weapon weapon = player.WeaponList[i];
                toggle.GetComponent<View>().UpdateUI(weapon.Data.Name);
                toggle.onSelect.AddListener(delegate{
                    string info = $"レベル：{weapon.Lev}\n攻撃力：{weapon.Atk}\n\n{weapon.Data.Info}";
                    rightView.UpdateUI(new List<string>(){weapon.Data.Name, info});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(weaponList.Count > 0) commandList[1].gameObject.SetActive(true);
                    else commandList[1].gameObject.SetActive(false);
                    if(player.WeaponList.Count >= 2) commandList[2].gameObject.SetActive(true);
                    else commandList[2].gameObject.SetActive(false);
                    // プレイヤー情報
                    player.UpdateCurrentWeapon(number);
                    playerStatusText.text = $"レベル：{player.Lev}\n体力　：{player.Hp}\n攻撃力：{player.Atk}\n（武器{player.WeaponNumber+1}）";
                });
            }
            else{
                toggle.GetComponent<View>().UpdateUI($"（武器{i+1}）");
                toggle.onSelect.AddListener(delegate{
                    rightView.UpdateUI(new List<string>(){"なし"});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(weaponList.Count > 0) commandList[1].gameObject.SetActive(true);
                    else commandList[1].gameObject.SetActive(false);
                    commandList[2].gameObject.SetActive(false);
                    // プレイヤー情報
                    player.UpdateCurrentWeapon(number);
                    playerStatusText.text = $"レベル：{player.Lev}\n体力　：{player.Hp}\n攻撃力：{player.Atk}\n（武器{player.WeaponNumber+1}）";
                });
            }
        }
        for(int i = 0; i < 4; i++)
        {
            ToggleUI toggle = equipToggleList[i+4];
            toggle.onSelect.RemoveAllListeners();
            List<Armor> armorList =  GameManager.I.Data.ArmorList;
            if(i == 0) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Head);
            else if(i == 1) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Chest);
            else if(i == 2) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Arm);
            else armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Leg);
            if(player.ArmorList[i].Data){
                Armor armor = player.ArmorList[i];
                toggle.GetComponent<View>().UpdateUI(armor.Data.Name);
                toggle.onSelect.AddListener(delegate{
                    string info = $"レベル：{armor.Lev}\n体力　：{armor.Hp}\n\n{armor.Data.Info}";
                    rightView.UpdateUI(new List<string>(){armor.Data.Name, info});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(armorList.Count > 0) commandList[1].gameObject.SetActive(true);
                    else commandList[1].gameObject.SetActive(false);
                    commandList[2].gameObject.SetActive(true);
                });
            }
            else{
                if(i == 0) toggle.GetComponent<View>().UpdateUI("（頭）");
                else if(i == 1) toggle.GetComponent<View>().UpdateUI("（胸）");
                else if(i == 2) toggle.GetComponent<View>().UpdateUI("（腕）");
                else toggle.GetComponent<View>().UpdateUI("（脚）");
                toggle.onSelect.AddListener(delegate{
                    rightView.UpdateUI(new List<string>(){"なし"});
                    number = int.Parse(toggle.gameObject.name);
                    // コマンドボタン初期化
                    if(armorList.Count > 0) commandList[1].gameObject.SetActive(true);
                    else commandList[1].gameObject.SetActive(false);
                    commandList[2].gameObject.SetActive(false);
                });
            }
        }
        // プレイヤー情報
        playerStatusText.text = $"レベル：{player.Lev}\n体力　：{player.Hp}\n攻撃力：{player.Atk}\n（武器{player.WeaponNumber+1}";

        // コマンド初期化
        OnCommandView(new List<(string name, UnityAction method)>(){
            ("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}),
            ("装備変更", delegate{
                preNumber = number;
                OffView();
                if(number < 4) OnWeaponView();
                else OnArmorView();
            }),
            ("外す", delegate{
                preNumber = number;
                OffView();
                if(number < 4) GameManager.I.Data.UpdateWeapon(null, number);
                else GameManager.I.Data.UpdateArmor(null, number-4);
                OnEquipView();
            }),
        });

        // 初期化
        equipWindowObj.SetActive(true);
        rightView.gameObject.SetActive(true);
        equipToggleList[preNumber].Select();
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

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>(){
            ("戻る", delegate{OffView(); OnEquipView();}), 
            ("装備", delegate{
                GameManager.I.Data.UpdateWeapon(weaponList[number], preNumber);
                OffView();
                OnEquipView();
            })
        });

        // 初期化
        squareScroll.SetActive(true);
        rightView.gameObject.SetActive(true);
        firstToggle?.Select();
    }

    private void OnArmorView()
    {
        // コンテンツ内をからに
        foreach(Transform child in squareContentTra) Destroy(child.gameObject);
        // コンテンツ内に必要なものを生成
        List<Armor> armorList = GameManager.I.Data.ArmorList;
        int typeNumber = preNumber - 4;
        if(typeNumber == 0) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Head);
        else if(typeNumber == 1) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Chest);
        else if(typeNumber == 2) armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Arm);
        else armorList = armorList.FindAll(x => x.Data.ArmorType == ArmorType.Leg);
        ToggleUI firstToggle = null;
        for(int i = 0; i < armorList.Count; i++)
        {
            GameObject obj = Instantiate(
                squareToggle, squareToggle.transform.position, Quaternion.identity, squareContentTra);
            obj.name = i.ToString();
            obj.GetComponent<View>().UpdateUI(armorList[i].Data.Name);
            ToggleUI toggle = obj.GetComponent<ToggleUI>();
            toggle.onSelect.AddListener(delegate{
                number = int.Parse(obj.name);
                Armor data = armorList[number];
                string info = $"レベル：{data.Lev}\n体力　：{data.Hp}\n\n{data.Data.Info}";
                rightView.UpdateUI(new List<string>(){data.Data.Name, info});
            });
            toggle.group = squareContentTra.GetComponent<ToggleGroup>();

            if(i == 0) firstToggle = toggle;
        }

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>(){
            ("戻る", delegate{OffView(); OnEquipView();}), 
            ("装備", delegate{
                GameManager.I.Data.UpdateArmor(armorList[number], typeNumber);
                OffView();
                OnEquipView();
            })
        });

        // 初期化
        squareScroll.SetActive(true);
        rightView.gameObject.SetActive(true);
        firstToggle?.Select();
    }

    // ダンジョンUI
    public void OnDungeonView()
    {
        PlayerController.I.isIdle = true;

        // コンテンツ内をからに
        foreach(Transform child in rectContentTra) Destroy(child.gameObject);
        // コンテンツ内に必要なものを生成
        List<DungeonData> dungeonDataList = 
            GameManager.I.DataBase.DungeonDataList.FindAll(x => x.Number <= GameManager.I.Data.CurrentDungeonNumber);
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

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>()
            {("閉じる", delegate{OffView(); PlayerController.I.isIdle = false;}), ("行く", GoDungeon)});

        // 初期化
        rectScroll.SetActive(true);
        rightView.gameObject.SetActive(true);
        firstToggle.Select();
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
