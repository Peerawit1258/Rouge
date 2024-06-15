using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

public class StatusWidget : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text countText;
    int count;

    StatusEffect status;
    [ReadOnly, SerializeField] string skillID;

    PlayerController playerController;
    EnemyController enemyController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetStatus(AddStatus add, EnemyController enemy = null)
    {
        status = add.statusEffect;

        icon.sprite = status.statusIcon;
        count = add.count;
        countText.text = count.ToString();

        if (enemy != null)
        {
            enemyController = enemy;
            if (add.statusEffect.effect == Effect.Stat)
                foreach (var status in add.statusEffect.statValues)
                {
                    enemyController.StatUp(status);
                }
        }
        else
        {
            playerController = GameManager.instance.turnManager.player;
            if (add.statusEffect.effect == Effect.Stat)
            {
                if (add.statusEffect.isSlot)
                {
                    foreach(var slot in add.statusEffect.slots)
                    {
                        if(slot.bonus >= 0)
                            GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box - 1].IncreaseValueBonusDMG(slot.bonus);
                        else
                            GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box - 1].DecreaseValueBonusDMG(slot.bonus);
                    }
                   
                }
                else
                {
                    foreach (var status in add.statusEffect.statValues)
                    {
                        StatValue set = new StatValue();
                        set.type = status.type;
                        if (add.statusEffect.id == "BN002")
                            set.value = status.value * count;
                        else
                            set.value = status.value;

                        //Debug.Log(status.value);
                        playerController.StatUp(set);
                    }
                }
            }
                
        }
            
    }

    public void SetNewCount(int num)
    {
        //Debug.Log(num + " " + count + " " + Mathf.Abs(num - count));
        #region PlayerOnly
        if (playerController != null)
        {
            if (status.id == "BN002")
            {
                foreach (var stat in status.statValues)
                {
                    StatValue set = new StatValue();
                    set.type = stat.type;
                    if (status.id == "BN002")
                        set.value = stat.value * Mathf.Abs(num - count);
                    else
                        set.value = stat.value;
                    playerController.StatUp(set);
                }
            }
        }
        #endregion
        count = num;
        countText.text = count.ToString();
    }

    public void DecreaseCount(int num = 1)
    {
        count -= num;
        if (count == 0)
        {
            DestroyStatus();
        }
        else
        {
            #region PlayerOnly
            if (playerController != null)
            {
                if (status.id == "BN002")
                {
                    foreach (var stat in status.statValues)
                    {
                        StatValue set = new StatValue();
                        set.type = stat.type;
                        if (status.id == "BN002")
                            set.value = stat.value * Mathf.Abs(num - count);
                        else
                            set.value = stat.value;
                        playerController.StatUp(set, false);
                    }
                }  
            }
            #endregion

            countText.text = count.ToString();
        }
    }

    public bool CheckSameStatus(AddStatus status)
    {
        if(status.statusEffect.statusName == this.status.statusName)
        {
            if (this.status.canStack)
            {
                int increase = +GameManager.instance.relicManagerSystem.IncreaseCapStack(status.statusEffect.id);
                if (status.count + count <= this.status.capStack + increase)
                    SetNewCount(status.count + count);
                else
                    SetNewCount(this.status.capStack + increase);
            }  
            else
                SetNewCount(status.count);
            return true;
        } 
        return false;
    }

    public void DestroyStatus()
    {
        Debug.Log("Sd");
        if (status.effect == Effect.Stat)
        {
            if (playerController != null)
            {
                if (status.isSlot)
                {
                    foreach (var slot in status.slots)
                    {
                        if (slot.bonus >= 0)
                            GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].DecreaseValueBonusDMG(slot.bonus);
                        else
                            GameManager.instance.skillOrderSystem.GetAllSlot()[slot.box].IncreaseValueBonusDMG(slot.bonus);
                    }
                }
                else
                {
                    foreach (var stat in status.statValues)
                    {
                        StatValue set = new StatValue();
                        set.type = stat.type;
                        if (status.id == "BN002")
                            set.value = stat.value * count;
                        else
                            set.value = stat.value;
                        playerController.StatUp(set, false);
                        //playerController.gaugeHp.statusWidgets.Remove(this);
                    }
                }

                foreach(var st in playerController.gaugeHp.statusWidgets)
                {
                    if (st.status.id == status.id)
                    {
                        playerController.gaugeHp.statusWidgets.Remove(st);
                        break;
                    }
                }

                Destroy(gameObject);

            }
            else
            {
                foreach (var stat in status.statValues)
                {
                    
                    enemyController.StatUp(stat, false);
                    //enemyController.gaugeHP.statusWidgets.Remove(this);
                }

                foreach (var st in enemyController.gaugeHP.statusWidgets)
                {
                    if (st.status.id == status.id)
                    {
                        enemyController.gaugeHP.statusWidgets.Remove(st);
                        break;
                    }
                }

                Destroy(gameObject);
            }
        }
        else
        {
            if(playerController != null)
            {
                foreach (var st in playerController.gaugeHp.statusWidgets)
                {
                    if (st.status.id == status.id)
                    {
                        playerController.gaugeHp.statusWidgets.Remove(st);
                        break;
                    }
                }

                Destroy(gameObject);
            }
            else
            {
                foreach (var st in enemyController.gaugeHP.statusWidgets)
                {
                    if (st.status.id == status.id)
                    {
                        enemyController.gaugeHP.statusWidgets.Remove(st);
                        break;
                    }
                }

                Destroy(gameObject);
            }
        }
    }

    public TriggerStatus GetTriggerStatus() => status.trigger;
    public StatusEffect GetStatus() => status;
    public int GetCount() => count;
    
}
