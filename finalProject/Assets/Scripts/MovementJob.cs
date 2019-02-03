using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Collections;

[BurstCompile]
public struct MovementJob : IJobParallelForTransform
{
    public float moveSpeed;
    public float deltaTime;
    public int lowerBoundX;
    public int upperBoundX;
    public int lowerBoundZ;
    public int upperBoundZ;
    [ReadOnly]
    public NativeList<float> randomNums;

    public void Execute(int index, TransformAccess transform)
    {
        //Vector3 randDir = new Vector3(randomX, 0f, randomZ);
        //pos += moveSpeed * deltaTime * new Vector3(0f, 0f, 1f);
        //Debug.Log(transform.rotation.eulerAngles.y);
        Vector3 pos = transform.position; 
        transform.rotation = Quaternion.AngleAxis(transform.rotation.eulerAngles.y + randomNums[index], Vector3.up);
        pos += transform.rotation * Vector3.forward * moveSpeed * deltaTime;

        if (pos.z < lowerBoundZ)
        {
            pos.z = upperBoundZ;
        }
        else if (pos.z > upperBoundZ)
        {
            pos.z = lowerBoundZ;
        }
        else if (pos.x < lowerBoundX)
        {
            pos.x = upperBoundX;
        }
        else if (pos.x > upperBoundX)
        {
            pos.x = lowerBoundX;
        }
        transform.position = pos;
    }
}