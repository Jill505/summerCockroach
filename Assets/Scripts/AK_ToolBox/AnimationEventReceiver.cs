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
    public void UIOpenSound()
    {
        SoundManager.Play("SFX_button-ui-sound");
    }
    public void ClockSound()
    {
        SoundManager.Play("SFX_click-metal-loud");
    }
    
}
