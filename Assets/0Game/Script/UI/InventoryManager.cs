using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryManager : MonoBehaviour, IDropHandler
{
    [SerializeField, TabGroup("UI")] private RectTransform inventoryPos;
    [SerializeField, TabGroup("UI")] private CanvasGroup canvasGroup;
    [SerializeField, TabGroup("UI")] private RectTransform skillPlace;
    [SerializeField, TabGroup("UI")] private Button iconBtn;
    [SerializeField, TabGroup("UI")] public RectTransform iconPos;
    [SerializeField, TabGroup("UI")] private Button backBtn;
    [SerializeField, TabGroup("UI")] private RectTransform placePos;

    [SerializeField] private GameObject skillPrefab;
    //[SerializeField] private TMP_Text placeText;

    [TabGroup("Setting"), SerializeField] float time;
    [TabGroup("Setting"), SerializeField] Ease ease;

    [ReadOnly] public bool isOpen = false;
    [ReadOnly] public int recieveCount = 0;
    [ReadOnly] public int removeCount = 0;
    [ReadOnly, ShowInInspector, TabGroup("2","Store")] List<SkillShow> skillShows = new List<SkillShow>();
    [ReadOnly, ShowInInspector, TabGroup("2", "Store")] List<SkillShow> activeSkill = new List<SkillShow>();
    [ReadOnly, ShowInInspector, TabGroup("2", "Store")] List<SkillShow> usableSkill = new List<SkillShow>();
    [ReadOnly, ShowInInspector, TabGroup("2", "Store")] List<SkillShow> cooldownSkill = new List<SkillShow>();

    ResultBattle resultBattle;
    // Start is called before the first frame update
    void Start()
    {
        resultBattle = GameManager.instance.resultBattle;
        iconBtn.interactable = true;
        backBtn.interactable = false;

        //placeCanvas.alpha = 0;
        //placeCanvas.blocksRaycasts = false;
        //placeCanvas.interactable = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag != null)
        //{
        //    PlaceForSkill place = GetPlace();
        //    if (place == null) return;

        //    SkillShow show = eventData.pointerDrag.GetComponent<SkillShow>();
        //    if (show.inventory || !CheckPlaceEmpty()) return;
        //    //place.SetSkill(show);
        //    show.GetObjPos().parent = skillPlace.transform;
        //    skillShows.Add(show);
        //    activeSkill.Add(show);
        //    recieveCount--;
        //    resultBattle.GetSkillShows().Remove(show);
        //    GameManager.instance.playerData.AddNewSkill(show.GetSkillAction());
        //    if (resultBattle.result)
        //    {
        //        if ((recieveCount == 0 && removeCount == 0) || resultBattle.GetSkillShows().Count == 0)
        //            resultBattle.CloseResultPanel();
        //    }
        //    else
        //    {
        //        if (recieveCount == 0 && removeCount == 0)
        //        {

        //        }
        //    }
        //}
    }

    public void SkillMoveToPlace(SkillShow show)
    {
        //PlaceForSkill place = GetPlace();
        //place.SetSkill(show);
        show.GetObjPos().parent = skillPlace.transform;
        GameManager.instance.playerData.AddNewSkill(show.GetSkillAction());
        activeSkill.Add(show);
    }

    bool isActive = false;
    public void OpenInventory()
    {
        if (isActive || GameManager.instance.shopSystem.GetCheckOpen()) return;
        canvasGroup.interactable = false;
        isActive = true;
        iconBtn.interactable = !iconBtn.interactable;
        backBtn.interactable = !backBtn.interactable;
        if (isOpen)
        {
            isOpen = false;
            inventoryPos.DOAnchorPosX(-710, time).SetEase(ease).OnComplete(()=>isActive = false);
        }
        else
        {
            isOpen = true;
            inventoryPos.DOAnchorPosX(710, time).SetEase(ease).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                isActive = false;
            });
        }
    }

    public void CreateSkillShow(List<SkillAction> skill, int cooldown = 0) // Triger when start
    {
        foreach(var action in skill)
        {
            //PlaceForSkill place = GetPlace();
            SkillShow show = Instantiate(skillPrefab, skillPlace).GetComponent<SkillShow>();
            show.SetSkillShow(action);
            skillShows.Add(show);
            if (cooldown > 0)
                cooldownSkill.Add(show);
            else
                activeSkill.Add(show);

            show.GetObjPos().parent = skillPlace.transform;
            GameManager.instance.playerData.AddNewSkill(show.GetSkillAction());
            //place.SetSkill(show);
        }
        
    }

    [Button]
    public void CreateSkillShow(SkillAction skill) // Add new skill
    {
        SkillShow show = Instantiate(skillPrefab, placePos).GetComponent<SkillShow>();
        show.SetSkillShow(skill, false);
        show.GetObjPos().sizeDelta = Vector2.zero;
        show.GetObjPos().DOSizeDelta(new Vector2(100, 100), time /2);
        show.GetObjPos().DOJumpAnchorPos(show.GetObjPos().anchoredPosition,30,1, time).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            skillShows.Add(show);
            activeSkill.Add(show);
            if (skillShows.Count <= 20)
            {
                show.GetObjPos().DOAnchorPos(new Vector2(-810, -540), 1).SetDelay(0.3f);
                show.GetObjPos().DOSizeDelta(Vector2.zero, 1).SetEase(Ease.OutQuart).SetDelay(1).OnComplete(() =>
                {
                    show.GetObjPos().sizeDelta = new Vector2(100, 100);
                    show.GetObjPos().parent = skillPlace;
                    //PlaceForSkill place = GetPlace();
                    //place.SetSkill(show);
                });
                
            }
        });
    }

    public void SetAmountRecieveSkill(int num)
    {
        recieveCount = num;
        if (skillShows.Count + recieveCount > 20)
            removeCount = skillShows.Count + recieveCount - 20;
    }

    public void RemoveSkill(SkillAction remove)
    {
        //foreach (var p in place)
        //{
        //    if (p.GetSkill().GetId() == remove.id)
        //    {
        //        skillShows.Remove(p.GetSkill());
        //        if (activeSkill.Contains(p.GetSkill())) activeSkill.Remove(p.GetSkill());
        //        else if (cooldownSkill.Contains(p.GetSkill())) cooldownSkill.Remove(p.GetSkill());
        //        Destroy(p.GetSkill().gameObject);
        //        p.ClearValue();
        //    }   
        //}
        //GameManager.instance.playerData.RemoveCurrentSkill(remove);
    }

    public void DecreaseCooldownSkill()
    {
        if (cooldownSkill.Count == 0) return;
        //foreach (var skill in cooldownSkill)
        //    skill.DecreaseCooldown(1);
        for (int i = cooldownSkill.Count - 1; i >= 0; i--)
            cooldownSkill[i].DecreaseCooldown(1);
    }

    public void ClearCooldownSkill()
    {
        for (int i = cooldownSkill.Count - 1; i >= 0; i--)
            cooldownSkill[i].ReadyToActionSkill();
    }

    public SkillShow GetActiveSkillShowWithSkill(SkillAction skill)
    {
        SkillShow show = new SkillShow();
        foreach(var s in activeSkill)
            if(s.GetSkillAction().id == skill.id)
                show = s;

        return show;
    }

    public List<SkillShow> GetSkillShows() => skillShows;
    public List<SkillShow> GetSkillActive() => activeSkill;
    public List<SkillShow> GetSkillUsable() => usableSkill;
    public List<SkillShow> GetSkillCoolDown() => cooldownSkill;
}
