using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;

public class ResultBattle : MonoBehaviour
{
    [SerializeField] GameObject resultObj;

    [TabGroup("Result"), SerializeField] TMP_Text titleText;
    [TabGroup("Result"), SerializeField] RectTransform titlePos;

    [TabGroup("Win"), SerializeField] CanvasGroup winPanel;
    [TabGroup("Win"), SerializeField] RectTransform skillDropPos;
    [TabGroup("Win"), SerializeField] TMP_Text skillDropText;
    [TabGroup("Win"), SerializeField] RectTransform skillPlace;
    [TabGroup("Win"), SerializeField] CanvasGroup skillPlaceCanvas;
    [TabGroup("Win"), SerializeField] RectTransform relicPlace;
    [TabGroup("Win"), SerializeField] RectTransform goldPos;
    [TabGroup("Win"), SerializeField] TMP_Text goldText;
    [TabGroup("Win"), SerializeField] TMP_Text goldValue;
    [TabGroup("Win"), SerializeField] GameObject nextButton;

    [TabGroup("Reward"), SerializeField] GameObject rewardObj;
    [TabGroup("Reward"), SerializeField] RectTransform rewardPlace;
    //[TabGroup("Reward"), SerializeField] List<CanvasGroup> rewardCanvas;
    //[TabGroup("Reward"), SerializeField] List<RectTransform> rewardPos;
    [TabGroup("Reward"), SerializeField] TMP_Text selectText;

    [TabGroup("Setup"), SerializeField] float time;

    [SerializeField] GameObject skillShowPrefab;
    [SerializeField] GameObject relicDetailPrefab;

    List<SkillShow> skillShows = new List<SkillShow>();
    List<RelicDetailWidget> relicDetails = new List<RelicDetailWidget>();

    EncounterManagementSystem encounterManagementSystem;
    // Start is called before the first frame update
    void Start()
    {
        encounterManagementSystem = GameManager.instance.encounterManagementSystem;
        //resultObj.SetActive(false);
        //resultText.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(false);
        rewardObj.SetActive(false);
        nextButton.SetActive(false);
    }

    #region Result
    [Button]
    public void StartWinResult() => StartCoroutine(WinResult());
    [ReadOnly] public bool result = false;
    IEnumerator WinResult()
    {
        result = true;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(false);
        GameManager.instance.skillOrderSystem.slotPlace.gameObject.SetActive(false);
        GameManager.instance.skillOrderSystem.skillPlace.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);
        ClearResult();
        winPanel.gameObject.SetActive(true);
        titleText.alpha = 0;
        skillDropText.alpha = 0;
        goldText.alpha = 0;
        //goldValue.text = "";
        yield return new WaitForSeconds(0.1f);
        //winPanel.gameObject.SetActive(true);

        //resultText.gameObject.SetActive(true);
        titleText.DOFade(1, time);
        titlePos.DOAnchorPosX(400, time).From().SetEase(Ease.InOutQuart);

        goldPos.DOAnchorPosX(-255, time).From().SetEase(Ease.InOutQuart).SetDelay(time / 4);
        goldText.DOFade(1, time).SetDelay(time / 4).OnComplete(() =>
        {
            DOVirtual.Int(0, encounterManagementSystem.gold, time, (x) => goldText.text = "Gold : +" + x.ToString());
            GameManager.instance.detailPanel.ChangeGoldValue(GameManager.instance.playerData.gold + encounterManagementSystem.gold);
        });

