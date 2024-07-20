using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillDesc : MonoBehaviour
{
    [SerializeField] CanvasGroup fadeDesc;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text descText;
    [SerializeField] TMP_Text cooldownText;
    [SerializeField] RectTransform descBox;
    [SerializeField] RectTransform statusPlace;
    [SerializeField] GameObject statusPrefab;
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

    public void FadeIn()
    {
        fadeDesc.DOFade(1, 0.2f);
    }

    public void FadeOut()
    {
        fadeDesc.DOFade(0, 0.2f);
    }

    private void CreateStatusInfo(StatusEffect status)
    {
        StatusInfo info = Instantiate(statusPrefab, statusPlace).GetComponent<StatusInfo>();
        info.SetInfo(status);
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
        if(descText.preferredHeight > 100)
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 190 + (descText.preferredHeight - 100));
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, 250 + ((descText.preferredHeight - 100) / 2));
        }
        else
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 190);
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, 250);
        }
    }
}
