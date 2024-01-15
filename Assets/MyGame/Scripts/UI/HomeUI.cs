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
    [Header("コマンド")]
    [SerializeField] // コマンドビュー
    private RectTransform commandViewTra = null;
    [SerializeField] // コマンドボタン
    private GameObject commandButton = null;

    [SerializeField, ReadOnly] // 番号
    private int number = 0;


    // ダンジョンUI
    public void OnDungeonView()
    {
        // コンテンツ内をからに
        foreach(Transform child in rectContentTra)
        {
            Destroy(child.gameObject);
        }
        // コンテンツ内に必要なものを生成
        List<DungeonData> dungeonDataList = GameManager.I.DataBase.DungeonDataList;
        for(int i = 0; i < dungeonDataList.Count; i++)
        {
            GameObject obj = Instantiate(
                rectToggle, rectToggle.transform.position, Quaternion.identity, rectContentTra);
            obj.GetComponentInChildren<TextMeshProUGUI>().text = dungeonDataList[i].Name;
            Toggle toggle = obj.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate(bool isOn){
                if(isOn){
                    for(int i = 0; i < rectContentTra.childCount; i++){
                        if(rectContentTra.GetChild(i).GetComponent<Toggle>().isOn){
                            number = i;
                            break;
                        }
                    }
                }
            });
            toggle.group = rectContentTra.GetComponent<ToggleGroup>();
            if(i == 0)
            {
                toggle.isOn = true;
                toggle.Select();
            }
        }

        // 表示
        rectScroll.SetActive(true);

        // コマンドボタンを初期化
        OnCommandView(new List<(string name, UnityAction method)>()
            {("閉じる", OffDungeonView),("行く",GoDungeon)});
    }

    private void GoDungeon()
    {
        GameManager.I.InitializeDungeon(number);
        GameManager.I.Fade(delegate{SceneManager.LoadScene("Dungeon");}, false);
    }

    private void OffDungeonView()
    {
        rectScroll.SetActive(false);
        OffCommandView();
    }

    // コマンド
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
