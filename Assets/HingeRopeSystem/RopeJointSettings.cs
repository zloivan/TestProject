using UnityEngine;

namespace HingeRopeSystem
{
    [CreateAssetMenu(fileName = "HingeRopeSystem/JointSettings", menuName = "JointSettings", order = 0)]
    public class RopeJointSettings : ScriptableObject
    {
        public Vector3 axis;
        public Vector3 swingAxis;
        [Header("Twist Limit")]
        [Space]
        public float twistLimitSpringDamper;
        public float twistLimitSpringSpring;
        [Header("Low Twist")]
        [Space]
        public float lowTwistLimitLimit;
        public float lowTwistLimitContactDistance;
        [Header("High Twist Limit")]
        [Space]
        public float highTwistLimitLimit;
        public float highTwistLimitContactDistance;
        [Header("Swing Limit Spring")]
        [Space]
        public float swingLimitSpringDamper;
        public float swingLimitSpringSpring;
        [Header("Swing1 Limit")]
        [Space]
        public float swing1LimitLimit;
        public float swing1LimitContactDistance;
        [Header("Swing2 Limit")]
        
        [Space]
        public float swing2LimitLimit;
        public float swing2LimitContactDistance;
        
    }
}