using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "BeeBall", menuName = "Items/BeeBall")]
    public class BeeBall : Item
    {
        [SerializeField] private GameObject beeBallModelPrefab;
        [SerializeField] private GameObject beeSwarmPrefab;
        [SerializeField] private float swarmTriggerHitstopDuration = 0.15f;
        [SerializeField] private float swarmTriggerRumbleDuration  = 0.2f;
        [SerializeField] private float swarmTriggerRumbleFrequency = 0.65f;
        [SerializeField] private int swarmMaxTurns                 = 3;

        public override void Use(Player player)
        {
            BeeBallBehaviour beeBall            = player.GolfballInstance.gameObject.AddComponent<BeeBallBehaviour>();
            beeBall.beeBallModelPrefab          = beeBallModelPrefab;
            beeBall.beeSwarmPrefab              = beeSwarmPrefab;
            beeBall.swarmTriggerHitstopDuration = swarmTriggerHitstopDuration;
            beeBall.swarmTriggerRumbleDuration  = swarmTriggerRumbleDuration;
            beeBall.swarmTriggerRumbleFrequency = swarmTriggerRumbleFrequency;
            beeBall.swarmMaxTurns               = swarmMaxTurns;
        }
    }
}
