using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // シングルトン用変数
    public static T I = null;

    // シングルトンのタイプ
    protected enum Type
    {
        DontDestroy,
        Destroy,
    }
    protected virtual Type type => Type.DontDestroy;

    private void Awake()
    {
        switch(type)
        {
            case Type.DontDestroy:
            {
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
                break;
            }

            case Type.Destroy:
            {
                I = (T)this;
                break;
            }
        }

        OnAwake();
    }

    protected virtual void OnAwake(){}
}
