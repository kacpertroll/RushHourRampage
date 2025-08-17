using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 10f;
    public float attackCooldown = 1f;
    public float attackDamage = 5f;

    public GameObject projectilePrefab;

    public Transform firePoint;

    private float attackTimer = 0f;
    private Transform currentTarget;
    private Animator animator;

    public bool IsTargetingEnemy { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null)
        {
            IsTargetingEnemy = true;
            FaceTarget();
            HandleAttack();
        }
        else
        {
            IsTargetingEnemy = false;
            animator.SetBool("isAttacking", false);
        }
    }

    private void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            animator.SetBool("isAttacking", true);
            ShootProjectile();
            attackTimer = attackCooldown;
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null && currentTarget != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Vector3 direction = (currentTarget.position - firePoint.position).normalized;
            projectile.transform.forward = direction;

            // Opcjonalnie dodaj siłę/komponent fizyki jeśli pocisk porusza się sam
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * 20f; // Prędkość pocisku
            }
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (currentTarget.position - transform.position).normalized;
        direction.y = 0; // Zablokuj rotację w osi Y

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = attackRange;
        currentTarget = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy.transform;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Zasięg ataku
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Linia do celu
        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position + Vector3.up, currentTarget.position + Vector3.up);
        }

        // Kierunek z firePoint
        if (firePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}
