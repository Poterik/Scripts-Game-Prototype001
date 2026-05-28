using UnityEngine;

public class HealCapsule : MonoBehaviour
{
    private int healPoint = 25;
    private float lifetime = 120f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerFighter pf = other.GetComponent<PlayerFighter>();
            int heal = healPoint + GameManager.Instance.recovery;
            pf.UpdateHealth(heal);
            Destroy(this.gameObject);
        }
    }
}
