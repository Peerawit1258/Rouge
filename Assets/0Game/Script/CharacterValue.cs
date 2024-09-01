using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CharacterValue : MonoBehaviour
{
    [TabGroup("Base")] public float maxHpValue;
    [TabGroup("Base")] public int hpValue;
    [TabGroup("Base")] public float atkValue;
    [TabGroup("Base")] public float defValue;

    [TabGroup("Buff"), Unit(Units.Percent), SerializeField] int damageBonus;
    [TabGroup("Buff"), Unit(Units.Percent), SerializeField] int damageReduce;

    bool isEnemy;

    float b_atk;
    float b_def;
    int p_atk;
    int p_def;
    int p_bonus;
    int p_reduce;
    public void SetStatValue(int hp, int atk, int def, int dmgReduce, bool isEnemy = false)
    {
        maxHpValue = hp;
        hpValue = hp;
        atkValue = atk;
        defValue = def;

        b_hp = maxHpValue;
        b_atk = atkValue;
        b_def = defValue;
        damageReduce = dmgReduce;

        this.isEnemy = isEnemy;
    }

    public void StatUp(StatValue stat, bool assign = true)
    {
        if (assign)
        {
            if (stat.type == StatType.Atk) p_atk += stat.value;
            else if (stat.type == StatType.Def) p_def += stat.value;
            else if (stat.type == StatType.DmgBonus) damageBonus += stat.value;
            else damageReduce += stat.value;
        }
        else
        {
            if (stat.type == StatType.Atk) p_atk -= stat.value;
            else if (stat.type == StatType.Def) p_def -= stat.value;
            else if (stat.type == StatType.DmgBonus) damageBonus -= stat.value;
            else damageReduce -= stat.value;
        }

        UpdateStatValue(stat.type);
    }

    float b_hp;
    public void BaseStatUpdate(StatValue stat, bool decrease = false)
    {
        if (!decrease)
        {
            if (stat.type == StatType.Atk) b_atk += stat.value;
            else if (stat.type == StatType.Def) b_def += stat.value;
            else if (stat.type == StatType.Hp) b_hp += stat.value;
        }
        else
        {
            if (stat.type == StatType.Atk) b_atk -= stat.value;
            else if (stat.type == StatType.Def) b_def -= stat.value;
            else if (stat.type == StatType.Hp) b_hp -= stat.value;
        }

        UpdateStatValue(stat.type);
    }

    public void UpdateStatValue(StatType type)
    {
        if(type == StatType.Atk)
        {
            float atkTarget = b_atk + (b_atk * p_atk / 100);
            if (atkValue != atkTarget)
            {
                if (!isEnemy)
                    GameManager.instance.detailPanel.ChangeAtkValue((int)atkTarget, (int)atkValue);
                atkValue = b_atk + (b_atk * p_atk / 100);
                if (atkValue < 1) atkValue = 1;
            }
        }
        else if(type == StatType.Def)
        {
            float defTarget = b_def + (b_def * p_def / 100);
            if (defValue != defTarget)
            {
                if (!isEnemy)
                    GameManager.instance.detailPanel.ChangeDefValue((int)defTarget, (int)defValue);
                defValue = defTarget;
                if (defValue < 0) defValue = 0;
            }
        }
        else if (type == StatType.Hp)
        {
            if(b_hp != maxHpValue)
            {
                float hpTarget = b_hp * (hpValue / maxHpValue);
                maxHpValue = b_hp;
                hpValue = (int)hpTarget;
                if (!isEnemy)
                    GameManager.instance.turnManager.player.gaugeHp.HpGaugeChange(hpValue, maxHpValue);
            }
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
