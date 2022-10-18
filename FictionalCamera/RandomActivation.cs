using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Audio;
using PowerGolfArena.Core;
using Cinemachine;

namespace PowerGolfArena.Entities
{
    public class RandomActivation : EntityBase
    {
        [SerializeField] private int    maxWallActiveRounds;
        [SerializeField] private float  maxItemDroneActiveTime;
        [SerializeField] private List<GameObject> effectItems = new List<GameObject>();
        [SerializeField] private float  activationChance;
        [SerializeField] private float BubbleDropInterval = 1.0f;
        [SerializeField] private float GoopDropInterval = 0.5f;

        [SerializeField] private List<GameObject> _walls = new List<GameObject>();
        [SerializeField] private List<GameObject> _itemDrones = new List<GameObject>();

        [Header("SFX")]
        public FMODUnity.EventReference announcerRandomEvent;

        private int _round = 0;
        private int _wallActivationRound;
        private float _droneActiveTime;

        private bool _activate = false;
        private bool _isActive = false;
        private bool _wallsActive = false;
        private bool _itemDronesActive = false;

        private void Awake()
        {
            if (!MatchManager.Instance.randomEvent)
                gameObject.SetActive(false);
            else
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    foreach (GameObject wall in _walls)
                        wall.SetActive(false);

                    foreach (GameObject itemDrone in _itemDrones)
                        itemDrone.SetActive(false);
                }
            }
        }

        private void Update()
        {
             if (_activate)
             {
                _wallsActive = Random.value > 0.5f;

                // temporary wall deactivation
                _wallsActive = false;

                _itemDronesActive = !_wallsActive;

                _isActive = true;
                _activate = false;

                if (_wallsActive)
                {
                    foreach (GameObject wall in _walls)
                        wall.SetActive(true);

                    _wallActivationRound = _round;
                }
                else if (_itemDronesActive)
                {

                    int range = Random.Range(0, effectItems.Count);
                    GameObject effect = effectItems[range];



                    foreach (GameObject itemDrone in _itemDrones)
                    {
                        itemDrone.SetActive(true);
                        itemDrone.GetComponent<FictionalMapItemDroneManager>().effectItem = effect;
                        itemDrone.GetComponent<FictionalMapItemDroneManager>().interval = GoopDropInterval;
                        if (range == 0)
                            itemDrone.GetComponent<FictionalMapItemDroneManager>().interval = BubbleDropInterval;
                    }
                }

                AudioManager.Instance.PlayOneShot(announcerRandomEvent);
                EventManager.Instance.PlayerEvents.RandomEventActivated?.Invoke();
            }

            if (_wallsActive)
            {
                if (_round >= _wallActivationRound + maxWallActiveRounds)
                {
                    _wallActivationRound = 0;
                    _round = 0;
                    _isActive = false;
                    _wallsActive = false;

                    foreach (GameObject wall in _walls)
                        wall.SetActive(false);
                }
            }
            else if (_itemDronesActive)
            {
                if (_droneActiveTime >= maxItemDroneActiveTime)
                {
                    _round = 0;
                    _droneActiveTime = 0;
                    _isActive = false;
                    _itemDronesActive = false;

                    foreach (GameObject itemDrone in _itemDrones)
                        itemDrone.SetActive(false);
                }
                else
                    _droneActiveTime += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            EventManager.Instance.PlayerEvents.ActivePlayerChanged += OnActivePlayerChanged;
        }

        protected override void OnDisableOrQuit()
        {
            EventManager.Instance.PlayerEvents.ActivePlayerChanged -= OnActivePlayerChanged;
        }

        private void OnActivePlayerChanged(Player player)
        {
            _round++;

            if (_round >= 4 && !_isActive)
                _activate = Random.value <= activationChance;
        }
    }
}
