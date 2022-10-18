using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PowerGolfArena.Core;
using PowerGolfArena.EventSystem;

namespace PowerGolfArena.UI
{
    public class UIItemInfo : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text text;

        private void Update()
        {
            if (MatchManager.Instance.ActivePlayer.CharacterInstance.Inventory.HasItem)
            {
                icon.color = Vector4.one;
                text.text = "Item Info";
            }
            else
            {
                icon.color = Vector4.zero;
                text.text = "";
            }
        }
    }
}
