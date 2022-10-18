using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    public class BouncyDrop : EntityBase
    {
        [SerializeField] private GameObject bouncyGoopPrefab;
        [SerializeField] private int MaxTurns;
        [SerializeField] private int MinTurns;
        [SerializeField] private bool normalCheck;

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 normal = collision.GetContact(0).normal;
            Vector3 binormal = Vector3.Cross(normal, Vector3.up);
            Vector3 tangent = Vector3.Cross(normal, binormal);

            Vector3 posUp = collision.GetContact(0).point + binormal;
            Vector3 posDown = collision.GetContact(0).point - binormal;

            Vector3 posLeft = collision.GetContact(0).point + tangent;
            Vector3 posRight = collision.GetContact(0).point - tangent;


            bool hasHit = true;

            if (normalCheck)
            {
                hasHit = hasHit && UnityEngine.Physics.Raycast(posUp, -collision.GetContact(0).normal, 0.1f, ~(1 << gameObject.layer));
                hasHit = hasHit && UnityEngine.Physics.Raycast(posDown, -collision.GetContact(0).normal, 0.1f, ~(1 << gameObject.layer));

                hasHit = hasHit && UnityEngine.Physics.Raycast(posLeft, -collision.GetContact(0).normal, 0.1f, ~(1 << gameObject.layer));
                hasHit = hasHit && UnityEngine.Physics.Raycast(posRight, -collision.GetContact(0).normal, 0.1f, ~(1 << gameObject.layer));
            }

            if (collision.gameObject.layer == 9 || collision.gameObject.layer == 7)
                UnityEngine.Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), collision.collider);
            else if (hasHit)
            {
                GameObject bouncyGoopObject = Instantiate(bouncyGoopPrefab, collision.GetContact(0).point, Quaternion.identity);
                BouncyGoop bouncyGoop = bouncyGoopObject.GetComponentInChildren<BouncyGoop>();
                bouncyGoop.maxTurns = MaxTurns;
                bouncyGoop.minTurns = MinTurns;

                bouncyGoopObject.transform.rotation = Quaternion.LookRotation(collision.GetContact(0).normal);
                bouncyGoopObject.transform.Rotate(90f, Random.rotation.y, 0f);

                Destroy(gameObject);
            }
        }
    }
}
