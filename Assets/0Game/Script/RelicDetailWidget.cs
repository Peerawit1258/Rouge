using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class RelicDetailWidget : MonoBehaviour
{
    [SerializeField] RelicWidget relicWidget;
    [SerializeField] TMP_Text relicName;
    [SerializeField] TMP_Text relicDescription;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform detailPos;

    public void SetupDetail(Relic relic, int delay = 0)
    {
        relicWidget.SetupRelic(relic);
        relicName.text = relic.relicName;
        relicDescription.text = relic.description;

        canvasGroup.alpha = 0;
        detailPos.DOAnchorPosY(50, 0.5f).From().SetEase(Ease.InOutQuart).SetDelay(delay * 0.3f);
        canvasGroup.DOFade(1, 0.5f).SetDelay(delay * 0.3f);
    }

    public void GetWidgettoInventory()
    {
        relicWidget.GetWidgetPos().parent = GameManager.instance.detailPanel.GetRelicPlace();
        GameManager.instance.detailPanel.GetRelicWidgets().Insert(0 , relicWidget);
        GameManager.instance.detailPanel.OrderRelic();
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }

    public RectTransform GetPos() => detailPos;
}
