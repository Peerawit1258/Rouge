using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [TabGroup("System")] public TurnManager turnManager;
    [TabGroup("System")] public DamageCalculator damageCalculator;
    [TabGroup("System")] public SkillOrderSystem skillOrderSystem;
    [TabGroup("System")] public EnemyTurnSystem enemyTurnSystem;
    [TabGroup("System")] public NumberDamageSystem numberDamageSystem;
    [TabGroup("System")] public StatusEffectSystem statusEffectSystem;
    [TabGroup("System")] public EncounterManagementSystem encounterManagementSystem;
    [TabGroup("System")] public AnimationChangeSceneSystem animationChangeSceneSystem;
    [TabGroup("System")] public RelicManagerSystem relicManagerSystem;
    [TabGroup("Battle")] public BattleSetup battleSetup;
    [TabGroup("Battle")] public GaugeHpEnemy gaugeHpEnemy;
    [TabGroup("UI")] public ResultBattle resultBattle;
    [TabGroup("UI")] public EventManager eventManager;
    [TabGroup("UI")] public DetailPanel detailPanel;
    [TabGroup("UI")] public InventoryManager inventoryManager;
    [TabGroup("UI")] public ShopSystem shopSystem;
    [TabGroup("UI")] public UpgradeSystem upgradeSystem;
    [TabGroup("UI")] public SkillDesc skillDesc;
    [TabGroup("Manager")] public AllData allData;
    [TabGroup("Manager")] public PlayerData playerData;
    [TabGroup("Test")] public List<SkillAction> skills;
    [TabGroup("Test")] public List<Relic> relics;

    public CameraControl mainCamera;
    [SerializeField] CharacterDetail playerDetail;
    [SerializeField] public EnemyGroup group ;
    [SerializeField] public bool isTest;
    [SerializeField] public bool isEvent;
    [SerializeField, ShowIf("@isEvent")] EventInfo eventTest;

    public static GameManager instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupBattleStage();
        List<SkillAction> currentSkill = new List<SkillAction>();
        List<Relic> currentRelic = new List<Relic>();
        if (isTest)
        {
            foreach (SkillAction action in skills)
                currentSkill.Add(action);

            foreach (Relic relic in relics)
                currentRelic.Add(relic);
        }
        else
        {
            currentSkill.Add(allData.GetSkillWithID("SA001"));
            currentSkill.Add(allData.GetSkillWithID("SB002"));
            currentSkill.Add(allData.GetSkillWithID("SH001"));
            currentSkill.Add(allData.GetSkillWithID("SA005"));
            currentSkill.Add(allData.GetSkillWithID("SB001"));

            if(relics.Count > 0)
                foreach (Relic relic in relics)
                    currentRelic.Add(relic);
        }

        if (currentSkill.Count > 0)
            inventoryManager.CreateSkillShow(currentSkill);
        if(currentRelic.Count > 0)
        {
            relicManagerSystem.AddRelic(currentRelic);
            //foreach (var info in currentRelic)
            //    detailPanel.CreateInfo(info);
        }
            
        
        
        //Debug.Log(t);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupBattleStage()
    {
        if (isTest)
        {
            encounterManagementSystem.stageName = encounterManagementSystem.stageDetail[0].stageName;
            encounterManagementSystem.IncreaseStageCount();
            encounterManagementSystem.stageDetail[0].encounterMap.SetActive(true);
            if (turnManager.player == null) battleSetup.SetupPlayerBattle(playerDetail);
            battleSetup.SetupEnemyBattle(encounterManagementSystem.stageDetail[0].startNode.enemyGroup.enemies);
            encounterManagementSystem.GetPreviousNode().Add(encounterManagementSystem.stageDetail[0].startNode.nodeID);
            detailPanel.SetEncounter(encounterManagementSystem.stageCount);

            DOVirtual.DelayedCall(1,() =>
            {
                Debug.Log("Create");
                turnManager.StartTurn();
            });
            return;
        }
        else
        {
            encounterManagementSystem.stageName = encounterManagementSystem.stageDetail[0].stageName;
            encounterManagementSystem.IncreaseStageCount();
            encounterManagementSystem.stageDetail[0].encounterMap.SetActive(true);
            if (turnManager.player == null) battleSetup.SetupPlayerBattle(playerDetail);
            if (isEvent)
            {
                
                DOVirtual.DelayedCall(1, () =>
                {
                    eventManager.EventSetup(eventTest);
                });
            }
            else
            {
                battleSetup.SetupEnemyBattle(encounterManagementSystem.stageDetail[0].startNode.enemyGroup.enemies);
                encounterManagementSystem.GetPreviousNode().Add(encounterManagementSystem.stageDetail[0].startNode.nodeID);
                detailPanel.SetEncounter(encounterManagementSystem.stageCount);

                DOVirtual.DelayedCall(1, () =>
                {
                    Debug.Log("Create");
                    turnManager.StartTurn();
                });
            }
            
        }
    }

}
