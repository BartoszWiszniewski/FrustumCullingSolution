using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrustumCullingSolution.Demo.Scripts
{
    public class FCSSpawner : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> prefabs;
    
        [SerializeField]
        private int objectsToSpawn = 10;
    
        [SerializeField]
        private Collider spawnArea;

        [SerializeField] 
        private float delay = 0.5f;

        private Coroutine _coroutine;
        private void Start()
        {
            if (spawnArea == null)
            {
                spawnArea = GetComponent<Collider>();
            }

            _coroutine = StartCoroutine(_Spawn());
        }

        private IEnumerator _Spawn()
        {
            for (var i = 0; i < objectsToSpawn; i++)
            {
                yield return new WaitForSeconds(delay);
                var prefab = prefabs.OrderBy(x => Random.Range(0, 10000)).FirstOrDefault();
                if (prefab != null)
                {
                    var bounds = spawnArea.bounds;

                    var randomPosition = new Vector3(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        Random.Range(bounds.min.z, bounds.max.z)
                    );

                    Instantiate(prefab, randomPosition, Quaternion.identity, transform);
                }
            }
        }

        private void OnDestroy()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
        }
    }
}
