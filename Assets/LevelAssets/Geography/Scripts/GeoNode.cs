using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoNode : MonoBehaviour
{
    private void OnDestroy()
    {
        transform.parent.transform.parent.GetComponent<RockGenerator>().geoNodes--;
    }
}
