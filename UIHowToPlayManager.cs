using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PowerGolfArena.Entities;

namespace PowerGolfArena.UI
{
    public class UIHowToPlayManager : EntityBase
    {
        [SerializeField] private Image howToPlayImage;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        public void SwitchToImage(Sprite sprite)
        {
            howToPlayImage.color = Vector4.one;
            howToPlayImage.sprite = sprite;
            textMeshProUGUI.text = "";
        }

        public void SwitchToText(TMP_Text text)
        {
            textMeshProUGUI.text = text.text;
            howToPlayImage.color = Vector4.zero;
        }

        private void OnEnable()
        {
            howToPlayImage.color = Vector4.zero;
            textMeshProUGUI.text = "";
        }
    }
}
