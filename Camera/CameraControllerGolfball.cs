using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities
{
    [RequireComponent(typeof(CinemachineFreeLook))]
    public class CameraControllerGolfball : CameraControllerBase
    {
        [SerializeField] private float cameraWorldUpAdaptionSpeed          = 0.0085f;
        [SerializeField] private float airbornCameraWorldUpAdaptionSpeed   = 0.05f;
        [Range(0.0f, 1.0f)]                                                
        [SerializeField] private float cameraWorldUpAdaptionAmount         = 0.75f;
        [Min(0)]                                                           
        [SerializeField] private float cameraWorldUpAdaptionPower          = 1.05f;
        [SerializeField] private float primaryDownwardAdaptionSpeedMult    = 0.9f;
        [Min(0)]                                                           
        [SerializeField] private float primaryDownwardAdaptionSpeedPower   = 0.75f;
        [SerializeField] private float secondaryDownwardAdaptionSpeedMult  = 37.5f;
        [Min(0)]
        [SerializeField] private float secondaryDownwardAdaptionSpeedPower = 6.5f;

        private Transform _cameraWorldUpOverride;
        private Vector3 cameraWorldUpAdaptionVelocity;

        private IEnumerator SetBindingModeDelayed(CinemachineTransposer.BindingMode bindingMode)
        {
            yield return new WaitUntil(() => MainCamera.Instance.CinemachineBrain.IsBlending);
            yield return new WaitWhile(() => MainCamera.Instance.CinemachineBrain.IsBlending);

            CinemachineCamera.m_BindingMode = bindingMode;
        }

        private void OnActivePlayerChanged(Player player)
        {
            if (Player != player)
                return;

            if (player.GolfballInstance.IsMoving)
                Follow.transform.localPosition = Vector3.zero;
            else
                Follow.transform.localPosition = lookAtOffset;

            CinemachineCamera.PreviousStateIsValid = false;

            StopAllCoroutines();
            StartCoroutine(SetBindingModeDelayed(CinemachineTransposer.BindingMode.WorldSpace));
        }
        private void OnGolfballStartedMoving(Golfball golfball)
        {
            if (Player != golfball.Player)
                return;

            Follow.transform.localPosition = Vector3.zero;

            StopAllCoroutines();
            CinemachineCamera.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        }
        private void OnGolfballStopppedMoving(Golfball golfball)
        {
            if (Player != golfball.Player)
                return;

            Follow.transform.localPosition = lookAtOffset;

            StopAllCoroutines();
            StartCoroutine(SetBindingModeDelayed(CinemachineTransposer.BindingMode.WorldSpace));
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            EventManager.Instance.PlayerEvents.ActivePlayerChanged   += OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.GolfballStartedMoving += OnGolfballStartedMoving;
            EventManager.Instance.PlayerEvents.GolfballStoppedMoving += OnGolfballStopppedMoving;
        }
        protected override void OnDisableOrQuit()
        {
            base.OnDisableOrQuit();

            EventManager.Instance.PlayerEvents.ActivePlayerChanged   -= OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.GolfballStartedMoving -= OnGolfballStartedMoving;
            EventManager.Instance.PlayerEvents.GolfballStoppedMoving -= OnGolfballStopppedMoving;
        }

        private void Awake()
        {
            _cameraWorldUpOverride = GameObject.FindWithTag("CameraWorldUpOverride").transform;
        }

        private void Update()
        {
            if (Player != MatchManager.Instance.ActivePlayer)
                return;

            Vector3 targetUp;
            float adaptionSpeed;

            if (Player.GolfballInstance.IsGrounded && !Player.GolfballInstance.IsInWater)
            {
                Vector3 projectedForward     = Vector3.ProjectOnPlane(MainCamera.Instance.CachedTransform.forward, Player.GolfballInstance.GroundNormal).normalized;
                float downwardAmount         = Mathf.Clamp01((Vector3.Dot(projectedForward, Player.GolfballInstance.GroundTangent) + 1) * 0.5f);
                float targetUpAdaptionAmount = cameraWorldUpAdaptionAmount * Mathf.Pow(downwardAmount, cameraWorldUpAdaptionPower);
                targetUp                     = Vector3.Lerp(Vector3.up, Player.GolfballInstance.GroundNormal, targetUpAdaptionAmount);

                adaptionSpeed  = cameraWorldUpAdaptionSpeed;
                adaptionSpeed += cameraWorldUpAdaptionSpeed * primaryDownwardAdaptionSpeedMult   * Mathf.Pow(downwardAmount, primaryDownwardAdaptionSpeedPower);
                adaptionSpeed += cameraWorldUpAdaptionSpeed * secondaryDownwardAdaptionSpeedMult * Mathf.Pow(downwardAmount, secondaryDownwardAdaptionSpeedPower);
            }
            else
            {
                targetUp      = Vector3.up;
                adaptionSpeed = airbornCameraWorldUpAdaptionSpeed;
            }

            _cameraWorldUpOverride.up = Vector3.SmoothDamp(_cameraWorldUpOverride.up, targetUp, ref cameraWorldUpAdaptionVelocity, Time.deltaTime / adaptionSpeed);
        }
    }
}
