using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CameraControl : MonoBehaviour
{
    [SerializeField] Transform cameraPos;
    [SerializeField] float duration;
    [SerializeField] float strength;
    [SerializeField] int vibrato;
    [SerializeField] float random;


    Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Button]
    public void CameraShake()
    {
        cameraPos.DOShakePosition(duration, strength, vibrato, random, false, true, ShakeRandomnessMode.Harmonic).OnComplete(() =>
        {
            cameraPos.DOMove(origin, 0.1f);
        });
          
    }
}
