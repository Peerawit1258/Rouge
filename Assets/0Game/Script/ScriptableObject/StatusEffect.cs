using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

[CreateAssetMenu(fileName = "NewStatus", menuName = "Battle/StatusEffect")]
public class StatusEffect : ScriptableObject
{
    [ReadOnly] public string id;
    public string statusName;
    [PreviewField] public Sprite statusIcon;
    public StatusType type;
    [SerializeField] UseThisObject use;
    [EnumToggleButtons] public TriggerStatus trigger;
    [ShowIf("@trigger == TriggerStatus.Action"), EnumToggleButtons, HideLabel] public SkillType skill;
    public Color color;
    public GameObject particleEffect;
    public bool canStack;
    //public bool useAllStack;
    [TextArea(4, 10)] public string descrition;

    [Title("Detail")]
    public Effect effect;
    [ShowIf("@effect == Effect.Stat && use == UseThisObject.Player")] public bool isSlot;
    [ShowIf("@effect == Effect.Stat && !isSlot")] public List<StatValue> statValues;
    [ShowIf("@effect == Effect.Stat && isSlot")] public List<DamageSlot> slots;
    
    [ShowIf("@effect == Effect.Regen"), Unit(Units.Percent)] public int heal;
    [ShowIf("@effect == Effect.DOT")] public int damage;
    [ShowIf("@canStack")] public int capStack ;

    [Button]
    public void SetId()
    {
        string[] st = name.Split("_");
        id = st[0];
        statusName = st[1];
    }

}

    [Serializable]
public class StatValue
{
    public StatType type;
    [Unit(Units.Percent)]public int value;
}

[Serializable]
public class AddStatus
{
    public StatusEffect statusEffect;
    public int count;
}

public enum StatType
{
    Atk,
    Def,
    DmgBonus,
    DmgReduce,
    Hp
}

public enum TriggerStatus
{
    Start,
    Action,
    End,
    Except
}

public enum Effect
{
    None,
    Stat,
    Regen,
    DOT
}

public enum StatusType
{
    Buff,
    Debuff,
    Special
}

public enum UseThisObject
{
    Both,
    Player,
    Enemy
}