using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RopePhysics_3

{
    public class RopeLogic : MonoBehaviour
    {

        [SerializeField] private List<RopeNode> _listOfNods;

        [SerializeField] private float _delayBeforeAddingNewNode;
        [SerializeField] private float _newNodeAddingDistance;
        [SerializeField] private int _maxOperationCount = 5;

        [SerializeField] private bool _drawGizmos;
        public void Start()
        {
            _listOfNods = new List<RopeNode>(GetComponentsInChildren<RopeNode>());
        }

        
        private float _timeout;
        private int _numberOfkinematic;
        private void FixedUpdate()
        {

            _numberOfkinematic = 0;
            for (int i = 0; i < _listOfNods.Count - 1; i++)
            {
                if ( _listOfNods[i].IsKinematic)
                {
                    _numberOfkinematic++;
                    if (_numberOfkinematic > 2)
                    {
                        Debug.Log($"<color=purple>HAS TWO KINEMATIC</color>");

                        break;
                    }
                }
                
                
            }
            
            if (_numberOfkinematic < 2)
            {
                return;
            }
           

            int currentOpenrationCount = 0;
            for (int i = 0; i < _listOfNods.Count - 1; i++)
            {
                if (Vector3.Distance(_listOfNods[i].transform.position, _listOfNods[i + 1].transform.position) > _newNodeAddingDistance)
                {
                    SpawnNewNode(i+1);
                    currentOpenrationCount++;
                    if (_maxOperationCount <= currentOpenrationCount)
                    {
                        return; 
                    }
                }
            }
        }

       

        private void SpawnNewNode(int nodeIndex)
        {
            //mylogs Probably remove this later
            if (Debug.isDebugBuild) Debug.Log($"<color=purple>[{Time.realtimeSinceStartup}] Spawn</color>");

             var node = Instantiate(_listOfNods[_listOfNods.Count - 1], this.transform);
             node.GetComponent<Renderer>().material.color = Color.green;
             var nodeToSpawn = _listOfNods[nodeIndex];

             var transform1 = nodeToSpawn.transform;
             
             var preSpawningNode = _listOfNods[nodeIndex - 1];
             
             preSpawningNode._nextNode = node;
             nodeToSpawn._prevNode = node;
             node._nextNode = nodeToSpawn;
             node._prevNode = preSpawningNode;
             node.Body.useGravity = false;
             node.Body.isKinematic = false;
             
             node.transform.position = Vector3.Lerp(transform1.position, preSpawningNode.transform.position, 0.5f);

            _listOfNods.Insert(nodeIndex, node);
        }

        private void OnDrawGizmos()
        {
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
                Gizmos.DrawSphere(Vector3.Lerp(posOfLast, posOfPreLast, 0.5f),.5f); 
                //posOfLast.x + (posOfPreLast.x - posOfLast.x) / 2;
            }
           
            
            
        }
    }
}