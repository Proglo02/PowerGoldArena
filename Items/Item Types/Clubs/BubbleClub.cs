using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "BubbleClub", menuName = "Items/BubbleClub")]
    public class BubbleClub : Item
    {
        [SerializeField] private GameObject clubModelPrefab;
        [SerializeField] private VisualEffect bubbleSpewEffect;
        [SerializeField] private GameObject bubblePrefab;
        [SerializeField] private float bubbleSpewEffectMinVelocity = 0.1f;
        [SerializeField] private float bubbleSpawnInterval         = 0.025f;
        [SerializeField] private float bubbleSpeed                 = 3.0f;

        public override void Use(Player player)
        {
            BubbleBallBehaviour bubbleBall         = player.GolfballInstance.gameObject.AddComponent<BubbleBallBehaviour>();
            bubbleBall.clubModelPrefab             = clubModelPrefab;
            bubbleBall.bubbleSpewEffect            = bubbleSpewEffect;
            bubbleBall.bubblePrefab                = bubblePrefab;
            bubbleBall.bubbleSpewEffectMinVelocity = bubbleSpewEffectMinVelocity;
            bubbleBall.bubbleSpawnInterval         = bubbleSpawnInterval;
            bubbleBall.bubbleSpeed                 = bubbleSpeed;
        }
    }
}
