using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Sockets;

public static class SocketTaskExtensions
{
	private class ReceiveAsyncHelper
	{
		private readonly CancellationToken _ct;

		private readonly ManualResetEvent _mre;
		
		public ReceiveAsyncHelper(CancellationToken ct)
		{
			_ct = ct;
			_ct.Register(RaiseCancelEvent);
			_mre = new ManualResetEvent(false);
		}

		private void RaiseCancelEvent()
		{
			_mre.Set();
			// Console.WriteLine(_ct.IsCancellationRequested == false
			// 	? "RaiseCancelEvent: CancellationRequested == false"
			// 	: "RaiseCancelEvent: CancellationRequested == true");
		}
		
		public int CancelExecute()
		{
			//Console.WriteLine("CancelExecute:Start");
			_mre.WaitOne();
			//Console.WriteLine("CancelExecute:End");
			return 0;
		}

		public void ReceiveContinueWith(Task<int> task)
		{
			_mre.Set(); // Освободить вспомогательную задачу-отмены
			// Console.WriteLine(_ct.IsCancellationRequested == false
			//  	? "ReceiveContinueWith: false"
			//  	: "ReceiveContinueWith: true");
		}
		
		public void CancelContinueWith(Task<int> task)
		{
			// Console.WriteLine(_ct.IsCancellationRequested == false
			// 	? "CancelContinueWith: false"
			// 	: "CancelContinueWith: true");
		}
	}
	


	public static async ValueTask<int> ReceiveAsync(this Socket socket, Memory<byte> buffer, SocketFlags socketFlags,
		CancellationToken cancellationToken = default(CancellationToken))
	{
		ReceiveAsyncHelper helper = new ReceiveAsyncHelper(cancellationToken);
		
		Task<int> taskReceive = socket.ReceiveAsync(buffer, socketFlags, fromNetworkStream: false, cancellationToken);
		Task<int> taskChanel = TaskEx.Run(helper.CancelExecute);
		
		taskReceive.ContinueWith(helper.ReceiveContinueWith);
		taskChanel.ContinueWith(helper.CancelContinueWith);
		
		
		Task<int> taskResult = await TaskEx.WhenAny(taskReceive, taskChanel);

		return await taskResult;
	}
	
