using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class SkillBuy : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform skillPos;
    [SerializeField] SkillShow skillShow;
    [SerializeField] CanvasGroup skillCanvas;
    [SerializeField] CanvasGroup descriptionFade;
    [SerializeField] TMP_Text skillName;
    [SerializeField] TMP_Text skillDesc;
    [SerializeField] TMP_Text costText;
    [SerializeField] TMP_Text discountText;
    [SerializeField] float time;
    [SerializeField] Color discountColor;

    int cost;
    bool canBuy;
    SkillDesc skillDetail;
    // Start is called before the first frame update
    void Start()
    {
        skillDetail = GameManager.instance.skillDesc;
    }

    public void SetupMerchandise(SkillAction skill)
    {
        skillShow.SetSkillShow(skill, false);

        skillName.text = skill.skillName;
        skillDesc.text = skill.description;

        cost = skillShow.GetPrice();
        if (GameManager.instance.relicManagerSystem.discount > 0)
        {
            costText.text = skillShow.GetPrice().ToString();
            costText.color = discountColor;

            cost *= ((100 - GameManager.instance.relicManagerSystem.discount) / 100);
            discountText.text = cost.ToString();
        }
        else
        {
            costText.text = cost.ToString();
            costText.color = Color.white;

            discountText.text = "";
        }
        
        CheckCurrentGold();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //descriptionFade.DOFade(1, time);
        skillDetail.SetDetailBox(skillShow.GetSkillAction());
        skillDetail.FadeIn(skillPos, 150);

        skillPos.DOScale(1.2f, time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillDetail.FadeOut();
        //descriptionFade.DOFade(0, time);
        skillPos.DOScale(1, time);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBuy)
        {
            GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - cost);

            skillDetail.FadeOut();
            GameManager.instance.inventoryManager.GetSkillShows().Add(skillShow);
            skillShow.GetObjPos().parent = GameManager.instance.inventoryManager.iconPos;

            skillCanvas.alpha = 0;
            skillCanvas.blocksRaycasts = false;
            skillCanvas.interactable = false;

            GameManager.instance.shopSystem.GetSkillBuys().Remove(this);
            GameManager.instance.shopSystem.OrderSkillBuy();
            GameManager.instance.shopSystem.CheckAllPrice();

            if (GameManager.instance.inventoryManager.GetSkillShows().Count <= 20)
            {
                skillShow.skillCanvas.DOFade(0, 1);
                skillShow.GetObjPos().DOAnchorPos(Vector2.zero, 1).SetEase(Ease.OutQuart);
                skillShow.GetObjPos().DOScale(1, 1);
                skillShow.GetObjPos().DOSizeDelta(Vector2.zero, 1).SetEase(Ease.OutQuart).OnComplete(() =>
                {
                    skillShow.skillCanvas.alpha = 1;
                    skillShow.GetObjPos().sizeDelta = new Vector2(100, 100);
                    GameManager.instance.inventoryManager.SkillMoveToPlace(skillShow);
                    Destroy(gameObject, 1);
                });
            }
            else
            {

            }
        }
    }

    public void CheckCurrentGold()
    {
        if (GameManager.instance.relicManagerSystem.discount > 0)
        {
            if (GameManager.instance.playerData.gold < cost)
            {
                discountText.color = Color.red;
                canBuy = false;
            }
            else
            {
                discountText.color = Color.white;
                canBuy = true;
            }
        }
        else
        {
            if (GameManager.instance.playerData.gold < cost)
            {
                costText.color = Color.red;
                canBuy = false;
            }
            else
            {
                costText.color = Color.white;
                canBuy = true;
            }
        }
        
    }

    public RectTransform GetSkillBuyPos() => skillPos;
    public SkillShow GetSkillShow() => skillShow;
}
