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
        
    }
    public void Damage(int damage)
    {
        health -= damage;
    }
}
