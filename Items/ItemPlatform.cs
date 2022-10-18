using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Entities;
using PowerGolfArena.EventSystem;
using PowerGolfArena.Audio;

namespace PowerGolfArena.Entities.Items
{
    public class ItemPlatform : EntityBase
    {
        [SerializeField] ItemBarrier PickUpBarrier;
        public bool HasItem => ItemObject != null;
        public ItemObject ItemObject { get; private set; }

        private void OnTriggerEnter(Collider collider)
        {
            if (!ItemObject)
                return;

            Golfball golfball = collider.gameObject.GetComponent<Golfball>();
            if (!golfball)
                return;

            golfball.Character.Inventory.AddItem(ItemObject.item);
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.itemPickup, transform.position);
            DespawnItem();
            EventManager.Instance.PlayerEvents.ItemChanged.Invoke(golfball.Character.Player);
        }

        public void SpawnItem(ItemObject itemObject)
        {
            PickUpBarrier.ShouldEnable();
            ItemObject      = Instantiate(itemObject, CachedTransform.position + new Vector3(0f, 1.0f, 0f), itemObject.transform.rotation, CachedTransform);
            ItemObject.name = Random.Range(0, 100).ToString();
            EventManager.Instance.PlayerEvents.ItemCreated?.Invoke(ItemObject);
        }

        private void DespawnItem()
        {
            PickUpBarrier.ShouldDisable();
            EventManager.Instance.PlayerEvents.ItemDestroyed?.Invoke(ItemObject);
            ItemObject.OnItemDestroyed();
            ItemObject = null;
        }
    }
}
