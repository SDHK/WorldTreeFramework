/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	public static partial class InputMapperTestRule
	{
		class InputTest1 : InputTestEventRule<InputMapperTest>
		{
			protected override void Execute(InputMapperTest self, InputData data)
			{
				self.Log($"InputTest:{data.Info.InputCode} !!!!!!!!!!!");
			}
		}

		/// <summary>
		/// 序列化流程
		/// </summary>
		[NodeRule(nameof(AwakeRule<InputMapperTest>))]
		private static void OnAwake(this InputMapperTest self)
		{
			//新建一个输入映射管理器
			self.World.AddComponent(out InputMapperManager manager);
			//添加一个输入映射组
			manager.AddGeneric(0L, out InputMapperGroup group);
			//添加一个输入映射器
			group.AddChild(out InputMapper mapper);
			//添加一个输入信息配置
			mapper.ConfigInfoList = new() {
						new InputInfo() {
							InputDeviceType = InputDeviceType.Keyboard,
							InputDeviceId = 0,
							InputType = InputType.Press,
							InputCode = (byte)InputBoardKey.A
						},
						new InputInfo() {
							InputDeviceType = InputDeviceType.Keyboard,
							InputDeviceId = 0,
							InputType = InputType.Press,
							InputCode = (byte)InputBoardKey.B
						}
					};
			mapper.IsChange = true;

			//全局事件保存
			//mapper.InputEvent = self.Core.PoolGetUnit(out GlobalRuleExecutorData data);
			//data.RuleTypeCode = self.TypeToCode<InputTestEvent>();

			//获取一个数据库代理，保存到数据库
			self.World.AddComponent(out LiteDBTestProxy _).Insert(manager);
		}

		/// <summary>
		/// 反序列化流程
		/// </summary>
		[NodeRule(nameof(AwakeRule<InputMapperTest>))]
		private static void OnAwake2(this InputMapperTest self)
		{

			//添加一个数据库代理
			self.World.AddComponent(out LiteDBTestProxy liteDB);

			//反序列化
			var node = liteDB.Find<InputMapperManager>(1827001676595200);

			//将管理器 嫁接到 世界节点组件分支 上
			node.TryGraftSelfToTree<ComponentBranch, long>(self.Type, self.World);



			//数据获取测试：
			if (node.TryGetGeneric(0L, out InputMapperGroup group))
			{
				if (group.TryGetChild(1826293313175552, out InputMapper mapper))
				{
					self.Log($"mapper:{mapper.InfoList == null}");
				}
			}
		}

		class Update : UpdateRule<InputMapperTest>
		{
			protected override void Execute(InputMapperTest self)
			{

			}
		}

	}
}