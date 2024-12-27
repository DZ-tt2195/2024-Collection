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
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            if (currentLocation == Location.Crossroads && Player.instance.lightCenter)
                MoveToLocation(Player.instance.leftDoor ? Location.Home : Location.Left);
            else if (currentLocation == Location.Crossroads && Player.instance.lightPath)
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