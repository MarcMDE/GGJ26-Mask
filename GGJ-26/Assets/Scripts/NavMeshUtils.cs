using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public static class NavMeshUtils
{
    /// <summary>
    /// Finds N non-overlapping points on the NavMesh within a specific area.
    /// </summary>
    public static List<Vector3> GetMultipleSafePoints(Vector3 centerSeed, int n, float circleRadius, float spacing)
    {
        List<Vector3> placedPoints = new List<Vector3>();
        int maxGlobalAttempts = 15;

        for (int attempt = 0; attempt < maxGlobalAttempts; attempt++)
        {
            placedPoints.Clear();

            for (int i = 0; i < n; i++)
            {
                // Golden Ratio Spiral (Fermat's Spiral) for optimal packing
                float angle = i * 137.5f * Mathf.Deg2Rad;
                float dist = (circleRadius * 2 + spacing) * Mathf.Sqrt(i);
                Vector3 candidate = centerSeed + new Vector3(Mathf.Cos(angle) * dist, 0, Mathf.Sin(angle) * dist);

                if (IsCircleClearOnNavMesh(candidate, circleRadius))
                {
                    placedPoints.Add(candidate);
                }
            }

            if (placedPoints.Count >= n) break;
        }
        return placedPoints;
    }

    public static bool IsCircleClearOnNavMesh(Vector3 center, float r)
    {
        int checkCount = 8; // Perimeter sampling
        for (int i = 0; i < checkCount; i++)
        {
            float angle = i * (360f / checkCount) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, 0, Mathf.Sin(angle) * r);
            Vector3 checkPoint = center + offset;

            NavMeshHit hit;
            // 1. Is the perimeter point on the mesh?
            if (!NavMesh.SamplePosition(checkPoint, out hit, 0.5f, NavMesh.AllAreas)) return false;

            // 2. Is there a wall between the center and the perimeter?
            if (NavMesh.Raycast(center, checkPoint, out hit, NavMesh.AllAreas)) return false;
        }
        return true;
    }
}