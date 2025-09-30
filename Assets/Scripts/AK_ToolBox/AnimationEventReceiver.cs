using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    public static bool prepared = false;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;
    public void ClockIdleAlready()
    {
        prepared = true;
        if (obj1 != null)
        {
            obj1.SetActive(false);
        }
        if (obj2 != null)
        {
            obj2.SetActive(false);
        }
        if (obj3 != null)
        {
            obj3.SetActive(false);
        }
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
