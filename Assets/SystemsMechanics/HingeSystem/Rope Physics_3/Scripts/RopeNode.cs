using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


namespace RopePhysics_3
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeNode : MonoBehaviour
    {
        [SerializeField] private NodeSettingsSO _settings;
        //[SerializeField] private float _targetDistance = 1f; // оптимальная дистанция между нодами

        // [SerializeField] private float
        //     _connectionForce =
        //         1000f; // сила соединения, влияет на скорость восстановления растояния и на взаимодействие с другими физ объектами

        //[SerializeField] private float maxVelocity;
        //[SerializeField] private bool clampVelocity;
        [SerializeField] private bool _drawGizmos;

        public RopeNode _prevNode;
        public RopeNode _nextNode;
        public Rigidbody Body => _body;
        public bool IsKinematic => _body.isKinematic;


        private Rigidbody _body;

        // private Vector3 _cachedTargetOffset;
        private Vector3 _nextNodeRelativePosition;
        private Vector3 _previewNodeRelativePosition;

        //[Range(0f, 1f)] [SerializeField] private float _drag;
        private Quaternion _facing;
        // [SerializeField] private Vector3 _facingCorrection;
        //private Color _thisNodColor;

        [SerializeField] private Transform _bodyCollider;

        private void Awake()
        {
            _body = GetComponent<Rigidbody>();
            //_currentTargetDistance = _settings._targetDistance;
            _localScale = _bodyCollider.localScale;

            _body.useGravity = false;
            // выключем гравитацию и считаем ее сами, тк физ движок постоянно аплаит ее после физ апдэйта (почему-то)

            _facing = Quaternion.Euler(Vector3.forward);
            //
            if (_settings._facingCorrection != Vector3.zero)
            {
                _facing *= Quaternion.Euler(_settings._facingCorrection);
            }

            //_thisNodColor = Random.ColorHSV();
        }

        //public bool CalculateRotations;


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
            //_cachedTargetOffset = targetOffset;

            Vector3 targetVelocity =
                targetOffset.normalized * _settings._connectionForce; // получаем скорость в направлении рест поинта
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

            float dragMultiplier = 1.0f - _settings._drag * Time.fixedDeltaTime;
            if (dragMultiplier < 0.0f) dragMultiplier = 0.0f;

            var targetVelocityNormalized = targetVelocity - targetVelocity.normalized * dot;

            //OverrideTransform dwcqw;
            //dwcqw.data.

            if (GetEdgeAngle() < _settings._minEdgeAngle + _settings.minEdgeAngleDelta)
            {
                //100% торможения = _settings._minEdgeAngle
                //0% торможения = _settings._minEdgeAngle + _settings.minEdgeAngleDelta;
                
                //angle/angle 90 = 0, 120 = 1 0 = 0, 30 = 1, 0 / 30 = 0, 30/30 = 1 (angle - min)/(max - min)

                //240 = 0, 270 = 1

                //60 = 0 90 = 1
                //
                float velocityDecreaseMultipler =
                    Mathf.Clamp((GetEdgeAngle() - _settings._minEdgeAngle) / _settings.minEdgeAngleDelta,0,1);
                
                
                _body.velocity +=
                    (GetUpVector(_nextNode, _prevNode).normalized * targetVelocityNormalized.magnitude * (1- velocityDecreaseMultipler)) ;
            }
            else
            {
                if (GetEdgeAngle() > 180 + (_settings._minEdgeAngle - _settings.minEdgeAngleDelta))
                {
                    // float velocityDecreaseMultipler =
                    //     Mathf.Clamp(((GetEdgeAngle() - 180)  + _settings._minEdgeAngle)) / _settings.minEdgeAngleDelta,0,1);
                    //
                    // _body.velocity +=
                    //     (-GetUpVector(_nextNode, _prevNode).normalized * targetVelocityNormalized.magnitude * (1- velocityDecreaseMultipler)) ;
                }
            }
            
            _body.velocity +=
                targetVelocityNormalized; // высчитываем все противолежащие нашему направлению офсета силы, когда созванивались, я это объяснял

            if (_settings.clampVelocity)
            {
                _body.velocity = Vector3.ClampMagnitude(_body.velocity, _settings.maxVelocity); // клэмпим)
            }

            _body.velocity *= dragMultiplier;
            
            _body.angularVelocity = Vector3.zero;

            //CheckAngleAndCorrect();
        }

        // [SerializeField] private float _minEdgeAngle;
        private void CheckAngleAndCorrect()
        {
            var angle = GetEdgeAngle();

            //Debug.DrawLine(_prevNode.transform.position + _prevNode.ConnectingVector);
            if (angle <= _settings._minEdgeAngle)
            {
                LimitEdgeBetweenNodes(angle);
            }
        }

        private float GetEdgeAngle()
        {
            if (_prevNode == null || _prevNode.ConnectingVector == Vector3.zero)
            {
                return 180;
            }

            var angle = Vector3.Angle(_prevNode.ConnectingVector, -ConnectingVector);
            return angle;
        }

        private void LimitEdgeBetweenNodes(float angle)
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild)
                Debug.Log(
                    $"<color=purple>Min Vector reached {_prevNode.name} and {name}:  <b>{Vector3.Angle(_prevNode.ConnectingVector, ConnectingVector)}</b></color>",
                    this);

            _body.AddForce(GetUpVector(_nextNode, _prevNode).normalized * angle * _settings.EdgeDecreeseForceMultiplier, ForceMode.Acceleration);
        }

        private void Update()
        {
            if (_prevNode != null && _settings.CalculateRotations)
            {
                AdjustRotation();
            }


            if (_isControllingBodyColliderLength)
            {
                AdjustColliderLength(_prevNode, _nextNode);
            }
        }

        private void AdjustRotation()
        {
            var pointDirection = _prevNode.transform.position - transform.position;

            var rotation = Quaternion.LookRotation(pointDirection.normalized,
                GetUpVector(_nextNode, _prevNode));

            rotation *= _facing;
            transform.rotation = rotation;
        }

        private bool _isColliderBodyScaleForced;

        private bool _wasEnabled;

        public async void DisableBodyCollider()
        {
            _isColliderBodyScaleForced = true;
            _wasEnabled = _bodyCollider.GetComponent<Collider>().enabled;
            _bodyCollider.GetComponent<Collider>().enabled = false;
            _localScale.x = 0f;
            await UniTask.DelayFrame(3);
            _bodyCollider.GetComponent<Collider>().enabled = _wasEnabled;

            _isColliderBodyScaleForced = false;
        }

        [Button]
        public void DisableKinematic()
        {
            //Debug.Break();
            _body.isKinematic = false;
        }

        [Button]
        public void EnableKinematic()
        {
            //Debug.Break();
            _body.isKinematic = true;
        }

        private Vector3 _localScale;

        private void AdjustColliderLength(RopeNode prevNode, RopeNode nextNode)
        {
            var transformPosition = transform.position;


            if (prevNode == null)
            {
                _localScale.x = (Vector3.Distance(nextNode.transform.position, transformPosition) / 4f) *
                                _settings._colliderLengthMultiplier;
                _bodyCollider.localScale = _localScale;
                return;
            }


            if (nextNode == null)
            {
                _localScale.x = (Vector3.Distance(prevNode.transform.position, transformPosition) / 4f) *
                                _settings._colliderLengthMultiplier;
                _bodyCollider.localScale = _localScale;
                return;
            }


            var min = Mathf.Min(Vector3.Distance(prevNode.transform.position, transformPosition),
                Vector3.Distance(nextNode.transform.position, transformPosition));

            _localScale.x = (min / 4f) * _settings._colliderLengthMultiplier;
            _bodyCollider.localScale = _localScale;
        }

        private float _currentTargetDistance;

        //этот метод просто возвращает рест поинт относительно другой ноды (направление от одной к другой нормализованное, умноженное на таргет дистанс)
        private Vector3 GetTargetRelativePos(RopeNode node)
        {
            Vector3 position = transform.position;
            Vector3 targetNodePosition = node.transform.position;

            Vector3 relativeDirection = position - targetNodePosition;
            Vector3 targetGlobalPos = targetNodePosition + relativeDirection.normalized * _settings._targetDistance;

            return targetGlobalPos - position;
        }


        private void OnDrawGizmosSelected()
        {
            if (_drawGizmos == false && _settings._drawGlobalGizmos == false)
            {
                return;
            }

            Vector3 upVector = GetUpVector(_nextNode, _prevNode);

            Gizmos.DrawLine(transform.position, upVector.normalized * 1f);
        }

        [FormerlySerializedAs("_controllBodyColliderLength")] [SerializeField]
        private bool _isControllingBodyColliderLength;

        //public bool _drawGlobalGizmos;

        private Vector3 GetUpVector(RopeNode nextNode, RopeNode prevNode)
        {
            var thisNodePosition = transform.position;

            Vector3 nextNodeDirection, normalWithNext = Vector3.zero;
            Vector3 prevNodeDirection, normalWithPrev = Vector3.zero;

            if (nextNode != null)
            {
                nextNodeDirection = nextNode.transform.position - thisNodePosition;
                normalWithNext =
                    Vector3.Cross(nextNodeDirection,
                        _settings._rotationOrientire.forward); //TODO Change global vector to local vetcor
                //result = normalWithNext;
            }


            if (prevNode != null)
            {
                prevNodeDirection = prevNode.transform.position - thisNodePosition;
                normalWithPrev =
                    Vector3.Cross(prevNodeDirection,
                        -_settings._rotationOrientire.forward); //TODO Change global vector to local vetcor
                //result = normalWithPrev;
            }


            if (_drawGizmos || _settings._drawGlobalGizmos)
            {
                Debug.DrawLine(thisNodePosition, thisNodePosition + normalWithNext, Color.red);
                Debug.DrawLine(thisNodePosition, thisNodePosition + normalWithPrev, Color.yellow);
            }

            var result = (normalWithNext + normalWithPrev) / 2;


            return result;
        }

        public void SetTempDistance(float targetDistance)
        {
            _currentTargetDistance = targetDistance;
        }

        public void ResetTargetDisnace()
        {
            DOTween.To(() => _currentTargetDistance, x => _currentTargetDistance = x, _settings._targetDistance, 3f);
        }

        private Vector3 ConnectingVector =>
            _prevNode != null ? _prevNode.transform.position - transform.position : Vector3.zero;
    }
}