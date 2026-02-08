namespace GameCore.Core;

public class ResourceManager
{
    private float resources = 50f;
    private readonly float BaseIncomeRate = 1f;  // 降低到每秒+1，增加游戏时长
    public ResourceManager()
    {
        resources = 50f;
    }
    public ResourceManager(float initResources)
    {
        resources = initResources;
    }
    public void Update(float deltaTime)
    {
        resources += BaseIncomeRate * deltaTime;
    }

    public bool CanAfford(float cost)
    {
        return resources >= cost;
    }

    public bool SpendResources(float cost)
    {
        if (CanAfford(cost))
        {
            resources -= cost;
            return true;
        }
        return false;
    }

    public float CurrentResources => resources;
}