// Kcp based on https://github.com/skywind3000/kcp
// Kept as close to original as possible.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ET
{
	/// <summary>
	/// KCP
	/// </summary>
	public partial class Kcp
	{
		// original Kcp has a define option, which is not defined by default:
		// #define FASTACK_CONSERVE

		/// <summary>
		/// 表示在无延迟模式下的最小重传超时时间 30ms
		/// </summary>
		public const int RTO_NDL = 30;             // no delay min rto
		/// <summary>
		/// 重传超时最小时间 100ms
		/// </summary>
		public const int RTO_MIN = 100;            // normal min rto
		/// <summary>
		/// 重传超时时间 200ms
		/// </summary>
		public const int RTO_DEF = 200;            // default RTO
		/// <summary>
		/// 重传超时时间最大值 60000ms
		/// </summary>
		public const int RTO_MAX = 60000;          // maximum RTO
		/// <summary>
		/// 命令：推送数据 ：表示这是一个数据包，接收方需要将其数据内容推送到接收缓冲区中进行处理。
		/// </summary>
		public const int CMD_PUSH = 81;            // cmd: push data
		/// <summary>
		/// 命令：确认数据包的接收。
		/// </summary>
		public const int CMD_ACK = 82;            // cmd: ack
		/// <summary>
		/// 命令：窗口探测（询问窗口大小）。
		/// </summary>
		public const int CMD_WASK = 83;            // cmd: window probe (ask)
		/// <summary>
		///	命令：告知窗口大小（告知/插入）
		/// </summary>
		public const int CMD_WINS = 84;            // cmd: window size (tell/insert)
		/// <summary>
		/// 表示需要发送 CMD_WASK 命令。
		/// </summary>
		public const int ASK_SEND = 1;             // need to send CMD_WASK
		/// <summary>
		/// 表示需要发送 CMD_WINS 命令
		/// </summary>
		public const int ASK_TELL = 2;             // need to send CMD_WINS
		/// <summary>
		/// 默认的发送窗口大小 32个数据包
		/// </summary>
		public const int WND_SND = 32;             // default send window
		/// <summary>
		/// 默认的接收窗口大小。128个数据包
		/// </summary>
		public const int WND_RCV = 128;            // 默认接收窗口。必须 >= 最大片段大小
		/// <summary>
		/// 默认最大传输单元1200字节
		/// </summary>
		public const int MTU_DEF = 1200;           // default MTU (reduced to 1200 to fit all cases: https://en.wikipedia.org/wiki/Maximum_transmission_unit ; steam uses 1200 too!)
		/// <summary>
		/// 快速确认的阈值 ：当发送方收到 3 个重复的确认包时，会触发快速重传机制，立即重传可能丢失的数据包。
		/// </summary>
		public const int ACK_FAST = 3;
		/// <summary>
		/// 更新间隔时间 100ms
		/// </summary>
		public const int INTERVAL = 100;
		/// <summary>
		/// 协议头部，24字节
		/// </summary>
		public const int OVERHEAD = 24;
		/// <summary>
		/// 最大分片数量。
		/// </summary>
		public const int FRG_MAX = byte.MaxValue;  // kcp encodes 'frg' as byte. so we can only ever send up to 255 fragments.
		/// <summary>
		/// 最大重传次数，如果一个数据包在指定的重传次数内仍然没有被确认，连接就会被认为已经断开。
		/// </summary>
		public const int DEADLINK = 20;            // default maximum amount of 'xmit' retransmissions until a segment is considered lost
		/// <summary>
		/// 初始慢启动阈值（Slow Start Threshold）在网络传输协议中，慢启动阈值用于控制拥塞窗口的增长速度，以避免网络拥塞。
		/// </summary>
		public const int THRESH_INIT = 2;
		/// <summary>
		/// 最小阈值 ：意味着即使在网络拥塞的情况下，发送方的拥塞窗口也不会小于 2。
		/// </summary>
		public const int THRESH_MIN = 2;
		/// <summary>
		/// 探测窗口大小的初始间隔时间 7000ms
		/// </summary>
		public const int PROBE_INIT = 7000;        // 7 secs to probe window size
		/// <summary>
		/// 探测窗口大小的探测间隔时间，上限为 120000 毫秒
		/// </summary>
		public const int PROBE_LIMIT = 120000;     // up to 120 secs to probe window
		/// <summary>
		/// 快速重传的最大次数限制
		/// </summary>
		public const int FASTACK_LIMIT = 5;        // max times to trigger fastack

		/// <summary>
		/// 保留字节
		/// </summary>
		public const int RESERVED_BYTE = 5; // 包头预留字节数 供et网络层使用

		// KCP 成员。

		/// <summary>
		/// 表示 KCP 连接的当前状态。
		/// </summary>
		internal int state;
		/// <summary>
		/// 表示会话的编号。
		/// </summary>
		readonly uint conv;          // conversation 对话 
		/// <summary>
		/// 最大传输单元
		/// </summary>
		internal uint mtu;
		/// <summary>
		/// 最大分段的大小
		/// </summary>
		internal uint mss;           // maximum segment size := MTU - OVERHEAD
		/// <summary>
		/// 表示未被确认的最早的序列号。
		/// </summary>
		internal uint snd_una;       // unacknowledged. e.g. snd_una is 9 it means 8 has been confirmed, 9 and 10 have been sent
		/// <summary>
		/// 表示下一个将要发送的数据包的序列号。
		/// </summary>
		internal uint snd_nxt;       // forever growing send counter for sequence numbers
		/// <summary>
		/// 表示下一个期望接收的数据包的序列号。
		/// </summary>
		internal uint rcv_nxt;       // forever growing receive counter for sequence numbers
		/// <summary>
		/// 表示慢启动阈值 ：当拥塞窗口大小 小于等于 ssthresh 时，使用慢启动算法。
		/// </summary>
		internal uint ssthresh;      // slow start threshold
		/// <summary>
		/// 表示往返时间（RTT）的平均偏差，用于测量 RTT 的抖动。
		/// </summary>
		internal int rx_rttval;      // average deviation of rtt, used to measure the jitter of rtt
		/// <summary>
		/// 表示平滑的往返时间（SRTT），即 RTT 的加权平均值。
		/// </summary>
		internal int rx_srtt;        // smoothed round trip time (a weighted average of rtt)
		/// <summary>
		/// 表示重传超时（RTO）的时间。 rx_rto = rx_srtt + 4 * rx_rttval
		/// </summary>
		internal int rx_rto;
		/// <summary>
		/// 表示重传超时（RTO）的最小值。
		/// </summary>
		internal int rx_minrto;
		/// <summary>
		/// 表示发送窗口的大小。:发送窗口的大小决定了发送方可以在没有收到接收方确认的情况下，连续发送的最大数据包数量。
		/// </summary>
		internal uint snd_wnd;       // send window
		/// <summary>
		/// 表示接收窗口的大小。:接收窗口的大小决定了接收方可以在没有发送确认的情况下，连续接收的最大数据包数量。
		/// </summary>
		internal uint rcv_wnd;       // receive window
		/// <summary>
		/// 表示远程窗口的大小。
		/// </summary>
		internal uint rmt_wnd;       // remote window
		/// <summary>
		/// 表示拥塞窗口的大小。:拥塞窗口的大小决定了发送方在没有收到接收方确认的情况下，连续发送的最大数据包数量。
		/// </summary>
		internal uint cwnd;          // congestion window
		/// <summary>
		/// 探测状态:用于记录探测请求的状态
		/// </summary>
		internal uint probe;
		/// <summary>
		/// 更新间隔时间 ms
		/// </summary>
		internal uint interval;
		/// <summary>
		/// 表示上次刷新（flush）的时间戳，以毫秒为单位。
		/// </summary>
		internal uint ts_flush;      // last flush timestamp in milliseconds
		/// <summary>
		/// 用于记录某个数据包的传输次数
		/// </summary>
		internal uint xmit;
		/// <summary>
		/// 用于决定是否启用快速模式。
		/// </summary>
		internal uint nodelay;       // not a bool. original Kcp has '<2 else' check.
		/// <summary>
		/// 用于指示 KCP 协议是否已经初始化更新。以确保某些初始化操作只执行一次。
		/// </summary>
		internal bool updated;
		/// <summary>
		/// 用于记录上一次探测操作的时间
		/// </summary>
		internal uint ts_probe;      // probe timestamp
		/// <summary>
		/// 用于控制探测操作的间隔时间。
		/// </summary>
		internal uint probe_wait;
		/// <summary>
		/// 用于表示在一个数据段被认为丢失之前，允许的最大重传次数。
		/// </summary>
		internal uint dead_link;     // maximum amount of 'xmit' retransmissions until a segment is considered lost
		/// <summary>
		/// 用于表示拥塞窗口（congestion window）增加的字节数。
		/// </summary>
		internal uint incr;
		/// <summary>
		/// 当前时间
		/// </summary>
		internal uint current;       // current time (milliseconds). set by Update.

		/// <summary>
		/// 快速重传的次数。
		/// </summary>
		internal int fastresend;
		/// <summary>
		/// 快速重传的最大次数限制
		/// </summary>
		internal int fastlimit;
		/// <summary>
		/// 用于控制是否禁用拥塞控制。
		/// </summary>
		internal bool nocwnd;        // congestion control, negated. heavily restricts send/recv window sizes.
		/// <summary>
		/// 用于存储待发送的数据段队列。
		/// </summary>
		internal readonly Queue<SegmentStruct> snd_Queue = new Queue<SegmentStruct>(16); // send queue
																						 // receive queue
		/// <summary>
		/// 用于存储接收到的数据段队列。
		/// </summary>
		internal readonly Queue<SegmentStruct> rcv_Queue = new Queue<SegmentStruct>(16);
		// snd_buffer 需要索引移除操作。
		// C# 的 LinkedList 为每个条目分配内存，所以我们暂时使用 List。
		// 发送缓冲区
		/// <summary>
		/// 用于存储已发送但尚未收到确认的数据段列表。
		/// </summary>
		internal readonly List<SegmentStruct> snd_bufList = new List<SegmentStruct>(16);
		// rcv_buffer 需要索引插入和向后迭代。
		// C# 的 LinkedList 为每个条目分配内存，所以我们暂时使用 List。
		// 接收缓冲区
		/// <summary>
		/// 用于存储已接收但尚未处理的数据段列表。
		/// </summary>
		internal readonly List<SegmentStruct> rcv_bufList = new List<SegmentStruct>(16);
		/// <summary>
		/// 用于存储待处理的确认（ACK）项列表。这些确认项包含了接收到的数据段的序列号和时间戳，用于告知对端哪些数据段已经成功接收。
		/// </summary>
		internal readonly List<AckItem> ackList = new List<AckItem>(16);

		/// <summary>
		/// 对象池
		/// </summary>
		private ArrayPool<byte> kcpSegmentArrayPool;

		// 内存缓冲区
		// 大小取决于 MTU。
		// MTU 可以在运行时更改，这会调整缓冲区大小。
		/// <summary>
		/// 用于存储字节数组，通常用于临时缓冲区或数据处理。
		/// </summary>
		internal byte[] buffers;

		// output function of type <buffer, size>
		/// <summary>
		/// 用于处理发送数据的操作。
		/// </summary>
		readonly Action<byte[], int> output;

		// 段池以避免在 C# 中的分配。
		// 这不是原始 C 代码的一部分。
		// readonly Pool<Segment> SegmentPool = new Pool<Segment>(
		//     // 创建新的段
		//     () => new Segment(),
		//     // 在重用前重置段
		//     (segment) => segment.Reset(),
		//     // 初始容量
		//     32
		// );


		// ikcp_create
		// 创建一个新的 KCP 控制对象，'conv' 必须在同一连接的两个端点中相等。
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Kcp(uint conv, Action<byte[], int> output)
		{
			this.conv = conv; // 设置会话标识符（conv）。
			this.output = output; // 设置输出回调函数。
			snd_wnd = WND_SND; // 设置发送窗口大小为默认值。
			rcv_wnd = WND_RCV; // 设置接收窗口大小为默认值。
			rmt_wnd = WND_RCV; // 设置远程窗口大小为默认值。
			mtu = MTU_DEF; // 设置最大传输单元（MTU）为默认值。
			mss = mtu - OVERHEAD; // 设置最大分段大小（MSS），减去协议开销。
			rx_rto = RTO_DEF; // 设置默认的重传超时时间（RTO）。
			rx_minrto = RTO_MIN; // 设置最小的重传超时时间（RTO）。
			interval = INTERVAL; // 设置更新间隔时间。
			ts_flush = INTERVAL; // 设置刷新时间戳为更新间隔时间。
			ssthresh = THRESH_INIT; // 设置慢启动阈值为初始值。
			fastlimit = FASTACK_LIMIT; // 设置快速确认的限制次数。
			dead_link = DEADLINK; // 设置死链路检测的阈值。


			//乘以 3 的原因是为了确保在处理数据包时有足够的缓冲区空间。
			//这个缓冲区需要同时容纳以下几种数据：发送的数据包 接收的数据包 可能的重传数据包
			buffers = new byte[(mtu + OVERHEAD) * 3 + RESERVED_BYTE];
		}

		// ikcp_segment_new
		// 我们保留了原始函数并添加了我们的池化机制。
		// 这样我们就不会在任何地方遗漏它。
		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// Segment SegmentNew() => SegmentPool.Take();

		// ikcp_segment_delete
		// 我们保留了原始函数并添加了我们的池化机制。
		// 这样我们就不会在任何地方遗漏它。
		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// void SegmentDelete(Segment seg) => SegmentPool.Return(seg);

		// 计算有多少数据包等待发送

		/// <summary>
		/// 等待发送
		/// </summary>
		public int WaitSnd => snd_bufList.Count + snd_Queue.Count;

		// ikcp_wnd_unused
		// 返回接收窗口中剩余的空间 (rcv_wnd - rcv_queue)



		/// <summary>
		/// 未使用的窗口
		/// </summary>
		/// <remarks>
		///方法的作用是计算并返回接收窗口中未使用的空间。
		///接收窗口是一个缓冲区，用于存储接收到但尚未处理的数据包。
		///这个方法通过计算接收窗口的总大小减去已使用的空间，
		///来确定还有多少空间可以用来接收新的数据包。
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ushort WndUnused()
		{
			//判断接收窗口中已经使用的空间是否超过了接收窗口的总大小。
			if (rcv_Queue.Count < rcv_wnd)
				//没有超过，返回接收窗口的总大小减去已使用的空间。
				return (ushort)(rcv_wnd - (uint)rcv_Queue.Count);
			return 0;
		}

		/// <summary>
		/// 从接收队列中读取数据
		/// </summary>
		public int Receive(Span<byte> data)
		{
			// kcp 的 ispeek 功能不支持。
			// 这使得“合并片段”代码显著简化，因为
			// 我们可以在 queue.Count > 0 时迭代并每次出队。
			// 如果我们必须考虑 ispeek，那么 count 将始终 > 0 并且
			// 我们必须在循环后才删除。

			int len = data.Length;

			if (rcv_Queue.Count == 0)
				return -1;

			int peeksize = PeekSize();

			if (peeksize < 0)
				return -2;

			if (peeksize > len)
				return -3;

			//是否需要快速恢复
			//快速恢复通常用于网络协议中，
			//当接收方的接收缓冲区已满时，
			//发送方需要采取措施来调整发送速率或进行其他恢复操作，以确保数据传输的顺畅。
			bool recover = rcv_Queue.Count >= rcv_wnd;

			// 合并片段。

			len = 0;
			ref byte dest = ref MemoryMarshal.GetReference(data);

			// 原始 KCP 迭代 rcv_queue 并在 !ispeek 时删除。
			// 在迭代时从 C# 队列中删除是不可能的，但
			// 我们可以更改为“while Count > 0”并每次删除。
			// （我们可以每次删除，因为我们删除了 ispeek 支持！）

			while (rcv_Queue.Count > 0)
			{
				// 与原始 kcp 不同，我们出队而不是仅获取
				// 条目。这没问题，因为我们在任何情况下都删除它。
				SegmentStruct seg = rcv_Queue.Dequeue();

				// 将段数据复制到我们的缓冲区中

				ref byte source = ref MemoryMarshal.GetReference(seg.WrittenBuffer);
				Unsafe.CopyBlockUnaligned(ref dest, ref source, (uint)seg.WrittenCount);
				dest = ref Unsafe.Add(ref dest, seg.WrittenCount);

				len += seg.WrittenCount;
				uint fragment = seg.SegHead.Frg;

				// 注意：不支持 ispeek 以简化此循环

				// 与原始 kcp 不同，我们不需要从队列中删除 seg
				// 因为我们已经将其出队。
				// 直接删除它
				seg.Dispose();
				if (fragment == 0)
					break;
			}


			// 将可用数据从 rcv_buf 移动到 rcv_queue
			int removed = 0;
#if NET7_0_OR_GREATER
            foreach (ref SegmentStruct seg in CollectionsMarshal.AsSpan(this.rcv_buf))
#else
			foreach (SegmentStruct seg in rcv_bufList)
#endif
			{
				if (seg.SegHead.Sn == rcv_nxt && rcv_Queue.Count < rcv_wnd)
				{
					// 不能在迭代时删除。记住要删除的数量
					// 并在循环后执行。
					// 注意：不要返回段。我们只将其添加到 rcv_queue
					++removed;
					// 添加
					rcv_Queue.Enqueue(seg);
					// 增加下一个段的序列号
					rcv_nxt++;
				}
				else
				{
					break;
				}
			}
			rcv_bufList.RemoveRange(0, removed);

			// 快速恢复
			if (rcv_Queue.Count < rcv_wnd && recover)
			{
				// 准备在 flush 中发送回 CMD_WINS
				// 告诉远程我的窗口大小
				probe |= ASK_TELL;
			}
			return len;
		}

		/// <summary>
		/// 查看接收队列中下一个消息的大小。
		/// 如果没有消息或消息尚未完整接收，则返回 -1。
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int PeekSize()
		{
			int length = 0;

			// 检查接收队列是否为空
			if (rcv_Queue.Count == 0) return -1;

			// 查看第一个段
			SegmentStruct seq = rcv_Queue.Peek();

			// 如果消息不需要分片，seg.frg 为 0
			// 在这种情况下，段的大小就是最终消息的大小
			if (seq.SegHead.Frg == 0) return seq.WrittenCount;

			// 检查是否已接收到所有分片部分
			// seg.frg 是第 n 个分片，但顺序是反的
			// 这样第一个接收到的段告诉我们消息有多少个分片
			// 例如，如果一个消息包含 3 个段：
			//   第一个段： .frg 是 2（反向索引）
			//   第二个段： .frg 是 1（反向索引）
			//   第三个段： .frg 是 0（反向索引）
			if (rcv_Queue.Count < seq.SegHead.Frg + 1) return -1;

			// 接收队列包含重建消息所需的所有分片
			// 累加所有分片的大小以获得完整消息的大小
			foreach (SegmentStruct seg in rcv_Queue)
			{
				length += seg.WrittenCount;
				if (seg.SegHead.Frg == 0) break;
			}

			return length;
		}


		/// <summary>
		/// 发送数据
		/// 将消息拆分为 MTU 大小的片段，并将它们添加到发送队列中。
		/// </summary>
		/// <param name="data">要发送的数据</param>
		/// <returns>操作结果</returns>
		public int Send(ReadOnlySpan<byte> data)
		{
			// 片段数量
			int count;
			int len = data.Length;
			int offset = 0;

			// 流模式：已移除。我们不希望发送 'hello' 并接收 'he' 'll' 'o'。
			// 我们希望始终接收 'hello'。

			// 计算 'len' 所需的片段数量
			if (len <= mss) count = 1;
			else count = (int)((len + mss - 1) / mss);

			// 重要提示：kcp 将 'frg' 编码为 1 个字节。
			// 因此我们最多只能支持 255 个片段。
			// （这将最大消息大小限制在大约 288 KB）
			// 这很难调试。让我们使这一点 100% 明显。
			if (count > FRG_MAX)
				ThrowFrgCountException(len, count);

			// 原始 kcp 使用 WND_RCV 常量而不是 rcv_wnd 运行时：
			// https://github.com/skywind3000/kcp/pull/291/files
			// 这始终将最大消息大小限制为 144 KB：
			// if (count >= WND_RCV) return -2;
			// 使用配置的 rcv_wnd 取消最大消息大小限制：
			if (count >= rcv_wnd) return -2;

			if (count == 0) count = 1;

			ref byte dataRef = ref MemoryMarshal.GetReference(data);

			// 片段
			for (int i = 0; i < count; i++)
			{
				int size = len > (int)mss ? (int)mss : len;
				SegmentStruct seg = new SegmentStruct(size, this.kcpSegmentArrayPool);

				if (len > 0)
				{
					Unsafe.CopyBlockUnaligned(ref MemoryMarshal.GetReference(seg.FreeBuffer), ref dataRef, (uint)size);
					dataRef = ref Unsafe.Add(ref dataRef, size);
					seg.Advance(size);
				}

				seg.SegHead.Frg = (byte)(count - i - 1);
				snd_Queue.Enqueue(seg);
				offset += size;
				len -= size;
			}

			return 0;
		}


		/// <summary>
		/// 更新确认时间
		/// </summary>
		/// <param name="rtt">往返时间</param>
		void UpdateAck(int rtt) // round trip time
		{
			// 参考：https://tools.ietf.org/html/rfc6298
			if (rx_srtt == 0)
			{
				// 如果这是第一次测量往返时间，直接设置
				rx_srtt = rtt;
				rx_rttval = rtt / 2;
			}
			else
			{
				// 计算新的往返时间偏差
				int delta = rtt - rx_srtt;
				if (delta < 0) delta = -delta;

				// 更新往返时间偏差和平滑往返时间
				rx_rttval = (3 * rx_rttval + delta) / 4;
				rx_srtt = (7 * rx_srtt + rtt) / 8;

				// 确保平滑往返时间至少为1
				if (rx_srtt < 1) rx_srtt = 1;
			}

			// 计算重传超时时间
			int rto = rx_srtt + Math.Max((int)interval, 4 * rx_rttval);

			// 将重传超时时间限制在最小和最大值之间
			rx_rto = Utils.Clamp(rto, rx_minrto, RTO_MAX);
		}


		/// <summary>
		/// 收缩发送缓冲区
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ShrinkBuf()
		{
			// 如果发送缓冲区中有数据
			if (snd_bufList.Count > 0)
			{
				// 获取发送缓冲区中的第一个段
				SegmentStruct seg = snd_bufList[0];
				// 更新未确认的最早序列号
				snd_una = seg.SegHead.Sn;
			}
			else
			{
				// 如果发送缓冲区为空，则将未确认的最早序列号设置为下一个待发送的序列号
				snd_una = snd_nxt;
			}
		}


		/// <summary>
		/// 解析确认包
		/// 从发送缓冲区中移除具有指定序列号的段
		/// </summary>
		/// <param name="sn">序列号</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ParseAck(uint sn)
		{
			// 如果序列号小于未确认的最早序列号或大于等于下一个待发送的序列号，则返回
			if (Utils.TimeDiff(sn, snd_una) < 0 || Utils.TimeDiff(sn, snd_nxt) >= 0)
				return;

			// 标记是否需要移除段
			bool needRemove = false;
			// 要移除的段的索引
			int removeIndex = 0;

#if NET7_0_OR_GREATER
    foreach (ref var seg in CollectionsMarshal.AsSpan(snd_bufList))
#else
			foreach (var seg in snd_bufList)
#endif
			{
				// 如果找到具有指定序列号的段
				if (sn == seg.SegHead.Sn)
				{
					// 标记需要移除
					needRemove = true;
					// 释放段资源
					seg.Dispose();
					break;
				}
				// 如果当前段的序列号大于指定的序列号，停止查找
				if (Utils.TimeDiff(sn, seg.SegHead.Sn) < 0)
				{
					break;
				}

				// 更新要移除的段的索引
				removeIndex++;
			}

			// 如果需要移除段，则从发送缓冲区中移除
			if (needRemove)
			{
				snd_bufList.RemoveAt(removeIndex);
			}
		}


		// ikcp_parse_una
		// 移除发送缓冲区中所有序列号小于una的未确认段
		/// <summary>
		/// 解析UNA，移除所有序列号小于una的未确认段
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ParseUna(uint una)
		{
			int removed = 0;
#if NET7_0_OR_GREATER
            foreach (ref SegmentStruct seg in CollectionsMarshal.AsSpan(snd_buf))
#else
			foreach (SegmentStruct seg in snd_bufList)
#endif
			{
				// 如果段的序列号小于una
				if (seg.SegHead.Sn < una)
				{
					// 不能在迭代时移除，记住要移除的数量，循环结束后再移除
					++removed;
					// 删除段
					seg.Dispose();
				}
				else
				{
					break;
				}
			}
			// 从发送缓冲区中移除已确认的段
			snd_bufList.RemoveRange(0, removed);
		}


		// ikcp_parse_fastack
		/// <summary>
		/// 解析快速确认
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void ParseFastack(uint sn, uint ts) // 序列号，时间戳
		{
			// sn 需要在 snd_una 和 snd_nxt 之间
			// if !(snd_una <= sn && sn < snd_nxt) return;

			// 如果 sn 小于 snd_una，则返回
			if (sn < snd_una)
				return;

			// 如果 sn 大于等于 snd_nxt，则返回
			if (sn >= snd_nxt)
				return;
#if NET7_0_OR_GREATER
            foreach (ref var seg in CollectionsMarshal.AsSpan(snd_buf))
            {
                // 如果 sn 小于 seg 的序列号，则跳出循环
                if (sn < seg.SegHead.sn)
                {
                    break;
                }
                // 如果 sn 不等于 seg 的序列号，则增加 fastack 计数
                else if (sn != seg.SegHead.sn)
                {
#if !FASTACK_CONSERVE
                    seg.fastack++;
#else
                    // 如果 ts 大于等于 seg 的时间戳，则增加 fastack 计数
                    if (Utils.TimeDiff(ts, seg.SegHead.ts) >= 0)
                    {
                        seg.fastack++;
                    }
#endif
                }
            }
#else
			for (int i = 0; i < this.snd_bufList.Count; i++)
			{
				SegmentStruct seg = this.snd_bufList[i];
				// 如果 sn 小于 seg 的序列号，则跳出循环
				if (sn < seg.SegHead.Sn)
				{
					break;
				}
				// 如果 sn 不等于 seg 的序列号，则增加 fastack 计数
				else if (sn != seg.SegHead.Sn)
				{
#if !FASTACK_CONSERVE
					seg.Fastack++;
					this.snd_bufList[i] = seg;
#else
                    // 如果 ts 大于等于 seg 的时间戳，则增加 fastack 计数
                    if (Utils.TimeDiff(ts, seg.SegHead.ts) >= 0)
                    {
                        seg.fastack++;
                        this.snd_buf[i] = seg;
                    }
#endif
				}
			}
#endif
		}


		// ikcp_ack_push
		// 添加一个确认项
		/// <summary>
		/// 确认推送
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void AckPush(uint sn, uint ts) // 序列号，时间戳
		{
			ackList.Add(new AckItem { serialNumber = sn, timestamp = ts });
		}


		// ikcp_parse_data
		/// <summary>
		/// 解析数据
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ParseData(ref SegmentStruct newseg)
		{
			uint sn = newseg.SegHead.Sn;

			// 如果 sn 超过接收窗口的范围，或者 sn 小于下一个接收序列号，则丢弃该段
			if (Utils.TimeDiff(sn, rcv_nxt + rcv_wnd) >= 0 ||
				Utils.TimeDiff(sn, rcv_nxt) < 0)
			{
				newseg.Dispose();
				return;
			}

			// 将段插入接收缓冲区
			InsertSegmentInReceiveBuffer(ref newseg);
			// 将准备好的段移动到接收队列
			MoveReceiveBufferReadySegmentsToQueue();
		}


		// 将段插入接收缓冲区，按 seg.sn 排序。
		// 如果已经存在相同 seg.sn 的段，则丢弃该段。
		// 为了性能，逆序遍历接收缓冲区。
		//
		// 注意：参见 KcpTests.InsertSegmentInReceiveBuffer 测试！
		// 注意：'插入或删除' 可以有不同的实现方式，但我们保持与原始 C 版 kcp 的一致性。
		/// <summary>
		/// 插入接收缓冲区
		/// </summary>
		/// <param name="newseg">新的段</param>
		internal void InsertSegmentInReceiveBuffer(ref SegmentStruct newseg)
		{
			bool repeat = false; // '重复'

			// 原始 C 版是逆序遍历，所以我们也需要这样做。
			// 注意如果 rcv_buf.Count == 0，i 变为 -1，不会进行循环。
			int i;
#if NET7_0_OR_GREATER
            Span<SegmentStruct> arr = CollectionsMarshal.AsSpan(rcv_buf);
            for (i = arr.Length-1; i>=0 ; i--)
            {
                ref SegmentStruct seg =ref arr[i];
#else
			for (i = rcv_bufList.Count - 1; i >= 0; i--)
			{
				SegmentStruct seg = rcv_bufList[i];
#endif
				if (seg.SegHead.Sn == newseg.SegHead.Sn)
				{
					// 找到重复的段，不会添加任何内容。
					repeat = true;
					break;
				}
				if (Utils.TimeDiff(newseg.SegHead.Sn, seg.SegHead.Sn) > 0)
				{
					// 该条目的 sn 小于 newseg.sn，所以停止
					break;
				}
			}

			// 没有重复？则插入。
			if (!repeat)
			{
				rcv_bufList.Insert(i + 1, newseg);
			}
			// 重复。直接删除。
			else
			{
				newseg.Dispose();
			}
		}

		// 将准备好的段从 rcv_buf 移动到 rcv_queue。
		// 仅移动按 rcv_nxt 序列顺序准备好的段。
		// 有些段可能仍然缺失，稍后插入。
		/// <summary>
		/// 移动接收缓冲区准备好的段到队列
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void MoveReceiveBufferReadySegmentsToQueue()
		{
			int removed = 0;
#if NET7_0_OR_GREATER
            foreach (ref var seg in CollectionsMarshal.AsSpan(rcv_buf))
#else
			foreach (var seg in rcv_bufList)
#endif
			{
				// 当段按 'rcv_nxt' 序列顺序时移动段。
				// 有些段可能仍然缺失，稍后插入，在这种情况下会立即停止
				// 因为段总是需要按确切的序列顺序接收。
				if (seg.SegHead.Sn == rcv_nxt && rcv_Queue.Count < rcv_wnd)
				{
					// 不能在迭代时删除。记住要删除的数量
					// 并在循环后执行删除。
					++removed;
					rcv_Queue.Enqueue(seg);
					// 增加下一个段的序列号
					rcv_nxt++;
				}
				else
				{
					break;
				}
			}
			rcv_bufList.RemoveRange(0, removed);
		}


		/// <summary>
		/// 输入
		/// </summary>
		/// <param name="data">接收到的低层数据包（例如UDP包）</param>
		/// <returns>处理结果，0表示成功，负数表示错误</returns>
		public int Input(Span<byte> data)
		{
			int offset = 0; // 当前数据偏移量
			int size = data.Length; // 数据长度
			uint prev_una = snd_una; // 记录之前的未确认序列号
			uint maxack = 0; // 最大的确认序列号
			uint latest_ts = 0; // 最新的时间戳
			int flag = 0; // 标志位

			// 如果数据为空或长度小于协议头部长度，返回错误
			if (data == null || size < OVERHEAD) return -1;

			while (true)
			{
				// 如果剩余数据不足以解码一个协议头部，退出循环
				if (size < OVERHEAD) break;

				// 读取协议头部
				var segHead = Unsafe.ReadUnaligned<SegmentHead>(ref MemoryMarshal.GetReference(data.Slice(offset)));
				offset += Unsafe.SizeOf<SegmentHead>(); // 更新偏移量
				uint conv_ = segHead.Conv; // 会话ID
				byte cmd = segHead.Cmd; // 命令类型
				byte frg = segHead.Frg; // 分片序号
				ushort wnd = segHead.Wnd; // 窗口大小
				uint ts = segHead.Ts; // 时间戳
				uint sn = segHead.Sn; // 序列号
				uint una = segHead.Una; // 未确认序列号
				uint len = segHead.Len; // 数据长度

				// 减少剩余数据长度
				size -= OVERHEAD;

				// 如果剩余数据不足以读取len长度的数据，返回错误
				if (size < len || (int)len < 0) return -2;

				// 验证命令类型
				if (cmd != CMD_PUSH && cmd != CMD_ACK && cmd != CMD_WASK && cmd != CMD_WINS)
					return -3;

				rmt_wnd = wnd; // 更新远端窗口大小
				ParseUna(una); // 解析未确认序列号
				ShrinkBuf(); // 收缩发送缓冲区

				if (cmd == CMD_ACK)
				{
					// 处理ACK命令
					if (Utils.TimeDiff(current, ts) >= 0)
					{
						UpdateAck(Utils.TimeDiff(current, ts)); // 更新ACK
					}
					ParseAck(sn); // 解析ACK
					ShrinkBuf(); // 收缩发送缓冲区
					if (flag == 0)
					{
						flag = 1;
						maxack = sn;
						latest_ts = ts;
					}
					else
					{
						if (Utils.TimeDiff(sn, maxack) > 0)
						{
#if !FASTACK_CONSERVE
							maxack = sn;
							latest_ts = ts;
#else
                    if (Utils.TimeDiff(ts, latest_ts) > 0)
                    {
                        maxack = sn;
                        latest_ts = ts;
                    }
#endif
						}
					}
				}
				else if (cmd == CMD_PUSH)
				{
					// 处理PUSH命令
					if (Utils.TimeDiff(sn, rcv_nxt + rcv_wnd) < 0)
					{
						AckPush(sn, ts); // 推送ACK
						if (Utils.TimeDiff(sn, rcv_nxt) >= 0)
						{
							SegmentStruct seg = new SegmentStruct((int)len, this.kcpSegmentArrayPool);
							seg.SegHead = new SegmentHead()
							{
								Conv = conv_,
								Cmd = cmd,
								Frg = frg,
								Wnd = wnd,
								Ts = ts,
								Sn = sn,
								Una = una,
							};
							if (len > 0)
							{
								data.Slice(offset, (int)len).CopyTo(seg.FreeBuffer); // 复制数据
								seg.Advance((int)len); // 更新数据长度
							}
							ParseData(ref seg); // 解析数据
						}
					}
				}
				else if (cmd == CMD_WASK)
				{
					// 处理WASK命令，准备在flush中发送CMD_WINS
					probe |= ASK_TELL;
				}
				else if (cmd == CMD_WINS)
				{
					// 处理WINS命令，不做任何操作
				}
				else
				{
					return -3; // 未知命令类型，返回错误
				}

				offset += (int)len; // 更新偏移量
				size -= (int)len; // 减少剩余数据长度
			}

			if (flag != 0)
			{
				ParseFastack(maxack, latest_ts); // 解析快速ACK
			}

			// 当数据包到达时更新拥塞窗口
			if (Utils.TimeDiff(snd_una, prev_una) > 0)
			{
				// 如果当前拥塞窗口小于远端窗口
				if (cwnd < rmt_wnd)
				{
					// 如果当前拥塞窗口小于慢启动阈值
					if (cwnd < ssthresh)
					{
						// 处于慢启动阶段，增加拥塞窗口和增量
						cwnd++;
						incr += mss;
					}
					else
					{
						// 处于拥塞避免阶段
						if (incr < mss) incr = mss; // 确保增量不小于MSS
													// 增加增量，使用加法增量算法
						incr += (mss * mss) / incr + (mss / 16);
						// 如果新的拥塞窗口大小不超过增量
						if ((cwnd + 1) * mss <= incr)
						{
							// 更新拥塞窗口大小
							cwnd = (incr + mss - 1) / ((mss > 0) ? mss : 1);
						}
					}
					// 如果拥塞窗口大小超过远端窗口
					if (cwnd > rmt_wnd)
					{
						// 将拥塞窗口大小限制为远端窗口大小
						cwnd = rmt_wnd;
						// 更新增量为远端窗口大小乘以MSS
						incr = rmt_wnd * mss;
					}
				}

			}

			return 0; // 成功处理数据包
		}


		/// <summary>
		/// 制造空间
		/// </summary>
		/// <param name="size">当前缓冲区已使用的大小</param>
		/// <param name="space">需要的额外空间大小</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void MakeSpace(ref int size, int space)
		{
			// 如果当前缓冲区已使用的大小加上需要的额外空间大小超过了MTU（最大传输单元）
			if (size + space > mtu)
			{
				// 调用输出回调函数，将当前缓冲区的数据输出
				output(buffers, size);
				// 重置缓冲区已使用的大小为0
				size = 0;
			}
		}

		/// <summary>
		/// 刷新缓冲区
		/// </summary>
		/// <param name="size">当前缓冲区已使用的大小</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void FlushBuffer(int size)
		{
			// 如果缓冲区中有数据（已使用的大小大于0）
			if (size > 0)
			{
				// 调用输出回调函数，将当前缓冲区的数据输出
				output(buffers, size);
			}
		}


		// ikcp_flush
		// 刷新剩余的ACK段。
		// 刷新可能会输出多个小于等于MTU的消息，这些消息来自MakeSpace / FlushBuffer。
		// 消息的数量取决于滑动窗口。
		// 由发送/接收窗口大小和拥塞控制配置。
		// 在拥塞控制下，窗口会非常小(!)。
		/// <summary>
		/// 刷新
		/// </summary>
		public void Flush()
		{
			int size = 0;     // 要刷新的字节数，相当于C语言中的'buffer ptr'。
			bool lost = false; // 丢失的段

			// 刷新前需要调用update
			if (!updated) return;

			// kcp只在这里堆栈分配一个段以提高性能，留下
			// 它的数据缓冲区为空，因为这个段的数据缓冲区从未
			// 被使用。这在C语言中是可以的，但在C#中我们的段是一个类，
			// 所以我们需要分配它，最重要的是，在返回之前不要忘记释放它。
			SegmentStruct seg = new SegmentStruct((int)this.mtu, this.kcpSegmentArrayPool);
			seg.SegHead.Conv = conv; // 设置会话ID
			seg.SegHead.Cmd = CMD_ACK; // 设置命令为ACK
			seg.SegHead.Wnd = WndUnused(); // 设置窗口大小
			seg.SegHead.Una = rcv_nxt; // 设置未确认的序列号


			// 刷新确认
#if NET7_0_OR_GREATER
            foreach (ref AckItem ack  in CollectionsMarshal.AsSpan(acklist))
#else
			foreach (AckItem ack in ackList)
#endif
			{
				MakeSpace(ref size, OVERHEAD);
				// ikcp_ack_get 将 ack[i] 分配给 seg.sn 和 seg.ts
				seg.SegHead.Sn = ack.serialNumber; // 设置段的序列号
				seg.SegHead.Ts = ack.timestamp; // 设置段的时间戳
				seg.Encode(buffers.AsSpan(size + RESERVED_BYTE), ref size); // 编码段并将其放入缓冲区
			}
			ackList.Clear(); // 清空确认列表


			// 探测窗口大小（如果远程窗口大小等于零）
			if (rmt_wnd == 0)
			{
				if (probe_wait == 0)
				{
					probe_wait = PROBE_INIT; // 初始化探测等待时间
					ts_probe = current + probe_wait; // 设置下次探测时间
				}
				else
				{
					if (Utils.TimeDiff(current, ts_probe) >= 0)
					{
						if (probe_wait < PROBE_INIT)
							probe_wait = PROBE_INIT; // 确保探测等待时间不小于初始值
						probe_wait += probe_wait / 2; // 增加探测等待时间
						if (probe_wait > PROBE_LIMIT)
							probe_wait = PROBE_LIMIT; // 确保探测等待时间不超过限制
						ts_probe = current + probe_wait; // 设置下次探测时间
						probe |= ASK_SEND; // 设置探测标志
					}
				}
			}
			else
			{
				ts_probe = 0; // 重置探测时间
				probe_wait = 0; // 重置探测等待时间
			}


			// 刷新窗口探测命令
			if ((probe & ASK_SEND) != 0)
			{
				seg.SegHead.Cmd = CMD_WASK; // 设置命令为窗口探测请求
				MakeSpace(ref size, OVERHEAD); // 为新的数据段腾出空间
				seg.Encode(buffers.AsSpan(size + RESERVED_BYTE), ref size); // 编码段并将其放入缓冲区
			}

			// 刷新窗口探测命令
			if ((probe & ASK_TELL) != 0)
			{
				seg.SegHead.Cmd = CMD_WINS; // 设置命令为窗口探测响应
				MakeSpace(ref size, OVERHEAD); // 为新的数据段腾出空间
				seg.Encode(buffers.AsSpan(size + RESERVED_BYTE), ref size); // 编码段并将其放入缓冲区
			}

			probe = 0; // 重置探测标志


			// 计算当前安全发送的窗口大小。
			// 取发送窗口和远程窗口中较小的值。
			// 作为我们的最大值
			uint cwnd_ = Math.Min(snd_wnd, rmt_wnd);

			// 双重否定：如果启用了拥塞窗口：
			// 将窗口大小限制为cwnd。
			//
			// 注意，这可能会严重限制窗口大小。
			// 在我们的最大消息大小测试中，使用超大窗口32k，
			// '拥塞窗口'将其从32,000限制到2。
			if (!nocwnd) cwnd_ = Math.Min(cwnd, cwnd_);


			// 将cwnd_ '窗口大小'的消息从snd_queue移动到snd_buf
			//   'snd_nxt' 是我们想要发送的序列号。
			//   'snd_una' 是尚未被确认的序列号。
			//   复制它们之间最多 'cwnd_' 的差异（滑动窗口）
			while (Utils.TimeDiff(snd_nxt, snd_una + cwnd_) < 0)
			{
				if (snd_Queue.Count == 0) break; // 如果发送队列为空，则退出循环

				SegmentStruct newseg = snd_Queue.Dequeue(); // 从发送队列中取出一个新的段

				newseg.SegHead.Conv = conv; // 设置会话ID
				newseg.SegHead.Cmd = CMD_PUSH; // 设置命令为推送数据
				newseg.SegHead.Wnd = seg.SegHead.Wnd; // 设置窗口大小
				newseg.SegHead.Ts = current; // 设置当前时间戳
				newseg.SegHead.Sn = snd_nxt; // 设置段的序列号
				snd_nxt += 1; // 增加下一个段的序列号
				newseg.SegHead.Una = rcv_nxt; // 设置未确认的序列号
				newseg.Resendts = current; // 设置重传时间戳
				newseg.Rto = rx_rto; // 设置重传超时时间
				newseg.Fastack = 0; // 初始化快速确认计数
				newseg.Xmit = 0; // 初始化传输计数
				snd_bufList.Add(newseg); // 将新段添加到发送缓冲区列表
			}


			// 计算重传次数
			uint resent = fastresend > 0 ? (uint)fastresend : 0xffffffff; // 如果设置了快速重传次数，则使用该值，否则使用最大值
			uint rtomin = nodelay == 0 ? (uint)rx_rto >> 3 : 0; // 如果未启用无延迟模式，则最小重传时间为rx_rto的八分之一

			// 刷新数据段
			int change = 0; // 初始化变化计数

#if NET7_0_OR_GREATER
            var sndBufArr = CollectionsMarshal.AsSpan(this.snd_buf);
            for (int i = 0; i < sndBufArr.Length; i++)
            {
                ref SegmentStruct segment = ref sndBufArr[i];
#else
			for (int i = 0; i < this.snd_bufList.Count; i++)
			{
				SegmentStruct segment = this.snd_bufList[i];
#endif
				bool needsend = false; // 是否需要发送

				// 初始传输
				if (segment.Xmit == 0)
				{
					needsend = true; // 需要发送
					segment.Xmit++; // 增加传输计数
					segment.Rto = this.rx_rto; // 设置重传超时时间
					segment.Resendts = this.current + (uint)segment.Rto + rtomin; // 设置重传时间戳
				}
				// 重传超时
				else if (Utils.TimeDiff(this.current, segment.Resendts) >= 0)
				{
					needsend = true; // 需要发送
					segment.Xmit++; // 增加传输计数
					this.xmit++; // 增加全局传输计数
					if (this.nodelay == 0)
					{
						segment.Rto += Math.Max(segment.Rto, this.rx_rto); // 增加重传超时时间
					}
					else
					{
						int step = (this.nodelay < 2) ? segment.Rto : this.rx_rto;
						segment.Rto += step / 2; // 增加重传超时时间
					}

					segment.Resendts = this.current + (uint)segment.Rto; // 更新重传时间戳
					lost = true; // 标记为丢失
				}
				// 快速重传
				else if (segment.Fastack >= resent)
				{
					if (segment.Xmit <= this.fastlimit || this.fastlimit <= 0)
					{
						needsend = true; // 需要发送
						segment.Xmit++; // 增加传输计数
						segment.Fastack = 0; // 重置快速确认计数
						segment.Resendts = this.current + (uint)segment.Rto; // 更新重传时间戳
						change++; // 增加变化计数
					}
				}

				if (needsend)
				{
					segment.SegHead.Ts = this.current; // 设置时间戳
					segment.SegHead.Wnd = seg.SegHead.Wnd; // 设置窗口大小
					segment.SegHead.Una = this.rcv_nxt; // 设置未确认的序列号

					int need = OVERHEAD + segment.WrittenCount; // 计算需要的空间
					this.MakeSpace(ref size, need); // 为数据段腾出空间

					segment.Encode(this.buffers.AsSpan(size + RESERVED_BYTE), ref size); // 编码数据段

					if (segment.WrittenCount > 0)
					{
						segment.WrittenBuffer.CopyTo(this.buffers.AsSpan(size + RESERVED_BYTE)); // 复制数据到缓冲区

						size += segment.WrittenCount; // 更新缓冲区大小
					}

					// 如果消息被重传了N次，但仍未收到确认，则发生死链
					if (segment.Xmit >= this.dead_link)
					{
						this.state = -1; // 设置状态为-1表示死链
					}
				}
#if !NET7_0_OR_GREATER
				this.snd_bufList[i] = segment; // 更新发送缓冲区列表中的段
#endif
			}


			// kcp 使用 stackalloc 分配 'seg'。我们的 C# 段是一个类，
			// 因此我们需要在完成后正确删除并将其返回到池中。
			// SegmentDelete(seg);

			seg.Dispose(); // 释放段的资源

			// 刷新剩余的段
			FlushBuffer(size); // 刷新缓冲区中的剩余数据段


			// 更新慢启动阈值
			// 速率减半，参考 https://tools.ietf.org/html/rfc6937
			if (change > 0)
			{
				uint inflight = snd_nxt - snd_una; // 计算在途数据量
				ssthresh = inflight / 2; // 将慢启动阈值设置为在途数据量的一半
				if (ssthresh < THRESH_MIN)
					ssthresh = THRESH_MIN; // 如果慢启动阈值小于最小阈值，则设置为最小阈值
				cwnd = ssthresh + resent; // 更新拥塞窗口大小
				incr = cwnd * mss; // 更新增量
			}


			// 拥塞控制，参考 https://tools.ietf.org/html/rfc5681
			if (lost)
			{
				// 原始 C 代码使用 'cwnd'，而不是 kcp->cwnd！
				ssthresh = cwnd_ / 2; // 将慢启动阈值设置为当前拥塞窗口的一半
				if (ssthresh < THRESH_MIN)
					ssthresh = THRESH_MIN; // 如果慢启动阈值小于最小阈值，则设置为最小阈值
				cwnd = 1; // 将拥塞窗口大小设置为1
				incr = mss; // 将增量设置为最大报文段大小
			}

			if (cwnd < 1)
			{
				cwnd = 1; // 如果拥塞窗口大小小于1，则将其设置为1
				incr = mss; // 将增量设置为最大报文段大小
			}

		}

		// ikcp_update
		// 更新状态（需要反复调用，每10ms-100ms调用一次），或者你可以通过调用 Check() 来确定何时再次调用（不需要 Input/Send 调用）。
		//
		// 'current' - 当前的时间戳（毫秒）。将其传递给 Kcp，这样 Kcp 就不需要执行任何秒表/时间差等代码。
		//
		// 使用 uint 类型的时间，可能是为了最小化带宽。
		// uint.max = 4294967295 毫秒 = 1193 小时 = 49 天
		/// <summary>
		/// 更新
		/// </summary>
		public void Update(uint currentTimeMilliSeconds)
		{
			current = currentTimeMilliSeconds; // 更新当前时间戳

			// 如果尚未更新，则设置 updated 标志并记录最后一次刷新时间
			if (!updated)
			{
				updated = true;
				ts_flush = current;
			}

			// slap 是自上次刷新以来的时间（毫秒）
			int slap = Utils.TimeDiff(current, ts_flush);

			// 硬限制：如果已经过去了10秒，则无论如何都要刷新
			if (slap >= 10000 || slap < -10000)
			{
				ts_flush = current;
				slap = 0;
			}

			// 每次最后一次刷新时间增加一个 'interval'。
			// 因此 slap >= 是一种检查间隔时间是否已经过去的奇怪方式。
			if (slap >= 0)
			{
				// 将最后一次刷新时间增加一个间隔
				ts_flush += interval;

				// 如果最后一次刷新时间仍然落后，则将其增加到当前时间 + 间隔
				// if (Utils.TimeDiff(current, ts_flush) >= 0) // 原始 kcp.c
				if (current >= ts_flush)                       // 更不容易混淆
				{
					ts_flush = current + interval;
				}
				Flush(); // 刷新
			}
		}


		// ikcp_check
		// 确定何时应该调用 update
		// 返回你应该在多少毫秒后调用 update，如果没有输入/发送调用。你可以在那个时间调用 update，而不是反复调用 update。
		//
		// 重要的是减少不必要的 update 调用。使用它来安排 update（例如，实现一个类似 epoll 的机制，或者在处理大量 kcp 连接时优化 update）。
		/// <summary>
		/// 检查
		/// </summary>
		public uint Check(uint current_)
		{
			uint ts_flush_ = ts_flush;
			// int tm_flush = 0x7fffffff; 原始 kcp: 无用的赋值
			int tm_packet = 0x7fffffff;

			if (!updated)
			{
				return current_; // 如果尚未更新，则返回当前时间
			}

			if (Utils.TimeDiff(current_, ts_flush_) >= 10000 ||
				Utils.TimeDiff(current_, ts_flush_) < -10000)
			{
				ts_flush_ = current_; // 如果时间差超过10秒或小于-10秒，则将最后一次刷新时间设置为当前时间
			}

			if (Utils.TimeDiff(current_, ts_flush_) >= 0)
			{
				return current_; // 如果时间差大于等于0，则返回当前时间
			}

			int tm_flush = Utils.TimeDiff(ts_flush_, current_); // 计算到下次刷新时间的时间差
#if NET7_0_OR_GREATER
    foreach (ref SegmentStruct seg in CollectionsMarshal.AsSpan(this.snd_buf))
#else
			foreach (SegmentStruct seg in snd_bufList)
#endif
			{
				int diff = Utils.TimeDiff(seg.Resendts, current_); // 计算到下次重发时间的时间差
				if (diff <= 0)
				{
					return current_; // 如果时间差小于等于0，则返回当前时间
				}
				if (diff < tm_packet) tm_packet = diff; // 更新最小的时间差
			}

			uint minimal = (uint)(tm_packet < tm_flush ? tm_packet : tm_flush); // 取最小的时间差
			if (minimal >= interval) minimal = interval; // 如果最小时间差大于等于间隔时间，则设置为间隔时间

			return current_ + minimal; // 返回当前时间加上最小时间差
		}


		/// <summary>
		/// 设置MTU（最大传输单元）大小。
		/// </summary>
		/// <param name="mtu">新的MTU值</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMtu(uint mtu)
		{
			// 如果传入的MTU值小于50或小于OVERHEAD（开销），则抛出MTU异常
			if (mtu < 50 || mtu < OVERHEAD)
				this.ThrowMTUException();

			// 根据新的MTU值重新分配缓冲区大小
			// 缓冲区大小 = (MTU + OVERHEAD) * 3 + RESERVED_BYTE
			buffers = new byte[(mtu + OVERHEAD) * 3 + RESERVED_BYTE];

			// 设置新的MTU值
			this.mtu = mtu;

			// 计算并设置MSS（最大报文段大小），MSS = MTU - OVERHEAD
			mss = mtu - OVERHEAD;
		}


		/// <summary>
		/// 设置更新间隔时间。
		/// </summary>
		/// <param name="interval">新的间隔时间（毫秒）</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetInterval(uint interval)
		{
			// 将间隔时间限制在10到5000毫秒之间
			if (interval > 5000)
				interval = 5000;
			else if (interval < 10)
				interval = 10;

			// 设置新的间隔时间
			this.interval = interval;
		}


		// ikcp_nodelay
		// 配置: https://github.com/skywind3000/kcp/blob/master/README.en.md#protocol-configuration
		//   nodelay : 是否启用无延迟模式，0 表示不启用；1 表示启用。
		//   interval ：协议内部工作间隔时间，单位为毫秒，例如 10 毫秒或 20 毫秒。
		//   resend ：快速重传模式，0 表示默认关闭，可以设置为 2（2 个 ACK 间隔将导致直接重传）。
		//   nc ：是否关闭流量控制，0 表示默认不关闭，1 表示关闭。
		// 普通模式: ikcp_nodelay(kcp, 0, 40, 0, 0);
		// 极速模式： ikcp_nodelay(kcp, 1, 10, 2, 1);

		/// <summary>
		/// 设置无延迟模式的参数。
		/// </summary>
		/// <param name="nodelay">是否启用无延迟模式，0 表示不启用，1 表示启用。</param>
		/// <param name="interval">协议内部工作间隔时间，单位为毫秒，默认值为 INTERVAL。</param>
		/// <param name="resend">快速重传模式，0 表示关闭，2 表示 2 个 ACK 间隔将导致直接重传，默认值为 0。</param>
		/// <param name="nocwnd">是否关闭流量控制，false 表示不关闭，true 表示关闭，默认值为 false。</param>
		public void SetNoDelay(uint nodelay, uint interval = INTERVAL, int resend = 0, bool nocwnd = false)
		{
			// 设置无延迟模式
			this.nodelay = nodelay;

			// 根据无延迟模式设置最小重传超时时间
			if (nodelay != 0)
			{
				rx_minrto = RTO_NDL; // 无延迟模式下的最小重传超时时间
			}
			else
			{
				rx_minrto = RTO_MIN; // 普通模式下的最小重传超时时间
			}

			// 将间隔时间限制在 10 到 5000 毫秒之间
			if (interval > 5000)
				interval = 5000;
			else if (interval < 10)
				interval = 10;
			this.interval = interval;

			// 设置快速重传模式
			if (resend >= 0)
			{
				fastresend = resend;
			}

			// 设置是否关闭流量控制
			this.nocwnd = nocwnd;
		}


		/// <summary>
		/// 设置发送和接收窗口大小。
		/// </summary>
		/// <param name="sendWindow">发送窗口大小。</param>
		/// <param name="receiveWindow">接收窗口大小。</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetWindowSize(uint sendWindow, uint receiveWindow)
		{
			// 如果发送窗口大小大于 0，则设置发送窗口大小
			if (sendWindow > 0)
			{
				snd_wnd = sendWindow;
			}

			// 如果接收窗口大小大于 0，则设置接收窗口大小
			if (receiveWindow > 0)
			{
				// 接收窗口大小必须大于等于最大分片大小
				rcv_wnd = Math.Max(receiveWindow, WND_RCV);
			}
		}

		/// <summary>
		/// 设置最小重传超时时间（RTO）。
		/// </summary>
		/// <param name="minrto">最小重传超时时间。</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMinrto(int minrto)
		{
			this.rx_minrto = minrto;
		}

		/// <summary>
		/// 设置字节数组池。
		/// </summary>
		/// <param name="arrayPool">字节数组池。</param>
		public void SetArrayPool(ArrayPool<byte> arrayPool)
		{
			this.kcpSegmentArrayPool = arrayPool;
		}

		/// <summary>
		/// 抛出 MTU 异常。
		/// </summary>
		/// <exception cref="ArgumentException">当 MTU 小于等于 50 或小于等于 OVERHEAD 时抛出。</exception>
		[DoesNotReturn]
		private void ThrowMTUException()
		{
			throw new ArgumentException("MTU 必须大于 50 且大于 OVERHEAD");
		}

		/// <summary>
		/// 抛出分片数量异常。
		/// </summary>
		/// <param name="len">发送数据的长度。</param>
		/// <param name="count">所需的分片数量。</param>
		/// <exception cref="Exception">当所需分片数量超过 FRG_MAX 时抛出。</exception>
		[DoesNotReturn]
		private void ThrowFrgCountException(int len, int count)
		{
			throw new Exception($"发送长度为 {len} 的数据需要 {count} 个分片，但 KCP 只能处理最多 {FRG_MAX} 个分片。");
		}

	}
}
