public class HPSystem
{
    public int hp;
    public bool is_defeated;

    public HPSystem(int hp)
    {
        this.hp = hp;
        this.is_defeated = false;
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
