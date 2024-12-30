using System.Collections;
using UnityEngine;
using Week4;

public class HatesSound : Enemy
{
    //if the central sound is turned on, it runs back home
    //if sound path is turned on while at crossroads, will run right if right door is open, otherwise runs to you
    //won't automatically walk into your room if the Likers are at crossroads

    private void Awake()
    {
        startLocation = Location.Right;
    }

    protected override IEnumerator WhileInRoom(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            if (currentLocation == Location.Crossroads && Player.instance.soundCenter)
                MoveToLocation(Player.instance.rightDoor ? Location.Home : Location.Right);
            else if (currentLocation == Location.Crossroads && Player.instance.soundPath && elapsedTime > 1.5f)
                MoveToLocation(Location.You);
            else if (currentLocation == Location.Crossroads && !CanAttack())
                elapsedTime = 0f;
            else
                elapsedTime += Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(Location.Crossroads);
                break;
            case Location.Crossroads:
                MoveToLocation(Location.You);
                break;
            case Location.Right:
                MoveToLocation(Player.instance.rightDoor ? Location.Right : Location.Crossroads);
                break;
        }

        bool CanAttack()
        {
            foreach (Enemy enemy in Player.instance.listOfEnemies)
                if (enemy.currentLocation == Location.Crossroads)
                    if (enemy is LikesSound)
                        return false;
            return true;
        }
    }
}
