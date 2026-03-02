using UnityEngine;

public class HPSystem : MonoBehaviour
{
    public int hp;
    public bool is_defeated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.hp = 10;
        this.is_defeated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DirectHit(int damage)
    {
        this.hp -= damage;

        if (this.hp <= 0)
        {
            this.hp = 0;
            this.is_defeated = true;
        }

    }

    public void Heal(int healing)
    {
        this.hp += healing;
    }

}
