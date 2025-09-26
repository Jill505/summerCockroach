using UnityEngine;
using UnityEngine.EventSystems;

public class RadarHoverState : MonoBehaviour, IPointerExitHandler
{
    private Animator radarAnim;
    

    private void Start()
    {
        radarAnim = GameObject.Find("RadarAnimation").GetComponent<Animator>();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        radarAnim.SetBool("Open", false);
    }
}
