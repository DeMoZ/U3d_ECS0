using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] Text _totalEnemy;
    [SerializeField] Text _addBtn;

    public static MainUI instance;

    private int _totalValue = 0;

    private void Awake()
    {
        instance = this;
    }

    public void SetTotalEnemyValue(int value)
    {
        _totalValue = value;
        _totalEnemy.text = value.ToString();
    }

    internal void SetAddAmoutn(int manualSpawnAmount)
    {
        _addBtn.text = string.Concat("Add ", manualSpawnAmount);
    }

    internal void DecreaseEnemyValue(int amount)
    {
        _totalEnemy.text = (_totalValue - 1).ToString();
    }
}
