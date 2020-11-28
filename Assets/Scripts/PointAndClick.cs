﻿using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Rendering;
using UnityEngine;

class PointAndClick : MonoBehaviour
{
    public static readonly float RAYCAST_DISTANCE = 1000;
    public Camera Cam;

    PhysicsWorld physicsWorld => World.DefaultGameObjectInjectionWorld.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;
    EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    void LateUpdate()
    {
        if (!Input.GetMouseButtonDown(0) || Cam == null) return;

        var screenPointToRay = Cam.ScreenPointToRay(Input.mousePosition);
        var rayInput = new RaycastInput
        {
            Start = screenPointToRay.origin,
            End = screenPointToRay.GetPoint(RAYCAST_DISTANCE),
            Filter = CollisionFilter.Default
        };

        if (!physicsWorld.CastRay(rayInput, out Unity.Physics.RaycastHit hit)) return;

        var selectedEntity = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
        var renderMesh = entityManager.GetSharedComponentData<RenderMesh>(selectedEntity);
        var mat = new UnityEngine.Material(renderMesh.material);
        mat.SetColor("_Color", UnityEngine.Random.ColorHSV());
        renderMesh.material = mat;

        entityManager.SetSharedComponentData(selectedEntity, renderMesh);
    }
}