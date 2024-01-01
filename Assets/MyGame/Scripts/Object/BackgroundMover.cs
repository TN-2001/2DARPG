using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] // マテリアルプロパティ名
    private string matPropName = null;
    [SerializeField] // 移動速度
    private Vector2 speed = new Vector2();

    // マテリアル
    private Material mat = null;


    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        float x = Mathf.Repeat(Time.time * speed.x, 1f);
        float y = Mathf.Repeat(Time.time * speed.y, 1f);
        mat.SetTextureOffset(matPropName, new Vector2(x,y));
    }

    private void OnDestroy()
    {
        mat.SetTextureOffset(matPropName, Vector2.zero);
    }
}
