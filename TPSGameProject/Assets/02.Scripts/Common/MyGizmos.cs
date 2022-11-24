using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL = 0, WAYPOINT}
    private const string wayPointFile = "Enemy";
    public Type type = Type.NORMAL;
    public Color _color = Color.red;
    public float _radius = 0.5f;
    private void OnDrawGizmos()
    {
        if(type == Type.NORMAL)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else
        {
            Gizmos.color = _color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1f, wayPointFile, true);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
