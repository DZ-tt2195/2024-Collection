using System.Collections;
using UnityEngine;
using Week4;

public class LikesLight : Enemy
{
    //at the crossroads:
    //if both doors are closed, or the central light is activated, it goes down and is unstoppable
    //if left door is open, it goes left and is unstoppable
    //if left door is closed, it goes right and can be sent home with the light path

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            if (currentLocation == Location.Crossroads && Player.instance.lightCenter)
                MoveToLocation(Location.Center);
            else if (currentLocation == Location.Right && Player.instance.lightPath)
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
                if (Player.instance.leftDoor && Player.instance.rightDoor)
                    MoveToLocation(Location.Center);
                else if (Player.instance.leftDoor)
                    MoveToLocation(Location.Right);
                else if (Player.instance.rightDoor || !Player.instance.leftDoor)
                    MoveToLocation(Location.Left);
                break;
            case Location.Left:
                MoveToLocation(Location.You);
                break;
            case Location.Right:
                MoveToLocation(Location.You);
                break;
            case Location.Center:
                MoveToLocation(Location.You);
                break;
        }
    }
}
