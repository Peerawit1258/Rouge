using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DamageCalculator : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public int DamageResult(float atk, int skillDMG, int targetDef, float damageBonus = 0, float targetDMGRe = 0)
    {
        int damage = (int)Mathf.Floor( (( (atk * skillDMG / 100) * (100 + damageBonus)/100) - targetDef) * (100 - targetDMGRe) / 100);
        if (damage <= 0) damage = 1;
        
        return damage;
    }

    public int TrueDamageResult(int atk, int skillDMG, int damageBonus = 0)
    {
        int damage = (int)Mathf.Floor(((atk * skillDMG / 100) * (100 + damageBonus) / 100));
        //if (damage <= 0) damage = 1;

        return damage;
    }

    //public void TargetTakenDamage(EnemyController target, int damage)
    //{
    //    target.DamageTaken(damage);
    //}
}
