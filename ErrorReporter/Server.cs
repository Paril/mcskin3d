using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using MCSkin3D.ExceptionHandler;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

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
					try
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
													client.Writer.Write("Currently testing a new version of the exception handler - as a result, 1.4 reports are deprecated. Thanks, though!");
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
												ErrorReport report;

												int length = client.Reader.ReadInt32();
												byte[] bytes = client.Reader.ReadBytes(length);

												using (MemoryStream ms = new MemoryStream(bytes),
													newStream = new MemoryStream())
												{
													using (var inflate = new InflaterInputStream(ms))
													{
														int len;
														byte[] buff = new byte[length];

														while ((len = inflate.Read(buff, 0, length)) > 0)
															newStream.Write(buff, 0, len);

														newStream.Position = 0;
														report = ErrorReport.FromStream(newStream);
													}
												}

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
					}
					catch
					{
					}

					Thread.Sleep(100);
				}

				_listener.Stop();
			}
			catch
			{
				Finished();
			}
		}

		void ReadReportFrom(ErrorReport report, MemoryStream newStream)
		{
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
