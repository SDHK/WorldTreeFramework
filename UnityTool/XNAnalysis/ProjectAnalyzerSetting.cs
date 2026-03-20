/****************************************

* 作者：闪电黑客
* 日期：2024/6/18 16:42

* 描述：

*/

using System.Collections.Generic;

namespace WorldTree.Analyzer
{
    /// <summary>
    /// 分析器设置
    /// </summary>
    public static class ProjectConfigHelper
    {
        public static List<DiagnosticConfigGroup> Configs = new()
        {
            new ListDiagnosticConfig(),
            new ArrayDiagnosticConfig(),
            new DictionaryDiagnosticConfig(),
            new HashSetDiagnosticConfig(),
            new QueueDiagnosticConfig(),
            new StackDiagnosticConfig(),

            new ObjectDiagnosticConfig()
        };
    }

    /// <summary>
    /// Unity环境分析器配置
    /// </summary>
    public class ProjectAnalyzerSetting : ProjectDiagnosticConfig
    {
        public ProjectAnalyzerSetting()
        {
            Authors = new HashSet<string> { "Author:WJS" };
            Add("Framework", ProjectConfigHelper.Configs);
            Add("Games", ProjectConfigHelper.Configs);
            Add("Assembly-CSharp", ProjectConfigHelper.Configs);
            Add("Assembly-CSharp-Editor", ProjectConfigHelper.Configs);
            Add("UnityMCPTool.Editor", ProjectConfigHelper.Configs);
        }
    }
}