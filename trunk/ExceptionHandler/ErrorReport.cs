using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Specialized;
using OpenTK.Graphics.OpenGL;
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
		public MethodBase Method { get; set; }
		public string FileName { get; set; }
		public int FileLine { get; set; }
		public int FileColumn { get; set; }

		public StackFrame(System.Diagnostics.StackFrame frame) :
			this()
		{
			Method = frame.GetMethod();

			FileName = frame.GetFileName();
			FileLine = frame.GetFileLineNumber();
			FileColumn = frame.GetFileColumnNumber();
		}
	}

	public struct ExceptionData
	{
		public Type Type { get; set; }
		public string Message { get; set; }
		public List<StackFrame> Frames { get; private set; }

		public ExceptionData(Exception exception) :
			this()
		{
			Type = exception.GetType();
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

		private ErrorReport() { }

		public string Name { get; set; }
		public string Email { get; set; }
		public string Description { get; set; }

		public List<ExceptionData> Data { get; private set; }

		public StringDictionary SoftwareData { get; private set; }
		public StringDictionary OpenGLData { get; private set; }

		public StringDictionary HardwareData { get; private set; }

		public StringDictionary UserData { get; private set; }

		#region Is64BitOperatingSystem (IsWow64Process)

		/// <summary>
		/// The function determines whether the current operating system is a 
		/// 64-bit operating system.
		/// </summary>
		/// <returns>
		/// The function returns true if the operating system is 64-bit; 
		/// otherwise, it returns false.
		/// </returns>
		public static bool Is64BitOperatingSystem()
		{
			if (IntPtr.Size == 8) // 64-bit programs run only on Win64
				return true;
			else // 32-bit programs run on both 32-bit and 64-bit Windows
			{
				// Detect whether the current process is a 32-bit process 
				// running on a 64-bit system.
				bool flag;
				return ((DoesWin32MethodExist("kernel32.dll", "IsWow64Process") &&
						 IsWow64Process(GetCurrentProcess(), out flag)) && flag);
			}
		}

		/// <summary>
		/// The function determins whether a method exists in the export 
		/// table of a certain module.
		/// </summary>
		/// <param name="moduleName">The name of the module</param>
		/// <param name="methodName">The name of the method</param>
		/// <returns>
		/// The function returns true if the method specified by methodName 
		/// exists in the export table of the module specified by moduleName.
		/// </returns>
		private static bool DoesWin32MethodExist(string moduleName, string methodName)
		{
			IntPtr moduleHandle = GetModuleHandle(moduleName);
			if (moduleHandle == IntPtr.Zero)
				return false;
			return (GetProcAddress(moduleHandle, methodName) != IntPtr.Zero);
		}

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string moduleName);

		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule,
													[MarshalAs(UnmanagedType.LPStr)] string procName);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWow64Process(IntPtr hProcess, out bool wow64Process);

		#endregion

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
					WriteNullTerminatedString(bw, ex.Type.FullName);
					WriteNullTerminatedString(bw, ex.Message);

					bw.Write((ushort)ex.Frames.Count);

					foreach (var frame in ex.Frames)
					{
						WriteNullTerminatedString(bw, frame.Method.DeclaringType.ToString());
						WriteNullTerminatedString(bw, frame.Method.ToString());

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

				foreach (DictionaryEntry data in SoftwareData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(OpenGLData.Count);

				foreach (DictionaryEntry data in OpenGLData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(HardwareData.Count);

				foreach (DictionaryEntry data in HardwareData)
				{
					WriteNullTerminatedString(bw, data.Key.ToString());
					WriteNullTerminatedString(bw, data.Value.ToString());
				}

				bw.Write(UserData.Count);

				foreach (DictionaryEntry data in UserData)
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
				
				return report;
			}
		}

		public static ErrorReport Construct(string name, string email, string description, Exception topException, bool hardware)
		{
			ErrorReport report = new ErrorReport();

			report.Name = name;
			report.Email = email;
			report.Description = description;

			report.Data = new List<ExceptionData>();

			report.UserData = new StringDictionary();

			for (var ex = topException; ex != null; ex = ex.InnerException)
				report.Data.Add(new ExceptionData(ex));

			// build GL info
			try
			{
				report.OpenGLData = new StringDictionary();
				report.OpenGLData.Add("vendor", Editor.GLVendor);
				report.OpenGLData.Add("version", Editor.GLVersion);
				report.OpenGLData.Add("renderer", Editor.GLRenderer);
				report.OpenGLData.Add("extensions", Editor.GLExtensions);
			}
			catch
			{
			}

			// build software info
			report.SoftwareData = new StringDictionary();

			report.SoftwareData.Add("osversion", Environment.OSVersion.ToString());
			report.SoftwareData.Add("is64bit", (Is64BitOperatingSystem() ? "true" : "false"));
			report.SoftwareData.Add("frameworkversion", Environment.Version.ToString());
			report.SoftwareData.Add("softwareversion", Program.Version.ToString());

			// build hardware info
			report.HardwareData = new StringDictionary();

			if (hardware)
			{
				var searcher =
					new ManagementObjectSearcher("Select * from Win32_VideoController");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (PropertyData prop in video.Properties)
					{
						if (prop.Value != null &&
							prop.Name != "SystemName")
							report.HardwareData.Add("video|" + prop.Name, prop.Value.ToString());
					}
				}

				searcher =
					new ManagementObjectSearcher("Select * from Win32_PhysicalMemoryArray");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (PropertyData prop in video.Properties)
					{
						if (prop.Value != null &&
							prop.Name != "SystemName")
							report.HardwareData.Add("memory|" + prop.Name, prop.Value.ToString());
					}
				}

				searcher =
					new ManagementObjectSearcher("Select * from Win32_Processor");

				foreach (ManagementObject video in searcher.Get())
				{
					foreach (PropertyData prop in video.Properties)
					{
						if (prop.Value != null &&
							prop.Name != "SystemName")
							report.HardwareData.Add("processor|" + prop.Name, prop.Value.ToString());
					}
				}
			}

			return report;
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