using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private bool spawnAtRandom;

    [SerializeField]
    private float minRange, maxRange;

    [SerializeField]
    private int chance, outOf;

    [SerializeField]
    private Spawner[] spawners;

    void OnEnable()
    {
        if (GameManager.IsGameStarted && spawnAtRandom)
            Randomize();
    }

    void Randomize()
    {
        float chances = (float)chance / (float)outOf;

        float randomValue = Random.Range(minRange, maxRange);
        if (randomValue <= chances)
        {
            for (int iter = 0; iter < spawners.Length; iter++)
            {

                Spawner spawner = spawners[iter];
                spawner.OnInit();

            }
        }
    }
}
