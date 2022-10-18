using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    public class ItemModel : EntityBase
    {
        [SerializeField] private Transform dissolveTransform;

        public float dissolveSpeed;

        private float _dissolveAmount;
        public float DissolveAmount
        {
            get => _dissolveAmount;
            set
            {
                _dissolveAmount = value;
                foreach (Renderer renderer in _renderers)
                {
                    if (!renderer)
                        continue;

                    renderer.material.SetFloat("_DissolveAmount", value);
                }
            }
        }

        private float _playerPosition;
        private float PlayerPosition
        {
            get => _playerPosition;
            set
            {
                _playerPosition = value;
                foreach (Renderer renderer in _renderers)
                {
                    if (!renderer)
                        continue;

                    renderer.material.SetFloat("_PlayerPosition", value);
                }
            }
        }

        private Renderer[] _renderers;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();

            DissolveAmount = 1;
        }

        private void Update()
        {
            PlayerPosition = dissolveTransform.position.y;
        }
    }
}
