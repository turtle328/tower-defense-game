using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class BatHandler : MonoBehaviour {

    #region Job System Stuff
    public GameObject batPrefab;
    TransformAccessArray transforms;
    NativeList<float> randNums;
    MovementJob moveJob;
    JobHandle moveHandle;
    float moveSpeed = 2f;
    int lowerBoundX = -23;
    int upperBoundX = 23;
    int lowerBoundZ = -17;
    int upperBoundZ = 28;
    readonly int initialBats = 5;
    bool batsEnabled = true;
    static BatHandler instance;

    //readonly int scaleFactor = 2;
    //int maxSize = 10;
    //float randDeg = 1f;
    #endregion

    void Randomizer()
    {
        moveHandle.Complete();
        for (int i = 0; i < randNums.Length; i++)
        {
            randNums[i] = Random.Range(-1f, 1f);
        }
    }

    // Use this for initialization
    void Start ()
    {
        instance = this;
        transforms = new TransformAccessArray(0, -1);
        randNums = new NativeList<float>(initialBats, Allocator.Persistent);
        InvokeRepeating("Randomizer", 0, .5f);
        InstantiateBatFlock();
    }
	
    public static BatHandler getInstance()
    {
        return instance;
    }

	// Update is called once per frame
	void Update ()
    {
        if (batsEnabled)
        {
            moveHandle.Complete();

            if (Input.GetKeyDown("space"))
            {
                SpawnBat();
            }

            moveJob = new MovementJob()
            {
                moveSpeed = moveSpeed,
                deltaTime = Time.deltaTime,
                lowerBoundX = lowerBoundX,
                upperBoundX = upperBoundX,
                lowerBoundZ = lowerBoundZ,
                upperBoundZ = upperBoundZ,
                randomNums = randNums
                //randDirDeg = randDeg,
            };
            moveHandle = moveJob.Schedule(transforms);

            JobHandle.ScheduleBatchedJobs();
        }
    }

    public void ToggleBats()
    {
        batsEnabled = !batsEnabled;
        for (int i = 0; i < transforms.length; i++)
            transforms[i].gameObject.SetActive(batsEnabled);
    }

    private void InstantiateBatFlock()
    {
        GameObject[] bats = new GameObject[initialBats];

        for (int i = 0; i < initialBats; i++)
        {
            bats[i] = SpawnBat();
        }

        bats[1].transform.position -= new Vector3(2f, 0, 2f);
        bats[2].transform.position -= new Vector3(4f, 0, 4f);
        bats[3].transform.position -= new Vector3(-2f, 0, 2f);
        bats[4].transform.position -= new Vector3(-4, 0, 4f);
    }

    private GameObject SpawnBat()
    {
        GameObject bat = Instantiate(batPrefab, new Vector3(0f, 5f, -12f), Quaternion.identity);
        bat.transform.localScale = new Vector3(10, 10, 10);
        transforms.Add(bat.transform);
        randNums.Add(0f);
        //if (randNums.Length < ++numBats) ResizeNativeArray();
        return bat;
    }

    private void OnDestroy()
    {
        moveHandle.Complete();
        transforms.Dispose();
        randNums.Dispose();
    }

    //private void ResizeNativeArray()
    //{
    //    maxSize *= scaleFactor;
    //    NativeArray<float> nativeArray = new NativeArray<float>(maxSize, Allocator.Persistent);
    //    moveHandle.Complete();
    //    randNums.Dispose();
    //    randNums = nativeArray;
    //}
}
