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
    [ShowIf("@type == RelicEffectType.Stat && !isSlot")] public List<StatValue> statValues;
    [ShowIf("@type == RelicEffectType.Stat && isSlot")] public List<DamageSlot> slots;

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

    [ShowIf("@type == RelicEffectType.Skill")] public int increaseRandomSkill;
}

public enum RelicEffectType{
    Stat,
    BuffDebuff,
    DOT,
    Money,
    Skill,
    Other
}

public enum RelicType
{
    Normal,
    Curse,
    KeyItem
}



