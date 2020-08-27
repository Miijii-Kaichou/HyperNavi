using UnityEngine;

public class PlayerSpawner : Spawner
{
    [SerializeField]
    private FollowBehind gameCamera;

    [SerializeField]
    private TouchDetection movementDetection, boostDetection;

    [SerializeField]
    private PlayerPawn player;

    protected override void OnEnable()
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

        GameManager.AssignPlayer(player);

        //Object placement
        player.transform.position = transform.position;
        player.transform.rotation = Quaternion.identity;

        //Touch area / detection assignment
        if (player != null)
        {
            player.GetController().AssignMovementDetection(movementDetection);
            player.GetController().AssignBoostDetection(boostDetection);
            gameCamera.AttachTarget(player.transform);
        }
    }
}
