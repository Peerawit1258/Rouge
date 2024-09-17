using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;

public class SkillOrderSystem : MonoBehaviour
{
    public int slotCount;
    public int skillCount;
    [Header("SkillCreate")]
    public GameObject skillPrefab;
    public Transform skillPlace;

    [Header("SkillSlot")]
    public GameObject slotPrefab;
    public Transform slotPlace;

    [TabGroup("Value"), SerializeField] int space;

    [ReadOnly] public SkillWidget selectWidget;
    public GameObject actionBtn;

    List<SkillWidget> allSkillWidget = new List<SkillWidget>();
    List<SlotSkill> allSlot = new List<SlotSkill>();
    DamageCalculator damageCalculator;
    TurnManager turnManager;
    StatusEffectSystem statusEffectSystem;
    RelicManagerSystem relicManagerSystem;
    InventoryManager inventoryManager;
    // Start is called before the first frame update
    void Start()
    {

        damageCalculator = GameManager.instance.damageCalculator;
        turnManager = GameManager.instance.turnManager;
        statusEffectSystem = GameManager.instance.statusEffectSystem;
        relicManagerSystem = GameManager.instance.relicManagerSystem;
        inventoryManager = GameManager.instance.inventoryManager;
        CreateSlot();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.D))
        {
            if (isSkillActive) return;
            currentSkill = 0;
            skillPlace.gameObject.SetActive(false);
            GameManager.instance.skillDesc.FadeOut();
            if (allSkillWidget.Count > 0)
                foreach (SkillWidget show in allSkillWidget)
                    show.skillShow.SetCoolDown();
            StartCoroutine(ActiveSkillAttack());
        }
    }

    public void ActionOrder()
    {
        if (isSkillActive) return;
        currentSkill = 0;
        skillPlace.gameObject.SetActive(false);
        actionBtn.gameObject.SetActive(false);
        GameManager.instance.skillDesc.FadeOut();
        if (allSkillWidget.Count > 0)
            foreach (SkillWidget show in allSkillWidget)
                show.skillShow.SetCoolDown(true);
        StartCoroutine(ActiveSkillAttack());
    }

    int currentSkill = 0;
    int heal = 0;
    [HideInInspector] public bool isSkillActive;
    IEnumerator ActiveSkillAttack()
    {
        isSkillActive = true;
        heal = 0;
        
        if (allSlot[currentSkill].skill != null)
        {
            if (turnManager.enemies.Count <= 0)
            {
                GameManager.instance.resultBattle.StartWinResult();
                yield break;
            }
            SkillAction skill = allSlot[currentSkill].skill;

            if (skill.shakeCam)
            {
                for (int i = 0; i < skill.shakeCount; i++)
                    GameManager.instance.mainCamera.CameraShake(i * 0.2f + 0.3f);
            }
            GameManager.instance.detailPanel.ShowSkillActionName(skill.skillName);
            switch (skill.GetSkillType())
            {
                case SkillType.Attack:
                    AttackSkill(skill);
                break;
                case SkillType.Debuff:
                    DebuffSkill(skill);
                break;
                case SkillType.Buff:
                    BuffSkill(skill);
                break;
                case SkillType.Heal:
                    HealSkill(skill);
                break;
            }
            statusEffectSystem.TriggerStatusAction(skill.GetSkillType());
            yield return new WaitUntil(() => !turnManager.player.animationAction.isAction);
            if (skill.GetCheckTakeDamage())
                turnManager.player.StartDamageTaken((int)turnManager.player.maxHpValue * skill.GetTakeDamagePercent() / 100, 0, true);

            if (skill.GetHaveCondition() && skill.GetConditonType() == ConditionType.EffectOrder &&
                allSlot[currentSkill].sameConditon && skill.GetSkillType() != SkillType.Attack && skill.GetSkillType() != SkillType.Debuff)
                AdditionEffectinCondition(skill.GetConditionDetail());

            yield return new WaitForSeconds(1f);
            allSlot[currentSkill].GetSkillWidget().skillShow.SetCoolDown(); 
        }
        currentSkill++;
        if (currentSkill < allSlot.Count)
            StartCoroutine(ActiveSkillAttack());
        else
        {
            yield return new WaitForSeconds(1);
            statusEffectSystem.TriggerStatusCount(TriggerStatus.End);
            turnManager.StartEnemyTurn();
        }
    }

    void AdditionEffectinCondition(ConditionDetail condition, List<EnemyController> enemies = null)
    {
        if (condition.condition != ConditionType.EffectOrder) return;

        switch (condition.c_skillType)
        {
            case SkillType.Attack:
                for(int i = 0;i < enemies.Count; i++)
                {
                    int percentSkill = condition.percentDmg;
                    int damageBonus = turnManager.player.GetDamageBonus() + allSlot[currentSkill].GetBonusDMG();

                    int damage = damageCalculator.DamageResult((int)turnManager.player.atkValue, percentSkill,
                                                (int)enemies[i].defValue, damageBonus, enemies[i].GetDamageReduce());
                    enemies[i].StartDamageTaken(damage, i * 0.2f + 0.2f);
                }  
                break;
            case SkillType.Buff:
                statusEffectSystem.GetStatusInPlayer(condition.c_status);
                break;
            case SkillType.Debuff:
                if (enemies.Count == 0) return;

                foreach (var enemy in enemies)
                {
                    if (condition.isRemove)
                        statusEffectSystem.RemoveBuff(condition.c_removeBuff, enemy);
                    else
                        statusEffectSystem.GetStatusInEnemy(enemy, condition.c_status);
                }
                break;
            case SkillType.Heal:
                if (condition.healType == HealType.Heal)
                {
                    if (condition.isPercent)
                        heal = ((int)turnManager.player.maxHpValue * condition.c_heal) / 100;
                    else
                        heal = condition.c_heal;
                    turnManager.player.StartHealHP(heal, 0);
                }
                else if (condition.healType == HealType.RemoveDebuff)
                {
                    statusEffectSystem.RemoveDebuff(null);
                }
                else
                {
                    statusEffectSystem.RemoveDebuff(condition.removeEffect);
                }
                break;
        }
    }

    #region Action
    private void AttackSkill(SkillAction skill)
    {
        if (turnManager.enemies.Count == 0) return;

        List<EnemyController> enemies = new List<EnemyController>();
        List<EnemyController> randomEnemy = new List<EnemyController>();
        StatusWidget widget = new StatusWidget();
        float ratio = turnManager.player.hpValue / turnManager.player.maxHpValue;
        //Debug.Log("Hp: " + ratio);
        turnManager.player.animationAction.AttackAction();

        if (skill.targetType == TargetType.Random)
        {
            for (int j = 0; j < skill.GetPercentDamage().Count; j++)
            {
                EnemyController enemy = turnManager.enemies[Random.Range(0, turnManager.enemies.Count)];
                int percentSkill = skill.GetPercentDamage()[j] + 
                    skill.GetDamageSpecific().CheckStatus(enemy.gaugeHP.CheckSameStatus(skill.GetDamageSpecific().damageForEffect)) + 
                    skill.GetHpPercentSkill(ratio);
                int damageBonus = turnManager.player.GetDamageBonus() + allSlot[currentSkill].GetBonusDMG();
                if (damageBonus < 0) damageBonus = 0;

                int damage = damageCalculator.DamageResult((int)turnManager.player.atkValue, percentSkill,
                                            (int)enemies[Random.Range(0, enemies.Count)].defValue, damageBonus, enemy.GetDamageReduce());
                if (turnManager.player.gaugeHp.CheckStatuswithID("BN002"))
                    damage *= 2;
                enemy.StartDamageTaken(damage, j * 0.2f + turnManager.player.animationAction.attackTime, false, skill.particleEffect);

                if (enemy.gaugeHP.CheckStatuswithID("BD001", ref widget))
                    turnManager.player.StartDamageTaken(widget.GetStatus().damage, j * 0.2f + turnManager.player.animationAction.attackTime + 0.2f);
                
                if (skill.GetIsHeal())
                {
                    if (skill.GetHealType() == HealType.Heal)
                    {
                        heal = (damage * skill.GetPercentHeal(ratio)) / 100;
                        turnManager.player.StartHealHP(heal, j);
                    }
                    else if (skill.GetHealType() == HealType.RemoveDebuff && j == 0)
                    {
                        statusEffectSystem.RemoveDebuff(null);
                    }
                    else if (skill.GetHealType() == HealType.RemoveSpecific && j == 0)
                    {
                        statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff());
                    }
                }

                if (skill.GetStatusTarget().Count > 0)
                    foreach (var status in skill.GetStatusTarget())
                        statusEffectSystem.GetStatusInEnemy(enemy, status);
                randomEnemy.Add(enemy);
            }
        }
        else
        {
            if (skill.targetType == TargetType.SingleTarget) enemies.Add(turnManager.targetEnemy);
            else enemies = turnManager.enemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = 0; j < skill.GetPercentDamage().Count; j++)
                {
                    int percentSkill = skill.GetPercentDamage()[j] + 
                        skill.GetDamageSpecific().CheckStatus(enemies[i].gaugeHP.CheckSameStatus(skill.GetDamageSpecific().damageForEffect)) +
                        skill.GetHpPercentSkill(ratio);
                    int damageBonus = turnManager.player.GetDamageBonus() + allSlot[currentSkill].GetBonusDMG();
                    if (damageBonus < 0) damageBonus = 0;
                    
                    int damage = damageCalculator.DamageResult((int)turnManager.player.atkValue, percentSkill,
                                                (int)enemies[i].defValue, damageBonus, enemies[i].GetDamageReduce());
                    if (turnManager.player.gaugeHp.CheckStatuswithID("BN002"))
                        damage *= 2;
                    enemies[i].StartDamageTaken(damage, (i + j) * 0.2f + turnManager.player.animationAction.attackTime, false);
                    if(enemies.Count > 1)
                    {
                        if (skill.particleEffect != null && i == 0) Instantiate(skill.particleEffect, enemies[1].gameObject.transform);
                    }
                    else
                    {
                        if (skill.particleEffect != null) Instantiate(skill.particleEffect, enemies[i].gameObject.transform);
                    }
                        
                    if (enemies[i].gaugeHP.CheckStatuswithID("BD001", ref widget))
                        turnManager.player.StartDamageTaken(widget.GetStatus().damage, (i + j) * 0.2f + turnManager.player.animationAction.attackTime + 0.2f);

                    if (skill.GetIsHeal())
                    {
                        if (skill.GetHealType() == HealType.Heal)
                        {
                            heal = (damage * skill.GetPercentHeal(ratio)) / 100;
                            turnManager.player.StartHealHP(heal, j);
                        }
                        else if (skill.GetHealType() == HealType.RemoveDebuff)
                        {
                            statusEffectSystem.RemoveDebuff(null);
                        }
                        else if (skill.GetHealType() == HealType.RemoveSpecific)
                        {
                            statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff());
                        }
                    }
                }
                if (skill.GetStatusTarget().Count > 0)
                    foreach (var status in skill.GetStatusTarget())
                        statusEffectSystem.GetStatusInEnemy(enemies[i], status);
                randomEnemy.Add(enemies[i]);
            }
        }

        if (skill.GetHaveCondition() && skill.GetConditonType() == ConditionType.EffectOrder &&
                allSlot[currentSkill].sameConditon)
            AdditionEffectinCondition(skill.GetConditionDetail(), randomEnemy);

        if (skill.GetStatusSelf().Count > 0)
            foreach (var status in skill.GetStatusSelf())
                statusEffectSystem.GetStatusInPlayer(status);

        if (skill.GetCheckHpCompare())
            if (skill.GetHpAbility().type == SkillType.Buff)
                statusEffectSystem.GetStatusInPlayer(skill.GetHpStatus(ratio));

    }

    private void DebuffSkill(SkillAction skill)
    {
        if (turnManager.enemies.Count == 0) return;

        List<EnemyController> enemies = new List<EnemyController>();
        List<EnemyController> randomEnemy = new List<EnemyController>();
        turnManager.player.animationAction.AttackAction();

        if (skill.targetType == TargetType.SingleTarget) enemies.Add(turnManager.targetEnemy);
        else enemies = turnManager.enemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (skill.targetType == TargetType.Random)
                randomEnemy.Add(enemies[Random.Range(0, enemies.Count)]);
            else
                randomEnemy.Add(enemies[i]);
        }
        foreach (var enemy in randomEnemy)
        {
            if (skill.GetCheckRemoveBuff())
            {
                statusEffectSystem.RemoveBuff(skill.GetRemoveEffectType(), enemy);
            }
            else
            {
                foreach (var status in skill.GetStatusTarget())
                {
                    statusEffectSystem.GetStatusInEnemy(enemy, status);
                }
            }
        }

        if (skill.GetHaveCondition() && skill.GetConditonType() == ConditionType.EffectOrder &&
                allSlot[currentSkill].sameConditon)
            AdditionEffectinCondition(skill.GetConditionDetail(), randomEnemy);
    }

    private void BuffSkill(SkillAction skill)
    {
        float ratio = turnManager.player.hpValue / turnManager.player.maxHpValue;
        turnManager.player.animationAction.BuffAction();
        foreach (var status in skill.GetStatusSelf())
        {
            statusEffectSystem.GetStatusInPlayer(status);
        }

        if (skill.GetCheckHpCompare())
        {
            if (skill.GetHpAbility().type == SkillType.Buff)
                statusEffectSystem.GetStatusInPlayer(skill.GetHpStatus(ratio));
        }
    }

    private void HealSkill(SkillAction skill)
    {
        float ratio = turnManager.player.hpValue / turnManager.player.maxHpValue;
        turnManager.player.animationAction.BuffAction();
        if (skill.GetHealType() == HealType.Heal)
        {
            heal = ((int)turnManager.player.maxHpValue * skill.GetPercentHeal(ratio)) / 100;
            turnManager.player.StartHealHP(heal, 0);
        }
        else if (skill.GetHealType() == HealType.RemoveDebuff)
        {
            statusEffectSystem.RemoveDebuff(null);
        }
        else
        {
            statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff());
        }
    }
    #endregion    
    #region SkillSlot
    public void CreateSlot()
    {
        if(allSlot.Count > 0)
        {
            foreach (var slot in allSlot)
                Destroy(slot.gameObject);
            allSlot.Clear();
        }
        for (int i = 0; i < slotCount; i++)
        {
            SlotSkill slot = Instantiate(slotPrefab, slotPlace).GetComponent<SlotSkill>();
            slot.numSlot = i + 1;
            allSlot.Add(slot);
        }
    }

    public void CreateSkillSlot()
    {
        skillPlace.gameObject.SetActive(true);
        if (inventoryManager.GetSkillActive().Count == 0) return;
        for(int i = 0; i < skillCount + relicManagerSystem.randomSkill; i++)
        {
            if (i > inventoryManager.GetSkillActive().Count + inventoryManager.GetSkillUsable().Count - 1)
            {
                break;
            }
            SkillAction skill;
            int random;
            random = Random.Range(0, inventoryManager.GetSkillActive().Count);
            skill = inventoryManager.GetSkillActive()[random].GetSkillAction();

            

            SkillWidget widget = Instantiate(skillPrefab,skillPlace).GetComponent<SkillWidget>();
            widget.skillShow = inventoryManager.GetSkillActive()[random];
            widget.SetDetail(skill);
            widget.name = skill.skillName + "_" + i;
            allSkillWidget.Add(widget);

            SkillShow show = inventoryManager.GetSkillActive()[random];
            Debug.Log(show.GetSkillAction().skillName);
            inventoryManager.GetSkillUsable().Add(show);
            inventoryManager.GetSkillActive().Remove(show);
            
        }
        OrderDistanceSkill();
        slotPlace.gameObject.SetActive(true);
    }
    [HideInInspector] public bool isOrder = false;
    [Button]
    public void OrderDistanceSkill()
    {
        if (allSkillWidget.Count == 0) return;
        float index = GetIndexFirstSkillCreate();
        float distance;
        isOrder = true;
        for (int i = 0; i < allSkillWidget.Count; i++)
        {
            //Debug.Log(index);
            distance = index * space;
            allSkillWidget[i].GetWidgetPos().DOAnchorPos(new Vector2(distance, 0), 0.2f).SetEase(Ease.InOutQuart);
            //if (!allSkillWidget[i].skillNumSlot.gameObject.activeSelf)
            //    allSkillWidget[i].skillNumSlot.gameObject.SetActive(true);
            index++;
            if (i == allSkillWidget.Count - 1)
                isOrder = false;
        }
    }

    public void ClearSlotSkill()
    {
        foreach(SlotSkill slot in allSlot)
            slot.ClearData(true);
        for(int i = 0; i < allSkillWidget.Count;i++)
            Destroy(allSkillWidget[i].gameObject);
        allSkillWidget.Clear();
    }

    #endregion
    #region Value
    private bool CheckAllSkillWidget(SkillAction skill)
    {
        if(allSkillWidget.Count == 0) return false;
        
        foreach (SkillWidget widget in allSkillWidget)
        {
            if (widget.skill.id == skill.id)
                return true;
        }

        return false;
    }

    private float GetIndexFirstSkillCreate()
    {
        switch (allSkillWidget.Count)
        {
            case 7: return -3;
            case 6: return -2.5f;
            case 5: return -2;
            case 4: return -1.5f;
            case 3: return -1;
            case 2: return -0.5f;
            default: return 0;
        }
    }
    
    public SlotSkill GetEmptySlotSkill()
    {
        SlotSkill slot = new SlotSkill();

        foreach(var slotSkill in allSlot)
        {
            if (slotSkill.skill == null)
            {
                slot = slotSkill;
                break;
            }
        }

        return slot;
    }

    public List<SlotSkill> GetAllSlot() => allSlot;
    public List<SkillWidget> GetAllSkillWidget() => allSkillWidget;
    #endregion
}
