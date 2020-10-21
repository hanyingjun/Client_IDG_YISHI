using IDG;

public class ZombieShow : PlayerShow{
}


public class ZombieData : PlayerData
{
    public override void Start()
    {
        base.Start();
        maxHealth = 300.ToFixed();
        _m_Hp = 300.ToFixed();
        this.tag = "Zombie";
        if(ai!=null){
       
        }
        else
        {
            ai=new AiEngine();
            ai.aiName="AI_test";
            ai=AddCommponent<AiEngine>(ai);
          
        }
        team = 2;
        move_speed =0.7f.ToFixed();
        weaponSystem.AddWeapon(WeaponId.丧尸);
    }

    protected override void Die()
    {
        base.Die();
        client.objectManager.Destory(view);
    }

    public override string PrefabPath()
    {
        return "Prefabs/Zombie";
    }
}

