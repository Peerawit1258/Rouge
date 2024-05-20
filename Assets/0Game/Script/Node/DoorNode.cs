using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class DoorNode : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Animator door;
    [SerializeField, ReadOnly] EncounterNode encounter;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnMouseUp()
    {
        Debug.Log(name);
        if (encounter == null) return;
        door.SetTrigger("Open");
        GameManager.instance.encounterManagementSystem.StartSetupNextEncounter(encounter);
        //GameManager.instance.battleSetup.SetupEnemyBattle(GameManager.instance.group.enemies);

        
        return;
    }

    public void DoorSetup(EncounterNode node)
    {
        if(node == null) return;
        door.SetTrigger("Close");
        sprite.sprite = node.icon;
        encounter = node;
        gameObject.SetActive(true);
    }

    public EncounterNode GetEncounter() => encounter;
}
