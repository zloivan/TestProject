using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sirenix.OdinInspector;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace RopePhysics_3

{
    public class RopeLengthLogic : MonoBehaviour
    {
        [SerializeField] private List<RopeNode> _listOfNods;

        [SerializeField] private List<RopeNode> _addedNode = new List<RopeNode>();
        [SerializeField] private float _delayBeforeAddingNewNode;
        [SerializeField] private float _newNodeAddingDistance;
        [SerializeField] private int _maxOperationCount = 5;
        [SerializeField] [Range(0f, 1f)] private float _timeScale;
        [SerializeField] private int _maxNodeCount = 20;
        [SerializeField] private bool _drawGizmos;

        public void Start()
        {
            _listOfNods = new List<RopeNode>(GetComponentsInChildren<RopeNode>());
        }


        private float _timeout;
        private int _numberOfkinematic;

        private void FixedUpdate()
        {
            HandleSpawnLogic();

            Time.timeScale = _timeScale;
        }

        [SerializeField] private ConstraintProvider _constraintProvider;
        private List<int> _kinematicIndexes = new List<int>();
        private int _lastKinematicCount;
        [SerializeField] [Range(0.01f, 1f)] private float distanceCorrector;

        private void HandleSpawnLogic()
        {
            _numberOfkinematic = 0;

            _kinematicIndexes.Clear();

            for (var i = 0; i < _listOfNods.Count; i++)
            {
                if (_listOfNods[i].IsKinematic)
                {
                    _kinematicIndexes.Add(i);
                }
            }

            if (_lastKinematicCount < _kinematicIndexes.Count)
            {
                ResetAllLength();
            }

            _lastKinematicCount = _kinematicIndexes.Count;

            if (_kinematicIndexes.Count < 2)
            {
                return;
            }
            //Пробегаемся по всем нодам, добавляем индексы кинематик нодов в массив
            //По завершению цикла, проверяем если в массиве больше 2 кинематиков то идем далье по методу
            //Запускаем колличество циклов равное колличество кинематиков - 1
            //Пробегаемся по нодам между кинематиками используя найденный индексы кинематиков


            int currentOpenrationCount = 0;

            for (int i = _kinematicIndexes[0]; i < _kinematicIndexes[_kinematicIndexes.Count - 1]; i++)
            {
                if (Vector3.Distance(_listOfNods[i].transform.position, _listOfNods[i + 1].transform.position) >
                    _newNodeAddingDistance)
                {
                    if (_listOfNods.Count >= _maxNodeCount)
                    {
                        for (int j = 0; j < _kinematicIndexes.Count - 1; j++)
                        {
                            var distance = Vector3.Distance(_listOfNods[_kinematicIndexes[j]].transform.position,
                                _listOfNods[_kinematicIndexes[j + 1]].transform.position);

                            var resultingTargetDistance = distance / (_kinematicIndexes[j + 1] - _kinematicIndexes[j]);

                            IncreaseDistanceForAll(_kinematicIndexes[j],
                                _kinematicIndexes[j + 1],
                                resultingTargetDistance * distanceCorrector);
                        }

                        ResetLength();

                        return;
                    }

                    _listOfNods[i].DisableBodyCollider();

                    _listOfNods[i + 1].DisableBodyCollider();

                    SpawnNewNode(i + 1);
                    currentOpenrationCount++;
                    if (_maxOperationCount <= currentOpenrationCount)
                    {
                        _constraintProvider.UpdateConstrains(_listOfNods.Select(e => e.transform).ToList());
                        return;
                    }
                }
            }

            _constraintProvider.UpdateConstrains(_listOfNods.Select(e => e.transform).ToList());
        }

        public RopeNode GetClosesetNode(Vector3 position)
        {
            float minDistance = 0f;

            RopeNode result = _listOfNods[0];
            for (int i = 0; i < _listOfNods.Count; i++)
            {
              var distance =  Vector3.Distance(_listOfNods[i].transform.position, position);
              if (distance < minDistance)
              {
                  result = _listOfNods[i];
                  minDistance = distance;
              }
            }

            return result;
        }
        
        private void ResetAllLength()
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>Reset all length</color>");

            foreach (var listOfNod in _listOfNods)
            {
                listOfNod.ResetTargetDisnace();
            }
        }

        private void ResetLength()
        {
            //throw new NotImplementedException();
        }

        // [OnValueChanged("UpdateNodes")] [SerializeField]
        // private bool _calculateRotations;

        // public void UpdateNodes()
        // {
        //     foreach (var variable in FindObjectsOfType<RopeNode>())
        //     {
        //         variable.CalculateRotations = _calculateRotations;
        //     }
        // }

        private void IncreaseDistanceForAll(int startIndex, int endIndex, float targetDistance)
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>Increasing length {targetDistance}</color>");

            for (int i = startIndex; i <= endIndex; i++)
            {
                _listOfNods[i].SetTempDistance(targetDistance);
            }
        }

        private void SpawnNewNode(int nodeIndex)
        {
            var nextNode = _listOfNods[nodeIndex];


            //var nextNodeTransform = nextNode.transform;

            var prevNode = _listOfNods[nodeIndex - 1];


            var node = Instantiate(prevNode,
                Vector3.Lerp(nextNode.transform.position, prevNode.transform.position, 0.5f),
                prevNode.transform.rotation,
                this.transform);

            node.transform.SetSiblingIndex(nodeIndex);
            node.DisableBodyCollider();

            //Debug.Break();
            _addedNode.Add(node);
            node.GetComponent<Renderer>().material.color = Color.green;

            prevNode._nextNode = node;
            nextNode._prevNode = node;
            node._nextNode = nextNode;
            node._prevNode = prevNode;
            node.Body.useGravity = false;
            node.Body.isKinematic = false;


            _listOfNods.Insert(nodeIndex, node);

            for (int i = 0; i < _listOfNods.Count; i++)
            {
                if (i == _listOfNods.Count - 1)
                {
                    _listOfNods[i].name =_listOfNods[i].IsKinematic ?  "Bone_END_Kinematic" : "Bone_END" ;
                    break;
                }
                
                _listOfNods[i].name =_listOfNods[i].IsKinematic ?  
                    $"Bone_{_listOfNods[i].transform.GetSiblingIndex()}_Kinematic" : 
                    $"Bone_{_listOfNods[i].transform.GetSiblingIndex()}";
            }
            
        }

        private void OnDrawGizmos()
        {
            if (_drawGizmos == false)
            {
                return;
            }

            if (_listOfNods != null && _listOfNods.Count > 0)
            {
                Gizmos.color = Color.blue;


                for (int i = 0; i < _listOfNods.Count - 1; i++)
                {
                    Gizmos.DrawLine(_listOfNods[i].transform.position, _listOfNods[i + 1].transform.position);
                }

                Gizmos.color = Color.green;

                var posOfLast = _listOfNods[_listOfNods.Count - 1].transform.position;
                var posOfPreLast = _listOfNods[_listOfNods.Count - 2].transform.position;
                Gizmos.DrawSphere(Vector3.Lerp(posOfLast, posOfPreLast, 0.5f), .5f);
                //posOfLast.x + (posOfPreLast.x - posOfLast.x) / 2;
            }
        }
    }
}