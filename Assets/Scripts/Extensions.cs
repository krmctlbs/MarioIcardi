using UnityEngine;

public static class Extensions
{
    private static LayerMask layerMask = LayerMask.GetMask("Default");
    public static bool Raycast(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if(rigidbody.bodyType == RigidbodyType2D.Kinematic){  // rigidbody.isKinematic or rigidbody.bodyType or?? rigidbody.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Kinematic 
            return false;
        }

        float radius = 0.25f;
        float distance = 0.375f;

        RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, radius, direction.normalized, distance, layerMask);
        return hit.collider != null && hit.rigidbody != rigidbody; //for safety we already used layermask 
    }

    public static bool DotTest(this Transform transform, Transform other, Vector2 testDirection)
    {

        Vector2 direction = other.position - transform.position; //direction of mario to block
        return Vector2.Dot(direction.normalized, testDirection) > 0.25f; //or 0.2??? //normalized _> makes the vector unit

    }
}
