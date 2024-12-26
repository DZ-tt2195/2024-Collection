using System.Collections;
using UnityEngine;
using Week4;

public class HatesLight : Enemy
{
    //if the central light is turned on, it runs back home
    //if light path is turned on while at crossroads, will run right if right door is open, otherwise runs to you
    //won't automatically walk into your room if the Likers are at crossroads

    private void Awake()
    {
        startLocation = Location.Left;
    }

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            if (currentLocation == Location.Crossroads && Player.instance.soundCenter)
                MoveToLocation(Player.instance.leftDoor ? Location.Home : Location.Left);
            else if (currentLocation == Location.Crossroads && Player.instance.soundPath)
                MoveToLocation(Location.You);

            time -= Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(Location.Crossroads);
                break;
            case Location.Crossroads:
                MoveToLocation(CanAttack() ? Location.You : Location.Crossroads);
                break;
            case Location.Left:
                MoveToLocation(Player.instance.leftDoor ? Location.Left : Location.Crossroads);
                break;
        }

        bool CanAttack()
        {
            foreach (Enemy enemy in Player.instance.listOfEnemies)
                if (enemy.currentLocation == Location.Crossroads)
                    if (enemy is LikesLight)
                        return false;
            return true;
        }
    }
}