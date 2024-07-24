using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class RelicBuy : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] RectTransform relicPos;
    [SerializeField] RelicWidget relicWidget;
    [SerializeField] CanvasGroup relicCanvas;
    [SerializeField] CanvasGroup descriptionFade;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDesc;
    [SerializeField] TMP_Text costText;
    [SerializeField] float time;

    int cost;
    bool canBuy;
    RelicManagerSystem relicManagerSystem;
    SkillDesc skillDesc;
    // Start is called before the first frame update
    void Start()
    {
        relicManagerSystem = GameManager.instance.relicManagerSystem;
        skillDesc = GameManager.instance.skillDesc;
        descriptionFade.alpha = 0;
    }

    public void SetupMerchandise(Relic relic)
    {
        relicWidget.SetupRelic(relic);

        relicName.text = relic.relicName;
        relicDesc.text = relic.description;

        cost = relicWidget.GetPrice();
        costText.text = cost.ToString();
        if (GameManager.instance.playerData.gold < cost)
        {
            costText.color = Color.red;
            canBuy = false;
        }
        else
        {
            canBuy = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //descriptionFade.DOFade(1, time);
        skillDesc.SetDetailBox(relicWidget.GetRelic());
        skillDesc.FadeIn();
        relicPos.DOScale(1.2f, time);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillDesc.FadeOut();
        relicPos.DOScale(1, time);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (canBuy)
        {
            skillDesc.FadeOut();
            GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold - cost);
            GetWidgettoInventory();
        }
        else
        {

        }
    }

    public void GetWidgettoInventory()
    {
        if (relicManagerSystem == null) relicManagerSystem = GameManager.instance.relicManagerSystem;

        relicWidget.GetWidgetPos().parent = GameManager.instance.detailPanel.GetRelicPlace();
        relicCanvas.alpha = 0;
        relicCanvas.blocksRaycasts = false;
        relicCanvas.interactable = false;

        GameManager.instance.detailPanel.GetRelicWidgets().Insert(0, relicWidget);
        GameManager.instance.detailPanel.OrderRelic();

        GameManager.instance.shopSystem.GetRelicBuys().Remove(this);
        GameManager.instance.shopSystem.OrderRelicBuy();
        GameManager.instance.shopSystem.CheckAllPrice();

        relicManagerSystem.AddRelic(relicWidget.GetRelic());

        descriptionFade.DOFade(0, time).OnComplete(() => Destroy(gameObject));
        //canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }

    public void CheckCurrentGold()
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

    public RectTransform GetRelicBuyPos() => relicPos;
}
