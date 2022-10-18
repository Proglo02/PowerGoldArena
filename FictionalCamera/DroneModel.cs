using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PowerGolfArena.Entities
{
    public class DroneModel : EntityBase
    {
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

        private void Start()
        {
            _renderers = GetComponentsInChildren<Renderer>();
        }

        private void Update()
        {
            PlayerPosition = CachedTransform.position.y;
        }
    } 
}
