using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HingeRopeSystem
{
    public class Rope : MonoBehaviour
    {
        [SerializeField] private RopeNode _nodePrefab;
        [SerializeField] private Transform _parentObject;

        [SerializeField] [Range(1, 1000)] private int _ropeLength;

        [SerializeField] private float _distanceBetweenNodes = 0.21f;

        [SerializeField] private bool _snapFirst, _snapLast;

        [SerializeField]
        private List<RopeNode> _nodes = new List<RopeNode>();

        [Button]
        public void DestroyRope()
        {
            if (_nodes != null)
            {
                for (int i = 0; i < _nodes.Count; i++)
                {
                    DestroyImmediate(_nodes[i].gameObject);
                }

                _nodes.Clear();
            }
        }

        [Button]
        public void SpawnRope()
        {
            var nodeCount = (int)(_ropeLength / _distanceBetweenNodes);

            for (int i = 0; i < nodeCount; i++)
            {
                
                var node = Instantiate(_nodePrefab, _parentObject.position + (i + 1) * (Vector3.down * _distanceBetweenNodes), Quaternion.identity, _parentObject.transform);

                node.SetupMass((nodeCount + 1) - i);
                _nodes.Add(node);
                
                if (i == 0)
                {
                    _nodes[i].RemoveJoint();
                }
                else
                {
                    node.ConnectWithNode(_nodes[i-1]);
                }
            }


            if (_snapFirst)
            {
                _nodes[0].IsSnapped = true;
            }

            if (_snapLast)
            {
                _nodes[_nodes.Count - 1].IsSnapped = true;
            }
            
        }
    }
}