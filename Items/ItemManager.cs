using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    public class ItemManager : EntityBase
    {
        [SerializeField] private List<ItemObject> itemPool = new List<ItemObject>();
        
        private ItemPlatform[] _itemPlatforms;
        private int _turnCount = 0;
        private float _timer   = 0.0f;
        private bool _allFull  = true;

        private void Awake()
        {
            _itemPlatforms = GetComponentsInChildren<ItemPlatform>();
        }

        private void FixedUpdate()
        {
            _allFull = true;
            foreach (ItemPlatform itemPlatform in _itemPlatforms)
            {
                if (!itemPlatform.HasItem)
                    _allFull = false;
            }

            if (!_allFull)
                _timer += Time.deltaTime;
            else
                _timer = 0.0f;

            if (_timer >= 1.0f)
            {
                _turnCount++;
                _timer = 0.0f;
            }

            if (_itemPlatforms.Length == 0 || itemPool.Count == 0)
                return;

            if (_turnCount == 3 && !_allFull)
            {
                int platformIndex = Random.Range(0, _itemPlatforms.Length);
                if (!_itemPlatforms[platformIndex].HasItem)
                {
                    int itemObjectIndex = Random.Range(0, itemPool.Count);
                    _itemPlatforms[platformIndex].SpawnItem(itemPool[itemObjectIndex]);
                    _turnCount = 0;
                }
            }
        }
    }
}
