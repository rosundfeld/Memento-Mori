using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator animator;

    // Hashes são mais performáticos que strings
    private static readonly int HashIsMoving  = Animator.StringToHash("isMoving");
    private static readonly int HashAttack    = Animator.StringToHash("attack");
    private static readonly int HashHit       = Animator.StringToHash("hit");
    private static readonly int HashDie       = Animator.StringToHash("die");

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool isMoving) =>
        animator.SetBool(HashIsMoving, isMoving);

    public void PlayAttack() =>
        animator.SetTrigger(HashAttack);

    public void PlayHit() =>
        animator.SetTrigger(HashHit);

    public void PlayDie() =>
        animator.SetTrigger(HashDie);
}