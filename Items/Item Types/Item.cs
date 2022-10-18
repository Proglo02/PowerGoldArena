using System.Collections;
using TMPro;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/Item")]
    public abstract class Item : ScriptableObject
    {
        public enum Type { None, Club, Ball };

        new public string name = "Item";
        public Type type       = Type.None;
        public Sprite itemInfoImage = null;

        public abstract void Use(Player player);
    }
}
