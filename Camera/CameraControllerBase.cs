using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities
{
    [RequireComponent(typeof(CinemachineFreeLook), typeof(CinemachineCollider))]
    public abstract class CameraControllerBase : EntityBase
    {
        [field: SerializeField] public Player Player { get; private set; }
        [field: SerializeField] public Transform Follow { get; private set; }
        [SerializeField] protected Vector3 lookAtOffset;

        private CinemachineFreeLook _cinemachineCamera;
        public CinemachineFreeLook CinemachineCamera
        {
            get
            {
                if (!_cinemachineCamera)
                    _cinemachineCamera = GetComponent<CinemachineFreeLook>();

                return _cinemachineCamera;
            }
        }

        private CinemachineCollider _cinemachineCollider;
        public CinemachineCollider CinemachineCollider
        {
            get
            {
                if (!_cinemachineCollider)
                    _cinemachineCollider = GetComponent<CinemachineCollider>();

                return _cinemachineCollider;
            }
        }

        public void ResetFollowAndLookAt()
        {
            CinemachineCamera.Follow = Follow;
            CinemachineCamera.LookAt = Follow;
        }

        public void OnTargetObjectWarped(Vector3 positionDelta)
        {
            CinemachineCamera.OnTargetObjectWarped(Follow, positionDelta);
        }

        private void OnGameStateEnabled(IState state)
        {
            if (state is GameStateMatch)
                CinemachineCamera.PreviousStateIsValid = false;
        }

        protected virtual void OnEnable()
        {
            EventManager.Instance.GameEvents.StateEnabled += OnGameStateEnabled;
        }
        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.GameEvents.StateEnabled -= OnGameStateEnabled;
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Follow)
                Follow.transform.localPosition = lookAtOffset;
        }
#endif
    }
}
