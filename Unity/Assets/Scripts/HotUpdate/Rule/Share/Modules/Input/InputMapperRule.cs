/****************************************

* 作者：闪电黑客
* 日期：2024/12/31 15:47

* 描述：

*/
namespace WorldTree
{
	public static class InputMapperRule
	{
		class InputGlobal : InputGlobalRule<InputMapper>
		{
			protected override void Execute(InputMapper self, InputData data)
			{
				if (self.InfoList.Count != self.config.InfoList.Count - 1)
				{
					if (data.Value.InputState == InputState.Start)
					{
						if (data.Info.InputType == InputType.Press)
						{
							if (self.config.InfoList[self.InfoList.Count] == data.Info)
							{
								self.InfoList.Add(data.Info);
							}
							else
							{
								self.InfoList.Clear();
							}
						}
					}
				}
				else if (self.config.InfoList[self.InfoList.Count] == data.Info)
				{
					//消息广播
					self.InputEvent.Send(data);
				}
			}
		}
	}
}