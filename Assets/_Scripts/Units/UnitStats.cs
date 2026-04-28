using UnityEngine;

[System.Serializable]
public class UnitStats : MonoBehaviour
{
    public float health;
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
