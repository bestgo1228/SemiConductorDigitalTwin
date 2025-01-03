using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ������ ���޵Ǹ� �Ǹ��� �ε尡 ���� �Ÿ���ŭ, ���� �ӵ��� ���� �Ǵ� �����Ѵ�.
/// ���� �Ǵ� ���� ��, ������ Limit Switch(LS)�� �۵��Ѵ�.
/// �Ӽ�: �Ǹ����ε�, Min-Max Range, Duration, ���Ĺ� Limit Switch
/// </summary>
public class SemiconMPSManager : MonoBehaviour
{
    [SerializeField] List<Transform> LithoDoor;
    [SerializeField] List<Transform> gateValveDoor;
    [SerializeField] float LithoMaxRange;
    [SerializeField] float LithoMinRange;
    [SerializeField] float GateValveMaxRange;
    [SerializeField] float GateValveMinRange;
    [SerializeField] float duration;

    public bool isUp = false;

    public int cycleCnt;
    public float cycleTime;

    private void Start()
    {

    }

    public void OnUpBtnClkEvent()
    {
        if (isUp) return;

        cycleCnt++;

        foreach (Transform l in LithoDoor)
        {
            StartCoroutine(MoveGate(l, LithoMinRange, LithoMaxRange, duration));
        }

        foreach (Transform gv in gateValveDoor)
        {
            StartCoroutine(MoveGate(gv, GateValveMinRange, GateValveMaxRange, duration));
        }

    }

    public void OnDownBtnClkEvent()
    {
        if (!isUp) return;

        foreach (Transform l in LithoDoor)
        {
            StartCoroutine(MoveGate(l, LithoMaxRange, LithoMinRange, duration));
        }

        foreach (Transform gv in gateValveDoor)
        {
            StartCoroutine(MoveGate(gv, GateValveMaxRange, GateValveMinRange, duration));
        }

    }

    IEnumerator MoveGate(Transform gate, float min, float max, float duration)
    {

        Vector3 minPos = new Vector3(gate.transform.localPosition.x, min, gate.transform.localPosition.z);
        Vector3 maxPos = new Vector3(gate.transform.localPosition.x, max, gate.transform.localPosition.z);

        float currentTime = 0;

        while (currentTime <= duration)
        {
            currentTime += Time.deltaTime;

            gate.localPosition = Vector3.Lerp(minPos, maxPos, currentTime / duration);

            cycleTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        isUp = !isUp;

    }

}

