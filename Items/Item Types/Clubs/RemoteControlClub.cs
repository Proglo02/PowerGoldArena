using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "RemoteControlClub", menuName = "Items/RemoteControlClub")]
    public class RemoteControlClub : Item
    {
        [SerializeField] private GameObject clubModelPrefab;
        [SerializeField] private float airStrafeForce = 50;

        public override void Use(Player player)
        {
            player.GolfballInstance.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

            RemoteControlBallBehaviour remoteControlBall = player.GolfballInstance.gameObject.AddComponent<RemoteControlBallBehaviour>();
            remoteControlBall.clubModelPrefab            = clubModelPrefab;
            remoteControlBall.airStrafeForce             = airStrafeForce;
        }
    }
}
