using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerData : MonoBehaviour
{
    public List<SkillAction> currentSkills = new List<SkillAction>();
    public List<Relic> currentRelics = new List<Relic>();
    public int gold;

    public void AddNewSkill(SkillAction skill)
    {
        currentSkills.Add(skill);
    }

    public void RemoveCurrentSkill(SkillAction skill = null)
    {
        if(skill != null)
        {
            currentSkills.Remove(skill);
        }
        else //Random Remove
        {
            currentSkills.RemoveAt(Random.Range(0, currentSkills.Count));
        }
    }

    public bool CheckAlreadyHaveSkill(SkillAction skill)
    {
        foreach (var sk in currentSkills)
        {
            if (sk.id == skill.id)
                return true;
        }

        return false;
    }

    public bool CheckAlreadyHaveRelic(Relic relic)
    {
        foreach (var rl in currentRelics)
        {
            if (rl.id == relic.id)
                return true;
        }

        return false;
    }

}
