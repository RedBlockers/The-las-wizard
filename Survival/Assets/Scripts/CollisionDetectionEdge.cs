using UnityEngine;

public class CollisionDetectionEdge : MonoBehaviour
{
    [SerializeField]
    private float radius;

    private Collider[] hitColliders;

    [SerializeField]
    private Vector3 centerOffset;

    [SerializeField]
    private PointDetectionEdge[] pointDetectionEdges;

    public MeshRenderer meshRenderer;

    public bool CheckConnection()
    {
        hitColliders = Physics.OverlapSphere(transform.position + centerOffset, radius);

        if (hitColliders.Length > 0)
        {
            foreach (var collider in hitColliders)
            {
                if (collider.CompareTag(transform.tag))
                {
                    return false;
                }else if (collider.CompareTag("Terrain"))
                {
                    return true;
                }
            }
        }

        foreach (var point in pointDetectionEdges)
        {
            point.CheckOverlap();
            if (point.connected)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + centerOffset, radius);
    }
}
