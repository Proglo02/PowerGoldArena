using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PowerGolfArena.UI
{ 
    public class UIClubSelect : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public void Select()
        {
            if(_image) _image.color = Color.red;
        }

        public void Deselecet()
        {
            if (_image) _image.color = Color.white;
        }
    }
}