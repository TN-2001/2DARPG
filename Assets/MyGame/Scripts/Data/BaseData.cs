using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData : ScriptableObject
{
    [SerializeField] // 名前
    private new string name = null;
    public string Name => name;
    [SerializeField, TextArea] // 情報
    private string info = null;
    public string Info => info;
}

public class Base
{
    private BaseData data = null;

    // 名前
    public string Name => data.Name;
    // 情報
    public string Info => data.Info;


    public Base(BaseData data)
    {
        this.data = data;
    }
}
