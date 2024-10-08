using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ShopSystem : MonoBehaviour
{
    [TabGroup("1", "Price")] public List<int> skillPrice = new List<int>();
    [TabGroup("1", "Price")] public List<int> relicPrice = new List<int>();

    [TabGroup("1", "Place"), SerializeField] RectTransform skillPlace;
    [TabGroup("1", "Place"), SerializeField] RectTransform relicPlace;
    [TabGroup("1", "Place"), SerializeField] RectTransform shopPlace;
    [TabGroup("1", "Place"), SerializeField] CanvasGroup shopCanvas;

    [TabGroup("1", "Setup"), SerializeField] float time;
    [TabGroup("1", "Setup"), SerializeField] Ease ease;

    [SerializeField] GameObject skillPrefab;
    [SerializeField] GameObject relicPrefab;

    List<SkillBuy> skillBuys = new List<SkillBuy>();
    List<RelicBuy> relicBuys = new List<RelicBuy>();
    // Start is called before the first frame update
    void Start()
    {
        shopPlace.anchoredPosition = new Vector2(0, 1000);
        shopCanvas.alpha = 0;
    }

    bool isOpen = false;
    [Button]
    public void OpenShop()
    {
        if (isOpen)
        {
            shopPlace.DOAnchorPosY(1000, time).SetEase(ease);
            shopCanvas.DOFade(0, time);

            isOpen = false;
        }
        else
        {
            if(skillBuys.Count > 0 || relicBuys.Count > 0) ClearShop(); // Remove-------------------
            CreateSkillForBuy();
            CreateRelicForBuy();

            shopPlace.DOAnchorPosY(0, time).SetEase(ease);
            shopCanvas.DOFade(1, time);
            
            isOpen =true;
        }
    }

    public void StartCloseShop()=> StartCoroutine(CloseShop());
    IEnumerator CloseShop()
    {
        shopPlace.DOAnchorPosY(1000, time).SetEase(ease);
        shopCanvas.DOFade(0, time);

        isOpen = false;
        yield return new WaitForSeconds(time);
        GameManager.instance.encounterManagementSystem.CreateNextDoorNode();
    }

    public void CreateSkillForBuy()
    {
        List<SkillAction> common = GameManager.instance.allData.GetSkillWithRarity(Rarity.Common);
        List<SkillAction> epic = GameManager.instance.allData.GetSkillWithRarity(Rarity.Epic);
        List<SkillAction> legendary = GameManager.instance.allData.GetSkillWithRarity(Rarity.Legendary);

        SkillAction skill = new SkillAction();
        //Debug.Log(common.Count + " " + epic.Count + " " + legendary.Count);
        for(int i = 0; i < 5; i++)
        {
            do
            {
                if(i < 2)
                {
                    if(common.Contains(skill)) common.Remove(skill);
                    if (common.Count == 0)
                    {
                        skill = null;
                        break;
                    }
                    skill = common[Random.Range(0, common.Count)];
                }else if(i < 4)
                {
                    if (epic.Contains(skill)) epic.Remove(skill);
                    if (epic.Count == 0)
                    {
                        skill = null;
                        break;
                    }
                    skill = epic[Random.Range(0, epic.Count)];
                }
                else
                {
                    if (legendary.Contains(skill)) legendary.Remove(skill);
                    if (legendary.Count == 0)
                    {
                        skill = null;
                        break;
                    }
                    skill = legendary[Random.Range(0, legendary.Count)];
                }
            } while (CheckAlreadyHaveSkillShop(skill.id));

            if(skill != null)
            {
                //Debug.Log("Skill " + skill.name + " " + skill.rarity);
                SkillBuy buy = Instantiate(skillPrefab, skillPlace).GetComponent<SkillBuy>();
                buy.SetupMerchandise(skill);

                skillBuys.Add(buy);
            }
        }
        OrderSkillBuy();
    }

    private bool CheckAlreadyHaveSkillShop(string id)
    {
        if(skillBuys.Count == 0) return false;
        foreach (var sk in skillBuys)
        {
            if (sk.GetSkillShow().GetSkillAction().id == id)
                return true;
        }

        return false;
    }

    public void CreateRelicForBuy()
    {
        List<Relic> common = GameManager.instance.allData.GetRelicWithRarity(Rarity.Common);
        List<Relic> epic = GameManager.instance.allData.GetRelicWithRarity(Rarity.Epic);
        List<Relic> legendary = GameManager.instance.allData.GetRelicWithRarity(Rarity.Legendary);

        Relic relic = new Relic();
        //Debug.Log(common.Count + " " + epic.Count + " " + legendary.Count);
        for (int i = 0; i < 5; i++)
        {
            do
            {
                if (i < 2)
                {
                    if (common.Contains(relic)) common.Remove(relic);
                    //Debug.Log("C " + common.Count);
                    if (common.Count == 0)
                    {
                        relic = null;
                        break;
                    }
                    relic = common[Random.Range(0, common.Count)];
                }
                else if (i < 4)
                {
                    if (epic.Contains(relic)) epic.Remove(relic);
                    //Debug.Log("E " + epic.Count);
                    if (epic.Count == 0)
                    {
                        relic = null;
                        break;
                    }
                    relic = epic[Random.Range(0, epic.Count)];
                }
                else
                {
                    if (legendary.Contains(relic)) legendary.Remove(relic);
                    //Debug.Log("L " + legendary.Count);
                    if (legendary.Count == 0)
                    {
                        relic = null;
                        break;
                    }
                    relic = legendary[Random.Range(0, legendary.Count)];
                }
            } while (GameManager.instance.playerData.CheckAlreadyHaveRelic(relic.id) || relic.id == "RN015");

            if (relic != null)
            {
                //Debug.Log("relic " + relic.name + " " + relic.rarity);
                RelicBuy buy = Instantiate(relicPrefab, relicPlace).GetComponent<RelicBuy>();
                buy.SetupMerchandise(relic);

                relicBuys.Add(buy);
            }
        }
        OrderRelicBuy();
    }

    public void ClearShop()
    {
        for(int i = 0; i < relicBuys.Count; i++)
            Destroy(relicBuys[i].gameObject);
        for(int i = 0; i < skillBuys.Count; i++)
            Destroy(skillBuys[i].gameObject);

        relicBuys.Clear();
        skillBuys.Clear();
    }

    public void OrderSkillBuy()
    {
        for(int i = 0; i < skillBuys.Count ;i++)
        {
            skillBuys[i].GetSkillBuyPos().anchoredPosition = new Vector2(i * 250, 0);
        }    
    }

    public void OrderRelicBuy()
    {
        for (int i = 0; i < relicBuys.Count; i++)
        {
            relicBuys[i].GetRelicBuyPos().anchoredPosition = new Vector2(i * 250, 0);
        }
    }

    public void CheckAllPrice()
    {
        foreach(var skill in skillBuys)
            skill.CheckCurrentGold();
        foreach(var relic in relicBuys)
            relic.CheckCurrentGold();
    }

    public bool GetCheckOpen() => isOpen;
    public List<SkillBuy> GetSkillBuys() => skillBuys;
    public List<RelicBuy> GetRelicBuys() => relicBuys;
}
