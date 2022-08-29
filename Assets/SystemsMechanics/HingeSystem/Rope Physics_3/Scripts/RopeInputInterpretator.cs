using System;
using System.Collections.Generic;
using Services.Input;
using Services.Input.Data;
using UnityEngine;
using Zenject;

namespace RopePhysics_3
{
    public class RopeInputInterpretator : MonoBehaviour
    {

        [Inject] private IInputService _inputService;
        [SerializeField] private float _dragOffset;
        
         [SerializeField] private Camera  _main;
        //
        // [SerializeField] private RopeLengthLogic _lengthLogic;
        
        private void Awake()
        {
            if (_main == null)
            {
                _main = Camera.main;
            }
            
            _inputService.AddPointerDownListener(HandlePointerDown);
            _inputService.AddPointerMoveListener(HandlePointerMove);
            _inputService.AddPointerUpListener(HandlePointerUp);
            
            Transform t = transform;
            Vector3 forward = t.forward;
            _raycastPlane = new Plane(forward, t.position + forward * _dragOffset);
        }

        private void OnDestroy()
        {
            _inputService.RemovePointerDownListener(HandlePointerDown);
            _inputService.RemovePointerMoveListener(HandlePointerMove);
            _inputService.RemovePointerUpListener(HandlePointerUp);
        }

        private InputPointer _currentInput;
        private void HandlePointerUp(InputPointer obj)
        {
            // if (_wasInitiallyKinematic == false)
            // {
            //     _seceted.DisableKinematic();
            // }
            _seceted?.DisableKinematic();
            _seceted = null;
        }

        private Vector3 _pos;
        private void HandlePointerMove(InputPointer obj)
        {
            if (_seceted == null)
            {
                return;
            }


            var ray = _main.ScreenToWorldPoint(obj.Position);

            _pos.x = _initialPositions.x + _dragOffset;
            //_pos.x += _dragOffset;
            _pos.z = ray.z;
            _pos.y = ray.y;
            _seceted.transform.position = _pos;
            
            // var ray = _main.ScreenPointToRay(obj.Position);
            //
            // //_initialPositions[obj.Id] = transform.InverseTransformPoint(tt.point);
            //  displacement = GetDisplace(ray, obj.Id);
            //
            //
            // //mylogs Probably remove this later
            // if (Debug.isDebugBuild) Debug.Log($"<color=purple>Displacement {displacement}</color>");
            //_seceted.transform.position = displacement;
            //_seceted.transform.position = obj.

        }

        private RopeNode _seceted;
        private Plane _raycastPlane;
       // private Dictionary<int, Vector3> _initialPositions = new Dictionary<int, Vector3>();
        private Vector3 _initialPositions ;

        private void HandlePointerDown(InputPointer obj)
        {
            var ray = _main.ScreenPointToRay(obj.Position);
            
            Debug.DrawRay(ray.origin,ray.direction * 100, Color.green,1f);
            var dd = Physics.Raycast(ray  ,out RaycastHit  tt);
            
            if ( dd  && tt.collider.isTrigger) 
            {
               // _raycastPlane.Raycast(ray, out float distance);
                
                //_initialPositions[obj.Id] = transform.InverseTransformPoint(ray.GetPoint(distance));
                
                _seceted = tt.transform.GetComponent<RopeNode>();
                _initialPositions = tt.transform.position;
                _wasInitiallyKinematic =  _seceted.IsKinematic;
                _seceted.EnableKinematic();
                
                //_initialPositions[obj.Id] = obj.Position;
            }
        }

        private Vector3 planePoint, displace;

        // private RopeNode _selected;
        private bool _wasInitiallyKinematic;
        private Vector3 GetDisplace(Ray ray, int pointerId)
        {
            _raycastPlane.Raycast(ray, out float distance);

             planePoint = transform.InverseTransformPoint(ray.GetPoint(distance));
            
            
             //displace = planePoint - _initialPositions[pointerId];
                
            return displace;
        }

        [SerializeField] private float _gizmoRadius = .2f;
        private Vector3 displacement;

        // private void OnDrawGizmos()
        // {
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawSphere(planePoint, _gizmoRadius);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawSphere(displacement, _gizmoRadius);
        //     
        //    // Gizmos.DrawWireCube(_raycastPlane);
        // }
    }
    
    

}
