namespace GameCore.Core;

using GameCore.Tower;
using GameCore.Grid;
public class PurificationSystem
{
    private GridCell[,] grid;
    private List<Tower> towers;

    public PurificationSystem(GridCell[,] grid, List<Tower> towers)
    {
        this.grid = grid;
        this.towers = towers;
    }
    // 计算净化进度
    public float CalculatePurificationProgress()
    {
        int totalCells = grid.GetLength(0) * grid.GetLength(1);
        float totalPollution = 0;

        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                totalPollution += grid[x, y].Pollution;
            }
        }

        float avgPollution = totalPollution / totalCells;
        return Math.Clamp(1f - avgPollution / 80f, 0f, 1f);
    }
    // 每回合执行净化
    public void ProcessPurification(float deltaTime)
    {
        foreach (var tower in towers)
        {
            PurifyAroundTower(tower, deltaTime);
            ApplyCorrosion(tower, deltaTime);
        }
    }

    // 塔净化周围格子
    void PurifyAroundTower(Tower tower, float dt)
    {
        for (int dx = -tower.Range; dx <= tower.Range; dx++)
        {
            for (int dy = -tower.Range; dy <= tower.Range; dy++)
            {
                int nx = tower.gridX + dx;
                int ny = tower.gridY + dy;
                if (IsInBounds(nx, ny) && grid[nx, ny].Pollution > 0)
                {
                    grid[nx, ny].Pollution -= tower.CleanPower * dt;
                    if (grid[nx, ny].Pollution < 0)
                        grid[nx, ny].Pollution = 0;
                }
            }
        }
    }
    private float CalculateEnvironmentalPollution(int x, int y, int range)
    {
        float totalPollution = 0;
        int neighborCount = 0;
        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                //跳过塔本身
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (IsInBounds(nx, ny))
                {
                    float distance = (float)Math.Sqrt(dx * dx + dy * dy);
                    float pollution = grid[nx, ny].Pollution;

                    // 距离衰减：越远影响越小
                    float distanceFactor = 1.0f / (1.0f + distance * 0.5f);
                    totalPollution += pollution * distanceFactor;
                    neighborCount++;
                }
            }
        }
        return neighborCount > 0 ? totalPollution / neighborCount : 0f;
    }

    // 塔在污染区被腐蚀
    void ApplyCorrosion(Tower tower, float dt)
    {
        //计算塔周围环境污染
        float environmentalPollution = CalculateEnvironmentalPollution(tower.gridX, tower.gridY, tower.Range);
        // 污染越严重，腐蚀越快 - 使用指数增长
        float pollutionFactor = 1.0f + (environmentalPollution / 100f) * 2.0f; // 污染因子
        float baseCorrosionRate = environmentalPollution / 100f * 3.0f * pollutionFactor; // 基础腐蚀率
        float finalCorrosionRate = baseCorrosionRate / tower.CorrosionResistance; // 计算抗腐蚀系数
        tower.durability -= finalCorrosionRate * dt;

        tower.durability = Math.Max(0, tower.durability);
    }

    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }
}