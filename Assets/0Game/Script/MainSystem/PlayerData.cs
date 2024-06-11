using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerData : MonoBehaviour
{
    public List<SkillAction> currentSkills = new List<SkillAction>();
    public List<Relic> currentRelics = new List<Relic>();
    public int gold;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) GameManager.instance.detailPanel.ChangeGoldValue(gold + 100);
    }

    #region Skill
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

    public bool CheckAlreadyHaveSkill(string id)
    {
        foreach (var sk in currentSkills)
        {
            if (sk.id == id)
                return true;
        }

        return false;
    }
    #endregion
    #region Relic
    public bool CheckAlreadyHaveRelic(string id)
    {
        foreach (var rl in currentRelics)
        {
            if (rl.id == id)
                return true;
        }

        return false;
    }
    #endregion
    #region Gold
    public int GetCurrentGold()
    {
        if (gold > 0)
            return gold;
        else
        {
            gold = 100;
            return 100;
        }   
    }

    public void ChangeGoldValue(int value)
    {
        gold = value;
    }
    #endregion
}
