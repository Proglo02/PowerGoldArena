using System.Collections;
using UnityEngine;

namespace PowerGolfArena.Entities.Items
{
    public class ItemObject : EntityBase
    {
        public Item item;
        private ItemModel[] _models;
        private ItemModel _model;

        [SerializeField] private float bobbingAmplitude = 0.25f;
        [SerializeField] private float bobbingSpeed     = 0.5f;
        [SerializeField] private float rotationSpeed    = 20f;
        [SerializeField] private float offsetY          = 0.5f;

        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = CachedTransform.position;
            _models = GetComponentsInChildren<ItemModel>();
        }

        private void Start()
        {
            OnItemCreated();
        }

        private void Update()
        {
            CachedTransform.Rotate(new Vector3(0.0f, rotationSpeed * Time.deltaTime, 0.0f), Space.World);

            Vector3 bobPosition;
            bobPosition               = _startPos;
            bobPosition.y            += Mathf.Sin(bobbingSpeed * Mathf.PI * Time.time) * bobbingAmplitude + offsetY;
            CachedTransform.position  = bobPosition;
        }

        private IEnumerator DissolveItem()
        {
                ItemModel itemModel = _model;
            while (itemModel.DissolveAmount < 1.0f)
            {
                itemModel.DissolveAmount = Mathf.Min(itemModel.DissolveAmount + itemModel.dissolveSpeed * Time.deltaTime, 1.0f);

                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

        private IEnumerator UndissolveItem()
        {
            ItemModel ItemModel = _model;
            while (ItemModel.DissolveAmount > 0.0f)
            {
                ItemModel.DissolveAmount = Mathf.Max(ItemModel.DissolveAmount - ItemModel.dissolveSpeed * Time.deltaTime, 0.0f);

                yield return new WaitForEndOfFrame();
            }
        }
        private void OnItemCreated()
        {
            foreach (ItemModel model in _models)
            {
                _model = model;
                StartCoroutine(UndissolveItem());
            }
        }

        public void OnItemDestroyed()
        {
            foreach (ItemModel model in _models)
            {
                _model = model;
                StartCoroutine(DissolveItem());
            }
        }
    }
}
