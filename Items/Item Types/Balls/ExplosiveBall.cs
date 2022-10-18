using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerGolfArena.Entities;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "ExplosiveBall", menuName = "Items/ExplosiveBall")]
    public class ExplosiveBall : Item
    {
        [SerializeField] private GameObject explosiveBallModelPrefab;
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float fuseTime                     = 3.0f;
        [SerializeField] private float explosionRadius              = 10.0f;
        [SerializeField] private float moveDelayAfterExplosion      = 1.5f;
        [SerializeField] private float playerExplodeHitstopDuration = 0.15f;
        [SerializeField] private float playerExplodeRumbleDuration  = 0.35f;
        [SerializeField] private float playerExplodeRumbleFrequency = 0.85f;

        public override void Use(Player player)
        {
            ExplosiveBallBehaviour explosiveBall       = player.GolfballInstance.gameObject.AddComponent<ExplosiveBallBehaviour>();
            explosiveBall.explosiveBallModelPrefab     = explosiveBallModelPrefab;
            explosiveBall.explosionPrefab              = explosionPrefab;
            explosiveBall.fuseTime                     = fuseTime;
            explosiveBall.explosionRadius              = explosionRadius;
            explosiveBall.moveDelayAfterExplosion      = moveDelayAfterExplosion;
            explosiveBall.playerExplodeHitstopDuration = playerExplodeHitstopDuration;
            explosiveBall.playerExplodeRumbleDuration  = playerExplodeRumbleDuration;
            explosiveBall.playerExplodeRumbleFrequency = playerExplodeRumbleFrequency;
        }
    }
}
