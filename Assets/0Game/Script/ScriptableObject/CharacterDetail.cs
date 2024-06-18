using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Battle/Character")]
public class CharacterDetail : ScriptableObject
{
    [PreviewField] public GameObject character;
    public string characterName;
    
    [TabGroup("CharacterType")] public CharacterType characterType;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy"), EnumToggleButtons] public EnemyType e_type;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy && e_type != EnemyType.Normal")] public string minion;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public int goldDrop;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public int expDrop;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public bool orderSkill;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public List<SkillAction> allSkill;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public List<SkillAction> skillDrop;
    [TabGroup("CharacterType"), ShowIf("@characterType == CharacterType.Enemy")] public List<Relic> relicDrop;
    
    [Header("Base")]
    [TabGroup("Stat")] public int maxHP;
    [TabGroup("Stat")] public int atk;
    [TabGroup("Stat")] public int def;

    [Header("Bonus")]
    [TabGroup("Stat"), Unit(Units.Percent)] public int damageBonus;
    [TabGroup("Stat"), Unit(Units.Percent)] public int damageReduce;

    [Header("StatusEffect")]
    [TabGroup("Stat")] public List<AddStatus> startEffect;

    [Button]
    public void ResetValue()
    {
        if (characterType != CharacterType.Player) return;

        maxHP = 50;
        atk = 20;
        def = 10;
    }
    
    [Button]
    public void SetName()
    {
        characterName = name.Split("_")[1];
    }
}

public enum CharacterType
{
    Player,
    Enemy,
    NPC
}

public enum EnemyType
{
    Normal,
    Elite,
    Boss,
    Special
}
