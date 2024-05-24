using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;

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
            
            
            //show.gameObject.transform.parent = skillPlace;
        }
    }

    [Button]
    public void OpenInventory()
    {
        canvasGroup.interactable = false;
        if (isOpen)
        {
            isOpen = false;
            inventoryPos.DOAnchorPosX(-710, time).SetEase(ease);
        }
        else
        {
            isOpen = true;
            inventoryPos.DOAnchorPosX(710, time).SetEase(ease).OnComplete(() => canvasGroup.interactable = true);
        }
    }

    public void CreateSkillShow(SkillAction skill)
    {
        SkillShow show = Instantiate(skillPrefab, skillPlace).GetComponent<SkillShow>();
        show.SetSkillShow(skill);

        skillShows.Add(show);
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

    public void RemoveSkill(SkillShow skill)
    {
        if (skill == null) return;
        foreach (var p in place)
        {
            if (p.GetSkill().GetId() == skill.GetId())
                p.ClearValue();
            else
                return;
        }

        skillShows.Remove(skill);
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
