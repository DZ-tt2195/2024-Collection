using System.Collections;
using UnityEngine;
using Week4;

public class LikesLight : Enemy
{
    //won't enter crossroads if LikesSound is there
    //at the crossroads:
    //if both doors are closed, or the central light is activated, it goes down and is unstoppable
    //if left door is open, it goes left and is unstoppable
    //if left door is closed, it goes right and can be sent home with the light path

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            if (currentLocation == Location.Crossroads && Player.instance.lightCenter)
                MoveToLocation(Location.You);
            else if (currentLocation == Location.Right && Player.instance.lightPath)
                MoveToLocation(Location.Home);

            time -= Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(CanMove() ? Location.Crossroads : Location.Home);
                break;
            case Location.Crossroads:
                if (Player.instance.leftDoor && Player.instance.rightDoor)
                    MoveToLocation(Location.You);
                else if (!Player.instance.leftDoor)
                    MoveToLocation(Location.Left);
                else if (Player.instance.leftDoor)
                    MoveToLocation(Location.Right);
                break;
            case Location.Left:
                MoveToLocation(Location.You);
                break;
            case Location.Right:
                MoveToLocation(Location.You);
                break;
        }
    }

    bool CanMove()
    {
        foreach (Enemy enemy in Player.instance.listOfEnemies)
            if (enemy.currentLocation == Location.Crossroads)
                if (enemy is LikesSound)
                    return false;
        return true;
    }
}
