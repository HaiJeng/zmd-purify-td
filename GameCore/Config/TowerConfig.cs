namespace GameCore.Config;

using System.Text.Json;
using System.Text.Json.Serialization;

public class TowerConfig
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("cost")]
    public required float Cost { get; set; }
    
    [JsonPropertyName("cleanPower")]
    public required float CleanPower { get; set; }
    
    [JsonPropertyName("range")]
    public required int Range { get; set; }
    
    [JsonPropertyName("maxDurability")]
    public required float MaxDurability { get; set; }
    
    [JsonPropertyName("corrosionResistance")]
    public float CorrosionResistance { get; set; } = 1.0f;
    
    [JsonPropertyName("decayFactor")]
    public float DecayFactor { get; set; } = 0.3f;
}
public class ConfigLoader
{
    public static List<TowerConfig> LoadTowers(string jsonPath)
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
            Assert.IsNotBlank(tower.Name, new InvalidOperationException($"塔配置中存在空的 name 字段: ID {tower.Id}"));
        }

        return towers;
    }
}