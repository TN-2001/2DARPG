using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StartUI : MonoBehaviour
{
    [SerializeField]
    private GameObject loadPanel;
    [SerializeField]
    private TextMeshProUGUI startText;

    private void Start()
    {
        GameManager.I.Input.actions.FindActionMap("Start").Enable();
    }

    private void Update()
    {
        if(GameManager.I.Input.actions["Start"].WasPerformedThisFrame())
        {
            loadPanel.SetActive(true);
            startText.text = "読み込み中";
            SceneManager.LoadSceneAsync("Main");
            GameManager.I.Input.actions.FindActionMap("Start").Disable();
        }
    }
}
