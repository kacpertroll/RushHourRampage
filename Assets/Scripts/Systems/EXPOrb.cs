using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public float expValue = 5;
    public float moveSpeed = 10f;

    private Transform player;
    private PlayerStatistics stats;

    private bool isMovingToPlayer = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        GameObject statManager = GameObject.FindGameObjectWithTag("Stat Manager");
        if (playerObj != null)
        {
            player = playerObj.transform;
            stats = statManager.GetComponent<PlayerStatistics>();
        }
    }

    private void Update()
    {
        if (player == null || stats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isMovingToPlayer && distance <= stats.pickupRange)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime
            );

            // jeśli orb dotknie gracza → dodaj exp i zniszcz się
            if (distance <= 0.5f)
            {
                stats.AddExp(expValue);
                Destroy(gameObject);
            }
        }
    }

    public void SetExpValue(float value)
    {
        expValue = value;
    }
}
