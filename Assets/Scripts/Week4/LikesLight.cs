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

    private void Awake()
    {
        startLocation = Location.Home;
    }

    protected override IEnumerator WhileInRoom(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            if (currentLocation == Location.Crossroads && Player.instance.lightCenter && elapsedTime > 2f)
                MoveToLocation(Location.You);
            else if (currentLocation == Location.Right && Player.instance.lightPath)
                MoveToLocation(Location.Home);

            elapsedTime += Time.deltaTime;
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
