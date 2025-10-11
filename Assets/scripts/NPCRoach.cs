using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class NPCRoach : MonoBehaviour
{
    public enum MoveMode
    {
        Straight,
        Snake,
        Robot
    }

    [Header("Roach Move Mode")]
    public MoveMode moveMode = MoveMode.Straight;

    [Header("Snake Move Settings")]
    public float snakeFrequency = 8f;   // ��ʳt��
    public float snakeAmplitude = 0.5f; // ��ʴT��

    [Header("Special Behavior")]
    public float radius = 2.0f;
    public float detectDistance = 10f;       // �g�u�����Z��
    public float socialRange = 3f;         // ���񪱮a�d��
    public float coolDownTime = 1.5f;          // �N�o�ɶ�
    private bool onSpecialCooldown = false;
    private bool inTalkRange = false;
    private Transform playerTarget;           // ���a BoxCollider ��m

    [Header("Z Move Settings")]
    public float zAmplitude = 0.25f;  // ���k�����Z��
    public float zFrequency = 2f;    // �C�q��u����ɶ�

    private float zStartTime;         // �Ψӭp���u����

    [Header("Ref Component")]
    public NewAllGameManager newAllGameManager;
    public AllGameManager allGameManager;
    public FoodGenManger foodGenManager;
    public CockroachManager cockroachManager;
    public NPCRoachManager nPCRoachManager;

    public Rigidbody myRb;

    [Header("Ref Objects")]
    public GameObject burstBlood;
    private GameObject Player;

    [Header("Roach Brain Logic")]
    public Coroutine brainLogic;

    public float moveSpeed = 7f;
    public Vector3 targetPos;

    public Vector3 nextPos;

    public bool finJ = false;

    public bool clogFin;

    public bool hasFemInZone;
    public bool hasFoodInZone;

    public Vector3 targetFemPos;
    public Vector3 targetFoodPos;

    public GameObject[] NPC;
    private System.Random rng;

    public float femRoachAdditional = 1.2f;
    public float foodRoachAdditional = 1.2f;

    [HideInInspector] public bool tellDyIamDead = false;

    [Header("Mutant Roach")]
    private bool isMutant = false;
    private bool isCharging = false;
    private Transform chargeTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        newAllGameManager = FindFirstObjectByType<NewAllGameManager>();
        allGameManager = FindFirstObjectByType<AllGameManager>();
        foodGenManager = FindFirstObjectByType<FoodGenManger>();
        cockroachManager = FindFirstObjectByType<CockroachManager>();
        nPCRoachManager = FindFirstObjectByType<NPCRoachManager>();
        Player = GameObject.Find("3DCockroach");

        nPCRoachManager.nPCRoaches.Add(this);

        rollTarget();

        //StartCoroutine(myCoroutineUpdate());
        if (gameObject.name.Contains("BlueNPCRoach"))
        {
            moveSpeed *= 1.5f;   // �t�ץ[��
            detectDistance = 15f; // �g�u�����Z����
            coolDownTime = 10f;  // �����N�o�ɶ�����
            isMutant = true;     // �аO��������
        }


        ////////RandomSelectSkin
        if(!isMutant)
        {
            for (int i = 0; i < NPC.Length; i++)
            {
                if (NPC[i] != null)
                {
                    NPC[i].SetActive(false);
                }
            }

            int seed = Guid.NewGuid().GetHashCode(); // �O�Ұߤ@
            rng = new System.Random(seed);

            int randomIndex = rng.Next(0, NPC.Length); // �οW���H����

            // �ҥΨ��� GameObject
            if (NPC[randomIndex] != null)
            {
                NPC[randomIndex].SetActive(true);
            }

            if (randomIndex == 1)
            {
                moveMode = MoveMode.Snake;
            }
            else if (randomIndex == 2)
            {
                moveMode = MoveMode.Robot;
            }
            else
            {
                moveMode = MoveMode.Straight;
            }
        }
        

        
    }
    bool fixedUpdateAllowCheckClog = false;

    private void FixedUpdate()
    {
        hasFemInZone = false;
        hasFoodInZone = false;
        fixedUpdateAllowCheckClog = false;
    }
    private void Update()
    {
        if (isMutant)
        {
            if (!onSpecialCooldown && !isCharging)
            {
                TryMutantDetectAndCharge();
            }
        }
        if (!fixedUpdateAllowCheckClog)
        {
            CanTrySpecialBehavior();
            if(findPlayer == true)
            {
                if(moveMode == MoveMode.Snake)
                {
                    FollowPlayer();
                }               
                else
                {
                    ScaredByPlayer();
                    Vector3 dir = (Player.transform.position - transform.position).normalized;
                    transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                }
            }
            else if (hasFemInZone)
            {
                //Do Track Fem
                goFem();
            }
            else
            {
                if (hasFoodInZone)
                {
                    //Do Track Food
                    goFood();
                }
                else
                {
                    //Do Normal
                    if (moveMode == MoveMode.Straight)
                        goTarget();
                    else if (moveMode == MoveMode.Snake)
                        goTargetSnake();
                    else if (moveMode == MoveMode.Robot)
                        goTargetZ();
                }
            }
            fixedUpdateAllowCheckClog = true;
        }
    }

    public IEnumerator myCoroutineUpdate()
    {
        yield return null;
    }

    public void goTarget()
    {
        transform.LookAt(nextPos);
        Vector3 rot3 = transform.localEulerAngles;
        rot3 = new Vector3(0, rot3.y, 0);
        transform.localEulerAngles = rot3;
        myRb.linearVelocity = transform.forward * moveSpeed;
    }

    public void goTargetSnake()
    {
        // �e�i��V
        Vector3 dir = (nextPos - transform.position).normalized;

        // �p�⥪�k��V (�Υ�e�n�D�X�k�V�q)
        Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;

        // Sine �i���ͥ��k�\��
        float sway = Mathf.Sin(Time.time * snakeFrequency) * snakeAmplitude;

        // �̲פ�V = �e�i��V + ���k�\��
        Vector3 swayDir = (dir + side * sway).normalized;

        // ���V�o�Ӥ�V
        transform.rotation = Quaternion.LookRotation(swayDir, Vector3.up);

        // ���i
        myRb.linearVelocity = swayDir * moveSpeed;
    }

    public void goTargetZ()
    {
        // ��l�� zStartTime�]�Ĥ@���I�s�ɡ^
        if (zStartTime == 0f) zStartTime = Time.time;

        // �p��e�i��V
        Vector3 dir = (nextPos - transform.position);
        float distance = dir.magnitude;

        if (distance < 0.1f)
        {
            myRb.linearVelocity = Vector3.zero;
            return;
        }

        dir.Normalize();

        // �p�⥪�k��V
        Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;

        // Z ����u����
        bool goRight = Mathf.FloorToInt((Time.time - zStartTime) * zFrequency) % 2 == 0;

        Vector3 offset = (goRight ? 1 : -1) * side * zAmplitude;

        // �̲פ�V
        Vector3 moveDir = (dir + offset).normalized;

        // ���V��V
        transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);

        // ����
        myRb.linearVelocity = moveDir * moveSpeed;
    }
    public void goFem()
    {
        transform.LookAt(targetFemPos);
        Vector3 rot3 = transform.localEulerAngles;
        rot3 = new Vector3(0, rot3.y, 0);
        transform.localEulerAngles = rot3;
        myRb.linearVelocity = transform.forward * moveSpeed * femRoachAdditional;
    }
    public void goFood()
    {
        transform.LookAt(targetFoodPos);
        Vector3 rot3 = transform.localEulerAngles;
        rot3 = new Vector3(0, rot3.y, 0);
        transform.localEulerAngles = rot3;
        myRb.linearVelocity = transform.forward * moveSpeed * foodRoachAdditional;
    }

    public void rollTarget()
    {
        int randomPlace = UnityEngine.Random.Range(0, foodGenManager.FoodPos.Count);
        nextPos = foodGenManager.FoodPos[randomPlace].transform.position;
        //Debug.Log("My Next dest is" + foodGenManager.FoodPos[randomPlace].transform.position);

        Invoke("rollTarget", UnityEngine.Random.Range(3, 5));
    }

    private bool findPlayer = false;
    private void CanTrySpecialBehavior()
    {
        if (moveMode == MoveMode.Snake)
        {
            if(!onSpecialCooldown)
            {
                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                // �ϥ� SphereCast ���N Raycast
                if (Physics.SphereCast(ray, radius, out hit, detectDistance))
                {
                    if (hit.collider != null && hit.collider.CompareTag("Player"))
                    {
                        playerTarget = hit.collider.transform;
                        findPlayer = true;
                    }
                }
            }
        }

        if (moveMode == MoveMode.Robot)
        {
            if (Vector3.Distance(transform.position, Player.transform.position) <= socialRange)
            {
                findPlayer = true;                
            }
        }

    }

    private Coroutine scaredRoutine = null;

    private void ScaredByPlayer()
    {
        if (scaredRoutine == null)
        {
            scaredRoutine = StartCoroutine(ScaredRoutine());
        }
    }

    private IEnumerator ScaredRoutine()
    {
        myRb.constraints &= ~RigidbodyConstraints.FreezePositionY;
        while (Vector3.Distance(transform.position, Player.transform.position) <= socialRange)
        {
            // ��a���D�]�� Rigidbody �O�q���W�h�^
            myRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            
            SoundManager.Play("SFX_rat-squeaks");

            // �� 5 ��A�U�@��
            yield return new WaitForSeconds(5f);
        }

        // ���a���}�d����_
        myRb.constraints |= RigidbodyConstraints.FreezePositionY;
        scaredRoutine = null;
        findPlayer = false;
    }

    private void FollowPlayer()
    {
        if (playerTarget == null) return;
        Vector3 dir = (playerTarget.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        myRb.linearVelocity = dir * moveSpeed;

        if (!inTalkRange && Vector3.Distance(transform.position, playerTarget.position) <= socialRange)
        {
            StartCoroutine(PlayerAttackRoutine());
        }
    }
    private IEnumerator PlayerAttackRoutine()
    {
        inTalkRange = true;
        myRb.linearVelocity = Vector3.zero;
        // �o�X�s�n�⦸
        string[] clips = { "SFX_talking-nya", "SFX_talking-nya2", "SFX_talking-nya3" };

        // �� GUID �ͦ� seed�A�T�O�C�������H�����P
        int seed = Guid.NewGuid().GetHashCode();
        System.Random rng = new System.Random(seed);

        for (int i = 0; i < 4; i++)
        {
            int randomIndex = rng.Next(0, clips.Length); // �q 0 �� clips.Length-1 �H��
            SoundManager.Play(clips[randomIndex]);
            yield return new WaitForSeconds(0.2f); // �s�n���j
        }

        yield return new WaitForSeconds(1f);
        playerTarget = null;
        onSpecialCooldown = true;
        inTalkRange = false;
        findPlayer = false;
        yield return new WaitForSeconds(coolDownTime);
        onSpecialCooldown = false;
    }

    private void TryMutantDetectAndCharge()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.SphereCast(ray, radius, out RaycastHit hit, detectDistance))
        {
            if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("NPCRoach"))
            {
                chargeTarget = hit.collider.transform;
                StartCoroutine(MutantChargeRoutine());
            }
        }
    }

    private bool hasHitTarget = false; // �����O�_�w�g����

    private IEnumerator MutantChargeRoutine()
    {
        isCharging = true;
        hasHitTarget = false; // �C���}�l�Ĩ볣���m
        float chargeDuration = 1.5f; // �̪��Ĩ�ɶ�
        float elapsed = 0f;

        while (elapsed < chargeDuration && chargeTarget != null && !hasHitTarget)
        {
            Vector3 dir = (chargeTarget.position - transform.position).normalized;
            myRb.linearVelocity = dir * (moveSpeed * 2.5f); // ���t�ק��
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ����Ĩ�
        myRb.linearVelocity = Vector3.zero;
        isCharging = false;
        chargeTarget = null;

        onSpecialCooldown = true;
        yield return new WaitForSeconds(coolDownTime);
        onSpecialCooldown = false;
    }



    public void areaContainFem()
    {

    }

    public void areaContainFood()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cockroachManager.dashing == true)
        {
            //KYS
            FindFirstObjectByType<CockroachManager>().PlayHungryAttentionFadeOnce();
            allGameManager.fuckNPCCollect++;
            allGameManager.AddScore(allGameManager.fuckNPCScore);
            SaveSystem.mySaveFile.NPCKillNum++;
            allGameManager.InRoundKillNpc++;
            Destroy(gameObject);
        }
    }
    [Obsolete]
    private void OnCollisionEnter(Collision collision)
    {
        if (!isMutant || !isCharging) return;

        GameObject other = collision.gameObject;

        // ���a�Q��
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                if(cockroachManager.shield >= 0)
                {
                    cockroachManager.shield = 0;
                    SoundManager.Play("SFX_shield-block");
                }
                else
                {
                    cockroachManager.PlayHungryAttentionFadeOnce();
                    cockroachManager.DecreaseHunger(10f);
                }
                StartCoroutine(KnockbackEntity(playerRb));
                SoundManager.Play("SFX_hit");
            }

            // ����Ĩ�
            isCharging = false;
            chargeTarget = null;
            onSpecialCooldown = true;
            StartCoroutine(MutantCooldown());
        }

        // ��L NPC �Q��
        if (other.CompareTag("NPCRoach"))
        {
            Rigidbody npcRb = other.GetComponent<Rigidbody>();
            if (npcRb != null)
            {
                StartCoroutine(KnockbackEntity(npcRb, () =>
                {
                    // ���������ᦺ�`
                    other.GetComponent<NPCRoach>()?.DynDestroy();
                }));
            }

            // ����Ĩ�
            isCharging = false;
            chargeTarget = null;
            onSpecialCooldown = true;
            StartCoroutine(MutantCooldown());
        }
    }

    [Obsolete]
    private IEnumerator KnockbackEntity(Rigidbody rb, Action onComplete = null)
    {
           
        float knockbackForce = 30f;   // ��l�O�D
        float duration = 1f;          // ��������ɶ�
        float elapsed = 0f;

        if (rb == null)
            yield break;
        // ������V�]�u�u X �b�^
        Vector3 knockDir = rb.transform.position - transform.position;
        knockDir.y = 0f;
        knockDir.z = 0f;
        knockDir.Normalize();

        while (elapsed < duration)
        {
           if (rb == null || rb.Equals(null))
           {
              yield break; // ������{�A�קKMissingReferenceException
           }
           float t = 1f - (elapsed / duration); // �v����p
           rb.velocity = knockDir * knockbackForce * t;

           elapsed += Time.deltaTime;     
            yield return null;
        }

        if (rb != null && !rb.Equals(null))
        {
            rb.velocity = Vector3.zero;
        }

        // ����������^�ա]NPC ���`�^
        //onComplete?.Invoke();

    }   

    private IEnumerator MutantCooldown()
    {
        yield return new WaitForSeconds(coolDownTime);
        onSpecialCooldown = false;
    }
    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        SoundManager.Play("SFX_Death_V1");
        tellDyIamDead = true;
        Instantiate(burstBlood, transform.position, Quaternion.Euler(0,-90,0));
        nPCRoachManager.nPCRoaches.Remove(this);
    }

    public void DynDestroy()
    {
        if (!gameObject.scene.isLoaded) return;
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // ��ܮg�u�����Z��
        Gizmos.color = Color.blue;
        Vector3 forward = transform.forward * detectDistance;
        Gizmos.DrawLine(transform.position, transform.position + forward);

        // ��ܧ����d��y
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, socialRange);

        if (detectDistance <= 0) return;

        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * detectDistance;

        Gizmos.color = Color.yellow;

        // �_�I�P���I�y
        Gizmos.DrawWireSphere(start, radius);
        Gizmos.DrawWireSphere(end, radius);

        // �s���u
        Gizmos.DrawLine(start + transform.up * radius, end + transform.up * radius);
        Gizmos.DrawLine(start - transform.up * radius, end - transform.up * radius);
        Gizmos.DrawLine(start + transform.right * radius, end + transform.right * radius);
        Gizmos.DrawLine(start - transform.right * radius, end - transform.right * radius);
    }
}
