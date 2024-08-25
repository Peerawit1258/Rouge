using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class SkillDesc : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeDesc;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text cooldownText;
    [SerializeField] RectTransform descBox;
    [SerializeField] RectTransform statusPlace;
    [SerializeField] GameObject statusPrefab;
    [SerializeField] float disY;
    [SerializeField] float disY2;

    List<StatusInfo> statusList = new List<StatusInfo>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    List<StatusEffect> allStatus = new List<StatusEffect>();
    public void SetDetailBox(SkillAction skill)
    {
        nameText.text = skill.skillName;
        descText.text = skill.description;
        cooldownText.text = "Cooldown:" + skill.cooldown;

        if(statusList.Count > 0)
        {
            for(int i = statusList.Count - 1; i >= 0; i--)
                Destroy(statusList[i].gameObject);
            statusList.Clear();
            allStatus.Clear();
        }

        if(skill.GetStatusTarget().Count > 0)
        {
            foreach (AddStatus add in skill.GetStatusTarget())
            {
                AddAllStatus(add.statusEffect);
            } 
        }
            
        if (skill.GetStatusSelf().Count > 0)
        {
            foreach (AddStatus add in skill.GetStatusSelf())
            {
                AddAllStatus(add.statusEffect);
            }  
        }

        if (skill.GetHaveCondition())
        {
            if(skill.GetConditonType() == ConditionType.EffectOrder && 
                (skill.GetConditionSkillType() == SkillType.Buff || 
                (skill.GetConditionSkillType() == SkillType.Debuff && !skill.GetConditionRemoveBuff())))
            {
                AddAllStatus(skill.GetConditionStatus().statusEffect);
            }
        }
            
        if (allStatus.Count > 0)
            foreach(StatusEffect effect in allStatus)
                CreateStatusInfo(effect);

        ChangeBoxSize();
    }

    public void SetDetailBox(Relic relic)
    {
        nameText.text = relic.relicName;
        descText.text = relic.description;
        cooldownText.text = "";

        if (statusList.Count > 0)
        {
            for (int i = statusList.Count - 1; i >= 0; i--)
                Destroy(statusList[i].gameObject);
            statusList.Clear();
            allStatus.Clear();
        }

        foreach(var set in relic.relicDetails)
        {
            if(set.type == RelicEffectType.DOT)
            {
                if(set.dotID != "")
                    allStatus.Add(GameManager.instance.allData.GetStatusWithID(set.dotID).statusEffect);
            }
            else if(set.type == RelicEffectType.TurnTrigger)
            {
                if (set.mainTrigger == SkillType.Buff || set.mainTrigger == SkillType.Debuff)
                    allStatus.Add(GameManager.instance.allData.GetStatusWithID(set.status.statusEffect.id).statusEffect);
            }

        }
        

        if (allStatus.Count > 0)
            foreach (StatusEffect effect in allStatus)
                CreateStatusInfo(effect);

        ChangeBoxSize();
    }
    [Button]
    public void FadeIn(RectTransform rect = null, float y = 0)
    {
        fadeDesc.DOFade(1, 0.2f);
        if (rect != null)
        {
            descBox.position = rect.position + new Vector3(0, y, 0);
        }
    }

    public void FadeOut()
    {
        fadeDesc.DOFade(0, 0.2f);
    }

    private void CreateStatusInfo(StatusEffect status)
    {
        StatusInfo info = Instantiate(statusPrefab, statusPlace).GetComponent<StatusInfo>();
        info.SetInfo(status);

        statusList.Add(info);
    }

    private void AddAllStatus(StatusEffect statusEffect)
    {
        if(allStatus.Count == 0) allStatus.Add(statusEffect);
        else
        {
            foreach (StatusEffect effect in allStatus)
                if (effect.id == statusEffect.id)
                    return;

            allStatus.Add(statusEffect);
        }
    }
    private void ChangeBoxSize()
    {
        if(descText.preferredHeight > 70)
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 150 + (descText.preferredHeight - 76));
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, disY + ((descText.preferredHeight - 76) / 2));
        }
        else
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 150);
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, disY);
        }
    }
}
