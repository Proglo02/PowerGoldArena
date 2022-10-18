using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using PowerGolfArena.Core;
using PowerGolfArena.Entities;
using PowerGolfArena.Input;

namespace PowerGolfArena.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class UIWinScreenManager : EntityBase
    {
        [SerializeField] private TextMeshProUGUI _ResultTypeText;
        [SerializeField] private TextMeshProUGUI _ResultText;
        [SerializeField] private List<Transform> _podiumTransform = new List<Transform>();
        [SerializeField] private List<Image> _playerIcons = new List<Image>();

        private List<Player> _players = new List<Player>();

        private int _menuIndex;

        private void Start()
        {
            foreach(Player player in MatchManager.Instance.Players)
            {
                _players.Add(player);
            }
            _players.Sort(SortPlayersByScore);

            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].CharacterInstance.enabled = true;
                _players[i].GolfballInstance.gameObject.SetActive(false);
                _players[i].GolfballInstance.CachedTransform.position = _podiumTransform[i].position;
                _players[i].GolfballInstance.CachedTransform.rotation = _podiumTransform[i].rotation;
                if (i < 3)
                    _players[i].CharacterInstance.CachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                else
                    _players[i].CharacterInstance.CachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 65f, 0f));

                if (_players[i].GolfballInstance.IsInWater)
                {
                    _players[i].ModelManager.CharacterModelInstance.Reset();
                    _players[i].CharacterInstance.AlignPositionWithBall();
                    _players[i].CharacterInstance.CachedTransform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                }
            }

            for (int i =0; i < _playerIcons.Count; i++)
            {
                if (i < _players.Count)
                {
                    _playerIcons[i].sprite = MatchManager.Instance.GetCharacterDataByPlayerID(_players[i].PlayerID).icon;
                }
                else
                    _playerIcons[i].color = Vector4.zero;
            }

            _menuIndex = 4;
            SwitchToDeaths();
        }

        private void OnBeginMatch(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                GameStates.Instance.SetStateMainMenu();
            }
        }

        private void OnCycleCharacter(Player player, InputAction.CallbackContext ctx)
        {
            if (ctx.phase == InputActionPhase.Performed)
            {
                SwitchStatistics(Mathf.RoundToInt(ctx.ReadValue<float>()));
            }
        }

        private void SwitchStatistics(int value)
        {
            _menuIndex += value;

            if (_menuIndex >= 5)
                _menuIndex = 0;
            else if (_menuIndex <= -1)
                _menuIndex = 4;

            switch(_menuIndex)
            {
                case 0: SwitchToItemsUsed(); 
                    break;

                case 1: SwitchToWallsBounced();
                    break;

                case 2: SwitchToDistanceTravelled(); 
                    break;

                case 3: SwitchToDeaths(); 
                    break;

                case 4: SwitchToLandedInWater(); 
                    break;
            }
        }

        private void SwitchToItemsUsed()
        {
            _ResultTypeText.text = "Items Used";
            _ResultTypeText.fontSize = 40;

            _ResultText.text = "";
            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                _ResultText.text = _ResultText.text + ": " + _players[i].itemsUsed + "\n";
            }
        }

        private void SwitchToWallsBounced()
        {
            _ResultTypeText.text = "Walls Bounced";
            _ResultTypeText.fontSize = 30;

            _ResultText.text = "";
            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                _ResultText.text = _ResultText.text + ": " + _players[i].wallsBounced + "\n";
            }
        }

        private void SwitchToDistanceTravelled()
        {
            _ResultTypeText.text = "Distance Travelled";
            _ResultTypeText.fontSize = 22;

            _ResultText.text = "";
            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                _ResultText.text = _ResultText.text + ": " + _players[i].distanceTravelled + "\n";
            }
        }

        private void SwitchToDeaths()
        {
            _ResultTypeText.text = "Deaths";
            _ResultTypeText.fontSize = 60;

            _ResultText.text = "";
            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                _ResultText.text = _ResultText.text + ": " + _players[i].deaths + "\n";
            }
        }

        private void SwitchToLandedInWater()
        {
            _ResultTypeText.text = "Landed In Water";
            _ResultTypeText.fontSize = 27;

            _ResultText.text = "";
            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                _ResultText.text = _ResultText.text + ": " + _players[i].landedInWater + "\n";
            }
        }

        private int SortPlayersByScore(Player player1, Player player2)
        {
            return player2.Score.CompareTo(player1.Score);
        }

        private void OnEnable()
        {
            InputManager.Instance.GameplayInputs.CycleCharacter += OnCycleCharacter;
            InputManager.Instance.MenuInputs.BeginMatch         += OnBeginMatch;
        }

        protected override void OnDisableOrQuit()
        {
            InputManager.Instance.GameplayInputs.CycleCharacter -= OnCycleCharacter;
            InputManager.Instance.MenuInputs.BeginMatch         -= OnBeginMatch;
        }
    }
}
