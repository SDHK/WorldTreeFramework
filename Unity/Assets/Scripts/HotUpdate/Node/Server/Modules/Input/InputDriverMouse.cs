/****************************************

* ���ߣ�����ڿ�
* ���ڣ�2025/1/2 17:04

* ������

*/
using System;
using System.Runtime.InteropServices;

namespace WorldTree.Server
{
	/// <summary>
	/// Windows�����
	/// </summary>
	public enum WindowMouseKey : uint
	{
		/// <summary>
		/// �����������¼���
		/// </summary>
		LeftButtonDown = 0x0201,
		/// <summary>
		/// ������̧���¼���
		/// </summary>
		LeftButtonUp = 0x0202,
		/// <summary>
		/// ����Ҽ������¼���
		/// </summary>
		RightButtonDown = 0x0204,
		/// <summary>
		/// ����Ҽ�̧���¼���
		/// </summary>
		RightButtonUp = 0x0205,
		/// <summary>
		/// ����м������¼���
		/// </summary>
		MiddleButtonDown = 0x0207,
		/// <summary>
		/// ����м�̧���¼���
		/// </summary>
		MiddleButtonUp = 0x0208
	}



	/// <summary>
	/// �������������
	/// </summary>
	public class InputDriverMouse : InputDriver
		, AsRule<Awake<InputDeviceManager>>
	{

		/// <summary>
		/// ����ͼ���깳�ӵĳ���
		/// </summary>
		public int WH_MOUSE_LL = 14;


		/// <summary>
		/// ���������Ϣ��ί��
		/// </summary>
		public LowLevelMouseProc mouseProc;

		/// <summary>
		/// �洢���ӵľ��
		/// </summary>
		public IntPtr mouseHookID = IntPtr.Zero;

	}

	/// <summary>
	/// ���������Ϣ��ί��
	/// </summary>
	/// <param name="nCode"></param>
	/// <param name="wParam"></param>
	/// <param name="lParam"></param>
	/// <returns></returns>
	public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);


	/// <summary>
	/// ���� POINT �ṹ��
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct POINT
	{
		/// <summary>
		/// X ����
		/// </summary>
		public int X;
		/// <summary>
		/// Y ����
		/// </summary>
		public int Y;
	}
}