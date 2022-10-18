using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    public class AirStrikeBallBehaviour : ItemBehaviourBase
    {
        [HideInInspector] public GameObject airStrikeBallModelPrefab;
        [HideInInspector] public GameObject airStrikeBallPrefab;
        [HideInInspector] public float flightTime;
        [HideInInspector] public float flightSpeed;
        [HideInInspector] public float dropInterval;

        private Vector3 _velocity;
        private float _drag;
        private bool _isDroppingBalls;

        private IEnumerator StopDroppingDelayed(float delay)
        {
            yield return new WaitForSeconds(delay);

            _isDroppingBalls = false;
        }
        private IEnumerator DropBalls()
        {
            while (_isDroppingBalls)
            {
                GameObject airStrikeBall = Instantiate(airStrikeBallPrefab, CachedTransform.position, Quaternion.identity);
                airStrikeBall.GetComponent<Rigidbody>().AddForce(Player.GolfballInstance.Rigidbody.velocity * 2, ForceMode.Impulse);

                yield return new WaitForSeconds(dropInterval);
            }

            Rigidbody rigidbody  = GetComponent<Rigidbody>();
            rigidbody.useGravity = true;
            rigidbody.drag       = _drag;
            Player.ModelManager.ResetGolfballModel();
            Destroy(this);
        }

        public void BeginFlight()
        {
            Rigidbody rigidbody  = GetComponent<Rigidbody>();
            rigidbody.angularVelocity = Vector3.zero;
            _velocity            = rigidbody.velocity.normalized * flightSpeed;
            _velocity.y          = 0f;
            rigidbody.useGravity = false;
            _drag                = rigidbody.drag;
            rigidbody.drag       = 0.0f;
            rigidbody.velocity   = _velocity;
            _isDroppingBalls     = true;

            StartCoroutine(DropBalls());
            StartCoroutine(StopDroppingDelayed(flightTime));
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
                Destroy(this);
                Rigidbody rigidbody = GetComponent<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.drag = _drag;
            }
        }

        private void Start()
        {
            Player.ModelManager.SetGolfballModel(airStrikeBallModelPrefab);
        }
        protected override void OnDestroyOrQuit()
        {
            Player.ModelManager.ResetGolfballModel();
        }
    }
}
