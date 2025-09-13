using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class MeteoriteManager : MonoBehaviour
{
    private CockroachMove mainMoveScript;
    public GameObject Meteorite;
    public GameObject Player;

    public Transform SpawnXpZp;
    public Transform SpawnXmZm;

    public Transform LandXpZp;
    public Transform LandXmZm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainMoveScript = GameObject.Find("3DCockroach").GetComponent<CockroachMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnMeteorite(true);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SpawnMeteorite(false);
        }
    }

    /// <summary>
    /// true = aim player
    /// false = random drop
    /// </summary>
    /// <param name="isAimPlayer"></param>
    public void SpawnMeteorite(bool isAimPlayer)
    {
        Vector3 RanSpawn = new Vector3(Random.Range(SpawnXmZm.position.x, SpawnXpZp.position.x), SpawnXpZp.position.y, Random.Range(SpawnXmZm.position.z, SpawnXpZp.position.z));
        Vector3 RanTo = new Vector3(Random.Range(LandXmZm.position.x, LandXpZp.position.x), LandXpZp.position.y, Random.Range(LandXmZm.position.z, LandXpZp.position.z));

        if (isAimPlayer && mainMoveScript.isInTheHole == false)
        {
            GameObject obj = Instantiate(Meteorite, RanSpawn, Quaternion.identity);
            obj.GetComponent<Meteorite>().from = RanSpawn;
            obj.GetComponent<Meteorite>().to = Player.transform.position;
        }
        else
        {
            GameObject obj = Instantiate(Meteorite, RanSpawn, Quaternion.identity);
            obj.GetComponent<Meteorite>().from = RanSpawn;
            obj.GetComponent<Meteorite>().to = RanTo;

        }
    }
}
