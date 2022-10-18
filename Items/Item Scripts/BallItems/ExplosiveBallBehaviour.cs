using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Entities;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;
using Cinemachine;

namespace PowerGolfArena.Entities.Items
{
    public class ExplosiveBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject explosiveBallModelPrefab;
        [HideInInspector] public GameObject explosionPrefab;
        [HideInInspector] public float fuseTime;
        [HideInInspector] public float explosionRadius;
        [HideInInspector] public float moveDelayAfterExplosion;
        [HideInInspector] public float playerExplodeHitstopDuration;
        [HideInInspector] public float playerExplodeRumbleDuration;
        [HideInInspector] public float playerExplodeRumbleFrequency;

        private CinemachineFreeLook _cinemachineCamera;
        private float _explosionCameraRadius;
        private bool _isExploding = false;

        private IEnumerator ExplodeDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            Explode();
        }

        private void Explode()
        {
            Golfball golfball    = Player.GolfballInstance;
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(golfball.transform.position, explosionRadius);

            foreach (Collider collider in colliders)
            {
                Character character = collider.gameObject.GetComponent<Character>();
                if (character && character != golfball.Character)
                    character.OnHit(Player, playerExplodeHitstopDuration, playerExplodeRumbleDuration, playerExplodeRumbleFrequency, false);
            }

            GameObject explosionObject        = Instantiate(explosionPrefab, CachedTransform.position, Quaternion.identity);
            Explosion explosion               = explosionObject.GetComponent<Explosion>();
            explosion.player                  = Player;
            explosion.moveDelayAfterExplosion = moveDelayAfterExplosion;
            explosion.SetRadius(explosionRadius);

            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();
            _isExploding = false;

            if (killzone)
            {
                _cinemachineCamera.m_Orbits[1].m_Radius = 3.75f;
                Destroy(this);
            }
        }

        private void Update()
        {
            if (_isExploding && _explosionCameraRadius < 15.0f)
            {
                _explosionCameraRadius += Time.deltaTime * 10.0f;
                _cinemachineCamera.m_Orbits[1].m_Radius = _explosionCameraRadius;
            }
        }

        private void Start()
        {
            StartCoroutine(ExplodeDelayed(fuseTime));

            Player.ModelManager.SetGolfballModel(explosiveBallModelPrefab);
            _cinemachineCamera = Player.CameraManager.GolfballCamera.CinemachineCamera;
            _isExploding         = true;
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.WantsToMoveCharacter += WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToChangeFocus += WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToTransitionPlayers += WantsToMove;
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetGolfballModel();
            EventManager.Instance.PlayerEvents.WantsToMoveCharacter     -= WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToChangeFocus       -= WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToTransitionPlayers -= WantsToMove;
        }

        private bool WantsToMove()
        {
            return !_isExploding;
        }
    }
}
