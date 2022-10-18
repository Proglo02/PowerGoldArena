using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    public class Bubble : EntityBase
    {
        [SerializeField] public int maxTurns;
        [SerializeField] public int minTurns;

        private int _startTurn;
        private int _turnsActive = 0;

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

        private void OnCollisionEnter(Collision collision)
        {
            Golfball golfball = collision.gameObject.GetComponent<Golfball>();
            if (golfball)
                Destroy(gameObject, 0.5f);
        }

        private void Awake()
        {
            _startTurn = MatchManager.Instance.TurnCount;
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.ActivePlayerChanged += OnActivePlayerChanged;
        }
        protected override void OnDestroyOrQuit()
        {
            EventManager.Instance.GameEvents.BeforeColliderRemoved?.Invoke(Collider);

            EventManager.Instance.PlayerEvents.ActivePlayerChanged -= OnActivePlayerChanged;
        }

        private void FixedUpdate()
        {
             Rigidbody.AddForce(Random.insideUnitSphere * 1.25f, ForceMode.Force);
        }

        private void OnActivePlayerChanged(Player player)
        {
            int turncount = MatchManager.Instance.TurnCount;

            if (turncount >= _startTurn + maxTurns)
                Destroy(gameObject);

            else if (turncount >= _startTurn + minTurns)
            {
                _turnsActive++;

                int range = Random.Range(_turnsActive, maxTurns - minTurns);
                if (range == maxTurns - minTurns)
                    Destroy(gameObject);
            }
        }
    }
}
