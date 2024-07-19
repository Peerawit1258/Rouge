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
    [SerializeField] GameObject statusPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetDetailBox(SkillAction skill)
    {
        nameText.text = skill.skillName;
        descText.text = skill.description;
        cooldownText.text = "Cooldown:" + skill.cooldown;

        ChangeBoxSize();
    }

    public void FadeIn()
    {
        fadeDesc.DOFade(1, 0.5f);
    }

    public void FadeOut()
    {
        fadeDesc.DOFade(0, 0.5f);
    }

    private void ChangeBoxSize()
    {
        if(descText.preferredHeight > 100)
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 190 + (descText.preferredHeight - 100));
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, 180 + ((descText.preferredHeight - 100) / 2));
        }
        else
        {
            descBox.sizeDelta = new Vector2(descBox.sizeDelta.x, 190);
            descBox.anchoredPosition = new Vector2(descBox.anchoredPosition.x, 180);
        }
    }
}
