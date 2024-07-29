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

    bool canClick;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnMouseEnter()
    {
        if (!canClick) return;
        GameManager.instance.battleSetup.SetPositionArrow(transform);
    }

    public void OnMouseUp()
    {
        Debug.Log(encounter.nodeID);
        if (encounter == null || !canClick) return;
        GameManager.instance.battleSetup.arrow.gameObject.SetActive(false);
        door.SetTrigger("Open");
        GameManager.instance.encounterManagementSystem.StartSetupNextEncounter(encounter);
        GameManager.instance.encounterManagementSystem.ClearDoorNode();
        return;
    }

    public void DoorSetup(EncounterNode node)
    {
        if(node == null) return;
        door.SetTrigger("Close");
        sprite.sprite = node.icon;
        encounter = node;
        gameObject.SetActive(true);
        transform.DOMoveY(-0.9f, 0.5f).From().SetEase(Ease.InOutQuart).OnComplete(()=>canClick = true);
    }

    public void ClearDoor() => encounter = null;
    public void SetCanClick(bool active) => canClick = active;
    public EncounterNode GetEncounter() => encounter;
}
