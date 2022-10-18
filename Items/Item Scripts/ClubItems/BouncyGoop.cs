using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    public class BouncyGoop : EntityBase
{
        [HideInInspector] public int maxTurns;
        [HideInInspector] public int minTurns;

        private int _startTurn;
        private int _turnsActive = 0;

        private MeshCollider _collider;
        public MeshCollider Collider
        {
            get
            {
                if (!_collider)
                    _collider = GetComponent<MeshCollider>();

                return _collider;
            }
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
    }
}
