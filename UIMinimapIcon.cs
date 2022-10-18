using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PowerGolfArena.UI
{
    public class UIMinimapIcon : MonoBehaviour
    {
        [HideInInspector] public Image image;
        [HideInInspector] public RectTransform rectTransform;

        private void Awake()
        {
            image         = GetComponent<Image>();
            rectTransform = GetComponent<RectTransform>();
        }
    }
}
