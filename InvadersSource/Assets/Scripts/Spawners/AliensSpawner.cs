using UnityEngine;
using Invaders.Combat;
using Invaders.Control;
using Invaders.Attributes;

namespace Invaders.Core
{
    public class AliensSpawner : MonoBehaviour
    {
        [Header("Spawner Configuration")]
        [SerializeField] private AliensConfig[] _numberAlienTypes;
        [System.Serializable]
        private struct AliensConfig
        {
#pragma warning disable 0649
            public GameObject AlienPrefab;
            public int RowsLength;
            public int Score;
#pragma warning restore 0649
        }

        [Header("Settings")]
        [SerializeField] private Transform _projectileHolder;
        [SerializeField] private int _columnsLength = 11;
        [SerializeField] private float _alienOffset = 0.8f;

        private AliensController _aliensController;


        private void Awake()
        {
            Initialize();
            SpawnAliens();
        }


        private void Initialize() => _aliensController = GetComponent<AliensController>();


        private void SpawnAliens()
        {
            float yAxis = 0f;

            for (int i = 0; i != _numberAlienTypes.Length; i++)
            {
                var alienType = _numberAlienTypes[i];

                for (int row = 0; row != alienType.RowsLength; row++)
                {
                    for (int xAxis = 0; xAxis != _columnsLength; xAxis++)
                    {
                        var offset = new Vector3(xAxis * _alienOffset, yAxis * _alienOffset);
                        var alienInstance = Instantiate(alienType.AlienPrefab, transform.position + offset, Quaternion.identity, this.transform);
                        SetupAlien(alienType, alienInstance);
                    }

                    yAxis--;
                }
            }
        }


        private void SetupAlien(AliensConfig alienType, GameObject alienInstance)
        {
            var alienShoot = alienInstance.GetComponent<AlienShoot>();
            var alienScore = alienInstance.GetComponent<Score>();

            alienShoot.SetProjectileHolder(_projectileHolder);
            alienScore.Initialize(alienType.Score);

            _aliensController.AddAlien(alienInstance);
        }
    }
}