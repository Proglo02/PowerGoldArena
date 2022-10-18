using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities
{
    public class CrowdManager : EntityBase
    {
        [SerializeField] private List<Material> materials = new List<Material>();
        [SerializeField] private float interval;
        [SerializeField] private float cheerTime;

        private List<float> timers = new List<float>();
        private List<float> startTimes = new List<float>();

        private bool characterHit;

        private float characterHitTimer;

        private float sit       = 0.5f;
        private float halfCheer = 1.5f;
        private float fullCheer = 2.5f;

        private void Awake()
        {
            for (int i = 0; i < materials.Count; i++)
            {
                float time = (interval / materials.Count) * i;
                timers.Add(time);
                startTimes.Add(time * 0.5f);
            }
        }

        private void Update()
        {
            for(int i = 0; i < materials.Count; i++)
            {
                if (timers[i] >= interval)
                {
                    if (!characterHit)
                    {
                        if (materials[i].GetFloat("_Flipbook_Tile") > 1.0f)
                            materials[i].SetFloat("_Flipbook_Tile", sit);
                        else
                            materials[i].SetFloat("_Flipbook_Tile", halfCheer);
                    }
                    else
                    {
                        if (materials[i].GetFloat("_Flipbook_Tile") > 2.0f)
                            materials[i].SetFloat("_Flipbook_Tile", halfCheer);
                        else
                            materials[i].SetFloat("_Flipbook_Tile", fullCheer);
                    }
                    timers[i] = startTimes[i];
                }
                else
                    timers[i] += Time.deltaTime;
            }

            if (characterHitTimer >= cheerTime)
            {
                characterHit = false;
                characterHitTimer = 0.0f;
            }
            else
                characterHitTimer += Time.deltaTime;
        }

        private void OnCharacterHit(Character character)
        {
            characterHit = true;
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.CharacterHit += OnCharacterHit;
        }

        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.CharacterHit -= OnCharacterHit;
        }
    }
}
