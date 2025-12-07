using TMPro;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{

    [SerializeField]
    int maxHealth = 100;

    int currentHealth;

    [SerializeField]
    TextMeshProUGUI enemyHealthText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        UpdateHealth();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateHealth()
    {
        if (enemyHealthText != null)
        {
            enemyHealthText.text = currentHealth.ToString();
        }
    }

    public void ApplyDamage(int damage)
    {
        currentHealth -= damage;

        UpdateHealth();

        if (currentHealth <= 0)
        {
            Destroy(enemyHealthText);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            ApplyDamage(bullet.GetDamage());

            Destroy(bullet.gameObject);
        }
    }
}
