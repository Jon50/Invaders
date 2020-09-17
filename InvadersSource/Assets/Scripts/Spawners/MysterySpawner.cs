using Invaders.Attributes;
using UnityEngine;

namespace Invaders.Core
{
    public class MysterySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _mysteryPrefab = default;
        [SerializeField] private Transform _spawnLocation = default;
        [SerializeField] private int[] _randomScore = default;

        private void Awake()
        {
            var mysteryInstance = Instantiate(_mysteryPrefab, _spawnLocation.position, _spawnLocation.rotation);

            var index = Random.Range(0, _randomScore.Length);
            mysteryInstance.GetComponent<Score>().Initialize(_randomScore[index]);
        }
    }
}