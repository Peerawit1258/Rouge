using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterValue : MonoBehaviour
{
    [TabGroup("Base")] public int maxHpValue;
    [TabGroup("Base")] public int hpValue;
    [TabGroup("Base")] public float atkValue;
    [TabGroup("Base")] public float defValue;

    [TabGroup("Buff"), Unit(Units.Percent), SerializeField] int damageBonus;
    [TabGroup("Buff"), Unit(Units.Percent), SerializeField] int damageReduce;

    [ShowInInspector]public Dictionary<string, AddStatus> effect = new Dictionary<string, AddStatus>();

    float b_atk;
    float b_def;
    int p_atk;
    int p_def;
    int p_bonus;
    int p_reduce;
    public void SetStatValue(int hp, int atk, int def, int dmgBonus, int dmgReduce)
    {
        maxHpValue = hp;
        hpValue = hp;
        atkValue = atk;
        defValue = def;
        Debug.Log((int)atkValue + " " + atkValue);
        b_atk = atkValue;
        b_def = defValue;

        damageBonus = dmgBonus;
        damageReduce = dmgReduce;
    }

    public void StatusEffectResult(AddStatus status)
    {
        switch (status.statusEffect.type)
        {
            case StatusType.Buff:
                StatusEffect buff = status.statusEffect;
                if (buff.effect == Effect.Stat)
                {
                    
                }
                break;
            case StatusType.Debuff:

                break;
        }
    }

    public void AttackUp(int percent) => atkValue += (b_atk * percent / 100);

    public void StatUp(StatValue stat, bool assign = true)
    {
        if (assign)
        {
            if (stat.type == StatType.Atk)
            {
                atkValue += (b_atk * stat.value / 100);
                p_atk += stat.value;
            }
            else if (stat.type == StatType.Def)
            {
                defValue += (b_def * stat.value / 100);
                p_def += stat.value;
            }
            else if (stat.type == StatType.DmgBonus) damageBonus += stat.value;
            else damageReduce += stat.value;
        }
        else
        {
            if (stat.type == StatType.Atk)
            {
                atkValue -= (b_atk * stat.value / 100);
                p_atk -= stat.value;
            }
            else if (stat.type == StatType.Def)
            {
                defValue -= (b_def * stat.value / 100);
                p_def -= stat.value;
            }
            else if (stat.type == StatType.DmgBonus) damageBonus -= stat.value;
            else damageReduce -= stat.value;
        }
    }

    public int GetDamageBonus()
    {
        if(damageBonus < 0) return 0;
        return damageBonus;
    }

    public int GetDamageReduce()
    {
        if (damageReduce < 0) return 0;
        else if (damageReduce > 90) return 90;
        return damageBonus;
    }

}
