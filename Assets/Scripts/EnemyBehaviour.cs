using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public float maxHealth = 10f;
    private float currentHealth;

    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    public GameObject deathEffect;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            animator.SetTrigger("isDead");
            DeathSequence();
        }
    }

    private void DeathSequence()
    {
        agent.isStopped = true;
        gameObject.tag = "Untagged";

        Invoke(nameof(Die), 1f);
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            var vfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            var ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(vfx, ps.main.duration);
            }
            else
            {
                var psChild = vfx.GetComponentInChildren<ParticleSystem>();
                if (psChild != null)
                {
                    Destroy(vfx, psChild.main.duration);
                }
                else
                {
                    Destroy(vfx, 2f); // fallback
                }
            }
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            ProjectileMove projectile = collision.gameObject.GetComponent<ProjectileMove>();
            if (projectile != null)
            {
                TakeDamage(projectile.attackDamage);
            }

            Destroy(collision.gameObject); // zniszcz pocisk po trafieniu
        }
    }
}
