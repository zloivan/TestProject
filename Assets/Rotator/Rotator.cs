using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Rotator : MonoBehaviour
{

    [SerializeField] private Transform _bodyToRotate;
    [SerializeField] private Transform _controllingBody;
    [SerializeField] private Vector3 _controllingVector;
    [SerializeField] private bool _traceBody;
    [ShowIf("_traceBody")]
    [SerializeField] private Transform _tracableBody;
    
    [SerializeField] private Vector3 _quaternion;
    private bool _isRotatedBody;

    private Quaternion _faceing;
    private void Start()
    {
        if (_controllingBody != null)
        {
            _isRotatedBody = true;
        }

        if (_tracableBody == null)
        {
            _traceBody = false;
        }
        
        
        _faceing = Quaternion.Euler(0,0,1);
        
        if (_quaternion != Vector3.zero)
        {
            _faceing *= Quaternion.Euler(_quaternion);
        }
    }

    private void Update()
    {
        Vector3 direction;
        
        if (_isRotatedBody && _traceBody == false)
        {
            direction = _controllingBody.up;
        }
        else
        {
            if (_isRotatedBody && _traceBody == false)
            {
                direction = _controllingVector;
            }
            else
            {
                direction = _tracableBody.position - transform.position;
            }
            
        }



        var rotation = Quaternion.LookRotation(direction.normalized);

        rotation *= _faceing;
        _bodyToRotate.rotation = rotation;
    }
}
