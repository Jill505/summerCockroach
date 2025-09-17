using UnityEngine;
using UnityEngine.EventSystems;

public class RadarUITrigger : MonoBehaviour, IPointerEnterHandler
{
    private Animator radarAnim;

    private void Start()
    {
        radarAnim = GameObject.Find("RadarAnimation").GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
      radarAnim.SetBool("Open", true);
    }
}
