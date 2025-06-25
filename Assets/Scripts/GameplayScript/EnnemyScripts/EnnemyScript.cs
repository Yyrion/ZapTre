using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnnemyScript : MonoBehaviour
{
    private NavMeshAgent _agent;
    public Transform _playerTransform;

    [Header("Explosion")]
    private bool _isExploding;
    public GameObject ExplosionPrefab;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = Random.Range(1.5f, 2.5f);
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) < 2f && !_isExploding)
        {
            _isExploding = true;
        } 
        
        if (_isExploding)
        {
            _agent.isStopped = true;
            GameObject explosion = Instantiate(ExplosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 0.5f);
            Collider[] hits = Physics.OverlapSphere(transform.position, 2f);

            foreach (Collider hit in hits)
            {
                HealthManager health = hit.GetComponent<HealthManager>();
                if (health != null && hit.CompareTag("Player"))
                {
                    health.TakeDamage(35);
                }
            }

            MasterScript.Master.EnnemyDied();
            Destroy(gameObject);
            
        } else
        {
            _agent.SetDestination(_playerTransform.position);
        }
    }
}
