using System.Collections;
using UnityEngine;
using Week4;

public class LikesSound : Enemy
{
    //won't enter crossroads if LikesLight is there
    //at the crossroads:
    //if both doors are closed, or the central sound is activated, it goes down and is unstoppable
    //if right door is open, it goes right and is unstoppable
    //if right door is closed, it goes left and can be sent home with the sound path

    private void Awake()
    {
        startLocation = Location.Home;
    }

    protected override IEnumerator WhileInRoom(float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            if (currentLocation == Location.Crossroads && Player.instance.soundCenter && elapsedTime > 1.5f)
                MoveToLocation(Location.You);
            else if (currentLocation == Location.Left && Player.instance.soundPath)
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
                else if (!Player.instance.rightDoor)
                    MoveToLocation(Location.Right);
                else if (Player.instance.rightDoor)
                    MoveToLocation(Location.Left);
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
                if (enemy is LikesLight)
                    return false;
        return true;
    }
}
