using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class HomeUI : Singleton<HomeUI>
{
    // シングルトンのタイプ
    protected override Type type => Type.Destroy;

    [Header("ショップUI")]
    [SerializeField] // ショップビュー
    private GameObject shopView = null;
    [Header("ダンジョンUI")]
    [SerializeField] // ダンジョンビュー
    private GameObject dungeonView = null;
    [SerializeField] // ダンジョンコンテンツ
    private RectTransform dungeonContentTra = null;
    [SerializeField] // ダンジョントグル
    private GameObject dungeonToggle = null;
    [Header("コマンドUI")]
    [SerializeField] // コマンドビュー
    private RectTransform commandViewTra = null;
    [SerializeField] // コマンドボタン
    private GameObject commandButton = null;

    [SerializeField, ReadOnly] // 番号
    private int number = 0;


    public void OnShopView()
    {
        shopView.SetActive(true);

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>()
            {("閉じる", OffShopView),("買う",null)});
    }

    private void OffShopView()
    {
        shopView.SetActive(false);
        OffCommandView();
    }


    public void OnDungeonView()
    {
        // コンテンツ内をからに
        foreach(Transform child in dungeonContentTra)
        {
            Destroy(child.gameObject);
        }
        // コンテンツ内に必要なものを生成
        List<DungeonData> dungeonDataList = GameManager.I.DataBase.DungeonDataList;
        for(int i = 0; i < dungeonDataList.Count; i++)
        {
            GameObject obj = Instantiate(
                dungeonToggle, dungeonToggle.transform.position, Quaternion.identity, dungeonContentTra);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = dungeonDataList[i].Name;
            obj.GetComponent<Toggle>().onValueChanged.AddListener(OnToggle);
            obj.GetComponent<Toggle>().group = dungeonContentTra.GetComponent<ToggleGroup>();
        }
        // ビューを表示
        dungeonView.SetActive(true);

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>()
            {("閉じる", OffDungeonView),("行く",GoDungeon)});
    }
    private void OnToggle(bool isOn)
    {
        if(isOn)
        {
            for(int i = 0; i < dungeonContentTra.childCount; i++)
            {
                if(dungeonContentTra.GetChild(i).GetComponent<Toggle>().isOn)
                {
                    number = i;
                    break;
                }
            }
        }
    }

    private void GoDungeon()
    {
        GameManager.I.InitializeDungeon(number);
        GameManager.I.Fade(delegate{SceneManager.LoadScene("Dungeon");}, false);
    }
    private void OffDungeonView()
    {
        dungeonView.SetActive(false);
        OffCommandView();
    }


    private void OnCommandView(List<(string name, UnityAction method)> nameList)
    {
        GameManager.I.state = GameManager.State.UI;

        // コマンドボタンを削除
        foreach(Transform child in commandViewTra)
        {
            Destroy(child.gameObject);
        }

        // コマンドボタンを生成
        for(int i = 0;  i < nameList.Count; i++)
        {
            GameObject obj = Instantiate(
                commandButton, commandButton.transform.position, Quaternion.identity, commandViewTra);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = nameList[i].name;
            obj.GetComponent<Button>().onClick.AddListener(nameList[i].method);
        }

        // ビュー表示
        commandViewTra.gameObject.SetActive(true);
    }

    private void OffCommandView()
    {
        GameManager.I.state = GameManager.State.Player;

        // ビューオフ
        commandViewTra.gameObject.SetActive(false);
    }
}
