using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PowerGolfArena.Core;
using PowerGolfArena.Entities.Items;

namespace PowerGolfArena.UI
{
    public class UIItenInfoMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text itemName;
        [SerializeField] private TMP_Text itemUseInfo;
        [SerializeField] private TMP_Text itemInfoText;
        [SerializeField] private Image itemInfoImage;

        [SerializeField] private List<TMP_Text> infoList = new List<TMP_Text>();

        private void OnEnable()
        {
            Item item = MatchManager.Instance.ActivePlayer.CharacterInstance.Inventory.Item;
            itemName.text = item.name;
            itemUseInfo.text = item.type == Entities.Items.Item.Type.Ball ? "Can only be used after you shoot" : "Can only be used before you shoot";
            itemInfoImage.sprite = item.itemInfoImage;

            switch(item.name)
            {
                case "Air Strike Ball": itemInfoText.text = infoList[0].text;
                    break;

                case "Bee Ball": itemInfoText.text = infoList[1].text;
                    break;

                case "Explosive Ball": itemInfoText.text = infoList[2].text;
                    break;

                case "Slammer Ball": itemInfoText.text = infoList[3].text;
                    break;

                case "Bounce Club":
                    itemInfoText.text = infoList[4].text;
                    break;

                case "Bubble Club":
                    itemInfoText.text = infoList[5].text;
                    break;

                case "Remote Control Club":
                    itemInfoText.text = infoList[6].text;
                    break;

                case "Scatter Club":
                    itemInfoText.text = infoList[7].text;
                    break;
            }
        }
    }
}
