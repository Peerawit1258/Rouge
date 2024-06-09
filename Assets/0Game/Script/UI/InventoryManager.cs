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
    [SerializeField] private RectTransform inventoryPos;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform skillPlace;
    [SerializeField] private RectTransform skillSparePos;
    [SerializeField] private RectTransform skillSparePlace;
    [SerializeField] private GameObject skillPrefab;
    [SerializeField] private List<PlaceForSkill> place;
    [SerializeField] private List<PlaceForSkill> spare;
    [SerializeField] private Button iconBtn;
    [SerializeField] private Button backBtn;
    //[SerializeField] private CanvasGroup placeCanvas;
    [SerializeField] private RectTransform placePos;
    //[SerializeField] private TMP_Text placeText;

    [TabGroup("Setting"), SerializeField] float time;
    [TabGroup("Setting"), SerializeField] Ease ease;

    [ReadOnly] public bool isOpen = false;
    [ReadOnly] public int recieveCount = 0;
    [ReadOnly] public int removeCount = 0;
    List<SkillShow> skillShows = new List<SkillShow>();

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
        if (eventData.pointerDrag != null)
        {
            PlaceForSkill place = GetPlace();
            if (place == null) return;

            SkillShow show = eventData.pointerDrag.GetComponent<SkillShow>();
            if (show.inventory || !CheckPlaceEmpty()) return;
            place.SetSkill(show);
            skillShows.Add(show);
            recieveCount--;
            resultBattle.GetSkillShows().Remove(show);

            if (resultBattle.result)
            {
                if ((recieveCount == 0 && removeCount == 0) || resultBattle.GetSkillShows().Count == 0)
                    resultBattle.CloseResultPanel();
            }
            else
            {
                if (recieveCount == 0 && removeCount == 0)
                {

                }
            }
        }
    }

    public void SkillMoveToPlace()
    {
        foreach(var show in skillShows)
        {
            if (!show.inventory)
            {
                PlaceForSkill place = GetPlace();
                place.SetSkill(show);
            }
        }
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

    public void CreateSkillShow(List<SkillAction> skill) // Triger when start
    {
        foreach(var action in skill)
        {
            PlaceForSkill place = GetPlace();
            SkillShow show = Instantiate(skillPrefab, place.pos).GetComponent<SkillShow>();
            show.SetSkillShow(action);
            skillShows.Add(show);

            
            place.SetSkill(show);
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
            if(skillShows.Count <= 20)
            {
                show.GetObjPos().DOAnchorPos(new Vector2(-810, -540), 1).SetDelay(0.3f);
                show.GetObjPos().DOSizeDelta(Vector2.zero, 1).SetEase(Ease.OutQuart).SetDelay(1).OnComplete(() =>
                {
                    show.GetObjPos().sizeDelta = new Vector2(100, 100);
                    PlaceForSkill place = GetPlace();
                    place.SetSkill(show);
                });
                
            }
        });
    }

    public void CreateSkillSpareShow(SkillAction skill)
    {
        SkillShow show = Instantiate(skillPrefab, skillSparePlace).GetComponent<SkillShow>();
        show.SetSkillShow(skill);

        //skillShows.Add(show);
    }

    public void SetAmountRecieveSkill(int num)
    {
        recieveCount = num;
        if (skillShows.Count + recieveCount > 20)
            removeCount = skillShows.Count + recieveCount - 20;
    }

    public void RemoveSkill(SkillAction remove)
    {
        foreach (var p in place)
        {
            if (p.GetSkill().GetId() == remove.id)
            {
                skillShows.Remove(p.GetSkill());
                Destroy(p.GetSkill().gameObject);
                p.ClearValue();
            }   
        }
        GameManager.instance.playerData.RemoveCurrentSkill(remove);
    }

    void ClearSpare()
    {
        foreach (var s in spare)
        {
            if (s.GetSkill() != null)
            {
                Destroy(s.GetSkill().gameObject);
                s.ClearValue();
            }   
        }
    }

    PlaceForSkill GetPlace()
    {
        foreach (var p in place)
        {
            if (p.GetSkill() == null)
                return p;
        }

        return null;
    }

    public bool CheckPlaceEmpty()
    {
        foreach (var p in place)
        {
            if (p.GetSkill() == null)
                return true;
        }

        return false;
    }

    public List<SkillShow> GetSkillShows() => skillShows;
}
