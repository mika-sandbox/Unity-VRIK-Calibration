using RootMotion.FinalIK;

using UnityEngine;

using Valve.VR;

namespace Mochizuki.VR.SteamVR
{
    // ReSharper disable once InconsistentNaming
    public class SteamVRHMD : SteamVRTrackerBase
    {
        public override bool IsActive => true;

        protected override SteamVR_Input_Sources InputSource => SteamVR_Input_Sources.Head;

        protected override GameObject TargetObject => transform.GetChild(1).gameObject; // transform.GetChild(0).gameObject;

        public void Calibrate(VRIK avatar, Vector3 viewPosition)
        {
        }
    }
}