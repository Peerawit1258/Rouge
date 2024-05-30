using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using TMPro;
using DG.Tweening;

public class EventManager : MonoBehaviour
{
    [TabGroup("Panel"), SerializeField] CanvasGroup panel;
    [TabGroup("Event"), SerializeField] TMP_Text title;
    [TabGroup("Event"), SerializeField] Image eventImg;
    [TabGroup("Event"), SerializeField] TMP_Text eventDetail;
    [TabGroup("Event"), SerializeField] RectTransform choicePlace;
    [TabGroup("Setup"), SerializeField] float time;

    [SerializeField] GameObject choicePrefab;
    [SerializeField] ChoiceWidget exitChoice;
    List<ChoiceWidget> choiceLists = new List<ChoiceWidget>();

    public UnityAction afterAction;
    // Start is called before the first frame update
    void Start()
    {
        panel.alpha = 0;
        panel.blocksRaycasts = false;
        panel.interactable = false;
        panel.gameObject.SetActive(false);
    }
    [Button]
    public void EventSetup(EventInfo info)
    {
        if (info == null) return;
        if (exitChoice != null) exitChoice.gameObject.SetActive(false);

        title.text = info.eventName;
        eventImg.sprite = info.eventImage;
        eventDetail.text = info.description;

        ClearChoice();

        for(int i = 0; i < info.choices.Count; i++)
        {
            ChoiceWidget choiceWidget = Instantiate(choicePrefab, choicePlace).GetComponent<ChoiceWidget>();
            choiceWidget.ChoiceSetup(info.choices[i]);
            choiceWidget.gameObject.name += "_" + i.ToString();
            choiceLists.Add(choiceWidget);
        }

        FadeIn();
    }

    public void NextEvent(EventInfo info)
    {
        if (info == null) return;
        if (exitChoice != null) exitChoice.gameObject.SetActive(false);

        eventImg.sprite = info.eventImage;
        eventDetail.text = info.description;

        ClearChoice();

        for (int i = 0; i < info.choices.Count; i++)
        {
            ChoiceWidget choiceWidget = Instantiate(choicePrefab, choicePlace).GetComponent<ChoiceWidget>();
            choiceWidget.ChoiceSetup(info.choices[i]);
            choiceWidget.gameObject.name += "_" + i.ToString();
            choiceLists.Add(choiceWidget);
        }

    }
    #region After
    public void AfterSelectChoice(ChoiceDetail choice)
    {
        if (choice.next)
        {
            NextEvent(choice.nextEvent);
        }
        else
        {
            eventImg.sprite = choice.afterEventImg;
            eventDetail.text = choice.afterEventDes;

            eventImg.DOColor(Color.black, time).From();

            ClearChoice();
            if (exitChoice != null) exitChoice.gameObject.SetActive(true);
        }
        
    }

    public void AfterSelectChoice(ChoiceDetail choice, string replace) // Reward Gold
    {
        eventImg.sprite = choice.afterEventImg;
        eventDetail.text = choice.afterEventDes.Replace("REPLACE", replace);

        eventImg.DOColor(Color.black, time).From();

        ClearChoice();
        if (exitChoice != null) exitChoice.gameObject.SetActive(true);
    }

    #endregion
    [Button]
    public void ExitEvent() => FadeOut();
    private void FadeIn()
    {
        panel.gameObject.SetActive(true);
        GameManager.instance.battleSetup.eventSymbol.SetActive(false);
        panel.DOFade(1, time).OnComplete(() =>
        {
            panel.blocksRaycasts = true;
            panel.interactable = true;
            if(afterAction != null)
            {
                afterAction.Invoke();
                afterAction = null;
                if (afterAction == null) Debug.Log("Event Complete");
            }
        });
    }

    private void FadeOut()
    {
        
        panel.DOFade(0, time).OnComplete(() =>
        {
            panel.blocksRaycasts = false;
            panel.interactable = false;
            panel.gameObject.SetActive(false);
        });
    }

    private void ClearChoice()
    {
        if (choiceLists.Count > 0)
        {
            foreach (var choice in choiceLists)
                Destroy(choice.gameObject);

            choiceLists.Clear();
        }
    }
} 
