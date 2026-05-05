using UnityEngine;

[CreateAssetMenu(fileName = "UnitStats", menuName = "Scriptable Objects/UnitStats")]
public class UnitStats : ScriptableObject
{
    public float maxHealth;
    public float mana;
    public float attackDamage;
    public float attackSpeed;
    public float moveSpeed;
    public float treat;
    public float attackRange;

    public UnitStats Clone()
    {
        return (UnitStats)this.MemberwiseClone();
    }
}
