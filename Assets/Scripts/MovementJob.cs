using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;
using System;

//https://www.youtube.com/watch?time_continue=108&v=WZ6-LxwxWEI&feature=emb_logo

//[ComputeJobOptimisation]
public struct MovementJob : IJobParallelForTransform
{
    public float moveSpeed;
    public float boundTop;
    public float boundBottom;
    public float deltatime;

    public void Execute(int index, TransformAccess transform)
    {
        //Vector3 pos = transform.position;
        //pos += moveSpeed * deltatime * (transform.rotation * Vector3.up);

        //if (pos.z < boundBottom)
        //{
        //    pos.z = boundTop;
        //}

        //transform.position = pos;

        transform.rotation=Quaternion.Euler(transform.rotation.x+ moveSpeed * deltatime, transform.rotation.y, transform.rotation.z);
    }

    
}
