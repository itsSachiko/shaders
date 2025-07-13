using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 1f;

    private bool isActiveTrail;

    private bool isActiveFirst;

    public float meshRefreshRate = .1f;

    public float meshDestroyDelay = .1f;

    public string shaderRef;
    public float shaderRate = .1f;
    public float shaderRefreshRate = .05f;

    public Material mat;

    public Transform updatedPos;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private void Start()
    {
        isActiveFirst = true;
    }
    private void Update()
    {
        if(isActiveFirst && !isActiveTrail)
        StartCoroutine(ActivateTrail(activeTime));
    }

    IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            for (int i = 0; i<skinnedMeshRenderers.Length; i++)
            {
                GameObject skinObj = new GameObject();
                skinObj.transform.SetLocalPositionAndRotation(updatedPos.position, updatedPos.rotation);

                MeshRenderer mr = skinObj.AddComponent<MeshRenderer>();
                MeshFilter mf = skinObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);
                mf.mesh = mesh;

                mr.material = mat;

                StartCoroutine(FadeAnimation(mr.material, 0, shaderRate, shaderRefreshRate));

                Destroy(skinObj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshRefreshRate);
        }
        isActiveTrail = false;
    }

    IEnumerator FadeAnimation(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderRef, valueToAnimate);

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
