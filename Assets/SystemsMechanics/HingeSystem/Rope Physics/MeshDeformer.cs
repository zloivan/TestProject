using System;
using UnityEngine;


namespace RopePhysics
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(Collider))]
    public class MeshDeformer : MonoBehaviour
    {
        [SerializeField] private float _springForce;
        private Mesh _deformationMesh;
        private Vector3[] _originalVerticies, _displacedVerticies, _vertexVelocities;
        [SerializeField] private float _damping;

        void Start()
        {
            _deformationMesh = GetComponent<MeshFilter>().mesh;
            _originalVerticies = _deformationMesh.vertices;

            _displacedVerticies = new Vector3[_originalVerticies.Length];
            for (int i = 0; i < _originalVerticies.Length; i++)
            {
                _displacedVerticies[i] = _originalVerticies[i];
            }

            // _displacedVerticies = _originalVerticies;
            _vertexVelocities = new Vector3[_originalVerticies.Length];
        }


        public void ApplyInput(Vector3 hitInfoPoint, float pushForc)
        {
            if (Camera.main is { })
            {
                Debug.DrawLine(Camera.main.transform.position, hitInfoPoint, Color.blue);
            }

            hitInfoPoint = transform.InverseTransformPoint(hitInfoPoint);
            for (int i = 0; i < _displacedVerticies.Length; i++)
            {
                AddForceToVertex(i, hitInfoPoint, pushForc);
            }
        }

        private void AddForceToVertex(int i, Vector3 hitInfoPoint, float pushForc)
        {
            Vector3 pointToVertex = _displacedVerticies[i] - hitInfoPoint;
            Debug.DrawLine(pointToVertex.normalized * 0f, pointToVertex.normalized * 1f, Color.magenta);

            var attenuteForce = pushForc / 1 + pointToVertex.sqrMagnitude;
            var velocity = attenuteForce * Time.deltaTime;
            _vertexVelocities[i] += pointToVertex.normalized * velocity;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _displacedVerticies.Length; i++)
            {
                Gizmos.DrawSphere(_displacedVerticies[i], .01f);
            }
        }

        private void Update()
        {
            for (int i = 0; i < _displacedVerticies.Length; i++)
            {
                UpdateVerticies(i);
            }

            _deformationMesh.vertices = _displacedVerticies;
            _deformationMesh.RecalculateNormals();
        }

        private void UpdateVerticies(int i)
        {
            var velocity = _vertexVelocities[i];
            Vector3 displacement = _displacedVerticies[i] - _originalVerticies[i];
            velocity -= displacement * _springForce * Time.deltaTime;

            velocity *= 1 - _damping * Time.deltaTime;
            _vertexVelocities[i] = velocity;

            _displacedVerticies[i] += velocity * Time.deltaTime;
        }
    }
}