using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartUI : MonoBehaviour
{
    [SerializeField] // スタートボタン
    private Button startButton = null;
    [SerializeField] // スタートテキスト
    private TextMeshProUGUI startText = null;


    private void Start()
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(OnStartButton);

        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void OnStartButton()
    {
        startButton.interactable = false;
        startText.text = "読み込み中";
        GameManager.I.Fade(delegate{SceneManager.LoadScene("Home");}, false, "Normal");
    }
}
