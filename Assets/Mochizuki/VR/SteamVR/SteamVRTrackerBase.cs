using System;
using System.Linq;

using RootMotion.FinalIK;

using UnityEngine;

using Valve.VR;

namespace Mochizuki.VR.SteamVR
{
    public abstract class SteamVRTrackerBase : MonoBehaviour
    {
        public abstract bool IsActive { get; } // BehaviourPose.isActive && BehaviourPose.isValid;

        protected abstract SteamVR_Input_Sources InputSource { get; }
        protected abstract GameObject TargetObject { get; }

        #region Calibration

        public void Calibrate(VRIK avatar)
        {
            TargetObject.transform.localPosition = Vector3.zero;
            TargetObject.transform.localRotation = Quaternion.identity;

            switch (InputSource)
            {
                case SteamVR_Input_Sources.LeftHand:
                    CalibrateHand(avatar, avatar.references.leftHand);
                    break;

                case SteamVR_Input_Sources.RightHand:
                    CalibrateHand(avatar, avatar.references.rightHand);
                    break;

                case SteamVR_Input_Sources.LeftFoot:
                    CalibrateLeg(avatar, avatar.references.leftToes ?? avatar.references.leftFoot);
                    break;

                case SteamVR_Input_Sources.RightFoot:
                    CalibrateLeg(avatar, avatar.references.rightToes ?? avatar.references.rightFoot);
                    break;

                case SteamVR_Input_Sources.Waist:
                    break;

                case SteamVR_Input_Sources.Head:
                    Debug.LogWarning("<b>[Mochizuki.VR]</b> Warning: Please use SteamVRHMD#Calibrate for calibrating head tracker.");
                    break;

                case SteamVR_Input_Sources.LeftKnee:
                    break;

                case SteamVR_Input_Sources.RightKnee:
                    break;

                case SteamVR_Input_Sources.LeftElbow:
                    break;

                case SteamVR_Input_Sources.RightElbow:
                    break;

                case SteamVR_Input_Sources.Any:
                case SteamVR_Input_Sources.LeftShoulder:
                case SteamVR_Input_Sources.RightShoulder:
                case SteamVR_Input_Sources.Chest:
                case SteamVR_Input_Sources.Gamepad:
                case SteamVR_Input_Sources.Camera:
                case SteamVR_Input_Sources.Keyboard:
                case SteamVR_Input_Sources.Treadmill:
                    throw new NotSupportedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CalibrateHand(VRIK avatar, Transform transform)
        {
        }

        // ReSharper disable once ParameterHidesMember
        private void CalibrateLeg(VRIK avatar, Transform lastFootBone)
        {
            // adjust tracker position to last transform position
            var target = TargetObject.transform;
            target.position = new Vector3(target.position.x, lastFootBone.position.y - avatar.references.root.position.y, target.position.z);

            // matches the tracker rotation and last transform rotation
            target.rotation = lastFootBone.rotation;

            // align the target forward axis with the tracker forward axis. the other axis remain unchanged.
            var forwardOfTracker = GetNearestAxis(avatar.references.root.forward, transform);
            var toForwardVector = transform.rotation * forwardOfTracker;

            var forwardOfTransform = GetNearestAxis(avatar.references.root.forward, lastFootBone);
            var fromForwardVector = lastFootBone.rotation * forwardOfTransform;

            // remove upwards/downwards vector
            toForwardVector.y = fromForwardVector.y;

            // rotate transform
            var angle = Vector3.Angle(lastFootBone.rotation * fromForwardVector, target.rotation * toForwardVector);
            var axis = -Vector3.Cross(toForwardVector, fromForwardVector);

            target.rotation = Quaternion.AngleAxis(angle, axis) * target.rotation;
        }

        // ReSharper disable once ParameterHidesMember
        private Vector3 GetNearestAxis(Vector3 vector, Transform transform)
        {
            var vector1 = Vector3.Dot(vector, transform.right);
            var vector2 = Vector3.Dot(vector, transform.up);
            var vector3 = Vector3.Dot(vector, transform.forward);
            var vectors = new[] { vector1, vector2, vector3 };
            var nearest = vectors.Select((w, i) => new { Value = w, Index = i }).OrderByDescending(w => Mathf.Abs(w.Value)).First();

            var x = nearest.Index == 0 ? Math.Sign(nearest.Value) * 1 : 0;
            var y = nearest.Index == 1 ? Math.Sign(nearest.Value) * 1 : 0;
            var z = nearest.Index == 2 ? Math.Sign(nearest.Value) * 1 : 0;

            return new Vector3(x, y, z);
        }

        #endregion

        #region Assign

        public void Assign(IKSolverVR solver)
        {
            if (!HasTargetObject())
            {
                var obj = new GameObject("Target");
                obj.transform.position = Vector3.zero;
                obj.transform.rotation = Quaternion.identity;
                obj.transform.parent = transform;
            }

            Assign(solver, TargetObject?.transform ?? transform, 1f);
        }

        public void UnAssign(IKSolverVR solver)
        {
            Assign(solver, null, 0f);
        }

        // ReSharper disable once ParameterHidesMember
        private void Assign(IKSolverVR solver, Transform transform, float weight)
        {
            switch (InputSource)
            {
                case SteamVR_Input_Sources.LeftHand:
                    solver.leftArm.target = transform;
                    solver.leftArm.positionWeight = weight;
                    solver.leftArm.rotationWeight = weight;
                    break;

                case SteamVR_Input_Sources.RightHand:
                    solver.rightArm.target = transform;
                    solver.rightArm.positionWeight = weight;
                    solver.rightArm.rotationWeight = weight;
                    break;

                case SteamVR_Input_Sources.LeftFoot:
                    solver.leftLeg.target = transform;
                    solver.leftLeg.positionWeight = weight;
                    solver.leftLeg.rotationWeight = weight;
                    break;

                case SteamVR_Input_Sources.RightFoot:
                    solver.rightLeg.target = transform;
                    solver.rightLeg.positionWeight = weight;
                    solver.rightLeg.rotationWeight = weight;
                    break;

                case SteamVR_Input_Sources.Waist:
                    solver.spine.pelvisTarget = transform;
                    solver.spine.pelvisPositionWeight = weight;
                    solver.spine.pelvisRotationWeight = weight;
                    solver.plantFeet = false;
                    break;

                case SteamVR_Input_Sources.Head:
                    solver.spine.headTarget = transform;
                    solver.spine.positionWeight = weight;
                    solver.spine.rotationWeight = weight;
                    solver.spine.maxRootAngle = 180f;
                    break;

                case SteamVR_Input_Sources.LeftKnee:
                    solver.leftLeg.bendGoal = transform;
                    solver.leftLeg.bendGoalWeight = weight;
                    break;

                case SteamVR_Input_Sources.RightKnee:
                    solver.rightLeg.bendGoal = transform;
                    solver.rightArm.bendGoalWeight = weight;
                    break;

                case SteamVR_Input_Sources.LeftElbow:
                    solver.leftArm.bendGoal = transform;
                    solver.leftArm.bendGoalWeight = weight;
                    break;

                case SteamVR_Input_Sources.RightElbow:
                    solver.rightArm.bendGoal = transform;
                    solver.rightArm.bendGoalWeight = weight;
                    break;

                case SteamVR_Input_Sources.LeftShoulder:
                case SteamVR_Input_Sources.RightShoulder:
                case SteamVR_Input_Sources.Chest:
                case SteamVR_Input_Sources.Any:
                case SteamVR_Input_Sources.Gamepad:
                case SteamVR_Input_Sources.Camera:
                case SteamVR_Input_Sources.Keyboard:
                case SteamVR_Input_Sources.Treadmill:
                    throw new NotSupportedException();

                // default:
                //     throw new ArgumentOutOfRangeException();
            }
        }

        private bool HasTargetObject()
        {
            return transform.childCount > 1;
        }

        #endregion
    }
}