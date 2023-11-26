using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattlerController
{
    void OnAttackEnd();
    void OnDamage(int damage);
}