	public static ValueTask<int> SendAsync(this Socket socket, ReadOnlyMemory<byte> buffer, SocketFlags socketFlags, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (MemoryMarshal.TryGetArray(buffer, out var bufferArray))
		{
			TaskCompletionSource<int> tcs = new TaskCompletionSource<int>(socket);
			socket.BeginSend(bufferArray.Array, bufferArray.Offset, bufferArray.Count, socketFlags, delegate(IAsyncResult iar)
			{
				TaskCompletionSource<int> taskCompletionSource = (TaskCompletionSource<int>)iar.AsyncState;
				try
				{
					taskCompletionSource.TrySetResult(((Socket)taskCompletionSource.Task.AsyncState).EndSend(iar));
				}
				catch (Exception exception2)
				{
					taskCompletionSource.TrySetException(exception2);
				}
			}, tcs);
			return new ValueTask<int>(tcs.Task);
		}
		byte[] poolArray = ArrayPool<byte>.Shared.Rent(buffer.Length);
		buffer.Span.CopyTo(poolArray);
		TaskCompletionSource<int> tcs2 = new TaskCompletionSource<int>(socket);
		socket.BeginSend(poolArray, 0, buffer.Length, socketFlags, delegate(IAsyncResult iar)
		{
			Tuple<TaskCompletionSource<int>, byte[]> tuple = (Tuple<TaskCompletionSource<int>, byte[]>)iar.AsyncState;
			try
			{
				tuple.Item1.TrySetResult(((Socket)tuple.Item1.Task.AsyncState).EndSend(iar));
			}
			catch (Exception exception)
			{
				tuple.Item1.TrySetException(exception);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tuple.Item2);
			}
		}, Tuple.Create(tcs2, poolArray));
		return new ValueTask<int>(tcs2.Task);
	}

	public static Task ConnectAsync(this Socket socket, EndPoint remoteEp)
	{
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>(socket);
		socket.BeginConnect(remoteEp, delegate(IAsyncResult iar)
		{
			TaskCompletionSource<bool> taskCompletionSource = (TaskCompletionSource<bool>)iar.AsyncState;
			try
			{
				((Socket)taskCompletionSource.Task.AsyncState).EndConnect(iar);
				taskCompletionSource.TrySetResult(result: true);
			}
			catch (Exception exception)
			{
				taskCompletionSource.TrySetException(exception);
			}
		}, tcs);
		return tcs.Task;
	}

	public static Task<int> SendAsync(this Socket socket, IList<ArraySegment<byte>> buffers, SocketFlags socketFlags)
	{
		throw new NotImplementedException("SocketTaskExtensions.SendAsync # args # 2");
	}

	public static Task<int> SendToAsync(this Socket socket, ArraySegment<byte> buffer, SocketFlags socketFlags, EndPoint remoteEP)
	{
		throw new NotImplementedException("SocketTaskExtensions.SendToAsync # args # 3");
	}

	public static Task<Socket> AcceptAsync(this Socket socket)
	{
		return AcceptAsync(socket, null);
	}
	
	private static Task<int> ReceiveAsync(this Socket socket, Memory<byte> buffer, SocketFlags socketFlags,
		bool fromNetworkStream, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return TaskExEx.FromCanceled<int>(cancellationToken);
		}

		if (MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)buffer, out ArraySegment<byte> bufferArray))
		{
			TaskCompletionSource<int> tcs2 = new TaskCompletionSource<int>(socket);
			if (bufferArray.Array != null)
			{
				socket.BeginReceive(bufferArray.Array, bufferArray.Offset, bufferArray.Count, socketFlags,
					delegate(IAsyncResult iar)
					{
						TaskCompletionSource<int> taskCompletionSource = (TaskCompletionSource<int>)iar.AsyncState;
						try
						{
							taskCompletionSource.TrySetResult(
								((Socket)taskCompletionSource.Task.AsyncState).EndReceive(iar));
						}
						catch (Exception exception2)
						{
							taskCompletionSource.TrySetException(exception2);
						}
					}, tcs2);
			}
			return tcs2.Task;
		}
		byte[] poolArray = ArrayPool<byte>.Shared.Rent(buffer.Length);
		TaskCompletionSource<int> tcs = new TaskCompletionSource<int>(socket);
		socket.BeginReceive(poolArray, 0, buffer.Length, socketFlags, delegate(IAsyncResult iar)
		{
			Tuple<TaskCompletionSource<int>, Memory<byte>, byte[]> tuple =
				(Tuple<TaskCompletionSource<int>, Memory<byte>, byte[]>)iar.AsyncState;
			try
			{
				int num = ((Socket)tuple.Item1.Task.AsyncState).EndReceive(iar);
				new ReadOnlyMemory<byte>(tuple.Item3, 0, num).Span.CopyTo(tuple.Item2.Span);
				tuple.Item1.TrySetResult(num);
			}
			catch (Exception exception)
			{
				tuple.Item1.TrySetException(exception);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(tuple.Item3);
			}
		}, Tuple.Create(tcs, buffer, poolArray));
		return tcs.Task;
	}

	private static Task<Socket> AcceptAsync(this Socket socket, Socket? acceptSocket)
	{
		TaskCompletionSource<Socket> tcs = new TaskCompletionSource<Socket>(socket);
		socket.BeginAccept(acceptSocket, 0, delegate(IAsyncResult iar)
		{
			TaskCompletionSource<Socket> taskCompletionSource = (TaskCompletionSource<Socket>)iar.AsyncState;
			try
			{
				taskCompletionSource.TrySetResult(((Socket)taskCompletionSource.Task.AsyncState).EndAccept(iar));
			}
			catch (Exception exception)
			{
				taskCompletionSource.TrySetException(exception);
			}
		}, tcs);
		return tcs.Task;
	}
}
