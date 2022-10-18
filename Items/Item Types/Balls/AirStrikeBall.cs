using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "AirStrikeBall", menuName = "Items/AirStrikeBall")]
    public class AirStrikeBall : Item
    {
        [SerializeField] private GameObject airStrikeBallModelPrefab;
        [SerializeField] private GameObject airStrikeBallPrefab;
        [SerializeField] private float flightTime   = 3.0f;
        [SerializeField] private float flightSpeed  = 25.0f;
        [SerializeField] private float dropInterval = 0.5f;

        public override void Use(Player player)
        {
            AirStrikeBallBehaviour airStrikeBall   = player.GolfballInstance.gameObject.AddComponent<AirStrikeBallBehaviour>();
            airStrikeBall.airStrikeBallModelPrefab = airStrikeBallModelPrefab;
            airStrikeBall.airStrikeBallPrefab      = airStrikeBallPrefab;
            airStrikeBall.flightTime               = flightTime;
            airStrikeBall.flightSpeed              = flightSpeed;
            airStrikeBall.dropInterval             = dropInterval;
            airStrikeBall.BeginFlight();
        }
    }
}
