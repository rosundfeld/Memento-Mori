using System.Collections.Generic;
using UnityEngine;

public class UnitTargeting : MonoBehaviour
{
    [Header("Pesos do Score")]
    [SerializeField] private float threatWeight  = 10f;
    [SerializeField] private float aggroWeight   = 15f;
    [SerializeField] private float distanceWeight = 2f;

    public float CalculateScore(Unit self, Unit target, float aggroAmount = 0f)
    {
        if (self == null || target == null) return float.MinValue;

        float distance  = Vector3.Distance(self.transform.position, target.transform.position);
        float treat     = target.Stats != null ? target.Stats.treat : 0f;

        float score = (treat     * threatWeight)
                    + (aggroAmount * aggroWeight)
                    + (1f / (1f + distance) * distanceWeight);

        return score;
    }

    public Unit FindBestTarget(Unit self, List<Unit> enemies)
    {
        if (self == null || enemies == null || enemies.Count == 0) return null;

        Unit  best      = null;
        float bestScore = float.MinValue;

        foreach (var e in enemies)
        {
            if (e == null || e == self) continue;

            float aggro = self.GetAggro(e);
            float score = CalculateScore(self, e, aggro);

            if (score > bestScore)
            {
                bestScore = score;
                best      = e;
            }
        }

        return best;
    }

    public bool IsInRange(Unit self, Unit target)
    {
        if (self == null || target == null || self.Stats == null) return false;
        return Vector3.Distance(self.transform.position, target.transform.position) <= self.Stats.attackRange;
    }
}
