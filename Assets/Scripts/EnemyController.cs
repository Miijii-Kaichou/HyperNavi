using System.Collections;

public class EnemyController : Controller
{
    PlayerPawn player;
    EnemyPawn pawn;

    protected override void OnEnable()
    {
        /*If a player hasn't been referenced,
         we'll assign the value to the player referenced by the 
        GameManager itself.*/
        if (GameManager.IsGameStarted)
        {
            player = GameManager.player;
        }
        
        Init();
    }


    /// <summary>
    /// Determine the direction that the player is going.
    /// </summary>
    /// <returns></returns>
    Direction DeterminePlayerDirection()
    {
        //Get the player's current direction
        if(player != null)
            return player.GetDirection();
        return default;
    }

    protected override void Init()
    {
        /*Determine the player's directions any given moment
         that the enemy spawns. Depending on the direction, the 
         enemy will go towards the player.*/
        pawn = AssociatedPawn as EnemyPawn;
        if (pawn != null)
        {
            switch (DeterminePlayerDirection())
            {
                case Direction.LEFT:
                    //Player is moving left,
                    //when enemy spawns, have it move to the right
                    pawn.MoveRight();
                    return;

                case Direction.RIGHT:
                    //Player is moving right
                    //when enemy spawns, have it move to the left
                    pawn.MoveLeft();
                    return;

                case Direction.UP:
                    //Player is moving Up
                    //when enemy spawns, have it move to the bottom
                    pawn.MoveDown();
                    return;

                case Direction.DOWN:
                    //Player is moving down
                    //when enemy spawns, hae it move to the top
                    pawn.MoveUp();
                    return;
            }
        }
    }
}
