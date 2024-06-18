using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AllData : MonoBehaviour
{
    [TabGroup("PlayerData")] public List<SkillAction> allSkill;
    [TabGroup("PlayerData")] public List<Relic> allRelics;

    [TabGroup("EnemyData")] public List<CharacterDetail> allEnemy;

    [TabGroup("GameData")] public List<StatusEffect> allStatus;
    [TabGroup("GameData")] public List<Relic> relicReward = new List<Relic>();

    private void Start()
    {

    }

    #region Skill
    public SkillAction GetSkillWithID(string id)
    {
        foreach (SkillAction action in allSkill)
            if (action.id == id)
                return action;

        return null;
    }

    public List<SkillAction> GetSkillWithRarity(Rarity rarity)
    {
        List<SkillAction> lists = new List<SkillAction>();

        foreach(SkillAction action in allSkill)
            if(action.rarity == rarity)
                lists.Add(action);

        return lists;
    }
    #endregion
    #region Relic
    public Relic GetRelicWithID(string id)
    {
        foreach (Relic relic in allRelics)
            if (relic.id == id)
                return relic;

        return null;
    }

    public List<Relic> GetRelicWithRarity(Rarity rarity)
    {
        List<Relic> lists = new List<Relic>();

        foreach (Relic relic in allRelics)
            if (relic.rarity == rarity && relic.relicType == RelicType.Normal)
                lists.Add(relic);

        return lists;
    }
    #endregion
    #region Status
    public AddStatus GetStatusWithID(string id)
    {
        AddStatus statusEffect = new AddStatus();

        foreach(StatusEffect status in allStatus)
            if(status.id == id)
                statusEffect.statusEffect = status;
        statusEffect.count = 1;

        return statusEffect;
    }
    #endregion
    #region Enemy
    public CharacterDetail GetEnemyWithName(string e_name)
    {
        foreach (var enemy in allEnemy)
            if (enemy.characterName == e_name)
                return enemy;

        return null;
    }
    #endregion
}
