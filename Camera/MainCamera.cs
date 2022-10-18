using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PowerGolfArena.Core;

namespace PowerGolfArena.Entities
{
    [RequireComponent(typeof(Camera), typeof(CinemachineBrain))]
    public class MainCamera : Singleton<MainCamera>
    {
        public Camera Camera { get; private set; }
        public CinemachineBrain CinemachineBrain { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (IsDuplicate)
                return;

            Camera           = GetComponent<Camera>();
            CinemachineBrain = GetComponent<CinemachineBrain>();
        }
    }
}
