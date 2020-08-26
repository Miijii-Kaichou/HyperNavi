using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    TouchDetection movementDetection, boostDetection;

    [SerializeField]
    PlayerPawn player;

    [SerializeField]
    FollowBehind gameCamera;

    [SerializeField]
    Transform spawnUnder;

    private void OnEnable()
    {
        InstantiatePlayer();
    }

    /// <summary>
    /// Spawn the player
    /// </summary>
    void InstantiatePlayer()
    {
        //Instanstiation
        player = Instantiate(player, spawnUnder);

        //Object placement
        player.transform.position = transform.position;
        player.transform.rotation = Quaternion.identity;

        //Touch area / detection assignment
        if (player != null)
        {
            player.GetController().AssignMovementDetection(movementDetection);
            player.GetController().AssignBoostDetection(boostDetection);
            gameCamera.AssignTarget(player.transform);
        }
    }
}
