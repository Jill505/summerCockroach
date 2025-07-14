using JetBrains.Annotations;
using UnityEngine;

public class CockroachManager : MonoBehaviour
{
    [Header("My Component References")]
    public CockroachMove myCockroachMove;
    public CameraLogic3D myCameraLogic; 

    [Header("Cockroach Values")]
    public int Hp = 1;

    public float autoModeCameraFlu = 0.1f;
    public float playerModeCameraFlu = 0.1f;
    
    public void JudgeCockroachDie()
    {
        if (Hp <= 0)
        {
            CockroachDie();
        }
    }
    public void CockroachDie()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
