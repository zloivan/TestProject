using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace RopePhysics_2
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class RopeNode : MonoBehaviour
    {
        [SerializeField] private bool _isFirstInChain;

        [SerializeField] private RopeNode _neigbor_1;
        [SerializeField] private RopeNode _neigbor_2;

        private Rigidbody _body;

        private float _targetDistance = 0.5f;

        private void Start()
        {
            _body = GetComponent<Rigidbody>();

            var allNodes = FindObjectsOfType<RopeNode>();


            List<(RopeNode, float)> nodeDistancePairs = new List<(RopeNode, float)>();

            foreach (var ropeNode in allNodes.Where(e => e != this))
            {
                nodeDistancePairs.Add((ropeNode,
                    Vector3.Distance(transform.position, ropeNode.transform.position)));
            }

            var ordered = nodeDistancePairs.OrderBy(e => e.Item2).ToArray();

            _neigbor_1 = ordered[0].Item1;

            if (_isFirstInChain)
            {
                return;
            }

            _neigbor_2 = ordered[1].Item1;
        }

        [SerializeField] private float _maxDistance = 2f;

        private void LateUpdate()
        {
            var distance1 = Vector3.Distance(transform.position, _neigbor_1.transform.position);
            if (_isFirstInChain == false
            )
            {
                var distance2 = Vector3.Distance(transform.position, _neigbor_2.transform.position);

                if (distance2 > _maxDistance)
                {
                    //mylogs Probably remove this later
                    if (Debug.isDebugBuild) Debug.Log($"<color=purple>Second neighbor is out of range</color>");
                }
            }


            if (distance1 > _maxDistance)
            {
                //mylogs Probably remove this later
                if (Debug.isDebugBuild) Debug.Log($"<color=purple>First neighbor is out of range</color>");
            }
        }

        private void FixedUpdate()
        {
            ApplyToNodeVelocity(_neigbor_1);
            ApplyToNodeVelocity(_neigbor_2);
        }

        [SerializeField] [Range(0, 1f)] private float _drag = .4f;

        private void ApplyToNodeVelocity(RopeNode node)
        {
            if (node == null) return;

            //Позиция текущей ноды
            Vector3 pos = transform.position;


            Vector3 relativeDirection = pos - node.transform.position;

            Debug.DrawLine(transform.position, relativeDirection, Color.green);

            Vector3 targetPos = node.transform.position + (relativeDirection.normalized * _targetDistance);

            Vector3 offset = targetPos - pos;

            _body.velocity += offset.normalized * (offset.magnitude - Vector3.Dot(offset, _body.velocity)) * _drag;
        }
    }
}