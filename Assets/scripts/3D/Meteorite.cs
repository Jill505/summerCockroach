using Unity.VisualScripting;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public Rigidbody myRb;
    public Animator myAnimator;
    public GameObject warningArea;

    public bool isLanded = false;
    public float MeteoriteSpeed = 10f;
    float metCalSpeed = 0f;

    public float clampDelta = 1f;

    public UnityEngine.Vector3 from;
    public UnityEngine.Vector3 to;

    public float CountDown = 0;

    GameObject warningA;

    public void vanish()
    {
        myAnimator.SetTrigger("vanish");
    }
    public void die()
    {
        Destroy(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CountDown = Random.Range(2f, 3f);

        //TODO: spawn a warning area at to;
        warningA = Instantiate(warningArea, to, Quaternion.Euler(90, 0, 0));
    }

    bool LClog = false;
    // Update is called once per frame
    void Update()
    {
        if (from == null || to == null)
        {
            Debug.LogError("Ak Error: Meteorite translate error");
            return;
        }
        if (CountDown > 0) CountDown -= Time.deltaTime;
        else {
            if (!isLanded)
            {
                UnityEngine.Vector3 dir = to - transform.position;
                myRb.linearVelocity = dir * MeteoriteSpeed;
                //myRb.linearVelocity = MeteoriteSpeed *  UnityEngine.Vector3.MoveTowards(from, to, 1f) * Time.deltaTime;
                if (UnityEngine.Vector3.Distance(transform.position, to) < clampDelta)
                {
                    isLanded = true;
                    myRb.linearVelocity = UnityEngine.Vector3.zero;
                }
            }
            else
            {
                if (!LClog)
                {
                    Destroy(warningA);
                    vanish();
                    Invoke("vanish", Random.Range(4f, 8f));
                    LClog = true;
                }
            }
        }
    }
    public void MeteorFlySound()
    {
        SoundManager.Play("SFX_meteorFly");
    }
    public void MeteorDownSound()
    {
        SoundManager.Play("SFX_meteorDown");
    }
}
