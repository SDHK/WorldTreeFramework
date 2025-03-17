/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
	public static class InputMapperRule
	{
		class Add : AddRule<InputMapper>
		{
			protected override void Execute(InputMapper self)
			{
				self.Core.PoolGetUnit(out self.InfoList);
			}
		}

		class Deserialize : DeserializeRule<InputMapper>
		{
			protected override void Execute(InputMapper self)
			{
				self.Core.PoolGetUnit(out self.InfoList);
				self.Core.GetGlobalRuleExecutor(self.Config.InputRuleType, out self.InputEvent);
			}
		}

		class Remove : RemoveRule<InputMapper>
		{
			protected override void Execute(InputMapper self)
			{
				self.InfoList.Dispose();
				self.InfoList = null;
			}
		}


		class InputGlobal : InputGlobalRule<InputMapper>
		{
			protected override void Execute(InputMapper self, InputData data)
			{
				if (self.InfoList == null) return;

				//输入开始状态
				if (data.Value.InputState.HasFlag(InputState.Start))
				{
					//判断前置按钮是否是区间型，并加入队列，只有一个的情况则直接判断，不加入队列
					if (self.InfoList.Count != self.Config.InfoList.Count - 1)
					{
						//前置按钮不是区间型则退出，不影响后续按钮
						if (data.Info.InputType != InputType.Press) return;

						//如果当前等于相等配置的按钮，则添加到队列，否则清空队列
						if (self.Config.InfoList[self.InfoList.Count] == data.Info)
						{
							self.InfoList.Add(data.Info);
						}
						else
						{
							self.InfoList.Clear();
						}
					}
					//判断最后一个是否等于配置
					else if (self.Config.InfoList[self.InfoList.Count] == data.Info)
					{
						//消息广播
						self.InputEvent.Send(data);
					}
					//不等于则清空队列
					else
					{
						self.InfoList.Clear();
					}
				}
				//输入结束状态
				else if (data.Value.InputState.HasFlag(InputState.End))
				{
					//如果只有一个按输入则退出
					if (self.Config.InfoList.Count == 1) return;

					//如果是区间型，判断是否是前置按钮，是则移除
					if (data.Info.InputType == InputType.Press)
					{
						bool isEnd = false;
						for (int i = 0; i < self.Config.InfoList.Count - 1; i++)
						{
							if (self.Config.InfoList[i] != data.Info) continue;
							isEnd = true;
							break;
						}
						if (isEnd) self.InfoList.Remove(data.Info);
					}
				}
			}
		}
	}
}