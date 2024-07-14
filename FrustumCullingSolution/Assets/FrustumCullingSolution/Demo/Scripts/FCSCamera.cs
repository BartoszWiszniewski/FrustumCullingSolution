using UnityEngine;

namespace FrustumCullingSolution.Demo.Scripts
{
    [RequireComponent(typeof(Camera))]
    public class FCSCamera : MonoBehaviour
    {
        private Camera _camera;
        public float speed = 10.0f;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            Camera.SetupCurrent(_camera);
        }

        void Update()
        {
            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");

            transform.position += new Vector3(x, 0f, z) * speed * Time.deltaTime; 
        }
    }
}
