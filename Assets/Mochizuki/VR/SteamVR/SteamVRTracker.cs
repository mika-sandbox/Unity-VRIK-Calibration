using UnityEngine;

using Valve.VR;

namespace Mochizuki.VR.SteamVR
{
    // ReSharper disable once InconsistentNaming
    [RequireComponent(typeof(SteamVR_Behaviour_Pose))]
    public class SteamVRTracker : SteamVRTrackerBase
    {
        public override bool IsActive => true; // BehaviourPose.isActive && BehaviourPose.isValid;

        #region BehaviourPose

        private SteamVR_Behaviour_Pose _behaviourPose;

        private SteamVR_Behaviour_Pose BehaviourPose => _behaviourPose != null ? _behaviourPose : _behaviourPose = GetComponent<SteamVR_Behaviour_Pose>();

        #endregion

        #region InputSource

        private SteamVR_Behaviour_Pose _pose;

        protected override SteamVR_Input_Sources InputSource => BehaviourPose.inputSource;

        #endregion

        #region TrackerModel

        private GameObject _trackerModel;

        private GameObject TrackerModel => _trackerModel != null ? _trackerModel : _trackerModel = transform.GetChild(0).gameObject;

        #endregion

        #region TargetObject

        private GameObject _targetObject;

        protected override GameObject TargetObject => _targetObject != null ? _targetObject : _targetObject = transform.GetChild(1).gameObject;

        #endregion
    }
}