using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int MaxHealth = 100;
    public bool isEnnemy = true;
    private int _health;

    public GameObject ExplosionPrefab;
    public GameObject ShieldPrefab;

    private void Start()
    {
        if (isEnnemy)
        {
            MaxHealth = 80 + Random.Range(0, 40);
        }
        _health = MaxHealth;
    }

    public void Heal(int health)
    {
        _health = Mathf.Min(_health + health, MaxHealth);

        if (!isEnnemy)
        {
            MasterScript.Master.LifeActualize(_health);
        }
    }


    public void TakeDamage(int damage)
    {
        _health -= damage;
        if (isEnnemy)
        {
            GameObject shield = Instantiate(ShieldPrefab, transform.position + 0.6f*Vector3.up, Quaternion.identity);
            Destroy(shield, 0.2f);
        } else
        {
            MasterScript.Master.LifeActualize(_health);
        }
        if (_health <= 0 && isEnnemy)
        {
            MasterScript.Master.EnnemyDied();
            GameObject explo = Instantiate(ExplosionPrefab, transform.position + 0.6f * Vector3.up, Quaternion.identity);
            Destroy(explo, 0.1f);
            Destroy(gameObject);

        } else if (_health <= 0 && !isEnnemy)
        {
            MasterScript.Master.GameOver();
        }
    }
}
