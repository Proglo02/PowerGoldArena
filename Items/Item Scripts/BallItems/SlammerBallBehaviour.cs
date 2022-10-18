using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using PowerGolfArena.Core;

namespace PowerGolfArena.Entities.Items
{
    public class SlammerBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject slammerBallModelPrefab;
        [HideInInspector] public GameObject shockwavePrefab;
        [HideInInspector] public float waveRadius;
        [HideInInspector] public float moveDelayAfterExplosion;
        [HideInInspector] public float playerCollideHitstopDuration;
        [HideInInspector] public float playerCollideRumbleDuration;
        [HideInInspector] public float playerCollideRumbleFrequency;

        private bool _isExploding;
        private float _explosionCameraRadius;

        private void OnCollisionEnter(Collision collision)
        {
            Golfball golfball    = Player.GolfballInstance;
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(golfball.transform.position, waveRadius);

            foreach (Collider collider in colliders)
            {
                Character character = collider.gameObject.GetComponent<Character>();
                if (character && character != golfball.Character && !character.HasBeenHit)
                    character.OnHit(Player, playerCollideHitstopDuration, playerCollideRumbleDuration, playerCollideRumbleFrequency, false);
            }

            GameObject shockwaveObject = Instantiate(shockwavePrefab, CachedTransform.position, Quaternion.identity);
            Explosion shockwave        = shockwaveObject.GetComponent<Explosion>();
            shockwave.SetRadius(waveRadius);
            shockwave.moveDelayAfterExplosion = moveDelayAfterExplosion;
            shockwave.player = Player;
            _isExploding = false;
            Destroy(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
            {
                Player.CameraManager.GolfballCamera.CinemachineCamera.m_Orbits[1].m_Radius = 3.75f;
                Destroy(this);
            }
        }

        private void Start()
        {
            Player.ModelManager.SetGolfballModel(slammerBallModelPrefab);
            Player.CameraManager.GolfballCamera.CinemachineCamera.m_Orbits[1].m_Radius = 15f;
            _isExploding = true;
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetGolfballModel();
        }
    }
}
