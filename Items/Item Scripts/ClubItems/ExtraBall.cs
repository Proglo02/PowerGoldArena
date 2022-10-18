using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Physics;
using PowerGolfArena.Core;
using PowerGolfArena.Audio;

namespace PowerGolfArena.Entities
{
    [RequireComponent(typeof(SphereCollider), typeof(MovementChecker), typeof(TerrainPhysicsReceiver))]
    public class ExtraBall : EntityBase
    {
        [SerializeField] private bool checkCollision = true;
        [SerializeField] private float magnusEffect  = 0.25f;

        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (!_rigidbody)
                    _rigidbody = GetComponent<Rigidbody>();

                return _rigidbody;
            }
        }

        private SphereCollider _collider;
        public SphereCollider Collider
        {
            get
            {
                if (!_collider)
                    _collider = GetComponent<SphereCollider>();

                return _collider;
            }
        }

        private GroundChecker _groundChecker;
        public GroundChecker GroundChecker
        {
            get
            {
                if (!_groundChecker)
                    _groundChecker = GetComponent<GroundChecker>();

                return _groundChecker;
            }
        }

        private MovementChecker _movementChecker;
        public MovementChecker MovementChecker
        {
            get
            {
                if (!_movementChecker)
                    _movementChecker = GetComponent<MovementChecker>();

                return _movementChecker;
            }
        }

        public bool IsGrounded => GroundChecker.IsGrounded;
        public bool IsMoving => MovementChecker.IsMoving;
        public Vector3 GroundNormal => GroundChecker.GroundNormal;
        public Vector3 GroundBinormal => GroundChecker.GroundBinormal;
        public Vector3 GroundTangent => GroundChecker.GroundTangent;

        private Player _player;

        private void CheckForCharacterCollision(Collision collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            if (character && _player.CharacterInstance != character && !character.HasBeenHit && checkCollision)
            {
                Golfball golfball     = _player.GolfballInstance;
                bool headHit = collision.collider.GetType() == typeof(SphereCollider);
                float velocity        = Rigidbody.velocity.magnitude;
                float hitstopDuration = golfball.PlayerCollisionHitstopDuration * velocity * golfball.PlayerCollisionHitstopVelocityImpact;
                float rumbleFrequency = golfball.PlayerCollisionRumbleFrequency * velocity * golfball.PlayerCollisionRumbleVelocityImpact;
                _player.CharacterInstance.Inventory.AddItem(character.Inventory.Item);
                character.RemoveItem();

                character.OnHit(_player, hitstopDuration, golfball.PlayerCollisionRumbleDuration, rumbleFrequency, headHit);
            }
        }
        private void CheckForTerrainCollision(Collision collision)
        {
            // play sfx
            TerrainPhysicsProvider terrainPhysicsProvider = collision.gameObject.GetComponent<TerrainPhysicsProvider>();
            if (!terrainPhysicsProvider) return;

            AudioManager.Instance.PlayOneShot(AudioManager.Instance.ballHitTerrain, collision.GetContact(0).point, "Velocity", Rigidbody.velocity.magnitude, "Terrain", (int)terrainPhysicsProvider.Material.ImpactSoundEffect);
        }
        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
                Destroy(gameObject);
        }

        private void ApplyMagnusEffect()
        {
            float magnusMagnitude = Rigidbody.velocity.magnitude * Mathf.Pow(2 * Mathf.PI * 0.1f, 2) * Rigidbody.angularVelocity.magnitude * 0.1f;
            Vector3 magnusForce = Vector3.Cross(Rigidbody.angularVelocity, Rigidbody.velocity);
            magnusForce = magnusForce.normalized;
            Rigidbody.AddForce(magnusForce * magnusMagnitude * magnusEffect, ForceMode.Force);
        }

        private void OnStoppedMoving()
        {
            Destroy(gameObject);
        }

        private void Update()
        {
            if (!IsGrounded)
                ApplyMagnusEffect();
        }

        private void OnEnable()
        {
            MovementChecker.StoppedMoving += OnStoppedMoving;
        }
        protected override void OnDisableOrQuit()
        {
            MovementChecker.StoppedMoving -= OnStoppedMoving;
        }

        private void Awake()
        {
            _player = MatchManager.Instance.ActivePlayer;
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }
        private void OnCollisionEnter(Collision collision)
        {
            CheckForCharacterCollision(collision);
            CheckForTerrainCollision(collision);
        }
    }
}
