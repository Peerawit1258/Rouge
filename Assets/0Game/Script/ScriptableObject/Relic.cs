using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName ="NewRelic",menuName = "Battle/Relic")]
public class Relic : ScriptableObject
{
    public string id;
    public string relicName;
    [PreviewField(50)] public Sprite icon;
    [EnumToggleButtons]public RelicType relicType;
    [ShowIf("@relicType != RelicType.Curse")]public Rarity rarity;
    public bool destroyWhenUse;
    [TextArea(4, 10)] public string description;

    [Title("Detail")]
    public List<RelicDetail> relicDetails;


    [Button]
    public void SetId()
    {
        string[] s = name.Split("_");
        id = s[0];
        if(s.Length == 2) relicName = s[1];
    }
}

[Serializable]
public class RelicDetail
{
    public RelicEffectType type;
    [ShowIf("@type == RelicEffectType.Stat")] public bool isSlot;
    [ShowIf("@type == RelicEffectType.Stat && !isSlot")] public bool hpCompare;
    [ShowIf("@type == RelicEffectType.Stat && hpCompare && !isSlot")] public Compare compare;
    [ShowIf("@type == RelicEffectType.Stat && hpCompare && !isSlot")] public float hpPercent;
    [ShowIf("@type == RelicEffectType.Stat && !isSlot")] public List<StatValue> statValues;
    [ShowIf("@type == RelicEffectType.Stat && isSlot")] public List<DamageSlot> slots;

    //[ShowIf("@type == RelicEffectType.HpAbility")] public HpAbility hpAbility;
    [ShowIf("@type == RelicEffectType.BuffDebuff")] public TriggerStatus trigger;
    [ShowIf("@type == RelicEffectType.BuffDebuff")] public StatusType statusType;
    //[ShowIf("@type == RelicType.BuffDebuff && statusType == StatusType.Debuff")] public AddStatus statu;
    [ShowIf("@type == RelicEffectType.BuffDebuff")] public AddStatus status;

    [ShowIf("@type == RelicEffectType.DOT")] public bool isStack;
    [ShowIf("@type == RelicEffectType.DOT && isStack")] public int stack;
    [ShowIf("@type == RelicEffectType.DOT && !isStack")] public string dotID;
    [ShowIf("@type == RelicEffectType.DOT && !isStack"), Unit(Units.Percent)] public int dotDamage;

    [ShowIf("@type == RelicEffectType.Money")] public bool isShop;
    [ShowIf("@type == RelicEffectType.Money && isShop"), Unit(Units.Percent)] public int discount;
    [ShowIf("@type == RelicEffectType.Money && !isShop"), Unit(Units.Percent)] public int extraMoney;

    [ShowIf("@type == RelicEffectType.Skill")] public SkillRelic skillRelic;
    [ShowIf("@type == RelicEffectType.Skill && skillRelic == SkillRelic.Random")] public int increaseRandomSkill;
    [ShowIf("@type == RelicEffectType.Skill && skillRelic == SkillRelic.Drop")] public int increaseDropSkill;

    [ShowIf("@type == RelicEffectType.Exp")] public int percentExp;

    public bool CheckHpCompare(float ratio)
    {
        if(!hpCompare) return false;

        if (compare == Compare.Lower)
        {
            if (ratio < hpPercent / 100) return true;
            else return false;
        }
        else if(compare == Compare.Greater)
        {
            if (ratio > hpPercent / 100) return true;
            else return false;
        }
        else
        {
            if (ratio == hpPercent / 100) return true;
            else return false;
        }
            
    }
}

public enum RelicEffectType{
    Stat,
    BuffDebuff,
    DOT,
    Money,
    Skill,
    Other,
    Exp
}

public enum RelicType
{
    Normal,
    Curse,
    KeyItem
}

public enum SkillRelic
{
    Random,
    Drop
}

[Serializable]
public class HpRelic
{
    public Compare hpCompare;
    public int hpPercent;
    
}




