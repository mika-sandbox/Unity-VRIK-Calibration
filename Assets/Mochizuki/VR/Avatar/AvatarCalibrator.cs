using System.Collections.Generic;
using System.Linq;

using Mochizuki.VR.SteamVR;

using RootMotion.FinalIK;

using UnityEngine;

namespace Mochizuki.VR.Avatar
{
    public class AvatarCalibrator : MonoBehaviour
    {
        [SerializeField]
        private SteamVRHMD HeadTracker;

        [SerializeField]
        private VRIK IK;

        [SerializeField]
        private float PlayerHeight;

        [SerializeField]
        private List<SteamVRTracker> Trackers;

        // ViewPosition equals to VRChatAvatarDescriptor.ViewPosition
        [SerializeField]
        private Vector3 ViewPosition;

        // Start is called before the first frame update
        private void Start() { }

        // Update is called once per frame
        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.C))
                return;

            IK.solver.FixTransforms();

            var scale = CalibrateAvatarScale();
            IK.gameObject.transform.localScale = Vector3.one * scale;

            HeadTracker.Assign(IK.solver);
            HeadTracker.Calibrate(IK, ViewPosition * scale);

            foreach (var tracker in GetActivatedTrackers())
            {
                tracker.Assign(IK.solver);
                tracker.Calibrate(IK);
            }

            // see: https://twitter.com/ikko/status/966894056142864385
            if (IK.solver.spine.pelvisTarget && IK.solver.leftLeg.target && IK.solver.rightLeg.target)
                IK.gameObject.AddComponent<VRIKRootController>();
        }

        private float CalibrateAvatarScale()
        {
            return PlayerHeight / ViewPosition.y;
        }

        private List<SteamVRTracker> GetActivatedTrackers()
        {
            return Trackers.Where(w => w.IsActive).ToList();
        }
    }
}