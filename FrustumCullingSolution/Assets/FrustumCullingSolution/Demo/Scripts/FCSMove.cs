using System;
using FrustumCullingSolution.Scripts;
using UnityEngine;

namespace FrustumCullingSolution.Demo.Scripts
{
    public class FCSMove : MonoBehaviour
    {
        public Transform start;
        public Transform end;
        public float speed = 5.0f;
        private float _startTime;
        private float _journeyLength;

        private void Awake()
        {
            transform.position = start.position;
        }

        private void Start()
        {
            _startTime = Time.time;
            _journeyLength = Vector3.Distance(start.position, end.position);
        }

        private void OnEnable()
        {
            var frustumCullingObject = GetComponent<FrustumCullingObject>();
            if (frustumCullingObject != null)
            {
                frustumCullingObject.onBecomeVisible.AddListener(BecomeVisible);
                frustumCullingObject.onBecomeInvisible.AddListener(BecomeInvisible);
            }
        }

        private void OnDisable()
        {
            var frustumCullingObject = GetComponent<FrustumCullingObject>();
            if (frustumCullingObject != null)
            {
                frustumCullingObject.onBecomeVisible.RemoveListener(BecomeVisible);
                frustumCullingObject.onBecomeInvisible.RemoveListener(BecomeInvisible);
            }
        }

        private void BecomeVisible()
        {
            Debug.Log($"{gameObject.name} become visible");
        }
    
        private void BecomeInvisible()
        {
            Debug.Log($"{gameObject.name} become invisible");
        }

        private void Update()
        {
            var distCovered = (Time.time - _startTime) * speed;
            var fracJourney = Mathf.PingPong(distCovered, _journeyLength) / _journeyLength;
            transform.position = Vector3.Lerp(start.position, end.position, fracJourney);
        }
    }
}
