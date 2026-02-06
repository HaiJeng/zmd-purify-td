namespace GameCore.Config;

using System.Text.Json;

public class TowerConfig
{
    public required int id { get; set; }
    public required string name { get; set; }
    public required float cost { get; set; }
    public required float cleanPower { get; set; }
    public required int range { get; set; }
    public required float maxDurability { get; set; }
}
public class ConfigLoader
{
    public List<TowerConfig> LoadTowers(string jsonPath)
    {
        Assert.IsTrue(File.Exists(jsonPath), new FileNotFoundException($"配置文件未找到: {jsonPath}"));

        string json = File.ReadAllText(jsonPath);
        Assert.IsNotBlank(json, new InvalidOperationException("配置文件为空"));

        var data = JsonSerializer.Deserialize<Dictionary<string, List<TowerConfig>>>(json)!;
        Assert.IsNotNull(data, new InvalidOperationException("配置文件解析失败"));
        Assert.IsTrue(data.ContainsKey("towers"), new InvalidOperationException("配置文件中缺少 'towers' 数据"));

        var towers = data["towers"]!;
        Assert.IsNotNull(towers, new InvalidOperationException("'towers' 数据为 null"));

        // 检查是否有任何塔配置缺失必要字段
        foreach (var tower in towers)
        {
            Assert.IsNotBlank(tower.name, new InvalidOperationException($"塔配置中存在空的 name 字段: ID {tower.id}"));
        }

        return towers;
    }
}