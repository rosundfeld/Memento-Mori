using UnityEngine;
using System.Collections.Generic;

public class UnitTargeting : MonoBehaviour
{
    public float CalculateScore(Unit self, Unit target)
    {
        float score = 0;

        float distance = Vector3.Distance(self.transform.position, target.transform.position);

        // 🎯 Prioridade: mais perto = melhor
        score += 10f / (distance + 0.1f);

        // 🩸 Prioridade: alvo com pouca vida
        score += (1f - (target.CurrentHealth / target.Stats.maxHealth)) * 5f;

        // 😡 Aggro: quem bateu em mim recentemente
        if (self.WasRecentlyAttackedBy(target))
            score += 15f;

        return score;
    }

    public Unit FindBestTarget(Unit self, List<Unit> enemies)
    {
        TargetInfo best = null;

        foreach (var enemy in enemies)
        {
            if (!IsInRange(self, enemy)) continue;

            float score = CalculateScore(self, enemy);

            if (best == null || score > best.score)
            {
                best = new TargetInfo
                {
                    unit = enemy,
                    score = score
                };
            }
        }

        return best?.unit;
    }

    public bool IsInRange(Unit self, Unit target)
    {
        float distance = Vector3.Distance(self.transform.position, target.transform.position);
        return distance <= self.Stats.attackRange;
    }
}
