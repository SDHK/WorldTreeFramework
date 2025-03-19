/****************************************

* 作者：闪电黑客
* 日期：2024/5/14 17:47

* 描述：

*/
namespace WorldTree
{
	public static class InputMapperTestRule
	{
		class InputTest1 : InputTestEventRule<InputMapperTest>
		{
			protected override void Execute(InputMapperTest self, InputData data)
			{
				self.Log($"InputTest:{data.Info.InputCode} !!!!!!!!!!!");
			}
		}

		class Awake : AwakeRule<InputMapperTest>
		{
			protected override void Execute(InputMapperTest self)
			{
				if (self != null) return;
				self.Root.AddComponent(out InputMapperManager manager);
				manager.AddNumberNode(0, out InputMapperGroup group);
				group.AddChild(out InputMapper mapper);
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

				mapper.InputEvent = self.Core.PoolGetUnit(out GlobalRuleExecutorData data);
				data.RuleTypeCode = self.TypeToCode<InputTestEvent>();

				self.Root.AddComponent(out LiteDBTestProxy liteDB);
				liteDB.Insert(manager);
			}
		}



		class Awake1 : AwakeRule<InputMapperTest>
		{
			protected override void Execute(InputMapperTest self)
			{
				//if (self != null) return;

				self.Root.AddComponent(out LiteDBTestProxy liteDB);
				var node = liteDB.Find<InputMapperManager>(1827001676595200);
				node.CutSelf()?.TryGraftSelfToTree<ComponentBranch, long>(self.Type, self.Root);
				if (node.TryGetNumberNode(0, out InputMapperGroup group))
				{
					if (group.TryGetChild(1826293313175552, out InputMapper mapper))
					{
						self.Log($"mapper:{mapper.InfoList == null}");
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
}