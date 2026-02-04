// 净化塔
namespace GameCore.Tower;
public class Tower {
    public int gridX, gridY;    //部署位置
    public float cleanPower;      // 净化力，每回合净化周围格子的污染值
    public int range;             // 范围，净化力作用的格子半径
    public float durability;      // 耐久，被污染会下降
    public float maxDurability; // 最大耐久
    
    public Tower(int x, int y, float cleanPower, int range) {
        gridX = x; gridY = y;
        this.cleanPower = cleanPower;
        this.range = range;
        maxDurability = 100f;
        durability = maxDurability;
    }
}