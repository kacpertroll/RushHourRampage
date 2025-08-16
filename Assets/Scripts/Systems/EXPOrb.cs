using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    public float expValue = 5;
    public float pickupRange = 3f; // dystans od gracza, kiedy orb zaczyna się przyciągać
    public float moveSpeed = 10f;

    private Transform player;
    private bool isMovingToPlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isMovingToPlayer && distance <= pickupRange)
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
                PlayerExpManager expManager = player.GetComponent<PlayerExpManager>();
                if (expManager != null)
                {
                    expManager.AddExp(expValue);
                }

                Destroy(gameObject);
            }
        }
    }

    public void SetExpValue(float value)
    {
        expValue = value;
    }
}
