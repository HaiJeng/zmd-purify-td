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
        for (int dx = -tower.range; dx <= tower.range; dx++)
        {
            for (int dy = -tower.range; dy <= tower.range; dy++)
            {
                int nx = tower.gridX + dx;
                int ny = tower.gridY + dy;
                if (IsInBounds(nx, ny) && grid[nx, ny].Pollution > 0)
                {
                    grid[nx, ny].Pollution -= tower.cleanPower * dt;
                    if (grid[nx, ny].Pollution < 0)
                        grid[nx, ny].Pollution = 0;
                }
            }
        }
    }

    // 塔在污染区被腐蚀
    void ApplyCorrosion(Tower tower, float dt)
    {
        var cell = grid[tower.gridX, tower.gridY];
        if (cell.Pollution > 0)
        {
            float corrosionRate = cell.Pollution / 100f * 0.5f; // 污染越高腐蚀越快
            tower.durability -= corrosionRate * dt;
        }
    }

    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }
}