using System.Collections;
using UnityEngine;
using Week4;

public class HatesLight : Enemy
{
    //if the central light is turned on, it runs back home
    //if light path is turned on while at crossroads, will run right if right door is open, otherwise runs to you
    //won't automatically walk your room if the Likers are at crossroads

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            if (currentLocation == Location.Crossroads && Player.instance.lightCenter)
                MoveToLocation(Location.Home);
            else if (currentLocation == Location.Crossroads && Player.instance.lightPath)
                MoveToLocation(Player.instance.leftDoor ? Location.You : Location.Left);

            time -= Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(Location.Crossroads);
                break;
            case Location.Crossroads:
                MoveToLocation(CanAttack() ? Location.You : Location.Home);
                break;
            case Location.Left:
                MoveToLocation(Location.You);
                break;
            case Location.Right:
                MoveToLocation(Location.You);
                break;
        }
    }

    bool CanAttack()
    {
        foreach (Enemy enemy in Player.instance.listOfEnemies)
        {
            if (enemy.currentLocation == Location.Crossroads)
            {
                if (enemy is LikesLight || enemy is LikesSound)
                {
                    return false;
                }
            }
        }
        return true;
    }
}