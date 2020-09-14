using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBeh : MonoBehaviour
{
    public static CameraBeh instance;

    [SerializeField] private float _speed = 3;

    [SerializeField] private bool _isInView = true;

    private Transform _transform;

    private Vector3 _center = Vector3.zero;

    private Camera _camera;

    private void Start()
    {
        instance = this;

        _transform = transform;
        _camera = GetComponent<Camera>();

        _transform.LookAt(_center);
    }

    void Update()
    {
        //if (_isInView)
        //{
        //    if ((_center - _transform.position).sqrMagnitude > 1)
        //        _transform.position += _transform.forward * _speed * Time.deltaTime;
        //}
        //else
        //{
        //    //_transform.position -= _transform.forward * _speed * Time.deltaTime;
        //    //_isInView = true;
        //}
    }

    public void CheckForViewPort(Vector3 pos)
    {
    }

    public void CheckForViewPort(Unity.Mathematics.float3 pos)
    {
        Vector3 screenPos = _camera.WorldToScreenPoint(pos);
        Rect rec = new Rect(0, 0, Screen.width, Screen.height);

        if (!rec.Contains(screenPos))
        {
            Vector3 nextPos = _transform.position - _transform.forward;
            _transform.position = Vector3.Slerp(_transform.position, nextPos, nextPos.magnitude * Time.deltaTime/10);
        }
    }
}
