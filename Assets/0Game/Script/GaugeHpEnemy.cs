using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeHpEnemy : MonoBehaviour
{
    public GameObject gaugeHp;
    public RectTransform parent;
    [SerializeField] GaugeHpWidget hpWidget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetPositionGauge(EnemyController enemy)
    {
        GaugeHpWidget gauge = Instantiate(gaugeHp, enemy.GetGuagePos().position, Quaternion.identity).GetComponent<GaugeHpWidget>();
        enemy.gaugeHP = gauge;
        gauge.gameObject.transform.parent = parent;
        gauge.name = "Gauge_" + enemy.name;
    }

    public void SetPlayerHpWidget(PlayerController player)
    {
        player.gaugeHp = hpWidget;
    }
}
