using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;
using System;

namespace PowerGolfArena.Entities
{
    public class PlayerCameraManager : EntityBase
    {
        public enum CameraFocus
        {
            Goflball,
            Character
        }

        [field: SerializeField] public Player Player { get; private set; }

        [SerializeField] private int focusPriority               = 1;
        [SerializeField] private int normalPriority              = 0;
        [SerializeField] private CameraFocus defaultFocus        = CameraFocus.Character;
        [SerializeField] private Layer firstCameraVisibleLayer;
        [SerializeField] private Layer firstCameraInvisibleLayer;

        [Space(7)]
        [SerializeField] private float focusToGolfballDelay      = 0.5f;
        [SerializeField] private float lookAtGolfballMaxDistance = 5;
        [SerializeField] private float focusToCharacterDelay     = 0.05f;

        [field: Space(7)]
        [field: SerializeField] public CameraControllerGolfball GolfballCamera { get; private set; }
        [field: SerializeField] public CameraControllerCharacter CharacterCamera { get; private set; }

        private CameraFocus _currentFocus;
        private Layer _currentCameraVisibleLayer;
        private Layer _currentCameraInvisibleLayer;

        private IEnumerator SetLookAtTargetDelayed(CinemachineFreeLook camera, Transform follow, Transform lookAt, float maxDistance)
        {
            yield return new WaitUntil(() =>
            {
                Vector3 offset = lookAt.position - follow.position;
                return offset.y > 0 || offset.magnitude > maxDistance;
            });

            camera.LookAt = lookAt;
        }

        private bool CanChangeFocus()
        {
            bool canChangeFocus = true;
            if (EventManager.Instance.PlayerEvents.WantsToChangeFocus is object)
            {
                foreach (Func<bool> shouldAllowFocusChange in EventManager.Instance.PlayerEvents.WantsToChangeFocus.GetInvocationList())
                    canChangeFocus = canChangeFocus && shouldAllowFocusChange();
            }

            return canChangeFocus;
        }

        private IEnumerator SetCameraFocusDelayed(CameraFocus focus, float delay)
        {
            yield return new WaitUntil(CanChangeFocus);
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(delay);

            if (focus == CameraFocus.Goflball)
                GolfballCamera.ResetFollowAndLookAt();

            else if (focus == CameraFocus.Character)
                CharacterCamera.ResetFollowAndLookAt();

            else
                throw new System.NotImplementedException();

            SetCameraFocus(focus);
        }
        public void SetCameraFocus(CameraFocus focus)
        {
            _currentFocus = focus;
            UpdateCameraPriorities();
        }
        public void UnfocusAllCameras()
        {
            GolfballCamera.CinemachineCamera.Priority  = normalPriority;
            CharacterCamera.CinemachineCamera.Priority = normalPriority;
        }

        private void UpdateCameraPriorities()
        {
            if (_currentFocus == CameraFocus.Goflball)
            {
                GolfballCamera.CinemachineCamera.Priority  = focusPriority;
                CharacterCamera.CinemachineCamera.Priority = normalPriority;
            }
            else if (_currentFocus == CameraFocus.Character)
            {

                CharacterCamera.CinemachineCamera.Priority = focusPriority;
                GolfballCamera.CinemachineCamera.Priority  = normalPriority;
            }
            else
                throw new System.NotImplementedException();
        }

        private void OnBeforeGameStateChanged(IState state)
        {
            if (!(state is GameStateMatch))
                return;

            _currentCameraVisibleLayer   = new Layer(firstCameraVisibleLayer.LayerIndex + Player.PlayerID);
            _currentCameraInvisibleLayer = new Layer(firstCameraInvisibleLayer.LayerIndex + Player.PlayerID);
            gameObject.layer             = _currentCameraVisibleLayer.LayerIndex;

            if (Player == MatchManager.Instance.ActivePlayer)
            {
                SetCameraFocus(defaultFocus);
                MainCamera.Instance.Camera.cullingMask |= _currentCameraVisibleLayer.Mask;
                MainCamera.Instance.Camera.cullingMask &= ~_currentCameraInvisibleLayer.Mask;
            }
            else
                UnfocusAllCameras();
        }
        private void OnActivePlayerChanged(Player player)
        {
            if (Player == player)
            {
                SetCameraFocus(CameraFocus.Character);
                MainCamera.Instance.Camera.cullingMask |= _currentCameraVisibleLayer.Mask;
                MainCamera.Instance.Camera.cullingMask &= ~_currentCameraInvisibleLayer.Mask;
            }
            else
            {
                UnfocusAllCameras();
                MainCamera.Instance.Camera.cullingMask &= ~_currentCameraVisibleLayer.Mask;
                MainCamera.Instance.Camera.cullingMask |= _currentCameraInvisibleLayer.Mask;
            }
        }
        private void OnGolfballStartedMoving(Golfball golfball)
        {
            if (Player != golfball.Player)
                return;

            StopAllCoroutines();
            StartCoroutine(SetLookAtTargetDelayed(CharacterCamera.CinemachineCamera, CharacterCamera.Follow, GolfballCamera.Follow, lookAtGolfballMaxDistance));
            StartCoroutine(SetCameraFocusDelayed(CameraFocus.Goflball, focusToGolfballDelay));
        }
        private void OnGolfballStoppedMoving(Golfball golfball)
        {
            if (Player != golfball.Player)
                return;

            StopAllCoroutines();
            StartCoroutine(SetCameraFocusDelayed(CameraFocus.Character, focusToCharacterDelay));
        }

        private void OnEnable()
        {
            EventManager.Instance.GameEvents.BeforeStateChanged      += OnBeforeGameStateChanged;
            EventManager.Instance.PlayerEvents.ActivePlayerChanged   += OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.GolfballStartedMoving += OnGolfballStartedMoving;
            EventManager.Instance.PlayerEvents.GolfballStoppedMoving += OnGolfballStoppedMoving;
        }
        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.GameEvents.BeforeStateChanged      -= OnBeforeGameStateChanged;
            EventManager.Instance.PlayerEvents.ActivePlayerChanged   -= OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.GolfballStartedMoving -= OnGolfballStartedMoving;
            EventManager.Instance.PlayerEvents.GolfballStoppedMoving -= OnGolfballStoppedMoving;
        }
    }
}
