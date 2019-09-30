using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shootflow : MonoBehaviour {

    /*
     * look down to floor
     * look up to ceiling
     * look around to shoot walls
     */

    public DirectionIndicator FloorTargetIndicator;
    public GameObject[] FloorUIs;

    public DirectionIndicator CeilingTargetIndicator;
    public GameObject[] CeilingUIs;

    public GameObject[] WallUIs;

    void Start()
    {
        GetComponent<HelloHolo.ShootHolo>().HitAction += HitReceived;

        FloorTargetIndicator.GotInvisible += FloorReached;
        CeilingTargetIndicator.GotInvisible += CeilingReched;
        CeilingTargetIndicator.gameObject.SetActive(false);
    }

    private void HitReceived(RaycastHit hit)
    {
        if (FloorTargetIndicator.gameObject.activeSelf)
        {
            FloorReached();
        }
        else if (CeilingTargetIndicator.gameObject.activeSelf)
        {
            CeilingReched();
        }
        else
        {
            SetActive(WallUIs, false);
            GetComponent<HelloHolo.ShootHolo>().HitAction -= HitReceived;
        }
    }

    private void CeilingReched()
    {
        CeilingTargetIndicator.GotInvisible -= CeilingReched;
        CeilingTargetIndicator.gameObject.SetActive(false);
        SetActive(CeilingUIs, false);
        SetActive(WallUIs, true);
    }

    private void FloorReached()
    {
        FloorTargetIndicator.GotInvisible -= FloorReached;
        FloorTargetIndicator.gameObject.SetActive(false);
        SetActive(FloorUIs, false);

        CeilingTargetIndicator.gameObject.SetActive(true);
        SetActive(CeilingUIs, true);
    }

    void SetActive(GameObject[] elements, bool active)
    {
        foreach (var item in elements)
            item.gameObject.SetActive(active);
    }

}
