using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Management;
using System.IO;
using System.Text;
using System.Collections;

namespace MCSkin3D.ExceptionHandler
{
	internal class NonDisposableBinaryWriter : BinaryWriter
	{
		public NonDisposableBinaryWriter(Stream s) :
			base(s)
		{
		}

		protected override void Dispose(bool disposing)
		{
		}
	}

	public struct StackFrame
	{
		public string Method { get; set; }
		public string MethodType { get; set; }
		public string FileName { get; set; }
		public int FileLine { get; set; }
		public int FileColumn { get; set; }

		public StackFrame(System.Diagnostics.StackFrame frame) :
			this()
		{
			Method = frame.GetMethod().ToString();
			MethodType = frame.GetMethod().DeclaringType.ToString();

			FileName = frame.GetFileName();
			FileLine = frame.GetFileLineNumber();
			FileColumn = frame.GetFileColumnNumber();
		}
	}

	public struct ExceptionData
	{
		public string Type { get; set; }
		public string Message { get; set; }
		public List<StackFrame> Frames { get; private set; }

		public ExceptionData(Exception exception) :
			this()
		{
			Type = exception.GetType().FullName;
			Message = exception.Message;
			Frames = new List<StackFrame>();

			var trace = new System.Diagnostics.StackTrace(exception, true);

			if (trace.FrameCount != 0)
				foreach (var frame in trace.GetFrames())
					Frames.Add(new StackFrame(frame));
		}
	}

	public class ErrorReport
	{
		public static readonly uint MagicHeader = 0xDEADF00D;
		public static readonly uint VersionHeader = 1;

		public string Name { get; set; }
		public string Email { get; set; }
		public string Description { get; set; }

		public List<ExceptionData> Data { get; set; }

		public Dictionary<string, string> SoftwareData { get; set; }
		public Dictionary<string, string> OpenGLData { get; set; }

		public Dictionary<string, string> HardwareData { get; set; }

		public Dictionary<string, string> UserData { get; set; }

		static void WriteNullTerminatedString(BinaryWriter writer, string s)
		{
			foreach (var c in s)
				writer.Write(c);

			writer.Write('\0');
		}

		static string ReadNullTerminatedString(BinaryReader reader)
		{
			string s = "";
			char c;

			while ((c = reader.ReadChar()) != '\0')
				s += c;

			return s;
		}

		public void Write(Stream writer)
		{
			if (!writer.CanWrite)
				throw new InvalidOperationException();

			using (var bw = new NonDisposableBinaryWriter(writer))
			{
				bw.Write(MagicHeader);
				bw.Write(VersionHeader);

				WriteNullTerminatedString(bw, Name);
				WriteNullTerminatedString(bw, Email);
				WriteNullTerminatedString(bw, Description);

				bw.Write(Data.Count);

				foreach (var ex in Data)
				{
					WriteNullTerminatedString(bw, ex.Type);
					WriteNullTerminatedString(bw, ex.Message);

					bw.Write((byte)ex.Frames.Count);

					foreach (var frame in ex.Frames)
					{
						WriteNullTerminatedString(bw, frame.Method);
						WriteNullTerminatedString(bw, frame.MethodType);

						bool hasData = !string.IsNullOrEmpty(frame.FileName);

						bw.Write(hasData);

						if (hasData)
						{
							WriteNullTerminatedString(bw, frame.FileName);
							bw.Write(frame.FileLine);
							bw.Write(frame.FileColumn);
						}
					}
				}

				bw.Write(SoftwareData.Count);

				foreach (var data in SoftwareData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(OpenGLData.Count);

				foreach (var data in OpenGLData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(HardwareData.Count);

				foreach (var data in HardwareData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(UserData.Count);

				foreach (var data in UserData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}
			}
		}

		public static ErrorReport FromStream(Stream reader)
		{
			if (!reader.CanRead)
				throw new InvalidOperationException();

			using (var br = new BinaryReader(reader))
			{
				ErrorReport report = new ErrorReport();

				report.SoftwareData = new Dictionary<string, string>();
				report.OpenGLData = new Dictionary<string, string>();
				report.HardwareData = new Dictionary<string, string>();
				report.UserData = new Dictionary<string, string>();

				var magic = br.ReadUInt32();
				var version = br.ReadUInt32();

				if (magic != MagicHeader)
					throw new FileLoadException();

				if (version != VersionHeader)
					throw new Exception();

				report.Name = ReadNullTerminatedString(br);
				report.Email = ReadNullTerminatedString(br);
				report.Description = ReadNullTerminatedString(br);

				int num = br.ReadInt32();

				while (--num >= 0)
				{
					ExceptionData data = new ExceptionData();
					
					data.Type = ReadNullTerminatedString(br);
					data.Message = ReadNullTerminatedString(br);

					short numFrames = br.ReadByte();

					while (--numFrames >= 0)
					{
						StackFrame frame = new StackFrame();

						frame.Method = ReadNullTerminatedString(br);
						frame.MethodType = ReadNullTerminatedString(br);

						bool hasData = br.ReadBoolean();

						if (hasData)
						{
							frame.FileName = ReadNullTerminatedString(br);
							frame.FileLine = br.ReadInt32();
							frame.FileColumn = br.ReadInt32();
						}

						data.Frames.Add(frame);
					}

					report.Data.Add(data);
				}

				num = br.ReadInt32();

				while (--num >= 0)
				{
					string key = ReadNullTerminatedString(br);
					string value = ReadNullTerminatedString(br);

					report.SoftwareData.Add(key, value);
				}

				num = br.ReadInt32();

				while (--num >= 0)
				{
					string key = ReadNullTerminatedString(br);
					string value = ReadNullTerminatedString(br);

					report.OpenGLData.Add(key, value);
				}

				num = br.ReadInt32();

				while (--num >= 0)
				{
					string key = ReadNullTerminatedString(br);
					string value = ReadNullTerminatedString(br);

					report.HardwareData.Add(key, value);
				}

				num = br.ReadInt32();

				while (--num >= 0)
				{
					string key = ReadNullTerminatedString(br);
					string value = ReadNullTerminatedString(br);

					report.UserData.Add(key, value);
				}

				return report;
			}
		}

		public override string ToString()
		{
			StringWriter writer = new StringWriter();

			writer.WriteLine("User Data");
			writer.WriteLine("---------------------------------");
			writer.WriteLine("Name: " + (string.IsNullOrEmpty(Name) ? "Anonymous" : Name));
			writer.WriteLine("Email: " + (string.IsNullOrEmpty(Email) ? "Anonymous" : Email));
			writer.WriteLine("Description: " + Description);
			writer.WriteLine();

			writer.WriteLine("Exception Data");
			writer.WriteLine("---------------------------------");

			foreach (var e in Data)
			{
				writer.WriteLine(e.Type);
				writer.WriteLine(e.Message);

				if (e.Frames.Count == 0)
					writer.WriteLine("No Stack Trace Available");
				else
				{
					foreach (var f in e.Frames)
					{
						writer.WriteLine(" " + f.MethodType + " :: " + f.Method);

						if (!string.IsNullOrEmpty(f.FileName))
							writer.WriteLine("  - " + f.FileName + " " + f.FileLine + ":" + f.FileColumn);
					}
				}
			}
			
			return writer.ToString();
		}
	}

	public static class ErrorReportPackets
	{
		public const byte ProtocolVersion = 2;

		public const byte ClientToServer_SendRequest = 0;
		public const byte ClientToServer_Data = 1;

		public const byte ServerToClient_GoAhead = 0;
		public const byte ServerToClient_Maintenence = 1;
		public const byte ServerToClient_GotIt = 2;
	}
}