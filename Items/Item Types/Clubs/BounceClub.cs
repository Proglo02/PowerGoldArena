using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "BounceClub", menuName = "Items/BounceClub")]
    public class BounceClub : Item
    {
        [SerializeField] private GameObject clubModelPrefab;
        [SerializeField] private PhysicMaterial bounceMaterial;
        [SerializeField] private GameObject bouncyDropPrefab;
        [SerializeField] private GameObject bouncyDropModelPrefab;
        [SerializeField] private float maxTime    = 7.0f;
        [SerializeField] private int maxGoopTurns = 4;

        public override void Use(Player player)
        {
            BounceBallBehaviour bounceBall   = player.GolfballInstance.gameObject.AddComponent<BounceBallBehaviour>();
            bounceBall.clubModelPrefab       = clubModelPrefab;
            bounceBall.bounceMaterial        = bounceMaterial;
            bounceBall.bouncyDropPrefab      = bouncyDropPrefab;
            bounceBall.bouncyDropModelPrefab = bouncyDropModelPrefab;
            bounceBall.maxTime               = maxTime;
            bounceBall.maxGoopTurns          = maxGoopTurns;
        }
    }
}
