using Dreamteck.Splines;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCluster : MonoBehaviour
{
    public static PlayerCluster Instance;
    public GameObject unitPrefab;

    public int unitsCount = 10;

    public List<Transform> unitTransformList;
    public List<SingleUnit> unitDataList;


    private SplineFollower splineFollower;
    private bool isStarted = false;

    public static event Action OnPlayerDeath;

    private void Awake()
    {
        Instance = this;
        splineFollower = GetComponent<SplineFollower>();
    }
    private void Start()
    {
        unitTransformList = new();
        unitDataList = new();
        StartFormation();
        OnScreenDraw.OnNewShapeDraw += OnScreenDraw_OnNewShapeDraw;
    }

    private void OnScreenDraw_OnNewShapeDraw(Vector3[] obj)
    {
        if (!isStarted)
        {
            foreach (Transform t in unitTransformList)
            {
                t.GetComponent<Animator>().SetTrigger("isRun");
            }
            splineFollower.follow = true;
            isStarted = true;
        }
        UpdateFormation(CurvePointsToFormationPoints(obj));
    }

    private Vector3[] CurvePointsToFormationPoints(Vector3[] curve)
    {
        Vector3[] formation = new Vector3[curve.Length];
        for (int i = 0; i < curve.Length; i++)
        {
            formation[i] = new Vector3(curve[i].x, 0, curve[i].y);
        }
        return formation;
    }

    public void UpdateFormation(Vector3[] formation)
    {
        int delta = Mathf.CeilToInt((float)formation.Length / (float)unitTransformList.Count);
        int index = 0;
        for (int i = 0; i < unitTransformList.Count; i++)
        {
            if (index >= formation.Length)
            {
                index = 0;
            }
            unitDataList[i].MoveToPosition(formation[index]);
            index += delta;
        }
    }
    public void StartFormation()
    {
        for (int i = 0; i < unitsCount; i++)
        {
            Transform m = Instantiate(unitPrefab, transform).transform;
            m.gameObject.name = "PlayerUnit";
            int row = i / 7;
            int column = i % 7;
            float x = column * 0.2f;
            float z = row * 0.2f;
            m.position = new Vector3(transform.position.x -0.6f + x, transform.position.y, transform.position.z - z);

            unitTransformList.Add(m);
            unitDataList.Add(m.GetComponent<SingleUnit>());
        }
    }
    internal void AddUnit(Vector3 position)
    {
        Transform m = Instantiate(unitPrefab, transform).transform;
        SingleUnit sm = m.GetComponent<SingleUnit>();
        m.position = position;
        unitTransformList.Add(m);

        unitDataList.Add(sm);

        sm.animator.SetTrigger("isRun");
        unitsCount++;
    }
    internal void KillUnit(SingleUnit singleUnit)
    {
        if(!unitDataList.Contains(singleUnit)) return;

        unitsCount--;
        if (unitsCount > 0)
        {
            unitDataList.Remove(singleUnit);
            unitTransformList.Remove(singleUnit.transform);
            singleUnit.transform.SetParent(null);
        }
        else
        {
            TriggerGameOver();
        }

    }

    private void TriggerGameOver()
    {
        splineFollower.follow = false;
        OnPlayerDeath?.Invoke();
    }
}
