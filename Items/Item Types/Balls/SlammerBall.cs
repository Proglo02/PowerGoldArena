using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerGolfArena.Entities;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "SlammerBall", menuName = "Items/SlammerBall")]
    public class SlammerBall : Item
    {
        [SerializeField] private GameObject slammerBallModelPrefab;
        [SerializeField] private GameObject shockwavePrefab;
        [SerializeField] private float waveRadius                   = 10f;
        [SerializeField] private float moveDelayAfterExplosion      = 1.5f;
        [SerializeField] private float playerCollideHitstopDuration = 0.15f;
        [SerializeField] private float playerCollideRumbleDuration  = 0.35f;
        [SerializeField] private float playerCollideRumbleFrequency = 0.85f;

        public override void Use(Player player)
        {
            Golfball golfball           = player.GolfballInstance;
            golfball.Rigidbody.velocity = Vector3.down * 10;

            SlammerBallBehaviour slammerBall         = golfball.gameObject.AddComponent<SlammerBallBehaviour>();
            slammerBall.slammerBallModelPrefab       = slammerBallModelPrefab;
            slammerBall.shockwavePrefab              = shockwavePrefab;
            slammerBall.waveRadius                   = waveRadius;
            slammerBall.moveDelayAfterExplosion      = moveDelayAfterExplosion;
            slammerBall.playerCollideHitstopDuration = playerCollideHitstopDuration;
            slammerBall.playerCollideRumbleDuration  = playerCollideRumbleDuration;
            slammerBall.playerCollideRumbleFrequency = playerCollideRumbleFrequency;
        }
    }
}
