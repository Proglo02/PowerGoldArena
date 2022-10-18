using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerGolfArena.Entities
{
    public class FictionalCircleCameraManager : MonoBehaviour
    {
        [SerializeField] private GameObject      FictionalCamera;
        [SerializeField] private int             NumCameras;
        [SerializeField] private float           Speed  = 3.0f;
        [SerializeField] private float           Radius = 100f;
        [SerializeField] private float           Height = 10f;
        [SerializeField] private float           PositionOffset;
        [SerializeField] private Vector3         RotationOffset;

        private List<FictionalCamera> _fictionalCameras = new List<FictionalCamera>();

        private void Awake()
        {
            for (int i = 0; i < NumCameras; i++)
            {
                GameObject fictionalCamera = Instantiate(FictionalCamera, transform);
                _fictionalCameras.Add(fictionalCamera.GetComponent<FictionalCamera>());
                float radians = (2 * Mathf.PI) / NumCameras;
                _fictionalCameras[i].transform.localPosition = new Vector3(Radius * Mathf.Cos((i + PositionOffset) * radians), Height, Radius * Mathf.Sin((i + PositionOffset) * radians));

                Vector3 rotation = new Vector3(RotationOffset.x, ((Mathf.Rad2Deg * radians * (i + PositionOffset)) + 180 + RotationOffset.y) * -1, RotationOffset.z);
                _fictionalCameras[i].transform.rotation = Quaternion.Euler(rotation);
            }
        }

        private void FixedUpdate()
        {
             Vector3 rotation = transform.rotation.eulerAngles;
             rotation.y += Speed * Time.deltaTime;
             transform.rotation = Quaternion.Euler(rotation);
        }
    }
}