using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using PowerGolfArena.Audio;

namespace PowerGolfArena.Entities.Items
{
    public class BeeBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject beeBallModelPrefab;
        [HideInInspector] public GameObject beeSwarmPrefab;
        [HideInInspector] public float swarmTriggerHitstopDuration;
        [HideInInspector] public float swarmTriggerRumbleDuration;
        [HideInInspector] public float swarmTriggerRumbleFrequency;
        [HideInInspector] public int swarmMaxTurns;

        private void OnCollisionEnter(Collision collision)
        {
            GameObject beeSwarmObject       = Instantiate(beeSwarmPrefab, CachedTransform.position, Quaternion.identity);
            BeeSwarm beeSwarm               = beeSwarmObject.GetComponent<BeeSwarm>();
            beeSwarm.player                 = Player;
            beeSwarm.triggerHitstopDuration = swarmTriggerHitstopDuration;
            beeSwarm.triggerRumbleDuration  = swarmTriggerRumbleDuration;
            beeSwarm.triggerRumbleFrequency = swarmTriggerRumbleFrequency;
            beeSwarm.maxTurns               = swarmMaxTurns;

            Destroy(this);

            AudioManager.Instance.PlayOneShot(AudioManager.Instance.beehiveSplash, transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
                Destroy(this);
        }

        private void Start()
        {
            Player.ModelManager.SetGolfballModel(beeBallModelPrefab);
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetGolfballModel();
        }
    }
}
