using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeUI : Singleton<FadeUI>
{
    protected override Type type => Type.Destroy;

    [SerializeField] // フェード画像
    private Image fadeImage = null;

    private void Start()
    {
        if(GameManager.I.state == GameManager.State.UI)
        {
            StartCoroutine(EFadeOut());
        }
    }

    public void Fade(UnityAction action)
    {
        StartCoroutine(EFade(action));
    }
    private IEnumerator EFade(UnityAction action)
    {
        yield return EFadeIn(action);
        yield return EFadeOut();
    }

    public void FadeIn(UnityAction action)
    {
        StartCoroutine(EFadeIn(action));
    }
    private IEnumerator EFadeIn(UnityAction action)
    {
        GameManager.I.state = GameManager.State.UI;

        fadeImage.enabled = true;

        float alpha = 0;
        while(alpha < 1)
        {
            fadeImage.color = new Color(0,0,0,alpha);

            yield return null;

            alpha += Time.deltaTime;
        }

        // 関数実行
        if(action != null)
        {
            action();
        }
    }

    private IEnumerator EFadeOut()
    {
        fadeImage.enabled = true;

        float alpha = 1f;
        while(alpha > 0)
        {
            fadeImage.color = new Color(0,0,0,alpha);

            yield return null;

            alpha -= Time.deltaTime;
        }

        fadeImage.enabled = false;

        GameManager.I.state = GameManager.State.Player;
    }
}
