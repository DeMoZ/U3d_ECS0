using System;
using UnityEngine;

namespace JobsMoveToClickPoint
{
    public class Clicker
    {
        private const string ClickerUiPath = "Clicker";

        private GameObject _clickUiPrefab;

        public Clicker(Action<Vector3> OnClickAction)
        {
            _clickUiPrefab = (GameObject) Resources.Load(ClickerUiPath);

            ClickArea clickArea = GameObject.Instantiate(_clickUiPrefab).GetComponentInChildren<ClickArea>();
            clickArea.RegisterListener(OnClickAction);
        }
    }
}