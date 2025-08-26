using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    private Slider healthSlider;
    public float smoothSpeed = 0.2f;

    private PlayerStatistics stats;
    private GameObject player;
    private Transform playerPos;
    private Transform cam;
    [Space]
    public bool hideBar = false;

    private void Start()
    {
        healthSlider = GetComponentInChildren<Slider>();
        GameObject statManager = GameObject.FindGameObjectWithTag("Stat Manager");
        player = GameObject.FindGameObjectWithTag("Player");
        if (statManager != null)
        {
            stats = statManager.GetComponent<PlayerStatistics>();
        }

        playerPos = player.transform;
        cam = Camera.main.transform;

        healthSlider.maxValue = stats.maxHP;
        healthSlider.value = stats.currentHP;
    }

    private void Update()
    {
        if (stats == null) return;

        // ustaw pozycjê nad graczem
        transform.position = playerPos.position;

        // p³ynna animacja
        healthSlider.value = Mathf.Lerp(
            healthSlider.value,
            stats.currentHP,
            Time.deltaTime * smoothSpeed
        );

        // pokazuj tylko jeœli hp < max
        if (hideBar)
        {
            if (stats.currentHP < stats.maxHP)
            {
                if (!healthSlider.gameObject.activeSelf)
                    healthSlider.gameObject.SetActive(true);
            }
            else
            {
                if (healthSlider.gameObject.activeSelf)
                    healthSlider.gameObject.SetActive(false);
            }
        }
        else if (!healthSlider.gameObject.activeSelf)
            healthSlider.gameObject.SetActive(true);

        // billboard – zawsze twarz¹ do kamery
        //transform.rotation = Quaternion.LookRotation(transform.position - cam.position);
    }
}
