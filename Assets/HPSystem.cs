using UnityEngine;

public class HPSystem : MonoBehaviour
{
    public int hp;
    public bool isdefeated;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.hp = 10;
        this.isdefeated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DirectHit(int damage)
    {
        this.hp -= damage;

        if(this.hp <= 0)
        {
            this.hp = 0;

            if(this.isdefeated == false)
            {
                this.isdefeated = true;
                UnityEngine.Debug.Log(this.name + " has been defeated");
            }
            
        }

    }

}
