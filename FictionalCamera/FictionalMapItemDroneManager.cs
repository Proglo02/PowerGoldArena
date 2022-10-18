using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGolfArena.Entities.Items;

namespace PowerGolfArena.Entities
{
    public class FictionalMapItemDroneManager : MonoBehaviour
    {
        [SerializeField] private GameObject FictionalCamera;
        [SerializeField] private GameObject WayPoints;
        [SerializeField] private int NumCameras;
        [SerializeField] private float Speed = 3.0f;
        [SerializeField] private float WaitTime = 2.0f;

        [HideInInspector] public GameObject effectItem;
        [HideInInspector] public float interval;

        private float _radius = 1.0f;
        private float _dropTimer = 0.0f;

        private List<FictionalCamera>   _fictionalCameras = new List<FictionalCamera>();
        private List<Vector3>        _wayPoints = new List<Vector3>();
        private List<bool>              _isOcupied = new List<bool>();

        private void Awake()
        {
            for (int i = 0; i < WayPoints.transform.childCount; i++)
            {
                _wayPoints.Add(WayPoints.transform.GetChild(i).gameObject.transform.position);
                _isOcupied.Add(false);
            }

            for (int i = 0; i < NumCameras; i++)
            {
                GameObject fictionalCamera = Instantiate(FictionalCamera, transform);
                _fictionalCameras.Add(fictionalCamera.GetComponent<FictionalCamera>());
                _fictionalCameras[i].transform.position = _wayPoints[i];
                _fictionalCameras[i].targetPosition = _wayPoints[i];
                _isOcupied[i] = true;
            }
        }

        private void Update()
        {
            for (int i = 0; i < NumCameras; i++)
            {
                if (Vector3.Distance(_fictionalCameras[i].transform.position, _fictionalCameras[i].targetPosition) < _radius)
                {
                    GetNewTarget(i, 0);
                }
                else if (_fictionalCameras[i].timer < WaitTime)
                {
                    _fictionalCameras[i].timer += Time.deltaTime;

                    if (_fictionalCameras[i].timer >= WaitTime && Random.value > WaitTime / 4f)
                        _fictionalCameras[i].timer -= WaitTime / 4; 
                }
                else
                    _fictionalCameras[i].transform.position = Vector3.MoveTowards(_fictionalCameras[i].transform.position, _fictionalCameras[i].targetPosition, Time.deltaTime * Speed);
            }

            if (_dropTimer >= interval)
            {
                DropItem();
                _dropTimer = 0;
            }
            else
                _dropTimer += Time.deltaTime;
        }

        private void GetNewTarget(int index, int Reccursion)
        {
            int newIndex = Random.Range(0, _wayPoints.Count);
            Vector3 offset = _wayPoints[newIndex] - _fictionalCameras[index].targetPosition;

            if (!_isOcupied[newIndex] && !UnityEngine.Physics.Raycast(_fictionalCameras[index].targetPosition, offset, out RaycastHit hit, Vector3.Distance(_fictionalCameras[index].targetPosition, _wayPoints[newIndex])))
            {
                _isOcupied[_wayPoints.IndexOf(_fictionalCameras[index].targetPosition)] = false;
                _isOcupied[newIndex] = true;
                _fictionalCameras[index].timer = 0f;

                _fictionalCameras[index].targetPosition = _wayPoints[newIndex];
                _fictionalCameras[index].transform.LookAt(_wayPoints[newIndex]);

                Vector3 rotation = _fictionalCameras[index].transform.rotation.eulerAngles;
                rotation.y -= 90;
                _fictionalCameras[index].transform.rotation = Quaternion.Euler(rotation);
            }
            else if (Reccursion < 5)
            {
                Reccursion++;
                GetNewTarget(index, Reccursion);
            }
            else
                _fictionalCameras[index].timer -= WaitTime / 4;
        }

        private void DropItem()
        {
            for (int i = 0; i < NumCameras; i++)
            {
                Instantiate(effectItem, _fictionalCameras[i].transform.position - new Vector3(0f, 0.5f, 0f), _fictionalCameras[i].transform.rotation);
            }
        }
    }
}
