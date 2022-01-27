using System;
using Sirenix.Serialization;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace RopePhysics_3
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeNode : MonoBehaviour
    {
        [SerializeField] private float _targetDistance = 1f; // оптимальная дистанция между нодами

        [SerializeField] private float
            _connectionForce =
                1000f; // сила соединения, влияет на скорость восстановления растояния и на взаимодействие с другими физ объектами

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
        [Range(0f, 1f)] [SerializeField] private float _drag;
        private Quaternion _facing;
        [SerializeField] private Vector3 _facingCorrection;
        private Color _thisNodColor;

        [SerializeField] private Transform _bodyCollider;
        
        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            
            _localScale = _bodyCollider.localScale;
            
            _body.useGravity = false;
            // выключем гравитацию и считаем ее сами, тк физ движок постоянно аплаит ее после физ апдэйта (почему-то)

            _facing = Quaternion.Euler(Vector3.forward);
            //
            if (_facingCorrection != Vector3.zero)
            {
                _facing *= Quaternion.Euler(_facingCorrection);
            }

            _thisNodColor = Random.ColorHSV();
            
        }

        public bool CalculateRotations;


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

            Vector3 targetVelocity =
                targetOffset.normalized * _connectionForce; // получаем скорость в направлении рест поинта
            float frameDelta =
                targetVelocity.magnitude *
                Time.fixedDeltaTime; // получаем расстояние, на которое толкнем ноду за след физ апдейт
            float deltaMagnitude = targetOffset.magnitude; // расстояние до таргет поинта

            if (
                frameDelta >
                deltaMagnitude) // если дельта больше расстояния, то домножаем скорость на относительное значение, что бы получить скорость для точного прирощения позиции
            {
                targetVelocity *= deltaMagnitude / frameDelta;
            }


            _body.AddForce(Physics.gravity, ForceMode.Acceleration);
            // применяем гравитацию
            var dot = Vector3.Dot(targetOffset, _body.velocity);

            float multiplier = 1.0f - _drag * Time.fixedDeltaTime;
            if (multiplier < 0.0f) multiplier = 0.0f;

            var targetVelocityNormalized = targetVelocity - targetVelocity.normalized * dot;

            OverrideTransform dwcqw;
            //dwcqw.data.

            _body.velocity +=
                targetVelocityNormalized; // высчитываем все противолежащие нашему направлению офсета силы, когда созванивались, я это объяснял
            if (clampVelocity)
            {
                _body.velocity = Vector3.ClampMagnitude(_body.velocity, maxVelocity); // клэмпим)
            }

            _body.velocity *= multiplier;
            _body.angularVelocity = Vector3.zero;
        }

        private void Update()
        {
            if (_prevNode != null && CalculateRotations)
            {
                var pointDirection = _prevNode.transform.position - transform.position;

                Debug.DrawLine(transform.position,
                    transform.position + pointDirection.normalized * .2f,
                    _thisNodColor, 1f);

                var rotation = Quaternion.LookRotation(pointDirection.normalized,
                    GetUpVector(_nextNode, _prevNode));

                rotation *= _facing;
                transform.rotation = rotation;
            }


            if (_controllBodyColliderLength)
            {
                AdjustColliderLength(_prevNode,_nextNode);
            }
        }

        private Vector3 _localScale;
        private void AdjustColliderLength(RopeNode prevNode, RopeNode nextNode)
        {
            var transformPosition = transform.position;

            
            if (prevNode == null)
            {
                _localScale.x = (Vector3.Distance(nextNode.transform.position, transformPosition) / 4f) * _multiplier;
                _bodyCollider.localScale = _localScale;
                return;
            }
            

            if (nextNode == null)
            {
                _localScale.x = (Vector3.Distance(prevNode.transform.position, transformPosition) / 4f) * _multiplier;
                _bodyCollider.localScale = _localScale;
                 return;
            }
            

            var min = Mathf.Min(Vector3.Distance(prevNode.transform.position, transformPosition), 
                Vector3.Distance(nextNode.transform.position, transformPosition));
            
            _localScale.x = (min / 4f )*_multiplier;
            _bodyCollider.localScale = _localScale;
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

        [SerializeField] private float _multiplier;

        private void OnDrawGizmosSelected()
        {
            if (_nextNode != null)
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"Distance to next node <color=blue><b>{Vector3.Distance(_nextNode.transform.position,transform.position)}</b></color>");

            }
            
            if (_prevNode != null)
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"Distance to prev node <color=red><b>{Vector3.Distance(_prevNode.transform.position,transform.position)}</b></color>");

            }
            if (_drawGizmos == false)
            {
                return;
            }

            // Gizmos.color = Color.blue;
            //
            // Gizmos.DrawLine(transform.position, transform.position + _previewNodeRelativePosition);
            //
            // Gizmos.color = Color.cyan;
            // Gizmos.DrawLine(transform.position, transform.position + _nextNodeRelativePosition);
            //
            // Gizmos.color = Color.green;


            // Gizmos.DrawSphere(transform.position + _cachedTargetOffset, .5f);


            //Gizmos.DrawRay(transform.position, transform.up * -10);

            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=blue>{transform.rotation.eulerAngles}</color>");

            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>{transform.rotation}</color>");

            Vector3 upVector = GetUpVector(_nextNode, _prevNode);

            Gizmos.DrawLine(transform.position, upVector.normalized * 1f);
            
        }

        [SerializeField] private Vector3 _someVector;
        [SerializeField] private bool _controllBodyColliderLength;

        private Vector3 GetUpVector(RopeNode nextNode, RopeNode prevNode)
        {
            var thisNodePosition = transform.position;

            Vector3 nextNodeDirection, normalWithNext = Vector3.zero;
            Vector3 prevNodeDirection , normalWithPrev = Vector3.zero;

            if (nextNode != null)
            {
                nextNodeDirection = nextNode.transform.position - thisNodePosition;
                normalWithNext = Vector3.Cross(nextNodeDirection, Vector3.forward);
                //result = normalWithNext;
            }
            

            if (prevNode != null)
            {
                prevNodeDirection = prevNode.transform.position - thisNodePosition;
                normalWithPrev = Vector3.Cross(prevNodeDirection, Vector3.back);
                //result = normalWithPrev;
            }

            
            
            if (_drawGizmos)
            {
                Debug.DrawLine(thisNodePosition, thisNodePosition + normalWithNext, Color.red);
                Debug.DrawLine(thisNodePosition, thisNodePosition + normalWithPrev, Color.yellow);
            }

            var result = (normalWithNext + normalWithPrev) / 2;

            

            return result;
        }
    }
}