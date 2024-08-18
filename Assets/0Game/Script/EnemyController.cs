using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class EnemyController : CharacterValue
{
    [Title("EnemyController")]

    [SerializeField] GameObject enemyObj;
    public AnimationAction animationAction;
    [SerializeField, ReadOnly] Vector3 enemyPos;
    [SerializeField] Transform gaugePos;
    
    [TabGroup("1","Setup"), ReadOnly] public GaugeHpWidget gaugeHP;
    [TabGroup("1","Setup"), ReadOnly] public List<SkillAction> allSkill;
    [TabGroup("1", "Drop"), SerializeField, ReadOnly] List<SkillAction> dropSkills = new List<SkillAction>();
    [TabGroup("1", "Drop"), SerializeField, ReadOnly] List<Relic> dropRelics = new List<Relic>();
    [TabGroup("1", "Drop"), SerializeField, ReadOnly] int gold;

    [ReadOnly, ShowInInspector] SkillAction currentSkill;
    [ReadOnly, ShowInInspector] CharacterDetail minion;
    public SkillAction GetCurrentSkill() => currentSkill;
    public void SetDrop(List<SkillAction> skills, List<Relic> relics, int gold)
    {
        foreach(var skill in skills)
            dropSkills.Add(skill);
        
        foreach (var relic in relics)
            dropRelics.Add(relic);
        this.gold = gold;
    }

    [ReadOnly] public EnemyType e_type;

    TurnManager turnManager;
    StatusEffectSystem statusEffectSystem;
    // Start is called before the first frame update
    void Start()
    {
        enemyPos = transform.position;
        turnManager = GameManager.instance.turnManager;
        statusEffectSystem = GameManager.instance.statusEffectSystem;

        StartCoroutine(StartAction());
    }

    IEnumerator StartAction()
    {
        if(e_type == EnemyType.Boss)
        {
            yield return new WaitUntil(() => animationAction.animator != null);
            animationAction.animator.SetTrigger("Start");
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseEnter()
    {
        //Debug.Log("over");
    }

    public void OnMouseExit()
    {
        //Debug.Log("Pver");
    }

    public void OnMouseUp()
    {
        if (turnManager.actionTurn != ActionTurn.player) return;
        turnManager.targetEnemy = this;
        GameManager.instance.battleSetup.arrow.position = gaugePos.position + new Vector3(0, 0.3f, 0) ;

    }
    bool orderSkill;
    int indexSkill = 0;
    public void SetInfoEnemy(CharacterDetail detail, bool isMinion = false)
    {
        SetStatValue(detail.maxHP, detail.atk, detail.def, detail.damageReduce, true);
        allSkill = detail.allSkill;

        orderSkill = detail.orderSkill;
        indexSkill = 0;
        e_type = detail.e_type;

        SetDrop(detail.skillDrop, detail.relicDrop, detail.goldDrop);
        if (detail.minion == "") Debug.Log("Have Minion");
        if(detail.minion != "" && e_type != EnemyType.Normal)
            minion = GameManager.instance.allData.GetEnemyWithName(detail.minion);
        GameManager.instance.gaugeHpEnemy.SetPositionGauge(this);
        if (!orderSkill)
            currentSkill = GetRandomSkill();
        else
        {
            currentSkill = GetSkill();
            indexSkill++;
            if (indexSkill >= allSkill.Count) indexSkill = 0;
        }

        //if(e_type != EnemyType.Boss || e_type != EnemyType.Special) animationAction.ShowInScene();
        if(isMinion) animationAction.ShowInScene();
    }


    #region taken
    public void StartDamageTaken(int damage, float delay = 0, bool notDie = false, GameObject effect = null)=> StartCoroutine(DamageTaken(damage, delay, notDie, effect));

    IEnumerator DamageTaken(int damage, float delay, bool notDie, GameObject effect)
    {
        yield return new WaitForSeconds(delay);
        hpValue -= damage;
        if(effect != null) Instantiate(effect, gameObject.transform);
        yield return new WaitForSeconds(0.2f);
        GameManager.instance.numberDamageSystem.CreateDamageNumber(gameObject.transform, damage);
        if (hpValue <= 0)
        {
            if (notDie)
            {
                hpValue = 1;
                gaugeHP.HpGaugeChange(hpValue, maxHpValue);

            }
            else
            {
                hpValue = 0;
                //ratio = (float)hpValue / maxHpValue;
                gaugeHP.HpGaugeChange(hpValue, maxHpValue);
                yield return new WaitForSeconds(0.5f);
                DestroySelf();
            }
        }
        else
        {
            gaugeHP.HpGaugeChange(hpValue, maxHpValue);
            animationAction.TakeDamageAction();
        }
    }

    public void StartDamageFromDebuff(int damage, float delay, StatusEffect status) => StartCoroutine(DamageFromDebuff(damage, delay, status));
    IEnumerator DamageFromDebuff(int damage, float delay, StatusEffect status)
    {
        yield return new WaitForSeconds(delay);

        if(status.id == "DD001")
        {

        }
        hpValue -= damage;
        GameManager.instance.numberDamageSystem.CreateDamageNumber(gameObject.transform, damage, status);
        if (hpValue <= 0) hpValue = 0;

        gaugeHP.HpGaugeChange(hpValue, maxHpValue);
        if (hpValue <= 0) DestroySelf();
    }

    public void StartHealHP(int value, float delay) => StartCoroutine(HealHP(value, delay));

    IEnumerator HealHP(int value, float delay)
    {
        yield return new WaitForSeconds(delay);
        hpValue += value;
        if (hpValue > maxHpValue) hpValue = (int)maxHpValue;

        GameManager.instance.numberDamageSystem.CreateHealNumber(gameObject.transform, value.ToString());

        gaugeHP.HpGaugeChange(hpValue, maxHpValue);

    }
    #endregion
    #region DealDamage
    public void StartEnemyAttack() => StartCoroutine(ActiveSkillAttack());

    IEnumerator ActiveSkillAttack()
    {
        if (turnManager.player.hpValue <= 0)
        {
            GameManager.instance.resultBattle.StartFinishPanel(false);
            yield break;
        }

        statusEffectSystem.TriggerStatusCount(TriggerStatus.Start, this);
        if(hpValue <= 0)
        {
            GameManager.instance.enemyTurnSystem.DecreaseCurrentEnemy();
            DestroySelf();
        }

        if (currentSkill.shakeCam)
        {
            for (int i = 0; i < currentSkill.shakeCount; i++)
                GameManager.instance.mainCamera.CameraShake(i * 0.2f);
        }
        GameManager.instance.detailPanel.ShowSkillActionName(currentSkill.skillName);
        switch (currentSkill.GetSkillType())
        {
            case SkillType.Attack:
                AttackSkill(currentSkill);
                break;
            case SkillType.Debuff:
                DebuffSkill(currentSkill);
                break;
            case SkillType.Buff:
                BuffSkill(currentSkill);
                break;
            case SkillType.Heal:
                HealSkill(currentSkill);
                break;
            case SkillType.Summon:
                foreach(var pos in GameManager.instance.battleSetup.GetEnemyPos())
                {
                    if(pos.childCount == 0)
                    {
                        EnemyController enemy = Instantiate(minion.character, pos).GetComponent<EnemyController>();
                        enemy.transform.localPosition = Vector3.zero;
                        enemy.SetInfoEnemy(minion, true);
                        //enemy.SetDrop(minion.skillDrop, enemies[i].relicDrop, enemies[i].goldDrop);
                        enemy.name = minion.characterName + "_minion";
                        GameManager.instance.gaugeHpEnemy.SetPositionGauge(enemy);
                        turnManager.enemies.Add(enemy);
                    }
                }
                break;
        }
        statusEffectSystem.TriggerStatusAction(currentSkill.GetSkillType(), this);
        if (currentSkill.GetCheckTakeDamage())
            StartDamageTaken((int)maxHpValue * currentSkill.GetTakeDamagePercent() / 100, 0, true);
        yield return new WaitForSeconds(0.5f);
        statusEffectSystem.TriggerStatusCount(TriggerStatus.End, this);

        if (!orderSkill)
            currentSkill = GetRandomSkill();
        else
        {
            currentSkill = GetSkill();
            indexSkill++;
            if (indexSkill >= allSkill.Count) indexSkill = 0;
        }

        
        ShowNextAction();
    }
    [Button]
    public void ShowNextAction()
    {
        if(turnManager == null) turnManager = GameManager.instance.turnManager;

        List<int> damage = new List<int>();
        if (currentSkill.GetSkillType() == SkillType.Attack)
        {
            for(int i = 0; i < currentSkill.GetPercentDamage().Count; i++)
            {
                Debug.Log(currentSkill.GetPercentDamage()[i] + " " + i);
                int percentSkill = currentSkill.GetPercentDamage()[i] +
                currentSkill.GetDamageSpecific().CheckStatus(turnManager.player.gaugeHp.CheckSameStatus(currentSkill.GetDamageSpecific().damageForEffect)) +
                currentSkill.GetHpPercentSkill(hpValue / maxHpValue);

                damage.Add(GameManager.instance.damageCalculator.DamageResult((int)atkValue, percentSkill,
                                                (int)turnManager.player.GetPlayerDef(), GetDamageBonus(), turnManager.player.GetDamageReduce()));
            }

            if(damage.Count > 1) gaugeHP.SetNextAction(SkillType.Attack, damage[0].ToString() + "x" + damage.Count.ToString());
            else gaugeHP.SetNextAction(SkillType.Attack, damage[0].ToString());
        }
        else
        {
            gaugeHP.SetNextAction(currentSkill.GetSkillType());
        }
    }

    public SkillAction GetSkill()
    {
        SkillAction skill = new SkillAction();
        do
        {
            skill = allSkill[Random.Range(0, allSkill.Count)];
        }while(allSkill.Count > 0);
        
        return skill;
    }

    private SkillAction GetRandomSkill()
    {
        Debug.Log("Random");
        if (allSkill.Count < 3) return allSkill[Random.Range(0, allSkill.Count)];
        SkillAction skill = new SkillAction();
        Debug.LogError(name + "i");
        do
        {
            skill = allSkill[Random.Range(0, allSkill.Count)];
        }while(!CheckRandomSkill(skill));
        if (skill.GetSkillType() == SkillType.Wait)
        {
            isWait = true;
            cd_wait = allSkill.Count;
        }
        else
        {
            if (cd_wait > 0)
            {
                cd_wait--;
                if(cd_wait <= 0) isWait = false;
            }
        }
        return skill;
    }

    bool isWait;
    int cd_wait;
    private bool CheckRandomSkill(SkillAction skill)
    {
        Debug.Log("Check");
        Debug.Log(skill.skillName);
        if (turnManager == null) turnManager = GameManager.instance.turnManager;
        if (skill.GetSkillType() == SkillType.Buff)
        {
            foreach(var self in skill.GetStatusSelf())
                foreach (var widget in gaugeHP.statusWidgets)
                    if (widget.GetStatus().id == self.statusEffect.id)
                        if(widget.GetCount() == 1) return true;
                        else return false;
        }else if(skill.GetSkillType() == SkillType.Heal)
        {
            
            if (skill.GetHealType() == HealType.Heal)
            {
                int heal = (int)maxHpValue * skill.GetPercentHeal(hpValue / maxHpValue) / 100;
                if (skill.targetType == TargetType.Self)
                { 
                    //if(heal == maxHpValue)
                    if (hpValue + heal > maxHpValue) return false;
                    else return true;
                }
                else
                {
                    if (turnManager == null) Debug.LogError("Null");
                    if (turnManager.CheckHpEnemy(heal)) return true;
                    else return false;
                }
                
            }
            else if (skill.GetHealType() == HealType.RemoveDebuff)
            {
                if (gaugeHP.GetAmountBuffDebuff(StatusType.Debuff) == 0) return false;
                else return true;
            }
            else
            {
                if (gaugeHP.GetStatuswithStatus(skill.GetRemoveDebuff()) == null) return false;
                else return true;
            }
        }else if(skill.GetSkillType() == SkillType.Debuff)
        {
            if (skill.GetCheckRemoveBuff())
            {
                if (turnManager.player.gaugeHp.GetAmountBuffDebuff(StatusType.Buff) == 0) return false;
            }
        }else if(skill.GetSkillType() == SkillType.Wait)
        {
            if(isWait) return false;
            //if(currentSkill != null)
            //    if (currentSkill.GetSkillType() == SkillType.Wait) return false;
        }else if(skill.GetSkillType() == SkillType.Attack)
        {
            if(currentSkill != null)
                if(currentSkill.id == skill.id) return false;
        }else if (skill.GetSkillType() == SkillType.Summon)
        {
            if(turnManager.enemies.Count == 3 || minion == null) return false;
        }
        Debug.Log("Check");
        return true;
    }

    int heal = 0;
    private void AttackSkill(SkillAction skill)
    {
        animationAction.AttackAction();
        StatusWidget widget = new StatusWidget();
        float ratio = hpValue / maxHpValue;
        for (int i = 0; i < skill.GetPercentDamage().Count; i++)
        {
            int percentSkill = skill.GetPercentDamage()[i] + 
                skill.GetDamageSpecific().CheckStatus(turnManager.player.gaugeHp.CheckSameStatus(skill.GetDamageSpecific().damageForEffect)) +
                skill.GetHpPercentSkill(ratio);

            int damage = GameManager.instance.damageCalculator.DamageResult((int)atkValue,percentSkill,
                                            (int)turnManager.player.GetPlayerDef(), GetDamageBonus(), turnManager.player.GetDamageReduce());
            if (gaugeHP.CheckStatuswithID("BN002"))
                damage *= 2;
            turnManager.player.StartDamageTaken(damage, i * 0.2f + animationAction.attackTime, skill.particleEffect);
            if (turnManager.player.hpValue <= 0) return;

            if (turnManager.player.gaugeHp.CheckStatuswithID("BD001", ref widget))
                StartDamageTaken(widget.GetStatus().damage, i * 0.2f + turnManager.player.animationAction.attackTime + 0.2f);

            if (skill.GetIsHeal())
            {
                if (skill.GetHealType() == HealType.Heal)
                {
                    heal = (damage * skill.GetPercentHeal(ratio)) / 100;
                    StartHealHP(heal, i);
                }
                else if (skill.GetHealType() == HealType.RemoveDebuff && i == 0)
                {
                    GameManager.instance.statusEffectSystem.RemoveDebuff(null, this);
                }
                else if (skill.GetHealType() == HealType.RemoveSpecific && i == 0)
                {
                    GameManager.instance.statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff(), this);
                }
            }

            if (skill.GetStatusTarget().Count > 0)
                foreach (var status in skill.GetStatusTarget())
                    GameManager.instance.statusEffectSystem.GetStatusInPlayer(status);
        }

        if (skill.GetStatusSelf().Count > 0)
            foreach (var status in skill.GetStatusSelf())
                GameManager.instance.statusEffectSystem.GetStatusInPlayer(status);

        if (skill.GetCheckHpCompare())
            if (skill.GetHpAbility().type == SkillType.Buff)
                statusEffectSystem.GetStatusInEnemy(this, skill.GetHpStatus(ratio));
    }
    private void DebuffSkill(SkillAction skill)
    {
        animationAction.AttackAction();
        if (skill.GetCheckRemoveBuff())
        {
            statusEffectSystem.RemoveBuff(skill.GetRemoveEffectType());
        }
        else
        {
            foreach (var status in skill.GetStatusTarget())
            {
                statusEffectSystem.GetStatusInPlayer(status);
            }
        }
    }

    private void BuffSkill(SkillAction skill)
    {
        float ratio = turnManager.player.hpValue / turnManager.player.maxHpValue;
        animationAction.BuffAction();
        foreach (var status in skill.GetStatusSelf())
        {
            if(skill.targetType == TargetType.Team)
            {
                foreach(var enemy in turnManager.enemies)
                    statusEffectSystem.GetStatusInEnemy(enemy, status);
            }else if(skill.targetType == TargetType.Self)
            {
                statusEffectSystem.GetStatusInEnemy(this, status);
            }
                
        }

        if (skill.GetCheckHpCompare())
            if (skill.GetHpAbility().type == SkillType.Buff)
                statusEffectSystem.GetStatusInEnemy(this, skill.GetHpStatus(ratio));
    }

    private void HealSkill(SkillAction skill)
    {
        float ratio = 0;
        animationAction.BuffAction();
        if (skill.GetHealType() == HealType.Heal)
        {
            ratio = hpValue / maxHpValue;
            if (skill.targetType == TargetType.Team)
            {
                foreach (var enemy in turnManager.enemies)
                {
                    heal = ((int)enemy.maxHpValue * skill.GetPercentHeal(ratio)) / 100;
                    enemy.StartHealHP(heal, 0);
                }
            }
            else if (skill.targetType == TargetType.Self)
            {
                heal = ((int)maxHpValue * skill.GetPercentHeal(ratio)) / 100;
                StartHealHP(heal, 0);
            }
            else if(skill.targetType == TargetType.SingleTarget)
            {
                EnemyController lowest = turnManager.GetEnemyLowestHP();
                heal = ((int)lowest.maxHpValue * skill.GetPercentHeal(ratio)) / 100;
                lowest.StartHealHP(heal, 0);
            }
        }
        else if (skill.GetHealType() == HealType.RemoveDebuff)
        {
            if (skill.targetType == TargetType.Team)
            {
                foreach (var enemy in turnManager.enemies)
                    statusEffectSystem.RemoveDebuff(null, enemy);
            }
            else if (skill.targetType == TargetType.Self)
            {
                statusEffectSystem.RemoveDebuff(null, this);
            }
        }
        else
        {
            if (skill.targetType == TargetType.Team)
            {
                foreach (var enemy in turnManager.enemies)
                    statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff(), enemy);
            }
            else if (skill.targetType == TargetType.Self)
            {
                statusEffectSystem.RemoveDebuff(skill.GetRemoveDebuff(), this);
            }
        }
    }
    #endregion
    void DestroySelf(float delay = 0.5f)
    {
        if (GameManager.instance.playerData.CheckAlreadyHaveRelic("RN010"))
        {
            statusEffectSystem.GetStatusInPlayer(GameManager.instance.allData.GetStatusWithID("BN002"));
        }

        GameManager.instance.turnManager.RemoveEnemy(this);
        //GameManager.instance.skillOrderSystem.RemoveEnemyinSlot(this);
        animationAction.DieAction(() =>
        {
            GameManager.instance.encounterManagementSystem.AddDrop(RandomSkillDrop(), RandomRelicDrop(), gold);
            Destroy(gaugeHP.gameObject, delay);
            
            Destroy(gameObject, delay);
        });
        
    }

    public Transform GetGuagePos() => gaugePos;
    public void SetGaugePos(Transform pos) => gaugePos = pos;
    public List<StatusWidget> GetStatusWidgets() => gaugeHP.statusWidgets;

    private SkillAction RandomSkillDrop()
    {
        if (dropSkills.Count == 0) return null;
        SkillAction skill = new SkillAction();
        //int num = 0;
        //do
        //{
        //    skill = dropSkills[Random.Range(0, dropSkills.Count)];
        //    if (num == dropSkills.Count) return null;
        //    num++;
        //} while (GameManager.instance.playerData.CheckAlreadyHaveSkill(skill.id));
        skill = dropSkills[Random.Range(0, dropSkills.Count)];

        return skill;
    }

    private Relic RandomRelicDrop()
    {
        if(dropRelics.Count == 0) return null;
        Relic relic = new Relic();
        int num = 0;
        do
        {
            relic = dropRelics[Random.Range(0, dropRelics.Count)];
            if (num == dropSkills.Count) return null;
            num++;
        } while (GameManager.instance.playerData.CheckAlreadyHaveRelic(relic.id));

        return relic;
    }
}
