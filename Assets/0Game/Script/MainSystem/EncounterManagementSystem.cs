using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

public class EncounterManagementSystem : SerializedMonoBehaviour
{
    //[TabGroup("Value"), SerializeField] Dictionary<string, int> stage = new Dictionary<string, int>();
    public List<StageDetail> stageDetail = new List<StageDetail>();
    [ReadOnly, TabGroup("Value")] public string stageName;
    [ReadOnly, TabGroup("Value")] public int stageCount = 0;
    [ReadOnly, TabGroup("Value")] public int allCount = 0;

    [TabGroup("Reward")]public List<SkillAction> skillDrops;
    [TabGroup("Reward")]public List<Relic> relicDrops;
    [TabGroup("Reward")] public int gold;

    [TabGroup("Set")] public int maxDelayShop;
    [TabGroup("Set")] public int maxDelayRest;
    [TabGroup("Set")] public List<Node> nodes;

    [SerializeField] List<DoorNode> doors;
    [SerializeField, ReadOnly] List<string> previousNode;
    public List<string> GetPreviousNode() => previousNode;

    TurnManager turnManager;
    // Start is called before the first frame update
    void Start()
    {
        turnManager = GameManager.instance.turnManager;
    }

    #region Drop
    public void AddDrop(SkillAction skill, Relic relic, int value)
    {
        if (skill != null)
        {
            if (!CheckAlreadySkillDrop(skill))
                skillDrops.Add(skill);
        }
        if(relic != null)
        {
            if(!CheckAlreadyRelicDrop(relic))
                relicDrops.Add(relic);
        }

        gold += value;
    }


    public bool CheckAlreadySkillDrop(SkillAction skill)
    {
        foreach (var sk in skillDrops)
        {
            if (sk.id == skill.id)
                return true;
        }

        return false;
    }

    public bool CheckAlreadyRelicDrop(Relic relic)
    {
        foreach (var rl in relicDrops)
        {
            if (rl.id == relic.id)
                return true;
        }

        return false;
    }
    #endregion
    #region Door
    public void CreateNextDoorNode()
    {
        StageDetail current = GetStageDetailwithName();
        
        if(stageCount == current.stageCount - 1)// Boss Stage
        {
            doors[1].DoorSetup(GetStageDetailwithName().bossNode);
            
        }
        else if (stageCount == current.stageCount - 2)// Before Boss
        {
            doors[1].DoorSetup(GetStageDetailwithName().shopNode[0]);
            doors[2].DoorSetup(GetStageDetailwithName().shopNode[0]);
        }
        else if (stageCount == current.stageCount)// Change map
        {
            doors[1].DoorSetup(null);
        }
        else
        {
            int num = Random.Range(2, 4);
            EncounterNode newNode = new EncounterNode();
            List<EncounterNode> lists = new List<EncounterNode>();
            for (int i = 0; i < num; i++)
            {
                lists = GetStageDetailwithName().GetEncounterList(GetNodeType());
                if(lists.Count > 1)
                {
                    do
                    {
                        newNode = lists[Random.Range(0, lists.Count)];
                    } while (CheckAlreadyHaveEncounter(newNode));
                }else
                    newNode = lists[0];


                doors[i].DoorSetup(newNode);
            }
        }
    }

    private Node GetNodeType()
    {
        Node node = Node.Normal;
        do
        {
            node = nodes[Random.Range(0, nodes.Count)];
        } while (CheckDoorNodewithType(node));
        return node;
    }

    private bool CheckDoorNodewithType(Node type)
    {
        if (type == Node.Normal && !GameManager.instance.isTest) return false;
        foreach (var door in doors)
        {
            if (door.GetEncounter() != null)
                if (door.GetEncounter().node == type)
                    return true;
        }

        if (type == Node.Shop)// Check Cooldown Shop
        {
            if (delayShop > 0)
                return true;
            else
                delayShop = maxDelayShop;
        }
        else if (type == Node.Rest) // Check Cooldown Rest
        {
            if (delayRest > 0)
                return true;
            else
                delayRest = maxDelayRest;
        }

        return false;
    }
    int delayShop = 0;
    int delayRest = 0;
    private bool CheckAlreadyHaveEncounter(EncounterNode node)
    {
        int count = 0;
        if (node.node == Node.Event) // Check Event
        {
            if (node.eventInfo.unlockEvent.Count > 0)
            {
                foreach (string id in node.eventInfo.unlockEvent)
                    if (previousNode.Contains(id))
                        count++;
                if (count == node.eventInfo.unlockEvent.Count)
                    return true;
                else
                    return false;
            }
        }
        else if (node.node == Node.Shop)// Check Cooldown Shop
        {
            if (delayShop > 0)
                return true;
            else
                delayShop = maxDelayShop;
        }
        else if (node.node == Node.Rest) // Check Cooldown Rest
        {
            if (delayRest > 0)
                return true;
            else
                delayRest = maxDelayRest;
        }

            

        foreach (var door in doors)
            if (door.GetEncounter() != null)
                if (door.GetEncounter().nodeID == node.nodeID)
                    return true;

        if (previousNode.Contains(node.nodeID))
            if(node.node != Node.Treasure && node.node != Node.Shop && node.node != Node.Rest)
                return true;


        return false;
    }

