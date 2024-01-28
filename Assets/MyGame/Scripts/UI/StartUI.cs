using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartUI : MonoBehaviour
{
    [Header("メニュー")]
    [SerializeField] // つづきからボタン
    private Button continueBtn = null;
    [SerializeField] // スタートボタン
    private Button startBtn = null;
    [Header("リスタート")]
    [SerializeField] // リスタートビュー
    private GameObject reStartObj = null;
    [SerializeField] // はいボタン
    private Button yesBtn = null;
    [SerializeField] // いいえボタン
    private Button noBtn = null;


    private void Start()
    {
        if(GameManager.I.Data.Player.Data == null)
        {
            continueBtn.gameObject.SetActive(false);
            startBtn.onClick.AddListener(delegate{
                GameManager.I.Init();
                FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Home");});
            });
        }
        else
        {
            continueBtn.onClick.AddListener(delegate{
                FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Home");});
            });
            startBtn.onClick.AddListener(delegate{
                reStartObj.SetActive(true);
                noBtn.Select();
            });

            yesBtn.onClick.AddListener(delegate{
                GameManager.I.Init();
                FadeUI.I.FadeIn(delegate{SceneManager.LoadScene("Home");});
            });
            noBtn.onClick.AddListener(delegate{
                reStartObj.SetActive(false);
                continueBtn.Select();
            });
        }

        FadeUI.I.FadeOut(delegate{
            if(GameManager.I.Data.Player.Data == null) startBtn.Select();
            else continueBtn.Select();
        });
    }
}
