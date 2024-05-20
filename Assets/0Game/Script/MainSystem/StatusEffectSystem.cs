using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class StatusEffectSystem : MonoBehaviour
{
    public GameObject statusWidgetPrefab;
    public GameObject statusWidgetEnemyPrefab;

    PlayerController player;
    RelicManagerSystem relicManagerSystem;
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.instance.turnManager.player;
        relicManagerSystem = GameManager.instance.relicManagerSystem;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #region gain
    [Button]
    public void GetStatusInEnemy(EnemyController enemy,AddStatus add)
    {
        if (add == null) return;
        if (enemy.gaugeHP.statusWidgets.Count > 0)
        {
            foreach(var widget in enemy.gaugeHP.statusWidgets)
            {
                if (widget.CheckSameStatus(add)) return;
            }
        }   

        if (enemy.e_type == EnemyType.Boss)
        {

        }
        else
        {
            StatusWidget widget = Instantiate(statusWidgetEnemyPrefab, enemy.gaugeHP.GetStatusPlace()).GetComponent<StatusWidget>();
            widget.SetStatus(add, enemy);
            enemy.GetStatusWidgets().Add(widget);
        }
        GameManager.instance.numberDamageSystem.CreateStatusText(enemy.transform, add.statusEffect);
        if (add.statusEffect.particleEffect != null) Instantiate(add.statusEffect.particleEffect, enemy.transform);

    }
    [Button]
    public void GetStatusInPlayer(AddStatus add)
    {
        if(add == null) return;
        if (player.gaugeHp.statusWidgets.Count > 0)
        {
            foreach (var wg in player.gaugeHp.statusWidgets)
            {
                if (wg.CheckSameStatus(add)) return;
            }
        }
        StatusWidget widget = Instantiate(statusWidgetPrefab, player.gaugeHp.GetStatusPlace()).GetComponent<StatusWidget>();
        widget.SetStatus(add);
        player.GetStatusWidgets().Add(widget);
        GameManager.instance.numberDamageSystem.CreateStatusText(player.transform, add.statusEffect);
        if (add.statusEffect.particleEffect != null) Instantiate(add.statusEffect.particleEffect, player.transform);
    }
    #endregion
    #region Trigger
    public void TriggerStatusCount(TriggerStatus trigger, EnemyController enemy = null)
    {
        if(enemy != null)
        {
            if (enemy.GetStatusWidgets().Count == 0) return;
            if (enemy.e_type == EnemyType.Boss)
            {

            }
            else
            {
                List<StatusWidget> status = enemy.gaugeHP.GetStatusWithTrigger(trigger);
                for(int i = 0; i < status.Count; i++)
                {
                    if(status[i].GetStatus().effect != Effect.Stat)
                        TriggerStatusEffect(status[i].GetStatus(), i * 0.2f, enemy);
                    status[i].DecreaseCount();
                }
            }
        }
        else
        {
            if(player.GetStatusWidgets().Count == 0) return;
            List<StatusWidget> status = GameManager.instance.turnManager.player.gaugeHp.GetStatusWithTrigger(trigger);
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].GetStatus().effect != Effect.Stat)
                    TriggerStatusEffect(status[i].GetStatus(), i * 0.2f, enemy);
                status[i].DecreaseCount();
            }
        }
    }

    public void TriggerStatusAction(SkillType type, EnemyController enemy = null)
    {
        if(enemy != null)
        {
            if (enemy.GetStatusWidgets().Count == 0) return;
            if (enemy.e_type == EnemyType.Boss)
            {

            }
            else
            {
                List<StatusWidget> status = enemy.gaugeHP.GetStatusWithTrigger(TriggerStatus.Action);
                for (int i = 0; i < status.Count; i++)
                {
                    if (status[i].GetStatus().skill == type)
                    {
                        if (status[i].GetStatus().effect != Effect.Stat)
                            TriggerStatusEffect(status[i].GetStatus(), i * 0.2f, enemy);
                        status[i].DecreaseCount();
                    }
                }
            }
        }
        else
        {
            List<StatusWidget> status = player.gaugeHp.GetStatusWithTrigger(TriggerStatus.Action);
            for (int i = 0; i < status.Count; i++)
            {
                if (status[i].GetStatus().skill == type)
                {
                    if (status[i].GetStatus().effect != Effect.Stat)
                        TriggerStatusEffect(status[i].GetStatus(), i * 0.2f, enemy);
                    status[i].DecreaseCount();
                }
            }
        }
    }

    public void TriggerStatusEffect(StatusEffect status,float delay = 0 , EnemyController enemy = null)
    {
        if (enemy != null)
        {
            switch (status.effect)
            {
                case Effect.DOT:
                    //if (status.useAllStack)
                    //    enemy.StartDamageFromDebuff(status.damage * enemy.gaugeHP.GetStatuswithStatus(status).GetCount(), delay, status);
                    //else
                        enemy.StartDamageFromDebuff(relicManagerSystem.DamageDOTIncrease(status.id, status.damage), delay, status);
                    break;
                case Effect.Regen:
                    enemy.StartHealHP(status.heal, delay);
                    break;
            }
        }
        else
        {
            switch (status.effect)
            {
                case Effect.DOT:
                    player.StartDamageFromDebuff(status.damage, delay, status);
                    break;
                case Effect.Regen:
                    player.StartHealHP(status.heal, delay);
                    break;
            }
        }
    }
    #endregion
    #region Remove
    public void RemoveDebuff(StatusEffect status, EnemyController enemy = null)
    {
        StatusWidget widget;
        if (status == null)
        {
            if (enemy != null)
            {
                if (enemy.GetStatusWidgets().Count == 0) return;
                widget = enemy.gaugeHP.GetStatuswithType(StatusType.Debuff);
                if (widget == null) return;
                enemy.gaugeHP.statusWidgets.Remove(widget);
            }
            else
            {
                if (player.GetStatusWidgets().Count == 0) return;
                widget = player.gaugeHp.GetStatuswithType(StatusType.Debuff);
                if (widget == null) return;
                player.gaugeHp.statusWidgets.Remove(widget);
            }
        }
        else
        {
            if (enemy != null)
            {
                if (enemy.GetStatusWidgets().Count == 0) return;
                widget = enemy.gaugeHP.GetStatuswithStatus(status);
                if (widget == null) return;
                enemy.gaugeHP.statusWidgets.Remove(widget);
            }
            else
            {
                if (player.GetStatusWidgets().Count == 0) return;
                widget = player.gaugeHp.GetStatuswithStatus(status);
                if (widget == null) return;
                player.gaugeHp.statusWidgets.Remove(widget);
            }
        }
        if (widget != null) widget.DestroyStatus();
    }

    public void RemoveBuff(Effect effect, EnemyController enemy = null)
    {
        StatusWidget widget;
        if(enemy != null)
        {
            if (enemy.GetStatusWidgets().Count == 0) return;
            widget = enemy.gaugeHP.GetStatuswithType(StatusType.Buff, effect);
            enemy.gaugeHP.statusWidgets.Remove(widget);
        }
        else
        {
            if (player.GetStatusWidgets().Count == 0) return;
            widget = player.gaugeHp.GetStatuswithType(StatusType.Buff, effect);
            player.gaugeHp.statusWidgets.Remove(widget);
        }
 
        if (widget != null) widget.DestroyStatus();
    }

    [Button]
    public void RemoveAllStatus(StatusType type, bool isAll = false)
    {
        if (!isAll)
        {
            int count = player.gaugeHp.GetAmountBuffDebuff(type);
            StatusWidget widget;

            for (int i = 0; i < count; i++)
            {
                widget = player.gaugeHp.GetStatuswithType(type, Effect.None);
                player.gaugeHp.statusWidgets.Remove(widget);
                widget.DestroyStatus();
            }
        }
        else
        {
            for (int i = 0; i < player.gaugeHp.statusWidgets.Count; i++)
                player.gaugeHp.statusWidgets[i].DestroyStatus();

            player.gaugeHp.statusWidgets.Clear();
        }
        
            
    }
    #endregion
}
