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

    public void SetupDetail(Relic relic)
    {
        relicWidget.SetupRelic(relic);
        relicName.text = relic.relicName;
        relicDescription.text = relic.description;
    }

    public void GetWidgettoInventory()
    {
        relicWidget.GetWidgetPos().parent = GameManager.instance.detailPanel.GetRelicPlace();
        GameManager.instance.detailPanel.OrderRelic();
        canvasGroup.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
    }
}