    public void SetActiveDoor(bool active)
    {
        foreach(var door in doors)
            door.gameObject.SetActive(active);
    }

    public void ClearDoorNode()
    {
        foreach (var door in doors)
            if (door.GetEncounter() != null)
                door.ClearDoor();
    }
    #endregion
    #region Stage
    [Button]
    public void StartSetupNextEncounter(EncounterNode node = null)
    {
        IncreaseStageCount();
        if (node == null)
        {
            if(stageName != "Final")
            {
                delayShop = 0;
                delayRest = 0;
                ChangeStage();
            }
        }
        else
        {
            if (delayShop > 0) delayShop--;
            if (delayRest > 0) delayRest--;
            GameManager.instance.animationChangeSceneSystem.BlackFade(1, 1, () =>
            {
                turnManager.player.animationAction.WalkInSceneAction();
                if (node.node == Node.Normal || node.node == Node.Elite || node.node == Node.Boss)
                {
                    GameManager.instance.battleSetup.SetupEnemyBattle(node.enemyGroup.enemies);
                    DOVirtual.DelayedCall(1, () =>
                    {
                        Debug.Log("Create");
                        GameManager.instance.turnManager.StartTurn();
                    });
                }
                else if(node.node == Node.Event)
                {
                    StartCoroutine(SetupEvent(node));
                }
                else if(node.node == Node.Treasure)
                {
                    //GameManager.instance.battleSetup.SetupSpecialEnemyBattle(node.chest, 1);EnemyController enemy;
                    Instantiate(node.chest.character, GameManager.instance.battleSetup.GetEnemyPos()[1]);
                    
                }
                else if(node.node == Node.Shop)
                {
                    GameManager.instance.shopSystem.OpenShop();
                }
                else if(node.node == Node.Rest)
                {
                    int heal = (int)Mathf.Floor(turnManager.player.maxHpValue * 25 / 100);
                    turnManager.player.StartHealHP(heal, 1);
                    GameManager.instance.upgradeSystem.ActiveUpgradeUI();
                    //CreateNextDoorNode();
                }

                previousNode.Add(node.nodeID);
                SetActiveDoor(false);
            }); 
        }
    }

    public void IncreaseStageCount()
    {
        stageCount++;
        allCount++;
        GameManager.instance.detailPanel.SetEncounter(allCount);
    }

    void ChangeStage()
    {
        for (int i = 0; i < stageDetail.Count; i++)
        {
            if (stageDetail[i].stageName == stageName)
            {
                if (i + 1 >= stageDetail.Count) return;
                stageDetail[i].encounterMap.SetActive(false);
                stageDetail[i + 1].encounterMap.SetActive(true);
                stageName = stageDetail[i + 1].stageName;
                stageCount = 1;
            }

        }
    }

    int GetCurrentStageCountwithName()
    {
        for(int i = 0; i < stageDetail.Count; i++)
        {
            if(stageDetail[i].stageName == stageName)
                return stageDetail[i].stageCount;
        }
        return 0;
    }

    StageDetail GetStageDetailwithName()
    {
        for (int i = 0; i < stageDetail.Count; i++)
        {
            if (stageDetail[i].stageName == stageName)
                return stageDetail[i];
        }
        return null;
    }
    #endregion

    IEnumerator SetupEvent(EncounterNode node)
    {
        GameManager.instance.battleSetup.eventSymbol.SetActive(true);
        //turnManager.player.animationAction.WalkInSceneAction();
        yield return new WaitUntil(()=>!turnManager.player.animationAction.isAction);
        yield return new WaitForSeconds(1);
        
        GameManager.instance.eventManager.EventSetup(node.eventInfo);

    }

    public void ResetValue()
    {
        skillDrops.Clear();
        relicDrops.Clear();
        gold = 0;
    }

    [Button]
    public void SetAllIDNode()
    {
        foreach(var stage in stageDetail)
        {
            foreach (var node in stage.normalNode)
                node.SetID();
            foreach (var node in stage.eliteNode)
                node.SetID();
            foreach (var node in stage.eventNode)
                node.SetID();
            foreach (var node in stage.treasureNode)
                node.SetID();
            
            if(stage.bossNode != null) stage.bossNode.SetID();
        }
    }
}

[Serializable]
public class StageDetail
{
    public GameObject encounterMap;
    public string stageName;
    [TabGroup("Detail", "Detail")]public int stageCount;
    [TabGroup("Detail", "Detail")]public EncounterNode startNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> normalNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> eliteNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> eventNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> treasureNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> shopNode;
    [TabGroup("Detail", "Detail")]public List<EncounterNode> restNode;
    [TabGroup("Detail", "Detail")]public EncounterNode bossNode;

    public List<EncounterNode> GetEncounterList(Node type)
    {
        if (type == Node.Treasure) return treasureNode;
        else if (type == Node.Elite) return eliteNode;
        else if (type == Node.Event) return eventNode;
        else if (type == Node.Shop) return shopNode;
        else if (type == Node.Rest) return restNode;
        else return normalNode;
    }

}