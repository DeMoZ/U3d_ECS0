using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] Text _addBtn;
    [SerializeField] Text _totalBots;
    [SerializeField] Text _totalBullets;
    [SerializeField] Text _totalEntities;

    public static MainUI instance;

    private void Awake()
    {
        instance = this;
    }

    internal void SetAddAmoutn(int manualSpawnAmount)
    {
        _addBtn.text = string.Concat("Add ", manualSpawnAmount);
    }

    public void SetValues(int bots, int bullets, int entities)
    {
        SetTotalEnemyValue(bots);
        _totalBullets.text = bullets.ToString();
        _totalEntities.text = entities.ToString();
    }
    public void SetTotalEnemyValue(int value)
    {
        _totalBots.text = value.ToString();
    }
    public void DebugLogFOrTest()
    {
        Debug.Log("LogWorks");
    }
}
