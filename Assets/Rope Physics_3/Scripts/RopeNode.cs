using System;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;


namespace RopePhysics_3
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeNode : MonoBehaviour
    {
        [SerializeField] private float _targetDistance = 1f; // оптимальная дистанция между нодами
        [SerializeField] private float _connectionForce = 1000f; // сила соединения, влияет на скорость восстановления растояния и на взаимодействие с другими физ объектами
        [SerializeField] private float maxVelocity;
        [SerializeField] private bool clampVelocity;
        [SerializeField] private bool _drawGizmos;
        
        public RopeNode _prevNode;
        public RopeNode _nextNode;
        public Rigidbody Body => _body;
        public bool IsKinematic => _body.isKinematic;

        
        
        private Rigidbody _body;

        private Vector3 _cachedTargetOffset;
        private Vector3 _nextNodeRelativePosition;
        private Vector3 _previewNodeRelativePosition;
        private float _prevDot;
        [Range(0f,1f)]
        [SerializeField] private float _drag;
        private Quaternion _facing;
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.useGravity = false;
            // выключем гравитацию и считаем ее сами, тк физ движок постоянно аплаит ее после физ апдэйта (почему-то)
            _facing = transform.rotation * Quaternion.Euler(new Vector3(-90,0,0));
        }

        public bool CalculateRotations;

        [SerializeField] private Vector3 _upDirection;
        private void FixedUpdate()
        {
            if (_body.isKinematic) return;

            int nodesCount = 0;
             
            var targetOffset = Vector3.zero; // офсет до точки "спокойствия"

            if (_prevNode != null)
            {
                 _previewNodeRelativePosition = GetTargetRelativePos(_prevNode);
                targetOffset += _previewNodeRelativePosition;
                nodesCount++;
            }

            if (_nextNode != null)
            {
                 _nextNodeRelativePosition = GetTargetRelativePos(_nextNode);
                targetOffset += _nextNodeRelativePosition;
                nodesCount++;
            }

            targetOffset /= nodesCount; // получаем среднюю точку офсета
            _cachedTargetOffset = targetOffset;
            
            Vector3 targetVelocity = targetOffset.normalized * _connectionForce; // получаем скорость в направлении рест поинта
            float frameDelta = targetVelocity.magnitude * Time.fixedDeltaTime; // получаем расстояние, на которое толкнем ноду за след физ апдейт
            float deltaMagnitude = targetOffset.magnitude; // расстояние до таргет поинта

            if (frameDelta > deltaMagnitude) // если дельта больше расстояния, то домножаем скорость на относительное значение, что бы получить скорость для точного прирощения позиции
            {
                targetVelocity *= deltaMagnitude / frameDelta;
            }

            
            _body.AddForce(Physics.gravity, ForceMode.Acceleration); 
            // применяем гравитацию
            var dot =  Vector3.Dot(targetOffset, _body.velocity);

            float multiplier = 1.0f - _drag * Time.fixedDeltaTime;
            if (multiplier < 0.0f) multiplier = 0.0f;
            
            var targetVelocityNormalized = targetVelocity - targetVelocity.normalized * dot;

            OverrideTransform dwcqw;
            //dwcqw.data.
            
            _body.velocity += targetVelocityNormalized; // высчитываем все противолежащие нашему направлению офсета силы, когда созванивались, я это объяснял
            if (clampVelocity)
            {
                _body.velocity = Vector3.ClampMagnitude(_body.velocity, maxVelocity); // клэмпим)
            }

            _body.velocity *= multiplier;
            _body.angularVelocity = Vector3.zero;
        }

        private void Update()
        {
            if (_nextNode != null && CalculateRotations)
            {
                var rotation = Quaternion.LookRotation((_nextNode.transform.position - transform.position).normalized,_upDirection);
                rotation *= _facing;

                transform.rotation = rotation;
            }
        }


       

        //этот метод просто возвращает рест поинт относительно другой ноды (направление от одной к другой нормализованное, умноженное на таргет дистанс)
        private Vector3 GetTargetRelativePos(RopeNode node)
        {
            Vector3 position = transform.position;
            Vector3 targetNodePosition = node.transform.position;

            Vector3 relativeDirection = position - targetNodePosition;
            Vector3 targetGlobalPos = targetNodePosition + relativeDirection.normalized * _targetDistance;

            return targetGlobalPos - position;
        }

        
        
        
        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos == false)
            {
                return;
            }
            
            Gizmos.color = Color.blue;
            
            Gizmos.DrawLine(transform.position,transform.position+ _previewNodeRelativePosition);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position,transform.position+ _nextNodeRelativePosition);
            
            Gizmos.color = Color.green;
            
            
            Gizmos.DrawSphere(transform.position + _cachedTargetOffset, .5f);

            
            Gizmos.DrawRay(transform.position, transform.up     * -10);

            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=blue>{transform.rotation.eulerAngles}</color>");

            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>{transform.rotation}</color>");

        }
    }
    
}