/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{

	/// <summary>
	/// 输入测试事件
	/// </summary>
	public interface InputTestEvent : InputEvent { }


	/// <summary>
	/// 测试
	/// </summary>
	public partial class InputMapperTest : Node
	, AsComponentBranch
	, AsChildBranch
	, ComponentOf<InitialDomain>
	, AsRule<Awake>
	, AsRule<InputTestEvent>
	{

		[NodeRule(nameof(InputTestEventRule<InputMapperTest>))]
		private static void OnInputTestEventRule(InputMapperTest self, InputData data)
		{
			self.Log($"InputTest:{data.Info.InputCode} !!!!!!!!!!!");
		}

		[NodeRule(nameof(UpdateRule<InputMapperTest>))]
		private static void OnUpdateRule(InputMapperTest self)
		{
		}

		/// <summary>
		/// 序列化流程
		/// </summary>
		[NodeRule(nameof(AwakeRule<InputMapperTest>))]
		private static void OnAwakeRule(InputMapperTest self)
		{
			//新建一个输入映射管理器
			self.World.AddComponent(out InputManager manager);
			//添加一个输入存档
			manager.AddGeneric("0", out InputArchive archive);
			//添加一个输入层
			archive.AddGeneric("0", out InputLayer layer);
			//添加一个输入组
			layer.AddGeneric("0", out InputGroup group);
			//添加一个输入绑定
			group.AddGeneric("0", out InputBind mapper);
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
			mapper.InputEvent = self.World.PoolGetUnit(out RuleBroadcastData data);
			data.RuleType = TypeInfo<InputTestEvent>.Code;


			//获取一个数据库代理，保存到数据库
			self.World.AddComponent(out DataBaseTestProxy _).Update(archive.GetKey<long>(), archive);
		}

		/// <summary>
		/// 反序列化流程
		/// </summary>
		//[NodeRule(nameof(AwakeRule<InputMapperTest>))]
		private static void OnAwake2(InputMapperTest self)
		{

			//添加一个数据库代理
			self.World.AddComponent(out DataBaseTestProxy liteDB);

			//反序列化
			InputArchive archive = liteDB.Find<InputArchive>(0L);

			//将管理器 嫁接到 世界节点组件分支 上
			self.World.AddComponent(out InputManager manager);

			archive.TryGraftSelfToTree<GenericBranch<string>, string>("0", manager);

			//数据获取测试：
			if (archive.TryGetGeneric("0", out InputLayer layer))
			{
				if (layer.TryGetGeneric("0", out InputGroup group))
				{
					if (group.TryGetGeneric("0", out InputBind mapper))
					{
						self.Log($"mapper!!!!:{mapper.InfoList == null}");
					}
				}
			}
		}



	}
}