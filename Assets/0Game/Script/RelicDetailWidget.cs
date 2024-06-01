using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class RelicDetailWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] RelicWidget relicWidget;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDescription;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform detailPos;

    bool select = false;
    RelicManagerSystem relicManagerSystem;

    private void Start()
    {
        relicManagerSystem = GameManager.instance.relicManagerSystem;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!select) return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!select) return;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!select) return;

        GameManager.instance.resultBattle.CloseReward(this);
    }

    public void SetupDetail(Relic relic, int delay = 0, bool select = false)
    {
        relicWidget.SetupRelic(relic);
        relicName.text = relic.relicName;
        relicDescription.text = relic.description;

        canvasGroup.alpha = 0;
        detailPos.DOAnchorPosY(50, 0.5f).From().SetEase(Ease.InOutQuart).SetDelay(delay * 0.3f);
        canvasGroup.DOFade(1, 0.5f).SetDelay(delay * 0.3f);

        this.select = select;
        if (select)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void GetWidgettoInventory()
    {
        if(relicManagerSystem == null) relicManagerSystem = GameManager.instance.relicManagerSystem;
        relicWidget.GetWidgetPos().parent = GameManager.instance.detailPanel.GetRelicPlace();
        GameManager.instance.detailPanel.GetRelicWidgets().Insert(0 , relicWidget);
        GameManager.instance.detailPanel.OrderRelic();
        relicManagerSystem.AddRelic(relicWidget.GetRelic());
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }

    public void CloseDetailWidget()
    {
        detailPos.DOAnchorPosY(-50, 0.5f).SetEase(Ease.InOutQuart);
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }

    public RectTransform GetPos() => detailPos;
    public RelicWidget GetWidget() => relicWidget;
}
