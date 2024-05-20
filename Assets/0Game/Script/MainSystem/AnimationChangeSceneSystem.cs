using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using Sirenix.OdinInspector;

public class AnimationChangeSceneSystem : MonoBehaviour
{
    [SerializeField] Image blackFade;
    [SerializeField] RectTransform moveFade;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    [Button] public void Test() => BlackFade();

    public void BlackFade(float time = 1, float delay = 0, UnityAction action = null)
    {
        blackFade.DOFade(1, time).OnComplete(() =>
        {
            blackFade.DOFade(0, time).SetDelay(delay);
            if(action != null) action.Invoke();
        });
    }

    [Button] public void Test2() => TransitionMove();
    public void TransitionMove(float time = 1, float delay = 2, UnityAction action = null)
    {
        moveFade.DOAnchorPosX(-42, time).SetEase(Ease.Linear).OnComplete(() =>
        {
            moveFade.DOAnchorPosX(-2046, time).SetEase(Ease.Linear).SetDelay(delay).OnComplete(()=> moveFade.anchoredPosition = new Vector2(1962, -23.5f));
            if (action != null) action.Invoke();
        });
    }
}
