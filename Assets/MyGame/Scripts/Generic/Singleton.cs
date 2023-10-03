using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // シングルトン用変数
    public static T I = null;
    
    // このままでいいときはなにも書かず、追加するときはoverideしてbase.Awake()、変更時はまるまるoverride
    protected virtual void Awake()
    {
        // シングルトン化
        if (I == null)
        {
            I = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else if (I != this)
        {
            Destroy(gameObject);
            return;
        }
    }
}
