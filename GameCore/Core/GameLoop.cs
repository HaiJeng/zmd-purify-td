namespace GameCore.Core;

using GameCore.Config;
using GameCore.Grid;
using GameCore.Tower;
using global::Grid.GridFactory;

public enum GameResult
{
    NotFinished,  // 游戏未结束
    Win,          // 胜利
    Lose          // 失败
}

public static class GameResultExtensions
{
    public static string GetDisplayText(this GameResult result)
    {
        return result switch
        {
            GameResult.NotFinished => "游戏进行中",
            GameResult.Win => $"🎉 胜利！",
            GameResult.Lose => $"💀 失败！",
            _ => "未知状态"
        };
    }
}

public class GameLoop
{
    private readonly GridCell[,] grid;
    private readonly List<Tower> towers = [];
    private readonly ResourceManager resource = new();
    private readonly PurificationSystem purification;
    private readonly BacklashSystem backlash = new();
    private float time = 0f;
    // 获取当前时间
    public float CurrentTime => time;
    // 获取当前资源
    public float CurrentResources => resource.CurrentResources;
    // 获取净化进度
    public float PurificationProgress => purification.CalculatePurificationProgress();
    // 判断是否反扑
    public bool IsBacklashActive => backlash.IsBacklashActive;

    public List<Tower> Towers => towers;

    public GameLoop(int width, int height, float initResources)
    {
        grid = GridFactory.CreateDefaultGrid(width, height);
        purification = new PurificationSystem(grid, towers);
        resource = new ResourceManager(initResources);
    }

    public void Tick(float deltaTime)
    {
        time += deltaTime;

        // 1. 资源增长（反扑期间减半）
        if (backlash.IsBacklashActive)
        {
            resource.Update(deltaTime * 0.5f); // 反扑期间资源增长减半
        }
        else
        {
            resource.Update(deltaTime); // 正常资源增长
        }

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
            IsGameOver = true;
            GameResult = GameResult.Win;
            Console.WriteLine($"{GameResult.GetDisplayText()} 用时 {time} 秒");
            return;
        }

        // 失败条件：所有塔损坏且资源不足重建
        bool allTowersBroken = towers.All(t => t.durability <= 0);
        bool cannotRebuild = resource.CurrentResources < 10; // 基础塔造价

        if (allTowersBroken && cannotRebuild)
        {
            IsGameOver = true;
            GameResult = GameResult.Lose;
            Console.WriteLine($"{GameResult.GetDisplayText()} 净化能力丧失，用时 {time} 秒");
        }
    }

    public bool PlaceTower(int x, int y, TowerConfig config, out string errorMessage)
    {
        errorMessage = "";

        if (!resource.SpendResources(config.Cost))
        {
            errorMessage = $"资源不足：需要{config.Cost}，当前{resource.CurrentResources}";
            return false;
        }

        if (grid[x, y].Pollution != 0)
        {
            errorMessage = $"位置不合法：({x},{y})污染值为{grid[x, y].Pollution}，必须为0";
            return false;
        }

        var tower = new Tower(x, y, config);
        towers.Add(tower);
        Console.WriteLine($"🏗️ 在 ({x},{y}) 建造{config.Name}，剩余资源: {resource.CurrentResources}");
        return true;
    }
    public bool IsGameOver { get; private set; }
    public GameResult GameResult { get; private set; } = GameResult.NotFinished;

    // 获取网格数据用于显示
    public GridCell[,] GetGrid() => grid;
    public List<Tower> GetTowers() => towers;
}