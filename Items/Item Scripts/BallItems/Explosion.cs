using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.VFX;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities.Items
{
    public class Explosion : EntityBase
    {
        [HideInInspector] public Player player;
        [HideInInspector] public float moveDelayAfterExplosion;

        [SerializeField] private VisualEffect explosionEffect;

        private bool _preventMove;

        public void SetRadius(float radius)
        {
            explosionEffect.SetFloat("ExplosionTexSize", radius);
        }

        private IEnumerator AllowMove(float delay)
        {
            yield return new WaitForSeconds(delay);

            _preventMove = false;
            player.CameraManager.GolfballCamera.CinemachineCamera.m_Orbits[1].m_Radius = 3.75f;
        }

        private void Start()
        {
            StartCoroutine(AllowMove(moveDelayAfterExplosion));
            _preventMove = true;
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.WantsToMoveCharacter     += WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToChangeFocus       += WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToTransitionPlayers += WantsToMove;
        }

        protected override void OnDestroyOrQuit()
        {
            EventManager.Instance.PlayerEvents.WantsToMoveCharacter     -= WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToChangeFocus       -= WantsToMove;
            EventManager.Instance.PlayerEvents.WantsToTransitionPlayers -= WantsToMove;
        }

        private bool WantsToMove()
        {
            return !_preventMove && !enabled;
        }
    }
}
