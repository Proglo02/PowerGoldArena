using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "ScatterClub", menuName = "Items/ScatterClub")]
    public class ScatterClub : Item
    {
        [SerializeField] private GameObject clubModelPrefab;
        [SerializeField] private GameObject scatterBallPrefab;
        [SerializeField] private GameObject scatterBallModelPrefab;
        [SerializeField] private float positionDeviation = 0.05f;
        [SerializeField] private float velocityDeviation = 0.15f;
        [SerializeField] private int numBalls            = 8;

        private Player _player;

        private IEnumerator SpawnScatterBallsDelayed(int frameDelay)
        {
            for (int i = 0; i < frameDelay; i++)
                yield return new WaitForFixedUpdate();

            SpawnScatterBalls();
        }
        private void SpawnScatterBalls()
        {
            Golfball golfball = _player.GolfballInstance;
            for (int i = 0; i < numBalls; i++)
            {
                Vector3 position    = golfball.CachedTransform.position + Random.insideUnitSphere * positionDeviation;
                Quaternion rotation = Random.rotation;

                GameObject scatterBallObject  = Instantiate(scatterBallPrefab, position, rotation);
                ExtraBall scatterBallInstance = scatterBallObject.GetComponent<ExtraBall>();
                UnityEngine.Physics.IgnoreCollision(golfball.Character.Collider, scatterBallInstance.Collider);

                Vector3 velocity                       = golfball.Rigidbody.velocity + Random.insideUnitSphere * velocityDeviation;
                scatterBallInstance.Rigidbody.velocity = velocity;

                scatterBallInstance.Rigidbody.angularDrag = golfball.Rigidbody.angularDrag;
                scatterBallInstance.Rigidbody.drag        = golfball.Rigidbody.drag;
            }
        }

        private void OnShotBegan(Character character)
        {
            if (_player != character.Player)
                return;

            _player.StartCoroutine(SpawnScatterBallsDelayed(1));

            EventManager.Instance.PlayerEvents.ShotBegan -= OnShotBegan;
        }
        private void OnShotEnded(Character character)
        {
            if (_player != character.Player)
                return;

            _player.ModelManager.ResetGolfballModel();
            _player.ModelManager.ResetClubModel();

            EventManager.Instance.PlayerEvents.ShotBegan -= OnShotEnded;
        }

        public override void Use(Player player)
        {
            _player = player;
            _player.ModelManager.SetGolfballModel(scatterBallModelPrefab);
            _player.ModelManager.SetClubModel(clubModelPrefab);

            EventManager.Instance.PlayerEvents.ShotBegan += OnShotBegan;
            EventManager.Instance.PlayerEvents.ShotEnded += OnShotEnded;
        }
    }
}
