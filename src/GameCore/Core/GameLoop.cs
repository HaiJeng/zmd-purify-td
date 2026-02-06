namespace GameCore.Core;

using GameCore.Config;
using GameCore.Grid;
using GameCore.Tower;
using global::Grid.GridFactory;

public class GameLoop
{
    private readonly GridCell[,] grid;
    private readonly List<Tower> towers = [];
    private readonly ResourceManager resource = new();
    private readonly PurificationSystem purification;
    private readonly BacklashSystem backlash = new();
    private float time = 0f;

    public GameLoop(int width, int height)
    {
        grid = GridFactory.CreateDefaultGrid(width, height);
        purification = new PurificationSystem(grid, towers);
    }

    public void Tick(float deltaTime)
    {
        time += deltaTime;

        // 1. 资源增长
        resource.Update(deltaTime);

        // 2. 净化 + 腐蚀
        purification.ProcessPurification(deltaTime);

        // 3. 计算进度，触发反扑
        float progress = purification.CalculatePurificationProgress();
        backlash.CheckAndTriggerBacklash(progress);

        // 4. 反扑计时
        backlash.UpdateBacklash(deltaTime);

        // 5. 胜负判断
        CheckWinLose();
    }

    private void CheckWinLose()
    {
        // 胜利条件：所有格子 pollution = 0
        bool isWin = true;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y].Pollution > 0)
                {
                    isWin = false;
                    break;
                }
            }
            if (!isWin) break;
        }

        if (isWin)
        {
            Console.WriteLine($"🎉 胜利！用时 {time} 秒");
            return;
        }

        // 失败条件：所有塔损坏且资源不足重建
        bool allTowersBroken = towers.All(t => t.durability <= 0);
        bool cannotRebuild = resource.CurrentResources < 10; // 基础塔造价

        if (allTowersBroken && cannotRebuild)
        {
            Console.WriteLine($"💀 失败！净化能力丧失，用时 {time} 秒");
        }
    }

    public void PlaceTower(int x, int y, TowerConfig config)
    {
        if (resource.SpendResources(config.cost))
        {
            var tower = new Tower(x, y, config.cleanPower, config.range)
            {
                maxDurability = config.maxDurability,
                durability = config.maxDurability
            };
            towers.Add(tower);
            Console.WriteLine($"🏗️ 在 ({x},{y}) 建造塔，剩余资源: {resource.CurrentResources}");
        }
        else
        {
            Console.WriteLine("❌ 资源不足！");
        }
    }
}