        if (encounterManagementSystem.skillDrops.Count > 0)
        {
            skillDropPos.DOAnchorPosX(-280, time).From().SetEase(Ease.InOutQuart).SetDelay(time / 2);
            skillDropText.DOFade(1, time).SetDelay(time / 2).OnComplete(() =>
            {
                //Debug.Log(encounterManagementSystem.skillDrops.Count);
                skillPlaceCanvas.alpha = 1;
                if (encounterManagementSystem.relicDrops.Count > 0)
                    CreateRelicDetail(encounterManagementSystem.relicDrops);
                for (int i = 0; i < encounterManagementSystem.skillDrops.Count; i++)
                {
                    SkillShow skill = Instantiate(skillShowPrefab, skillPlace).GetComponent<SkillShow>();
                    skill.GetObjPos().anchoredPosition = new Vector2(i * 130 + 50, 50);
                    skill.SetSkillShowAnimation(encounterManagementSystem.skillDrops[i], i);

                    skillShows.Add(skill);
                }
                GameManager.instance.inventoryManager.SetAmountRecieveSkill(1);
                //if (!GameManager.instance.inventoryManager.isOpen)
                //{
                //    GameManager.instance.inventoryManager.OpenInventory();
                    
                //}
                nextButton.gameObject.SetActive(true);
                //if (!GameManager.instance.inventoryManager.CheckPlaceEmpty())
                //    nextButton.gameObject.SetActive(true);
                //else nextButton.gameObject.SetActive(false);

            });
        }
        else
        {
            nextButton.gameObject.SetActive(true);
        }



    }

    public void CloseResultPanel()
    {
        StopCoroutine(WinResult());
        if(GameManager.instance.inventoryManager.isOpen)
            GameManager.instance.inventoryManager.OpenInventory();
        GameManager.instance.inventoryManager.removeCount = 0;
        GameManager.instance.inventoryManager.recieveCount = 0;

        if (relicDetails.Count > 0)
        {
            foreach (var detail in relicDetails)
                detail.GetWidgettoInventory();
        }
        relicDetails.Clear();

        // winPanel.gameObject.SetActive(false);
        result = false;
        titleText.DOFade(0, time);
        titlePos.DOAnchorPosX(400, time).SetEase(Ease.InOutQuart).OnComplete(()=> titlePos.anchoredPosition = new Vector2(545, titlePos.anchoredPosition.y));

        goldPos.DOAnchorPosX(-255, time).SetEase(Ease.InOutQuart).SetDelay(time / 4).OnComplete(() => goldPos.anchoredPosition = new Vector2(-205, goldPos.anchoredPosition.y)); ;
        goldText.DOFade(0, time).SetDelay(time / 4);

        skillDropPos.DOAnchorPosX(-280, time).SetEase(Ease.InOutQuart).SetDelay(time / 2).OnComplete(() => skillDropPos.anchoredPosition = new Vector2(-230, skillDropPos.anchoredPosition.y)); ;
        skillDropText.DOFade(0, time).SetDelay(time / 2);
        skillPlace.DOAnchorPosX(190, time).SetDelay(3*time / 4).OnComplete(()=> skillPlace.anchoredPosition = new Vector2(240, skillPlace.anchoredPosition.y));
        skillPlaceCanvas.DOFade(0, time).SetDelay(3*time / 4).OnComplete(() =>
        {
            
            winPanel.gameObject.SetActive(false);
            encounterManagementSystem.CreateNextDoorNode();
            GameManager.instance.relicManagerSystem.TriggerRelicEffect(TriggerStatus.End);
            GameManager.instance.turnManager.turnCount = 0;
            nextButton.gameObject.SetActive(false);
        });
    }

    #endregion
    #region Reward
    [Button]
    public void StartRewardPanel() => StartCoroutine(RewardPanel());
    IEnumerator RewardPanel()
    {
        rewardObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        selectText.GetComponent<RectTransform>().DOAnchorPosX(50, time).From().SetEase(Ease.InOutQuart);
        selectText.DOFade(1, time);
        yield return new WaitForSeconds(0.5f);
        CreateSelectRelicDetail();
        //for(int i = 0; i < 3; i++)
        //{
        //    rewardCanvas[i].DOFade(1, time).SetDelay(i * 0.3f);
        //    rewardPos[i].DOAnchorPosY(50, time).SetDelay(i * 0.3f).SetEase(Ease.InOutQuart);
        //}
    }

    public void CloseReward(RelicDetailWidget widget = null)
    {
        if (widget != null)
        {
            foreach (RelicDetailWidget detail in relicDetails)
                if (detail.GetWidget().GetRelic().id == widget.GetWidget().GetRelic().id)
                    detail.GetWidgettoInventory();
                else
                    detail.CloseDetailWidget();
        }
        else
        {
            foreach (RelicDetailWidget detail in relicDetails)
                detail.CloseDetailWidget();

        }
        relicDetails.Clear();

        selectText.GetComponent<RectTransform>().DOAnchorPosX(-50, time).SetEase(Ease.InOutQuart);
        selectText.DOFade(0, time).OnComplete(() =>
        {
            selectText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            GameManager.instance.encounterManagementSystem.CreateNextDoorNode();
            rewardObj.SetActive(false);
        });
    }
    #endregion

    void CreateRelicDetail(List<Relic> relics)
    {
        for(int i = 0;i < relics.Count; i++)
        {
            RelicDetailWidget detail = Instantiate(relicDetailPrefab, relicPlace).GetComponent<RelicDetailWidget>();
            detail.GetPos().anchoredPosition = new Vector2(i * 400, 0);
            detail.SetupDetail(relics[i], i);

            relicDetails.Add(detail);
        }
    }

    void CreateSelectRelicDetail()
    {
        int num = GameManager.instance.allData.relicReward.Count;
        for (int i = 0; i < 3; i++)
        {
            Relic relic = new Relic();
            do
            {
                relic = GameManager.instance.allData.relicReward[Random.Range(0, num)];
            }while(!CheckRandomRelic(relic));
            RelicDetailWidget detail = Instantiate(relicDetailPrefab, rewardPlace).GetComponent<RelicDetailWidget>();
            detail.GetPos().anchoredPosition = new Vector2(-400 + (i * 400), 0);
            detail.SetupDetail(relic, i, true);

            relicDetails.Add(detail);
        }
    }
    public void StartCreateSpecificRelicDetail(Relic relic) => StartCoroutine(CreateSpecificRelicDetail(relic));
    IEnumerator CreateSpecificRelicDetail(Relic relic)
    {
        rewardObj.SetActive(true);
        RelicDetailWidget detail = Instantiate(relicDetailPrefab, rewardPlace).GetComponent<RelicDetailWidget>();
        detail.GetPos().anchoredPosition = new Vector2(0, 0);
        detail.SetupDetail(relic);

        relicDetails.Add(detail);

        yield return new WaitForSeconds(1.5f);
        CloseReward(detail);
    }

    private bool CheckRandomRelic(Relic relic)
    {
        if (GameManager.instance.playerData.CheckAlreadyHaveRelic(relic.id) || relic == null) return false;
        foreach (var detail in relicDetails)
            if (detail.GetWidget().GetRelic().id == relic.id)
                return false;

        return true;
    }


    private void ClearResult()
    {
        if(skillShows.Count > 0)
        {
            for (int i = 0; i < skillShows.Count; i++)
                Destroy(skillShows[i].gameObject);
            skillShows.Clear();
        }
        
    }

    public List<SkillShow> GetSkillShows() => skillShows;

    public RectTransform GetSkillPlace() => skillPlace;

}
