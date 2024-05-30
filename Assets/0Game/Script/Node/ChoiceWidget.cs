using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;

public class ChoiceWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TMP_Text txt;
    [SerializeField] bool exit;

    [ReadOnly, SerializeField] ChoiceDetail choiceDetail;
    EventManager eventManager;

    bool notPass;
    // Start is called before the first frame update
    void Start()
    {
        eventManager = FindAnyObjectByType<EventManager>();
    }

    public void ChoiceSetup(ChoiceDetail detail)
    {
        choiceDetail = detail;
        txt.text = choiceDetail.sentence;

        if (!CheckRequired(detail)) 
        { 
            notPass = true;

        }
    }

    private bool CheckRequired(ChoiceDetail detail)
    {
        if (choiceDetail.required)
        {
            if (choiceDetail.requiredType == RewardType.Gold)
            {
                if (GameManager.instance.playerData.gold < choiceDetail.useGold)
                {
                    Debug.Log("less money");
                    return false;
                }
            }
            else if (choiceDetail.requiredType == RewardType.Relic)
            {
                if (!GameManager.instance.playerData.CheckAlreadyHaveRelic(choiceDetail.useRelic.id))
                {
                    Debug.Log("not found");
                    return false;
                }
            }
            else if (choiceDetail.requiredType == RewardType.Skill)
            {
                if (!GameManager.instance.playerData.CheckAlreadyHaveSkill(choiceDetail.useSkill.id))
                {
                    Debug.Log("not found");
                    return false;
                }
            }
        }

        return true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (notPass) return;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (notPass) return;
    }
    int gold = 0;
    SkillAction skill;
    Relic relic;
    string replace;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (notPass) return;
        if (exit)
        {
            eventManager.ExitEvent();
            GameManager.instance.encounterManagementSystem.CreateNextDoorNode();
            return;
        }
        if (choiceDetail.required)
        {
            switch (choiceDetail.requiredType)
            {
                case RewardType.Skill:
                    break;
                case RewardType.Relic:
                    break;
                case RewardType.Gold:
                    GameManager.instance.playerData.gold -= choiceDetail.useGold;
                    if (GameManager.instance.playerData.gold < 0) GameManager.instance.playerData.gold = 0;
                    break;
                case RewardType.Hp:
                    GameManager.instance.turnManager.player.StartDamageTaken(choiceDetail.useHp, 0, true);
                    break;
            }
        }
        switch (choiceDetail.type) 
        {
            case ChoiceType.Reward:
                if (choiceDetail.reward.isRandom)
                {
                    int num;
                    do
                    {
                        num = Random.Range(0, 3);
                    }while(!CheckRandomItem(num));
                    switch (num)
                    {
                        case 0:
                            //int gold = choiceDetail.reward.RandomReward<int>();
                            gold = choiceDetail.reward.RandomGold();
                            GameManager.instance.playerData.gold += gold;
                            replace = "+" + gold.ToString() + " Gold";
                            
                            break;
                        case 1:
                            skill = choiceDetail.reward.RandomSkill();
                            //GameManager.instance.
                            replace = skill.skillName + "(Skill)";
                            //GameManager.instance.allData.s.Add()
                            break;
                        case 2:
                            relic = choiceDetail.reward.RandomRelic();
                            replace = relic.relicName + "(Relic)";
                            break;
                        default:
                            //int n = choiceDetail.reward.RandomReward<int>();
                        break;
                    }
                }
                else
                {
                    if (choiceDetail.reward.rewardType == RewardType.Gold)
                    {
                        GameManager.instance.playerData.gold += choiceDetail.reward.RandomGold();
                        replace = "+" + gold.ToString() + " Gold";
                    }
                    else if(choiceDetail.reward.rewardType == RewardType.Skill)
                    {
                        skill = choiceDetail.reward.RandomSkill();
                        replace = skill.skillName + "(Skill)";
                    }
                    else
                    {
                        relic = choiceDetail.reward.RandomRelic();
                        replace = relic.relicName + "(Relic)";
                    }
                }
                eventManager.AfterSelectChoice(choiceDetail, replace);
            break;
            case ChoiceType.Heal:
                if(choiceDetail.heal.healType == HealType.Heal)
                {
                    int value = GameManager.instance.turnManager.player.hpValue * choiceDetail.heal.percentHeal / 100;
                    GameManager.instance.turnManager.player.StartHealHP(value, 0);
                    replace = "+" + value.ToString() + " HP";
                }
                else
                {
                    if (choiceDetail.heal.isAll)
                    {
                        GameManager.instance.statusEffectSystem.RemoveAllStatus(StatusType.Special, true);
                    }
                    else
                    {
                        GameManager.instance.statusEffectSystem.RemoveAllStatus(StatusType.Debuff);
                    }
                }
                eventManager.AfterSelectChoice(choiceDetail);
                break;
            case ChoiceType.BuffDebuff:
                foreach(var status in choiceDetail.status)
                {
                    GameManager.instance.statusEffectSystem.GetStatusInPlayer(status);
                }
                    
                break;
            case ChoiceType.Remove:

            break;
            case ChoiceType.Enemy:
                break;
            default:
                eventManager.AfterSelectChoice(choiceDetail);
                break;
        } 
    }

    public bool CheckRandomItem(int num)
    {
        if(num == 0)
        {
            return true;
        }
        else if(num == 1)
        {
            if (choiceDetail.reward.skills.Count == 0)
                return false;
        }
        else if (num == 2)
        {
            if (choiceDetail.reward.relics.Count == 0)
                return false;

        }
            
        return true;
    }
    IEnumerator StartCreateDoor()
    {
        eventManager.ExitEvent();
        yield return new WaitForSeconds(1);
        GameManager.instance.encounterManagementSystem.CreateNextDoorNode();
    }
}
