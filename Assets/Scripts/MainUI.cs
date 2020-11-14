using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    [SerializeField] Text _totalEnemy ;
    
    public void SetTotalEnemyValue(int value)
    {
        _totalEnemy.text = value.ToString();
    }
}
