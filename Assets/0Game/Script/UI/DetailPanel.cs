using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class DetailPanel : MonoBehaviour
{
    [TabGroup("Detail"), SerializeField] private TMP_Text turnText;
    [TabGroup("Detail"), SerializeField] private TMP_Text skillText;
    [TabGroup("Detail"), SerializeField] private TMP_Text actionTurnText;
    [TabGroup("Detail"), SerializeField] private RectTransform actionTurnPos;

    [TabGroup("Relic"), SerializeField] private RectTransform relicPlace;
    [TabGroup("Relic"), SerializeField] private TMP_Text relicCount;
    [TabGroup("Relic"), SerializeField] private GameObject relicInfoPrefab;
    [TabGroup("Relic"), SerializeField] private RectTransform infoPlace;
    [TabGroup("Relic"), SerializeField] private CanvasGroup relicInfoFade;

    [TabGroup("Info"), SerializeField] private TMP_Text atkValue;
    [TabGroup("Info"), SerializeField] private TMP_Text defValue;
    [TabGroup("Info"), SerializeField] private TMP_Text goldValue;
    [TabGroup("Info"), SerializeField] private TMP_Text encounterValue;

    List<RelicWidget> relicWidgets = new List<RelicWidget>();
    public List<RelicWidget> GetRelicWidgets() => relicWidgets;
    // Start is called before the first frame update
    void Start()
    {
        relicInfoFade.alpha = 0;
    }

    public void ActionSkillActive() => GameManager.instance.skillOrderSystem.ActionOrder();

    #region TopDetailPanel
    public void SetStartDetail(int atk, int def, int gold)
    {
        atkValue.text = atk.ToString();
        defValue.text = def.ToString();
        goldValue.text = gold.ToString();
    }
    public void ChangeAtkValue(int value, int baseValue) 
    {
        Debug.Log(value);
        DOVirtual.Int(baseValue, value, 0.5f, (x) => {
            if (x >= 1)
                atkValue.text = x.ToString();
        });
    }
    public void ChangeDefValue(int value, int baseValue) 
    {
        DOVirtual.Int(baseValue, value, 0.5f, (x) => {
            if (x >= 0)
                defValue.text = x.ToString();
        });
    }
    public void ChangeGoldValue(int value)
    {
        int currentGold = GameManager.instance.playerData.gold;
        DOVirtual.Int(currentGold, value, 0.5f, (x) => goldValue.text = x.ToString());
        GameManager.instance.playerData.gold = value;
    }
    public void SetEncounter(int count) => encounterValue.text = count.ToString();
    #endregion
    Tween tween;
    public void ShowSkillActionName(string name)
    {
        //skillText.DO
        skillText.text = name;
        if(tween != null) tween.Kill();
        tween = skillText.DOFade(1, 0.2f).OnComplete(() => tween = skillText.DOFade(0, 0.2f).SetDelay(0.5f));
    }

    public void SetTurn(int turn) => turnText.text = "Turn" + turn.ToString();

    public void ShowActionTurn(ActionTurn turn, UnityAction action)
    {
        actionTurnText.text = turn.ToString() + " Turn";
        if(turn == ActionTurn.player)
        {
            actionTurnPos.anchoredPosition = new Vector2(-950, 0);
            actionTurnPos.DOAnchorPosX(0, 0.5f).SetEase(Ease.InOutQuart);
            actionTurnText.DOFade(1, 0.5f).OnComplete(() =>
            {
                actionTurnPos.DOAnchorPosX(150, 1f).SetEase(Ease.Linear).SetDelay(1);
                actionTurnText.DOFade(0, 1).SetDelay(1).OnComplete(() =>
                {
                    if(action != null) action.Invoke();
                });
            });
        }
        else
        {
            actionTurnPos.anchoredPosition = new Vector2(950, 0);
            actionTurnPos.DOAnchorPosX(0, 0.5f).SetEase(Ease.InOutQuart);
            actionTurnText.DOFade(1, 0.5f).OnComplete(() =>
            {
                actionTurnPos.DOAnchorPosX(-150, 1).SetEase(Ease.Linear).SetDelay(1);
                actionTurnText.DOFade(0, 1).SetDelay(1).OnComplete(() =>
                {
                    if (action != null) action.Invoke();
                });
            });
        }
    }

    public void OrderRelic()
    {
        if(relicWidgets.Count == 0) return;
        for(int i = 0;i < relicWidgets.Count;i++)
        {
            if (relicWidgets[i].GetWidgetPos().sizeDelta != Vector2.one)
                relicWidgets[i].GetWidgetPos().DOScale(1, 0.5f);

            relicWidgets[i].GetWidgetPos().DOAnchorPos(new Vector2(i * 65, 0), 0.5f);
            if (i > 8) relicWidgets[i].GetCanvasGroup().DOFade(0, 0.5f);
        }
        relicCount.text = relicWidgets.Count.ToString();
    }

    bool infoOpen = false;
    public void OpenRelicInfo()
    {
        if (infoOpen)
        {
            relicInfoFade.alpha = 0;
            infoOpen = false;
        }
        else
        {
            relicInfoFade.alpha = 1;
            infoOpen = true;
        }
    }

    [ReadOnly] public List<GameObject> infoGroup = new List<GameObject>();
    public RelicInfo CreateInfo(Relic relic)
    {
        RelicInfo info = Instantiate(relicInfoPrefab, infoPlace).GetComponent<RelicInfo>();
        info.AssignInfo(relic);
        infoGroup.Add(info.gameObject);

        return info;
    }

    public void RemoveRelicInfo(RelicWidget widget)
    {
        infoGroup.Remove(widget.relicInfo.gameObject);
        Destroy(widget.relicInfo.gameObject);
        widget.relicInfo = null;

    }

    public RectTransform GetRelicPlace() => relicPlace;
}
