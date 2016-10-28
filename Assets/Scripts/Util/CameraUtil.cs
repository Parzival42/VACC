using UnityEngine;

/// <summary>
/// Utility class for usel camera based methods like raycasting, etc...
/// </summary>
public static class CameraUtil
{
    /// <summary>
    /// Returns the Ray pointing from the camera base to the scene based on the given screen position.
    /// </summary>
    public static Ray ScreenPointToRayFor(this Camera cam, Vector3 screenPosition)
    {
        return cam.ScreenPointToRay(screenPosition);
    }

    /// <summary>
    /// Calculates the collision for the given ray based on the layermask.
    /// The hitPoint contains the collision data.
    /// 
    /// If the return value is false, the hitpoint will contain (0, 0, 0).
    /// </summary>
    public static bool Collisionfor(Ray r, int layerMask, float maxDistance, out Vector3 hitPoint)
    {
        RaycastHit hit;
        return Collisionfor(r, layerMask, maxDistance, out hitPoint, out hit);
    }

    /// <summary>
    /// Calculates the collision for the given ray based on the layermask.
    /// The hitPoint contains the collision data.
    /// 
    /// If the return value is false, the hitpoint will contain (0, 0, 0).
    /// </summary>
    public static bool Collisionfor(Ray r, int layerMask, float maxDistance, out Vector3 hitPoint, out RaycastHit hit)
    {
        if (Physics.Raycast(r.origin, r.direction, out hit, maxDistance, 1 << layerMask))
        {
            hitPoint = hit.point;
            return true;
        }
        else
            hitPoint = Vector3.zero;

        return false;
    }

    /// <summary>
    /// Calculates the collision for the given ray based on the layermask.
    /// The hitPoint contains the collision data.
    /// 
    /// If the return value is false, the hitpoint will contain (0, 0, 0).
    /// </summary>
    public static bool CollisionFor(this Camera cam, Ray r, int layerMask, out Vector3 hitPoint)
    {
        return Collisionfor(r, layerMask, cam.farClipPlane, out hitPoint);
    }

    /// <summary>
    /// Calculates the collision for the given ray based on the layermask.
    /// The hitPoint contains the collision data.
    /// 
    /// If the return value is false, the hitpoint will contain (0, 0, 0).
    /// </summary>
    public static bool CollisionFor(this Camera cam, Ray r, int layerMask, out Vector3 hitPoint, out RaycastHit hit)
    {
        return Collisionfor(r, layerMask, cam.farClipPlane, out hitPoint, out hit);
    }
}
