using System;
namespace MCSkin3D.ExceptionHandler
{
	public struct ErrorReport
	{
		public string Exception { get; set; }
		public string SoftwareInfo { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string HardwareInfo { get; set; }
		public string ExtraInfo { get; set; }

		public string ToString(DateTime received)
		{
			return "Received Report @ " + received.ToString() + "\r\n\r\n" +
					"Software: " + SoftwareInfo + "\r\n\r\n" +
					"Name: " + Name + "\r\n" +
					"Email: " + Email + "\r\n" +
					"Hardware: " + HardwareInfo + "\r\n" +
					"Extra: " + ExtraInfo + "\r\n" + "\r\n" +
					Exception;
		}

		public override string ToString()
		{
			return ToString(DateTime.Now);
		}
	}

	public static class ErrorReportPackets
	{
		public const byte ProtocolVersion = 1;

		public const byte ClientToServer_SendRequest = 0;
		public const byte ClientToServer_Data = 1;

		public const byte ServerToClient_GoAhead = 0;
		public const byte ServerToClient_Maintenence = 1;
		public const byte ServerToClient_GotIt = 2;
	}

	public enum ErrorReportContents
	{
		Name = 1,
		Email = 2,
		Hardware = 4,
		Extra = 8
	}
}