using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RopePhysics
{
    public class MeshDeformationInput : MonoBehaviour
    {
        [SerializeField] private float _pushForc = 10f;
    
        [SerializeField] private Camera _camera;
    
        [SerializeField] private float _forceOffset = .1f;
        // Start is called before the first frame update
        void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }
    
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
            }
            
        }
    
        private Vector3 _forcePoint;
        private void HandleInput()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
    
            if (Physics.Raycast(ray,  out RaycastHit  hit ))
            {
                var deformer = hit.collider.GetComponent<MeshDeformer>();
                
                if (deformer != null)
                {
                    var hitInfoPoint = hit.point;   
                    Debug.DrawLine(hitInfoPoint, hit.normal * 5, Color.green);
                    Debug.DrawLine(hitInfoPoint, hit.normal * -5, Color.red);
                    
                    hitInfoPoint += hit.normal * _forceOffset;
                    _forcePoint = hitInfoPoint;
                    
                    deformer.ApplyInput(hitInfoPoint , _pushForc);
                }
            }
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_forcePoint, _forceOffset);
        }
    }

}
