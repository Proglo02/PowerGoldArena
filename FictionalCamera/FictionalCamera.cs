using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities
{
    public class FictionalCamera : MonoBehaviour
    {
        [SerializeField] private bool DodgeGolfball;
        [HideInInspector] public Vector3 targetPosition;
        [HideInInspector] public float timer;

        private Vector3 targetPositionAvoid;
        private bool _move = false;


        private void OnTriggerEnter(Collider other)
        {
            Golfball golfball = other.GetComponent<Golfball>();

            if (golfball && DodgeGolfball)
            {
                targetPositionAvoid = transform.position;
                targetPositionAvoid.y += 2.0f;
                _move = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Golfball golfball = other.GetComponent<Golfball>();

            if (golfball && DodgeGolfball)
            {
                targetPositionAvoid = transform.position;
                _move = false;
            }
        }

        private void Update()
        {
            if (_move)
                transform.position = Vector3.MoveTowards(transform.position, targetPositionAvoid, Time.deltaTime * 60f);
        }
    }
}
