
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {

        RaycastHit hit;
        if (Physics.Raycast(position + new Vector3(0,30,0), -transform.up, out hit, 60, layerMask))
        {
            if (hit.transform.CompareTag("Terrain"))
            {
                return new Vector3(position.x, hit.point.y, position.z);

            }
        }
        return position;
    }
}
