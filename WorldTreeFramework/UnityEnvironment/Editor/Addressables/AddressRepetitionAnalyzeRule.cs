
/****************************************

* 作者： 闪电黑客
* 日期： 2022/12/19 16:57

* 描述： 地址重复分析器
* 
* 给AdderssableAnalyze窗口添加分析重复地址的规则

*/

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;

/// <summary>
/// 地址重复分析器
/// </summary>
public class AddressRepetitionAnalyzeRule : AnalyzeRule
{
    /// <summary>
    /// 饿汉单例
    /// </summary>
    [InitializeOnLoadMethod]
    private static void SingletonEager()
    {
        AnalyzeSystem.RegisterNewRule<AddressRepetitionAnalyzeRule>();
    }

    public override string ruleName => "地址重复分析器";

    /// <summary>
    /// 刷新进行分析 
    /// </summary>
    /// <returns>返回结果列表</returns>
    public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
    {
        Dictionary<string, List<AddressableAssetEntry>> adderssCount = new Dictionary<string, List<AddressableAssetEntry>>();

        foreach (var group in settings.groups)
        {
            foreach (var entrie in group.entries)
            {
                if (!adderssCount.TryGetValue(entrie.address, out List<AddressableAssetEntry> entries))
                {
                    entries = new List<AddressableAssetEntry>();
                    adderssCount.Add(entrie.address, entries);
                }
                entries.Add(entrie);
            }
        }

        List<AnalyzeResult> Analyzes = new List<AnalyzeResult>();
        foreach (var adderssItem in adderssCount)
        {
            if (adderssItem.Value.Count != 1)
            {
                foreach (var entry in adderssItem.Value)
                {
                    Analyzes.Add(new AnalyzeResult() { resultName = $"{adderssItem.Key}:[{entry.parentGroup.Name}]:{entry.AssetPath}", severity = MessageType.Warning });
                }
            }
        }
        if (Analyzes.Count == 0)
        {
            Analyzes.Add(new AnalyzeResult() { resultName = "未发现重复的地址", severity = MessageType.Info });
        }
        return Analyzes;
    }
}

