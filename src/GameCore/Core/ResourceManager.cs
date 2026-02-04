namespace GameCore.Core;
public class ResourceManager {
    private float resources = 50f;
    private float baseIncomeRate = 2f;  // 每秒+2
    
    public void Update(float deltaTime) {
        resources += baseIncomeRate * deltaTime;
    }
    
    public bool CanAfford(float cost) {
        return resources >= cost;
    }
    
    public bool SpendResources(float cost) {
        if (CanAfford(cost)) {
            resources -= cost;
            return true;
        }
        return false;
    }
}