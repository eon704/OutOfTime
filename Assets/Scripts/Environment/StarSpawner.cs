using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Environment
{
  public class StarSpawner : MonoBehaviour
  {
    [SerializeField] private GameObject starPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private float minInterval = 1f;
    [SerializeField] private float maxInterval = 3f;
    [SerializeField] private float spawnScale = 0.05f;

    private void Start()
    {
      StartCoroutine(SpawnStars());
    }

    private IEnumerator SpawnStars()
    {
      while (true)
      {
        var spawnPoint = spawnPoints.GetRandom();
        var star = Instantiate(starPrefab, spawnPoint.position, spawnPoint.rotation, transform);
        star.transform.localScale = Vector3.one * spawnScale;
        
        var starTargetPosition = spawnPoint.position.z - 400;
        star.transform
          .DOMoveZ(starTargetPosition, 7f)
          .SetEase(ease)
          .OnComplete(() => Destroy(star));
        
        var waitTime = Random.Range(minInterval, maxInterval);
        yield return new WaitForSeconds(waitTime);
      }
    }
  }
}