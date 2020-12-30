using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JobsMoveToClickPoint
{
    public class ClickArea : MonoBehaviour, IPointerClickHandler
    {
        private event Action<Vector3> _onClickHandler;

        public void OnPointerClick(PointerEventData eventData)
        {
            var ray = Camera.main.ScreenPointToRay(eventData.position);

            if (Physics.Raycast(ray, out var hit))
                _onClickHandler?.Invoke(hit.point);
        }

        public void RegisterListener(Action<Vector3> onClickAction) => 
            _onClickHandler += onClickAction;
    }
}