using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class SkillBuy : MonoBehaviour
{
    [SerializeField] RectTransform skillPos;
    [SerializeField] SkillShow skillShow;
    [SerializeField] CanvasGroup descriptionFade;
    [SerializeField] TMP_Text skillName;
    [SerializeField] TMP_Text skillDesc;
    [SerializeField] TMP_Text costText;
    [SerializeField] float time;

    int cost;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetupMerchandise(SkillAction skill)
    {
        skillShow.SetSkillShow(skill, false);

        skillName.text = skill.skillName;
        skillDesc.text = skill.description;

        cost = skillShow.GetPrice();
        costText.text = cost.ToString();
    }
}
