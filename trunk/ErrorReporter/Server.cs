using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using MCSkin3D.ExceptionHandler;

namespace ClientTest
{
	class ConnectedClient
	{
		public TcpClient Client;
		public DateTime LastReceived;
		public BinaryReader Reader;
		public BinaryWriter Writer;
	}
	
	public class ReceivedReportEventArgs : EventArgs
	{
		public ErrorReport Report { get; set; }

		public ReceivedReportEventArgs(ref ErrorReport report) :
			base()
		{
			Report = report;
		}
	}

	public class Server
	{
		public IPEndPoint EndPoint { get; set; }
		public bool Running { get; private set; }

		public Server(int port)
		{
			EndPoint = new IPEndPoint(IPAddress.Any, port);
		}

		public event EventHandler<ReceivedReportEventArgs> ReceivedReport;

		Thread _thread;
		TcpListener _listener;
		List<ConnectedClient> _clients = new List<ConnectedClient>();
		object _lockObject = new object();

		string _maintenenceString = null;
		public string MaintenenceString
		{
			get { return _maintenenceString; }
			set
			{
				lock (_lockObject)
					_maintenenceString = value;
			}
		}

		void RunServerThread()
		{
			try
			{
				_listener = new TcpListener(EndPoint);
				_listener.Start();

				while (true)
				{
					lock (_lockObject)
					{
						while (_listener.Pending())
						{
							ConnectedClient client = new ConnectedClient();
							client.Client = _listener.AcceptTcpClient();
							client.LastReceived = DateTime.Now;
							var stream = client.Client.GetStream();
							client.Reader = new BinaryReader(stream);
							client.Writer = new BinaryWriter(stream);
							_clients.Add(client);
						}

						List<ConnectedClient> removeClients = new List<ConnectedClient>();

						foreach (var client in _clients)
						{
							if ((DateTime.Now - client.LastReceived).TotalMinutes >= 1) // timeout
								removeClients.Add(client);
							else if (client.Client.Available != 0)
							{
								client.LastReceived = DateTime.Now;

								byte packet = client.Reader.ReadByte();

								switch (packet)
								{
								case ErrorReportPackets.ClientToServer_SendRequest:
									{
										byte protocol = client.Reader.ReadByte();

										if (protocol != ErrorReportPackets.ProtocolVersion)
										{
											removeClients.Add(client);

											client.Writer.Write(ErrorReportPackets.ServerToClient_Maintenence);
											client.Writer.Write("Your version of the ExceptionHandler protocol is older than the server! Try updating the program.");
										}
										else if (MaintenenceString != null)
										{
											removeClients.Add(client);

											client.Writer.Write(ErrorReportPackets.ServerToClient_Maintenence);
											client.Writer.Write(MaintenenceString);
										}
										else
											client.Writer.Write(ErrorReportPackets.ServerToClient_GoAhead);
									}
									break;
								case ErrorReportPackets.ClientToServer_Data:
									{
										byte bits = client.Reader.ReadByte();
										ErrorReport report = new ErrorReport();

										report.Exception = client.Reader.ReadString();
										report.SoftwareInfo = client.Reader.ReadString();

										if ((bits & (byte)ErrorReportContents.Name) != 0)
											report.Name = client.Reader.ReadString();
										if ((bits & (byte)ErrorReportContents.Email) != 0)
											report.Email = client.Reader.ReadString();
										if ((bits & (byte)ErrorReportContents.Hardware) != 0)
											report.HardwareInfo = client.Reader.ReadString();
										if ((bits & (byte)ErrorReportContents.Extra) != 0)
											report.ExtraInfo = client.Reader.ReadString();

										ReceivedReport(this, new ReceivedReportEventArgs(ref report));

										client.Writer.Write(ErrorReportPackets.ServerToClient_GotIt);
										removeClients.Add(client);
									}
									break;
								}
							}
						}

						foreach (var c in removeClients)
						{
							c.Reader.Close();
							c.Writer.Close();
							c.Client.Close();
							_clients.Remove(c);
						}

						if (Running == false)
							break;
					}

					Thread.Sleep(100);
				}

				_listener.Stop();
			}
			catch (Exception ex)
			{
				Finished();
			}
		}

		public void ListenForReports()
		{
			Running = true;
			_thread = new Thread(RunServerThread);
			_thread.Start();
		}

		void Finished()
		{
			Running = false;
		}

		public void Close()
		{
			Finished();
			_thread.Abort();
		}
	}
}
