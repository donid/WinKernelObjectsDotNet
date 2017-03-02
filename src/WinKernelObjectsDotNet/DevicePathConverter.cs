using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class allows to convert a device path to a DOS-path.
	/// Also works for serial ports e.g. '\Device\Serial0' -> 'COM1'
	/// </summary>
	public class DevicePathConverter
	{

		private const int MAX_PATH = 260;
		private const string cNetworkDevicePrefix = @"\Device\LanmanRedirector\";
		private readonly static Lazy<IList<Tuple<string, string>>> _lazyDeviceMap = new Lazy<IList<Tuple<string, string>>>(BuildDeviceMap, true);

		/// <summary>
		/// Converts the device path to a DOS-path.
		/// </summary>
		/// <param name="devicePath">The device path (e.g. '\Device\HarddiskVolume2\Windows\System32').</param>
		/// <returns>The corresponding DOS-path (e.g. 'C:\Windows\System32').</returns>
		public static string ConvertDevicePathToDosPath(string devicePath)
		{
			IList<Tuple<string, string>> deviceMap = _lazyDeviceMap.Value;

			Tuple<string, string> foundItem = deviceMap.FirstOrDefault(item => IsMatch(item.Item1, devicePath));
			if (foundItem == null)
			{
				return null;
			}
			return string.Concat(foundItem.Item2, devicePath.Substring(foundItem.Item1.Length));
		}

		private static bool IsMatch(string devicePathStart, string fullDevicePath)
		{
			if (!fullDevicePath.StartsWith(devicePathStart, StringComparison.InvariantCulture))
			{
				return false;
			}
			if (devicePathStart.Length == fullDevicePath.Length)
			{
				return true;
			}
			return fullDevicePath[devicePathStart.Length] == '\\';
		}

		/// <summary>
		/// Builds the list of Tuples, where item1 is the device-path and item2 is the DOS-name.
		/// The list contains logical drives, a network device and serial ports.
		/// </summary>
		/// <returns></returns>
		private static IList<Tuple<string, string>> BuildDeviceMap()
		{
			IEnumerable<string> logicalDrives = Environment.GetLogicalDrives().Select(drive => drive.Substring(0, 2));
			var driveTuples = logicalDrives.Select(drive => Tuple.Create(NormalizeDeviceName(QueryDosDevice(drive)), drive));

			IEnumerable<string> portNames = System.IO.Ports.SerialPort.GetPortNames();
			var serialPortTuples = portNames.Select(port => Tuple.Create(QueryDosDevice(port), port));

			IList<Tuple<string, string>> result = driveTuples.Concat(serialPortTuples).ToList();
			var networtDevice = Tuple.Create(cNetworkDevicePrefix.Substring(0, cNetworkDevicePrefix.Length - 1), "\\");
			result.Add(networtDevice);
			return result;
		}

		/// <summary>
		/// Returns the windows device-path for the given MS-DOS drive letter.
		/// </summary>
		/// <param name="dosDevice">A drive letter (e.g. 'C:') or a serial-port (e.g. 'COM1').</param>
		/// <returns></returns>
		/// <exception cref="Win32Exception"></exception>
		private static string QueryDosDevice(string dosDevice)
		{
			StringBuilder targetPath = new StringBuilder(MAX_PATH);
			int queryResult = NativeMethods.QueryDosDevice(dosDevice, targetPath, MAX_PATH);
			if (queryResult == 0)
			{
				throw new Win32Exception();
			}
			return targetPath.ToString();
		}

		private static string NormalizeDeviceName(string deviceName)
		{
			if (deviceName.StartsWith(cNetworkDevicePrefix, StringComparison.InvariantCulture))
			{
				string shareName = deviceName.Substring(deviceName.IndexOf('\\', cNetworkDevicePrefix.Length) + 1);
				return string.Concat(cNetworkDevicePrefix, shareName);
			}
			return deviceName;
		}
	}
}