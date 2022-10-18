using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    [RequireComponent(typeof(Rigidbody))]
    public class BubbleBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject clubModelPrefab;
        [HideInInspector] public VisualEffect bubbleSpewEffect;
        [HideInInspector] public GameObject bubblePrefab;
        [HideInInspector] public float bubbleSpewEffectMinVelocity;
        [HideInInspector] public float bubbleSpawnInterval;
        [HideInInspector] public float bubbleSpeed;

        private List<Bubble> _bubbles = new List<Bubble>();
        private Transform _bubbleSpewEffectTransform;
        private Collider _collider;
        private bool _hasCollided;

        private IEnumerator CreateBubbles()
        {
            while (!_hasCollided)
            {
                Vector3 rotation         = new Vector3(Random.Range(-120, 60), Random.Range(-180, 180), 0);
                GameObject bubbleObject  = Instantiate(bubblePrefab, CachedTransform.position, Quaternion.Euler(rotation));
                Bubble bubble            = bubbleObject.GetComponent<Bubble>();
                _bubbles.Add(bubble);
                
                UnityEngine.Physics.IgnoreCollision(_collider, bubble.Collider);
                bubble.Rigidbody.AddForce(Random.insideUnitSphere * bubbleSpeed, ForceMode.Impulse);

                yield return new WaitForSeconds(bubbleSpawnInterval);
            }
        }

        private void OnShotBegan(Character character)
        {
            VisualEffect bubbleSpewEffectInstance = Instantiate(bubbleSpewEffect, CachedTransform);
            _bubbleSpewEffectTransform            = bubbleSpewEffectInstance.transform;

            StartCoroutine(CreateBubbles());
        }
        private void OnShotEnded(Character character)
        {
            foreach (Bubble bubble in _bubbles)
                UnityEngine.Physics.IgnoreCollision(_collider, bubble.Collider, false);

            Destroy(_bubbleSpewEffectTransform.gameObject);
            Destroy(this);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
            {
                Destroy(_bubbleSpewEffectTransform.gameObject);
                Destroy(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            _hasCollided = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.ShotBegan += OnShotBegan;
            EventManager.Instance.PlayerEvents.ShotEnded += OnShotEnded;
        }
        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.ShotBegan -= OnShotBegan;
            EventManager.Instance.PlayerEvents.ShotEnded -= OnShotEnded;
        }

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            Player.ModelManager.SetClubModel(clubModelPrefab);
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetClubModel();
        }

        private void Update()
        {
            float velocitySqr   = Player.GolfballInstance.Rigidbody.velocity.sqrMagnitude;
            float minVeloctySqr = bubbleSpewEffectMinVelocity * bubbleSpewEffectMinVelocity;

            if (velocitySqr > minVeloctySqr && Player.CharacterInstance.HasShot)
                _bubbleSpewEffectTransform.rotation = Quaternion.LookRotation(Player.GolfballInstance.Rigidbody.velocity);
        }
    }
}
