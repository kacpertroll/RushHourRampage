using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("References")]
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    [Header("Economy")]
    public GameObject expOrbPrefab;
    public float expReward = 1.0f;
    public float damage = 20f;

    [Header("VFX")]
    public GameObject deathEffect;
    public Material flashMaterial; // materia³ do flasha przy obra¿eniach
    private Renderer[] meshRenderers;
    private Material[] originalMaterials;

    [Header("Physics")]
    public float knockbackForce = 3f;
    public float knockbackDuration = 0.2f;


    private bool isKnockedback = false;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<Renderer>();
        originalMaterials = new Material[meshRenderers.Length];
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            originalMaterials[i] = meshRenderers[i].material;
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (player != null && currentHealth > 0f)
        {
            agent.SetDestination(player.position);
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    public void TakeDamage(float damage, Vector3 hitSource)
    {
        currentHealth -= damage;

        StartCoroutine(FlashEffect());
        StartCoroutine(Knockback(hitSource));

        if (currentHealth <= 0f)
        {
            animator.SetTrigger("isDead");
            DeathSequence();
        }
    }

    private System.Collections.IEnumerator FlashEffect()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = flashMaterial;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material = originalMaterials[i];
        }
    }

    private void DeathSequence()
    {
        agent.isStopped = true;
        gameObject.tag = "Untagged";
        gameObject.GetComponent<CapsuleCollider>().enabled = false;

        Invoke(nameof(Die), 0.5f); // czekamy na animacjê
    }

    private void Die()
    {
        if (deathEffect != null)
        {
            var vfx = Instantiate(deathEffect, transform.position, Quaternion.identity);
            var ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
                Destroy(vfx, ps.main.duration);
            else
                Destroy(vfx, 2f);
        }

        if (expOrbPrefab != null)
        {
            GameObject orb = Instantiate(expOrbPrefab, transform.position, Quaternion.identity);
            ExpOrb orbScript = orb.GetComponent<ExpOrb>();
            if (orbScript != null)
            {
                orbScript.SetExpValue(expReward);
            }
        }

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator Knockback(Vector3 sourcePosition)
    {
        isKnockedback = true;
        agent.isStopped = true;

        Vector3 dir = (transform.position - sourcePosition).normalized;
        Vector3 targetPos = transform.position + dir * knockbackForce;

        float elapsed = 0f;
        while (elapsed < knockbackDuration)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (currentHealth > 0f)
        {
            agent.isStopped = false;
            isKnockedback = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.attackDamage, collision.transform.position);
            }

            Destroy(collision.gameObject);
        }
    }

}
