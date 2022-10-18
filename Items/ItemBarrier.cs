using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PowerGolfArena.Entities
{
    public class ItemBarrier : EntityBase
    {
        [SerializeField] private float MaxScale;
        [SerializeField] private float MinScale;
        [SerializeField] private float MaxAlpha;
        [SerializeField] private float ScalingSpeed;
        [SerializeField] private float AlpaSpeed;

        private Renderer _renderer;

        private float _scale = 0.0f;
        private float _alpha = 0.0f;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();

            _scale                     = MinScale;
            CachedTransform.localScale = new Vector3(_scale, _scale, _scale);

            _renderer.material.SetFloat("_BeaconAlpha", _alpha);
        }

        private IEnumerator ScaleUp()
        {
            CachedTransform.localScale = new Vector3(MinScale, MinScale, MinScale);
            while (_scale < MaxScale)
            {
                _scale = Mathf.Clamp(CachedTransform.localScale.x, MinScale, MaxScale);
                _scale                     += ScalingSpeed * Time.deltaTime;
                CachedTransform.localScale  = new Vector3(_scale, _scale, _scale);

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator AlphaUp()
        {
            _renderer.material.SetFloat("_BeaconAlpha", 0);
            while (_alpha < MaxAlpha)
            {
                _alpha = Mathf.Clamp(_alpha, 0, MaxAlpha);
                _alpha += AlpaSpeed * Time.deltaTime;
                _renderer.material.SetFloat("_BeaconAlpha", _alpha);

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator ScaleDown()
        {
            while (_scale > MinScale)
            {
                _scale = Mathf.Clamp(CachedTransform.localScale.x, MinScale, MaxScale);
                _scale                     -= ScalingSpeed * Time.deltaTime;
                CachedTransform.localScale  = new Vector3(_scale, _scale, _scale);

                yield return new WaitForEndOfFrame();
            }
            CachedTransform.localScale = new Vector3(MinScale, MinScale, MinScale);
        }
        private IEnumerator AlphaDown()
        {
            while (_alpha > 0.0f)
            {
                _alpha = Mathf.Clamp(_alpha, 0, MaxAlpha);
                _alpha -= AlpaSpeed * Time.deltaTime;
                _renderer.material.SetFloat("_BeaconAlpha", _alpha);

                yield return new WaitForEndOfFrame();
            }
            _renderer.material.SetFloat("_BeaconAlpha", 0);
        }


        public void ShouldEnable()
        {
            StartCoroutine(ScaleUp());
            StartCoroutine(AlphaUp());
        }

        public void ShouldDisable()
        {
            StartCoroutine(ScaleDown());
            StartCoroutine(AlphaDown());
        }
    }
}
