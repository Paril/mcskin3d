using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MCSkin3D.ExceptionHandler
{
	public class SendEventArgs : EventArgs
	{
		public SendEventArgs(string status, bool failed)
		{
			StatusString = status;
			Failed = failed;
		}

		public string StatusString { get; set; }
		public bool Failed { get; set; }
	}

	public class Client
	{
		private Thread _thread;
		private WaitHandle wh;

		public Client(ErrorReport report, string host, int port)
		{
			Report = report;
			EndPoint = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
		}

		public ErrorReport Report { get; set; }
		public IPEndPoint EndPoint { get; set; }
		public bool Running { get; private set; }

		public event EventHandler<SendEventArgs> SendFinished;

		private void RunSendThread()
		{
			try
			{
				var client = new TcpClient();
				IAsyncResult ar = client.BeginConnect(EndPoint.Address, EndPoint.Port, null, null);
				wh = ar.AsyncWaitHandle;
				try
				{
					if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5), false))
					{
						client.Close();
						throw new TimeoutException();
					}

					client.EndConnect(ar);
				}
				finally
				{
					if (!wh.SafeWaitHandle.IsClosed)
						wh.Close();
				}

				NetworkStream stream = client.GetStream();

				var writer = new BinaryWriter(stream);
				var reader = new BinaryReader(stream);

				writer.Write(ErrorReportPackets.ClientToServer_SendRequest);
				writer.Write(ErrorReportPackets.ProtocolVersion);

				while (true)
				{
					if (client.Available != 0)
					{
						byte packet = reader.ReadByte();

						switch (packet)
						{
							case ErrorReportPackets.ServerToClient_GoAhead:
							{
								writer.Write(ErrorReportPackets.ClientToServer_Data);

								byte bits = 0;

/*								if (!string.IsNullOrEmpty(Report.Name))
									bits |= (byte) ErrorReportContents.Name;
								if (!string.IsNullOrEmpty(Report.Email))
									bits |= (byte) ErrorReportContents.Email;
								if (!string.IsNullOrEmpty(Report.HardwareInfo))
									bits |= (byte) ErrorReportContents.Hardware;
								if (!string.IsNullOrEmpty(Report.ExtraInfo))
									bits |= (byte) ErrorReportContents.Extra;

								writer.Write(bits);

								writer.Write(Report.Exception);
								writer.Write(Report.SoftwareInfo);

								if ((bits & (byte) ErrorReportContents.Name) != 0)
									writer.Write(Report.Name);
								if ((bits & (byte) ErrorReportContents.Email) != 0)
									writer.Write(Report.Email);
								if ((bits & (byte) ErrorReportContents.Hardware) != 0)
									writer.Write(Report.HardwareInfo);
								if ((bits & (byte) ErrorReportContents.Extra) != 0)
									writer.Write(Report.ExtraInfo);*/
							}
								break;
							case ErrorReportPackets.ServerToClient_Maintenence:
							{
								string maintenenceString = reader.ReadString();

								Finished();
								SendFinished(this, new SendEventArgs(maintenenceString, true));
							}
								break;
							case ErrorReportPackets.ServerToClient_GotIt:
							{
								Finished();
								SendFinished(this, new SendEventArgs(null, false));
							}
								break;
						}
					}

					if (Running == false)
						break;

					Thread.Sleep(100);
				}

				writer.Close();
				reader.Close();

				stream.Close();
				client.Close();
			}
			catch (Exception ex)
			{
				Finished();
				SendFinished(this, new SendEventArgs(ex.Message, true));
			}
		}

		public void SendToServer()
		{
			Running = true;
			_thread = new Thread(RunSendThread);
			_thread.Start();
		}

		private void Finished()
		{
			Running = false;
		}

		public void Abort()
		{
			wh.Close();
			_thread.Abort();
			Finished();
		}
	}
}