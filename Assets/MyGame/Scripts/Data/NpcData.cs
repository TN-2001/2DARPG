using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/NpcData")]
public class NpcData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField, TextArea] // 情報
    private string info = null;
    public string Info => info;
    [SerializeField] // イメージ
    private Sprite image = null;
    public Sprite Image => image;
}
