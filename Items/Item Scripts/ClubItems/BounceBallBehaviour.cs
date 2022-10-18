using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Audio;

namespace PowerGolfArena.Entities.Items
{
    public class BounceBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject clubModelPrefab;
        [HideInInspector] public PhysicMaterial bounceMaterial;
        [HideInInspector] public GameObject bouncyDropPrefab;
        [HideInInspector] public GameObject bouncyDropModelPrefab;
        [HideInInspector] public float maxTime;
        [HideInInspector] public int NumGoops = 16;
        [HideInInspector] public int maxGoopTurns;

        private Collider _collider;

        private IEnumerator DestroyDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            _collider.material = null;
            Destroy(this);
        }

        private void OnShotBegan(Character character)
        {
            if (Player != character.Player)
                return;

            SphereCollider collider = gameObject.GetComponent<SphereCollider>();
            collider.material       = bounceMaterial;

            StartCoroutine(DestroyDelayed(maxTime));
        }
        private void OnShotEnded(Character character)
        {
            if (Player != character.Player)
                return;

            StopAllCoroutines();

            _collider.material = null;
            Destroy(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            for (int i = 0; i < NumGoops; i++)
            {
                Vector3 offset              = new Vector3(0f, 0.1f, 0f);
                GameObject bouncyDropObject = Instantiate(bouncyDropPrefab, collision.GetContact(0).point + offset, Quaternion.identity);
                Rigidbody bouncyDrop        = bouncyDropObject.GetComponent<Rigidbody>();
                float radians               = (2 * Mathf.PI) / NumGoops;
                Vector3 rotation            = new Vector3(0f, ((Mathf.Rad2Deg * radians * i) - 180) * -1, 45f);

                bouncyDropObject.transform.position += new Vector3(1 * Mathf.Cos(i * radians), 1, 1 * Mathf.Sin(i * radians));
                bouncyDropObject.transform.rotation  = Quaternion.Euler(rotation);

                bouncyDrop.AddForce(bouncyDropObject.transform.forward * Random.Range(1, 10), ForceMode.Impulse);
                bouncyDrop.AddForce(bouncyDropObject.transform.up      * Random.Range(1, 10), ForceMode.Impulse);
                UnityEngine.Physics.IgnoreCollision(_collider, bouncyDropObject.GetComponent<Collider>(), true);
            }

            AudioManager.Instance.PlayOneShot(AudioManager.Instance.bounceBall, transform.position);
        }

        private void OnTriggerEnter(Collider other)
        {
            CheckForKillzoneTrigger(other);
        }

        private void CheckForKillzoneTrigger(Collider other)
        {
            Killzone killzone = other.GetComponent<Killzone>();

            if (killzone)
            {
                _collider.material = null;
                Destroy(this);
            }
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
            Player.ModelManager.SetGolfballModel(bouncyDropModelPrefab);
            Player.ModelManager.SetClubModel(clubModelPrefab);
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetGolfballModel();
            Player.ModelManager.ResetClubModel();
        }
    }
}
