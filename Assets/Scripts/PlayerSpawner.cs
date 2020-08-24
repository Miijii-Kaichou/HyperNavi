using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    PlayerPawn player;

    [SerializeField]
    Transform spawnUnder;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        InstantiatePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InstantiatePlayer()
    {
        player = Instantiate(player, spawnUnder);
        player.transform.position = transform.position;
        player.transform.rotation = Quaternion.identity;
    }
}
