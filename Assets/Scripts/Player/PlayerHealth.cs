using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private PlayerStatistics stats;

    private bool isImmune = false;
    private Renderer[] renderers; // wszystkie meshe gracza

    private void Start()
    {
        GameObject statManager = GameObject.FindGameObjectWithTag("Stat Manager");
        if (statManager != null)
        {
            stats = statManager.GetComponent<PlayerStatistics>();
        }
        renderers = GetComponentsInChildren<Renderer>();
    }

    public void TakeDamage(float amount)
    {
        if (isImmune) return;

        stats.currentHP -= amount;
        Debug.Log("You took damage! Your HP is: " + stats.currentHP);
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);

        if (stats.currentHP <= 0)
        {
            Die();
        }

        StartCoroutine(ImmunityCoroutine());
    }

    private IEnumerator ImmunityCoroutine()
    {
        isImmune = true;
        float elapsed = 0f;

        while (elapsed < stats.immunityTime)
        {
            // miganie co 0.1s
            SetRenderersEnabled(false);
            yield return new WaitForSeconds(0.1f);
            SetRenderersEnabled(true);
            yield return new WaitForSeconds(0.1f);

            elapsed += 0.2f;
        }

        isImmune = false;
        SetRenderersEnabled(true); // upewnij siê ¿e widoczny
    }

    private void SetRenderersEnabled(bool state)
    {
        foreach (var rend in renderers)
        {
            if (rend != null)
                rend.enabled = state;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(projectile.attackDamage);
                Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyBehaviour enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
            if (enemy != null)
            {
                TakeDamage(enemy.damage);
            }
        }
    }


    public void Heal(float amount)
    {
        stats.currentHP += amount;
        stats.currentHP = Mathf.Clamp(stats.currentHP, 0, stats.maxHP);
    }

    private void Die()
    {
        Debug.Log("Player dead!");
        // respawn, game over screen itp.
    }
}
