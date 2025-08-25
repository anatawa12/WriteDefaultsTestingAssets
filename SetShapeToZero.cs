using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;

//[DefaultExecutionOrder(-10000000)]
public class SetShapeToZero : MonoBehaviour
{
    public PlayerLoopTiming timing;
    public float[] preSetWeights = new float[0];

    private void Start()
    {
        PrintPlayerLoop(PlayerLoop.GetDefaultPlayerLoop(), "Default Player Loop");
        PrintPlayerLoop(PlayerLoop.GetCurrentPlayerLoop(), "Current Player Loop");
    }
    private static void PrintPlayerLoop(PlayerLoopSystem playerLoopSystem, string prefix)
    {
        StringBuilder sb = new();
        // Pass the default Player loop to the recursive print function
        RecursivePlayerLoopPrint(playerLoopSystem, sb, 0);
        Debug.Log(prefix + ": " + sb.ToString());
    }
    // Loop through the Player loop tree structure and add the names of the systems to the StringBuilder
    private static void RecursivePlayerLoopPrint(PlayerLoopSystem playerLoopSystem, StringBuilder sb, int depth)
    {
        if (depth == 0)
        {
            sb.AppendLine("ROOT NODE");
        }
        else if (playerLoopSystem.type != null)
        {
            for (int i = 0; i < depth; i++)
            {
                sb.Append("\t");
            }
            sb.AppendLine(playerLoopSystem.type.Name);
        }
        if (playerLoopSystem.subSystemList != null)
        {
            depth++;
            foreach (var s in playerLoopSystem.subSystemList)
            {
                RecursivePlayerLoopPrint(s, sb, depth);
            }
            depth--;
        }
    }

    async void Update()
    {
        //StartCoroutine(nameof(MainCoroutine));
        await Cysharp.Threading.Tasks.UniTask.Yield(timing);
        SetWeightZero(GetComponent<SkinnedMeshRenderer>());
    }

    /*
    IEnumerator MainCoroutine()
    {
        //while (true)
        do
        {
            yield return new WaitForEndOfFrame();
            SetWeightZero(GetComponent<SkinnedMeshRenderer>());
        } while (false);
    }

    private void FixedUpdate()
    {
        
        SetWeightZero(GetComponent<SkinnedMeshRenderer>());
    }
    // */

    private void SetWeightZero(SkinnedMeshRenderer smr)
    {
        if (smr == null) return;
        var mesh = smr.sharedMesh;
        if (mesh == null) return;
        // call SetBlendShapeWeight
        if (preSetWeights.Length != mesh.blendShapeCount) preSetWeights = new float[mesh.blendShapeCount];
        for (var i = 0; i < mesh.blendShapeCount; i++)
        {
            preSetWeights[i] = smr.GetBlendShapeWeight(i);
            smr.SetBlendShapeWeight(i, 0f);
        }
    }
}
