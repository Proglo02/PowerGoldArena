using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Core;
using PowerGolfArena.UI;
using PowerGolfArena.Entities.Items;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.Entities
{
    public class UIMiniMap : EntityBase
    {
        [SerializeField] private float MapZoom;
        [SerializeField] private float MapPlaySize = 165.0f;
        [SerializeField] private float MapSize     = 256.0f;
        [SerializeField] private GameObject PlayerIconPrefab;
        [SerializeField] private GameObject PlayerArrowPrefab;
        [SerializeField] private GameObject PlayerArrowBackPrefab;
        [SerializeField] private GameObject ItemPrefab;
        [SerializeField] private Sprite ItemBallSprite;
        [SerializeField] private Sprite ItemClubSprite;

        private RectTransform _rectTransform;
        private RectTransform _childTransform;

        private List<UIMinimapIcon> _playerIcons      = new List<UIMinimapIcon>();
        private List<UIMinimapIcon> _playerArrows     = new List<UIMinimapIcon>();
        private List<UIMinimapIcon> _playerArrowBacks = new List<UIMinimapIcon>();
        private List<UIMinimapIcon> _itemIcons        = new List<UIMinimapIcon>();
        private List<ItemObject> _itemObjects         = new List<ItemObject>();

        private void Awake()
        {
            _rectTransform             = GetComponent<RectTransform>();
            _childTransform            = GetComponent<RectTransform>().GetChild(1).GetComponent<RectTransform>();
            _childTransform.localScale = new Vector3(MapZoom, MapZoom, 1f);

            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                GameObject playerIcon                                 = Instantiate(PlayerIconPrefab, _rectTransform);
                GameObject playerArrowBack                            = Instantiate(PlayerArrowBackPrefab, _rectTransform);
                GameObject playerArrow                                = Instantiate(PlayerArrowPrefab, _rectTransform);
                playerIcon.GetComponent<UIMinimapIcon>().image.sprite = MatchManager.Instance.GetCharacterDataByPlayerID(i).icon;
                playerArrow.GetComponent<UIMinimapIcon>().image.color = MatchManager.Instance.PlayerColors[i].coreColor;
                _playerIcons.Add(playerIcon.GetComponent<UIMinimapIcon>());
                _playerArrows.Add(playerArrow.GetComponent<UIMinimapIcon>());
                _playerArrowBacks.Add(playerArrowBack.GetComponent<UIMinimapIcon>());
            }

            EventManager.Instance.PlayerEvents.ItemCreated                    += OnItemCreated;
            EventManager.Instance.PlayerEvents.ItemDestroyed                  += OnItemDestroyed;
            EventManager.Instance.PlayerEvents.ActivePlayerChanged            += OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.ShotEnded                      += OnShotEnded;
            EventManager.Instance.PlayerEvents.PlayerTransitionFadeOutStarted += OnPlayerTransitionFadeOutStarted;
            EventManager.Instance.PlayerEvents.CharacterHit                   += OnCharacterHit;
            StartCoroutine(OnMapChangeDelayed(1));
        }
        protected override void OnDestroyOrQuit()
        {
            EventManager.Instance.PlayerEvents.ItemCreated                    -= OnItemCreated;
            EventManager.Instance.PlayerEvents.ItemDestroyed                  -= OnItemDestroyed;
            EventManager.Instance.PlayerEvents.ActivePlayerChanged            -= OnActivePlayerChanged;
            EventManager.Instance.PlayerEvents.ShotEnded                      -= OnShotEnded;
            EventManager.Instance.PlayerEvents.PlayerTransitionFadeOutStarted -= OnPlayerTransitionFadeOutStarted;
            EventManager.Instance.PlayerEvents.CharacterHit                   -= OnCharacterHit;
        }

        private void Update()
        {
            if (!MatchManager.Instance.ActivePlayer)
                return;

            Quaternion cameraRotation  = MainCamera.Instance.CachedTransform.rotation;
            Vector3 rotation           = _rectTransform.eulerAngles;
            rotation.z                 = cameraRotation.eulerAngles.y;
            _rectTransform.eulerAngles = rotation;

            Vector3 activeGolfballPos   = MatchManager.Instance.ActivePlayer.GolfballInstance.CachedTransform.position * -(MapSize / MapPlaySize) * MapZoom;
            Vector3 newActiveAncorPos = _childTransform.anchoredPosition;

            newActiveAncorPos.x = activeGolfballPos.x;
            newActiveAncorPos.y = activeGolfballPos.z;
            _childTransform.anchoredPosition = newActiveAncorPos;
            OnMapChange();
        }

        private void OnItemCreated(ItemObject itemObject)
        {
            _itemObjects.Add(itemObject);
            int index = _itemObjects.IndexOf(itemObject);

            GameObject itemIcon = Instantiate(ItemPrefab, _rectTransform);
            _itemIcons.Add(itemIcon.GetComponent<UIMinimapIcon>());

            if (_itemObjects[index].item.type == Item.Type.Ball)
                _itemIcons[index].image.sprite = ItemBallSprite;
            else
                _itemIcons[index].image.sprite = ItemClubSprite;

            Vector3 offset      = (MatchManager.Instance.ActivePlayer.GolfballInstance.CachedTransform.position - _itemObjects[index].CachedTransform.position) * -(MapSize / MapPlaySize) * MapZoom;
            Vector3 newAncorPos = _itemIcons[index].rectTransform.anchoredPosition;

            newAncorPos.x = offset.x;
            newAncorPos.y = offset.z;

            _itemIcons[index].rectTransform.anchoredPosition = newAncorPos;
        }

        private void OnItemDestroyed(ItemObject itemObject)
        {
            Destroy(_itemIcons[_itemObjects.IndexOf(itemObject)].gameObject);
            _itemIcons.RemoveAt(_itemObjects.IndexOf(itemObject));
            _itemObjects.Remove(itemObject);
        }

        private void OnMapChange()
        {
            Player activePlayer       = MatchManager.Instance.ActivePlayer;
            Transform activeTransform = activePlayer.GolfballInstance.CachedTransform;
            float mapScale            = (MapSize / MapPlaySize) * MapZoom;
            float arrowOffset         = MapSize * 0.6f;

            for (int i = 0; i < _itemIcons.Count; i++)
            {
                Vector3 offset      = (activeTransform.position - _itemObjects[i].CachedTransform.position) * -mapScale;
                Vector3 newAncorPos = _itemIcons[i].rectTransform.anchoredPosition;

                newAncorPos.x = offset.x;
                newAncorPos.y = offset.z;

                _itemIcons[i].rectTransform.anchoredPosition = newAncorPos;
            }

            for (int i = 0; i < MatchManager.Instance.PlayerCount; i++)
            {
                Vector3 offset           = (activeTransform.position - MatchManager.Instance.Players[i].GolfballInstance.CachedTransform.position) * -mapScale;
                Vector3 offsetNormalized = offset.normalized * 1.1f;

                Vector3 newAncorPos      = _playerIcons[i].rectTransform.anchoredPosition;
                Vector3 newAncorPosArrow = _playerArrows[i].rectTransform.anchoredPosition;

                newAncorPos.x = offset.x;
                newAncorPos.y = offset.z;

                newAncorPosArrow.x = offsetNormalized.x;
                newAncorPosArrow.y = offsetNormalized.z;

                _playerIcons[i].rectTransform.anchoredPosition  = newAncorPos;
                _playerArrows[i].rectTransform.anchoredPosition = newAncorPosArrow * arrowOffset;
                _playerArrowBacks[i].rectTransform.anchoredPosition = newAncorPosArrow * arrowOffset;

                Vector3 eulerRotation                     = _playerIcons[i].rectTransform.eulerAngles;
                eulerRotation.z                           = 0.0f;
                _playerIcons[i].rectTransform.eulerAngles = eulerRotation;

                if (offset.magnitude > arrowOffset)
                {
                    if (_playerArrows[i].image.color.a < 1)
                    {
                        Vector4 color = _playerArrows[i].image.color;
                        color.w       = 1f;

                        _playerArrows[i].image.color = color;
                        _playerArrowBacks[i].image.color = Vector4.one;
                    }

                    Vector3 toPosition    = MatchManager.Instance.Players[i].GolfballInstance.CachedTransform.position;
                    toPosition.y          = 0f;
                    Vector3 fromPosition  = activeTransform.position;
                    fromPosition.y        = 0f;
                    Vector3 dir           = (toPosition - fromPosition).normalized;
                    float angle           = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
                    if (angle < 0) angle += 360;

                    _playerArrows[i].rectTransform.localEulerAngles = new Vector3(0, 0, angle - 90);
                    _playerArrowBacks[i].rectTransform.localEulerAngles = new Vector3(0, 0, angle - 90);
                }
                else
                {
                    Vector4 color = _playerArrows[i].image.color;
                    color.w       = 0f;

                    _playerArrows[i].image.color = color;
                    _playerArrowBacks[i].image.color = Vector4.zero;
                }
            }
        }

        private IEnumerator OnMapChangeDelayed(int frameDelay)
        {
            for (int i = 0; i < frameDelay; i++)
                yield return new WaitForEndOfFrame();

            OnMapChange();
        }

        private void OnShotEnded(Character character)
        {
            OnMapChange();
        }

        private void OnActivePlayerChanged(Player player)
        {
            OnMapChange();
        }

        private void OnPlayerTransitionFadeOutStarted(Player player)
        {
            Vector4 color                             = _playerIcons[player.PlayerID].image.color;
            color.w                                   = 1.0f;
            _playerIcons[player.PlayerID].image.color = color;
            OnMapChange();
        }

        private void OnCharacterHit(Character character)
        {
            Vector4 color                                       = _playerIcons[character.Player.PlayerID].image.color;
            color.w                                             = 0.0f;
            _playerIcons[character.Player.PlayerID].image.color = color;
        }
    }
}
