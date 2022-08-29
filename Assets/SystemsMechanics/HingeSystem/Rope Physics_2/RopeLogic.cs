using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RopePhysics_2
{
    public class RopeLogic : MonoBehaviour
    {
        [SerializeField] private float _nodeDistance;

        [SerializeField] private List<Rigidbody> _listOfNods;

        public void Start()
        {
            _listOfNods = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        }

        private void Update()
        {
            throw new NotImplementedException();
        }

        private void OnDrawGizmos()
        {
            if (_listOfNods != null && _listOfNods.Count > 0)
            { Gizmos.color = Color.blue;

            
                for (int i = 0; i < _listOfNods.Count - 1; i++)
                {
                    Gizmos.DrawLine(_listOfNods[i].position, _listOfNods[i + 1].position);
                }
                
            }
           
        }
    }
}