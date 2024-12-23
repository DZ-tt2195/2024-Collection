using System.Collections;
using UnityEngine;
using Week4;

public class HatesLight : Enemy
{ 
    //walks down to center, then if the central light is turned on, it runs back home
    //if light path is turned on, will run left if left door is open, otherwise runs to center
    //won't enter your room if the Likers are at crossroads

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            if (currentLocation == Location.Center && Player.instance.lightCenter)
                MoveToLocation(Location.Home);

            time -= Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(Location.Crossroads);
                break;
            case Location.Crossroads:
                MoveToLocation(Location.Center);
                break;
            case Location.Left:
                MoveToLocation(Location.You);
                break;
            case Location.Right:
                MoveToLocation(Location.You);
                break;
            case Location.Center:
                MoveToLocation(CanAttack() ? Location.You : Location.Crossroads);
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