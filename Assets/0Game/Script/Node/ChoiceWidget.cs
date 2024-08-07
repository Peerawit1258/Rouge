using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using DG.Tweening;

public class ChoiceWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] RectTransform rect;
    [SerializeField] TMP_Text txt;
    [SerializeField] bool exit;
    [SerializeField] GameObject notPassObj;

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
            txt.color = Color.red;
            notPassObj.SetActive(true);
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
                    if(choiceDetail.disappear)
                        GameManager.instance.inventoryManager.RemoveSkill(choiceDetail.useSkill);
                    
                    break;
                case RewardType.Relic:
                    if (choiceDetail.disappear)
                        GameManager.instance.relicManagerSystem.RemoveRelic(choiceDetail.useRelic);
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

        if(choiceDetail.rate > 0)
        {
            int random = Random.Range(0, 100);
            if (random >= choiceDetail.rate)
            {
                eventManager.NextEvent(choiceDetail.failEvent);
                return;
            } 
        }
        switch (choiceDetail.type) 
        {
            case ChoiceType.Reward:
                if (choiceDetail.reward.rewardType == RewardType.Gold)
                {
                    gold = choiceDetail.reward.RandomGold();
                    GameManager.instance.playerData.gold += gold;// Change
                    replace = "+" + gold.ToString() + " Gold";
                }
                else if (choiceDetail.reward.rewardType == RewardType.Skill)
                {
                    skill = choiceDetail.reward.RandomSkill();
                    eventManager.SetAfterEvent(() => GameManager.instance.inventoryManager.CreateSkillShow(skill));
                    replace = skill.skillName + "(Skill)";
                }
                else
                {
                    relic = choiceDetail.reward.RandomRelic();
                    eventManager.SetAfterEvent(() => GameManager.instance.resultBattle.StartCreateSpecificRelicDetail(relic));
                    replace = relic.relicName + "(Relic)";
                }

                if (choiceDetail.exit) eventManager.ExitEvent();
                else eventManager.AfterSelectChoice(choiceDetail, replace);
                break;

            case ChoiceType.Heal:
                if(choiceDetail.heal.healType == HealType.Heal)
                {
                    int value = GameManager.instance.turnManager.player.hpValue * choiceDetail.heal.percentHeal / 100;
                    eventManager.SetAfterEvent(() => GameManager.instance.turnManager.player.StartHealHP(value, 0));
                    replace = "+" + value.ToString() + " HP";
                }
                else
                {
                    if (choiceDetail.heal.isAll)
                        eventManager.SetAfterEvent(() => GameManager.instance.statusEffectSystem.RemoveAllStatus(StatusType.Special, true));
                    else
                        eventManager.SetAfterEvent(() => GameManager.instance.statusEffectSystem.RemoveAllStatus(StatusType.Debuff));
                }

                if (choiceDetail.exit) eventManager.ExitEvent();
                else eventManager.AfterSelectChoice(choiceDetail);
                break;

            case ChoiceType.BuffDebuff:
                eventManager.SetAfterEvent(() => {
                    foreach (var status in choiceDetail.status)
                        GameManager.instance.statusEffectSystem.GetStatusInPlayer(status);
                });

                if (choiceDetail.exit) eventManager.ExitEvent();
                else eventManager.AfterSelectChoice(choiceDetail);
                break;

            case ChoiceType.Enemy:
                eventManager.SetAfterEvent(() =>
                {
                    //eventManager.ExitEvent();
                    GameManager.instance.battleSetup.SetupEnemyBattle(choiceDetail.enemyGroup.enemies);
                    DOVirtual.DelayedCall(1, () =>
                    {
                        Debug.Log("Create");
                        GameManager.instance.turnManager.StartTurn();
                    });
                });

                if(choiceDetail.next) eventManager.AfterSelectChoice(choiceDetail);
                else eventManager.ExitEvent();
                break;

            case ChoiceType.BaseStat:
                eventManager.SetAfterEvent(() => GameManager.instance.turnManager.player.BaseStatUpdate(choiceDetail.statValue));
                if (choiceDetail.exit) eventManager.ExitEvent();
                else eventManager.AfterSelectChoice(choiceDetail);
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
