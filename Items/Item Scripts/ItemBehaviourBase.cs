using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    [RequireComponent(typeof(Golfball))]
    public abstract class ItemBehaviourBase : EntityBase
    {
        protected Player Player => _golfball.Player;
        private Golfball _golfball;

        protected virtual void Awake()
        {
            _golfball = GetComponent<Golfball>();
        }
    }
}
