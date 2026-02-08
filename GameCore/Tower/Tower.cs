// 净化塔
namespace GameCore.Tower;

using GameCore.Config;

public class Tower
{
    public int gridX, gridY;    // 部署位置
    public TowerConfig config;  // 塔配置引用
    
    public float durability;      // 当前耐久
    public float maxDurability;   // 最大耐久
    
    public Tower(int x, int y, TowerConfig config)
    {
        gridX = x;
        gridY = y;
        this.config = config;
        maxDurability = config.MaxDurability;
        durability = maxDurability;
    }
    
    // 便捷属性访问
    public float CleanPower => config.CleanPower;
    public int Range => config.Range;
    public float CorrosionResistance => config.CorrosionResistance;
    public float DecayFactor => config.DecayFactor;
}