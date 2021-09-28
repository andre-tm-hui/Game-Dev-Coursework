using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public string sortingLayer;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            r.sortingLayerName = sortingLayer;
        }
    }
}
