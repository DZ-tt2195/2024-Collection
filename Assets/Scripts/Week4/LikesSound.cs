using System.Collections;
using UnityEngine;
using Week4;

public class LikesSound : Enemy
{
    //at the crossroads:
    //if both doors are closed, or the central sound is activated, it goes down and is unstoppable
    //if right door is open, it goes right and is unstoppable
    //if right door is closed, it goes left and can be sent home with the sound path

    protected override IEnumerator WhileInRoom(float time)
    {
        while (time > 0)
        {
            switch (currentLocation)
            {
                case Location.Crossroads:
                    if (Player.instance.soundCenter) MoveToLocation(Location.Center);
                    break;
                case Location.Left:
                    if (Player.instance.soundPath) MoveToLocation(Location.Home);
                    break;
            }

            time -= Time.deltaTime;
            yield return null;
        }

        switch (currentLocation)
        {
            case Location.Home:
                MoveToLocation(Location.Crossroads);
                break;
            case Location.Crossroads:
                if (Player.instance.leftDoor && Player.instance.rightDoor) MoveToLocation(Location.Center);
                else if (Player.instance.leftDoor || !Player.instance.rightDoor) MoveToLocation(Location.Right);
                else if (Player.instance.rightDoor) MoveToLocation(Location.Left);
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
