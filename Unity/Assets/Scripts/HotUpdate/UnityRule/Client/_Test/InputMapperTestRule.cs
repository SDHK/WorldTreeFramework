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
		//[NodeRule(nameof(AwakeRule<InputMapperTest>))]
		private static void OnAwake(this InputMapperTest self)
		{
			//新建一个输入映射管理器
			self.World.AddComponent(out InputManager manager);
			//添加一个输入存档
			manager.AddGeneric(0L, out InputArchive archive);
			//添加一个输入层
			archive.AddGeneric(0L, out InputLayer layer);
			//添加一个输入组
			layer.AddGeneric(0L, out InputGroup group);
			//添加一个输入绑定
			group.AddGeneric(0L, out InputBind mapper);
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
			mapper.InputEvent = self.Core.PoolGetUnit(out GlobalRuleExecutorData data);
			data.RuleTypeCode = self.TypeToCode<InputTestEvent>();


			//获取一个数据库代理，保存到数据库
			self.World.AddComponent(out LiteDBTestProxy _).Update(archive.GetKey<long>(), archive);
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
			InputArchive archive = liteDB.Find<InputArchive>(0L);

			//将管理器 嫁接到 世界节点组件分支 上
			self.World.AddComponent(out InputManager manager);

			archive.TryGraftSelfToTree(self.TypeToCode<GenericBranch<long>>(), 0L, manager);

			//数据获取测试：
			if (archive.TryGetGeneric(0L, out InputLayer layer))
			{
				if (layer.TryGetGeneric(0L, out InputGroup group))
				{
					if (group.TryGetGeneric(0L, out InputBind mapper))
					{
						self.Log($"mapper!!!!:{mapper.InfoList == null}");
					}
				}
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
