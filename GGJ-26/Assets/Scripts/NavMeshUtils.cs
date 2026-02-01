using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public static class NavMeshUtils
{
    /// <summary>
    /// Finds N non-overlapping points by first picking a random valid seed within bounds.
    /// </summary>
    public static List<Vector3> GetMultipleSafePoints(Bounds bounds, int n, float circleRadius, float spacing, int attempts)
    {
        List<Vector3> placedPoints = new List<Vector3>();
        Vector3 seedPoint = Vector3.zero;
        bool seedFound = false;

        // 1. Find a random valid Seed Point within the bounds
        for (int i = 0; i < attempts; i++)
        {
            float rx = Random.Range(bounds.min.x, bounds.max.x);
            float rz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 randomPos = new Vector3(rx, bounds.center.y, rz);

            // Check if this random spot can actually hold a circle
            if (IsCircleClearOnNavMesh(randomPos, circleRadius))
            {
                seedPoint = randomPos;
                seedFound = true;
                break;
            }
        }

        if (!seedFound) return placedPoints; // Return empty list if no space found

        // 2. Pack the remaining points around that seed
        for (int i = 0; i < attempts; i++)
        {
            float rx = Random.Range(bounds.min.x, bounds.max.x);
            float rz = Random.Range(bounds.min.z, bounds.max.z);
            Vector3 candidate = new Vector3(rx, bounds.center.y, rz);

            bool shouldContinue = false;
            foreach(var p in placedPoints)
            {
                if (Vector3.Distance(candidate, p) < spacing) shouldContinue = true;
            }

            if(shouldContinue) continue;

            if (IsCircleClearOnNavMesh(candidate, circleRadius))
            {
                placedPoints.Add(candidate);

                if(placedPoints.Count == n) break;
            }
        }

        return placedPoints;
    }

    public static bool IsCircleClearOnNavMesh(Vector3 center, float r)
    {
        // First, snap the center to the NavMesh
        NavMeshHit centerHit;
        int walkableMask = 1 << NavMesh.GetAreaFromName("Walkable");
        if (!NavMesh.SamplePosition(center, out centerHit, 2.0f, walkableMask))
            return false;

        Vector3 snappedCenter = centerHit.position;

        // Check 8 points on the perimeter
        int checkCount = 8;
        for (int i = 0; i < checkCount; i++)
        {
            float angle = i * (360f / checkCount) * Mathf.Deg2Rad;
            Vector3 checkPoint = snappedCenter + new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);

            NavMeshHit hit;
            // Point must be on NavMesh
            if (!NavMesh.SamplePosition(checkPoint, out hit, 0.5f, NavMesh.AllAreas))
                return false;

            // Must have clear line of sight (no walls between center and edge)
            if (NavMesh.Raycast(snappedCenter, checkPoint, out hit, NavMesh.AllAreas))
                return false;
        }
        return true;
    }
}