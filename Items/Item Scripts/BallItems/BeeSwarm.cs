using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    [RequireComponent(typeof(Collider))]
    public class BeeSwarm : EntityBase
    {
        [SerializeField] private VisualEffect beeHiveAOEEffect;

        [HideInInspector] public Player player;
        [HideInInspector] public GameObject beeBallModelPrefab;
        [HideInInspector] public float triggerHitstopDuration;
        [HideInInspector] public float triggerRumbleDuration;
        [HideInInspector] public float triggerRumbleFrequency;
        [HideInInspector] public int maxTurns;

        private Collider _collider;
        private int _startTurn;

        private void CheckForCharacterTrigger(Collider other)
        {
            Character character = other.gameObject.GetComponent<Character>();
            bool headHit = other.GetType() == typeof(SphereCollider);
            if (character && character.Player != player)
                character.OnHit(player, triggerHitstopDuration, triggerRumbleDuration, triggerRumbleFrequency, headHit);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForCharacterTrigger(other);
        }

        private void OnActivePlayerChanged(Player player)
        {
            if (MatchManager.Instance.TurnCount >= _startTurn + maxTurns)
                Destroy(gameObject);
        }

        private void Awake()
        {
            _collider           = GetComponent<Collider>();
            _collider.isTrigger = true;

            _startTurn = MatchManager.Instance.TurnCount;
        }
        private void Start()
        {
            beeHiveAOEEffect.SetVector4("PlayerColor", (Vector4)MatchManager.Instance.PlayerColors[player.PlayerID].coreColor);
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.ActivePlayerChanged += OnActivePlayerChanged;
        }
        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.ActivePlayerChanged -= OnActivePlayerChanged;
        }
    }
}
