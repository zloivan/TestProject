using System;
using UnityEngine;

namespace HingeRopeSystem
{
    [RequireComponent(typeof(Rigidbody))]
    public class RopeNode : MonoBehaviour
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private CharacterJoint _characterJoint;

        //[SerializeField] private RopeJointSettings _jointSettings;

        public bool IsSnapped;
        public float NodeLength; //TODO

       
        // private void Setup()
        // {
        //     if (_rigidbody == null)
        //     {
        //         _rigidbody = GetComponent<Rigidbody>();
        //
        //         if (_rigidbody == null)
        //         {
        //             _rigidbody = gameObject.AddComponent<Rigidbody>();
        //         }
        //     }
        //
        //     if (_characterJoint == null)
        //     {
        //         _characterJoint = GetComponent<CharacterJoint>();
        //         if (_characterJoint == null)
        //         {
        //             _characterJoint = gameObject.AddComponent<CharacterJoint>();
        //             SetupJoint(_characterJoint);
        //         }
        //     }
        // }
        //
        // private void SetupJoint(CharacterJoint joint) //TODO 
        // {
        //     joint.axis = _jointSettings.axis;
        //     joint.swingAxis = _jointSettings.swingAxis;
        //     //_characterJoint
        //     SoftJointLimitSpring twistLimitSpring = new SoftJointLimitSpring
        //         {damper = _jointSettings.twistLimitSpringDamper, spring = _jointSettings.twistLimitSpringSpring};
        //     joint.twistLimitSpring = twistLimitSpring;
        //
        //     SoftJointLimit lowTwistLimit = new SoftJointLimit
        //     {
        //         limit = _jointSettings.lowTwistLimitLimit,
        //         contactDistance = _jointSettings.lowTwistLimitContactDistance
        //     };
        //
        //     joint.lowTwistLimit = lowTwistLimit;
        //
        //     SoftJointLimit highTwistLimit = new SoftJointLimit
        //     {
        //         limit = _jointSettings.highTwistLimitLimit,
        //         contactDistance = _jointSettings.highTwistLimitContactDistance
        //     };
        //
        //     joint.highTwistLimit = highTwistLimit;
        //
        //     SoftJointLimitSpring swingLimitSpring = new SoftJointLimitSpring
        //     {
        //         damper = _jointSettings.swingLimitSpringDamper,
        //         spring = _jointSettings.swingLimitSpringSpring
        //     };
        //
        //     joint.swingLimitSpring = swingLimitSpring;
        //
        //     SoftJointLimit swing1Limit = new SoftJointLimit
        //     {
        //         limit = _jointSettings.swing1LimitLimit,
        //         contactDistance = _jointSettings.swing1LimitContactDistance
        //     };
        //
        //     joint.swing1Limit = swing1Limit;
        //
        //     SoftJointLimit swing2Limit = new SoftJointLimit
        //     {
        //         limit = _jointSettings.swing2LimitLimit,
        //         contactDistance = _jointSettings.swing2LimitContactDistance
        //     };
        //
        //     joint.swing2Limit = swing2Limit;
        // }

        public void RemoveJoint()
        {
            DestroyImmediate(_characterJoint);
        }

        // private void OnValidate()
        // {
        //     Setup();
        // }

        public void ConnectWithNode(RopeNode node)
        {
            _characterJoint.connectedBody = node._rigidbody;
        }

        private void Update()
        {
            _rigidbody.isKinematic = IsSnapped;
        }

        public void SetupMass(float newMass)
        {
            _rigidbody.mass = newMass;
        }
    }
}