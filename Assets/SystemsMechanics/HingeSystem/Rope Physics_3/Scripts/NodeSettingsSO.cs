using UnityEngine;

namespace RopePhysics_3
{
    [CreateAssetMenu(fileName = "NodeSetting")]
    public class NodeSettingsSO : ScriptableObject
    {
        public float _targetDistance = 1f;
        public float _connectionForce = 1000f;
        public float maxVelocity;
        public bool clampVelocity;
        [Range(0f, 1f)] public float _drag;
        public Vector3 _facingCorrection;
        public bool CalculateRotations;
        public float _minEdgeAngle;
        public Transform _rotationOrientire;
        public float _colliderLengthMultiplier;
        public bool _drawGlobalGizmos;
        public float EdgeDecreeseForceMultiplier;
        public float minEdgeAngleDelta = 30f;
    }
}