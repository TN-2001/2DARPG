using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class FadeUI : MonoBehaviour
{
    public static FadeUI I = null;

    [SerializeField] // フェード画像
    private Image fadeImage = null;
    // EventSystem
    private EventSystem eventSystem = null;
    

    private void Awake()
    {
        I = this;
        eventSystem = EventSystem.current;
    }

    public void Fade(UnityAction action)
    {
        StartCoroutine(EFade(action));
    }
    private IEnumerator EFade(UnityAction action)
    {
        yield return EFadeIn(action);
        yield return EFadeOut(null);
    }

    public void FadeIn(UnityAction action)
    {
        StartCoroutine(EFadeIn(action));
    }
    private IEnumerator EFadeIn(UnityAction action)
    {
        eventSystem.enabled = false;

        fadeImage.enabled = true;

        float alpha = 0;
        while(alpha < 1){
            fadeImage.color = new Color(0,0,0,alpha);

            yield return null;

            alpha += Time.deltaTime;
        }
        fadeImage.color = new Color(0,0,0,1);

        // 関数実行
        if(action != null) action();
    }

    public void FadeOut(UnityAction action)
    {
        StartCoroutine(EFadeOut(action));
    }
    private IEnumerator EFadeOut(UnityAction action)
    {
        eventSystem.enabled = false;
        fadeImage.enabled = true;

        float alpha = 1f;
        while(alpha > 0)
        {
            fadeImage.color = new Color(0,0,0,alpha);

            yield return null;

            alpha -= Time.deltaTime;
        }

        fadeImage.enabled = false;
        eventSystem.enabled = true;

        // 関数実行
        if(action != null) action();
    }
}
