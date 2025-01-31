using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            DIE();
        }
    }
    public void Damage(int damage)
    {
        health -= damage;
    }
    void DIE()
    {
        Destroy(gameObject);
    }

}
