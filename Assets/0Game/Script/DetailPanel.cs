using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Events;

public class DetailPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text skillText;
    [SerializeField] private TMP_Text actionTurnText;
    [SerializeField] private RectTransform actionTurnPos;
    [SerializeField] private RectTransform relicPlace;
    [SerializeField] private TMP_Text relicCount;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

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

    }

    public RectTransform GetRelicPlace() => relicPlace;
}
