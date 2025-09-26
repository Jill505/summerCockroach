using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public static bool prepared = false;
    public void ClockIdleAlready()
    {
        prepared = true;  
    }
    public void ClockIdleNotAlready()
    {
        prepared = false;
    }
}
