using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinKernelObjectsDotNet;

namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class provides easy to use methods for the most common use-cases of the WinKernelObjectsDotNet library.
	/// </summary>
	public sealed class KernelObjectsFacade
	{

		/// <summary>
		/// Gets the processes that are using the specified file or serial-port.
		/// </summary>
		/// <param name="path">The path (e.g. 'c:\somefolder\somefile.txt' or 'COM1').</param>
		/// <returns></returns>
		public Process[] GetProcessesUsingPath(string path)
		{
			return GetProcessIdUsingPath(path).Select(GetProcessForId).Where(item => item != null).ToArray();
		}

		/// <summary>
		/// Gets the process-Ids of the processes that are using the specified file or serial-port.
		/// </summary>
		/// <param name="path">The path (e.g. 'c:\somefolder\somefile.txt' or 'COM1').</param>
		/// <returns></returns>
		public IEnumerable<int> GetProcessIdUsingPath(string path)
		{
			ObjectTypeFilter fileTypeFilter = new ObjectTypeFilter("File");

			IEnumerable<int> processIds = KernelObject.EnumerateAllObjects()
							.Where(fileTypeFilter.IsMatch)
							.Select(info => Tuple.Create(info.UniqueProcessId, GetDevicePathFromInfo(info)))
							.Where(item => !string.IsNullOrEmpty(item.Item2))
							.Select(item => Tuple.Create(item.Item1, DevicePathConverter.ConvertDevicePathToDosPath(item.Item2)))
							.Where(item => item.Item2 == path)
							.Select(item => item.Item1);

			return processIds;
		}

		private List<int> _pidsToIgnore = new List<int>();

		private static Process GetProcessForId(int pid)
		{
			try
			{
				return Process.GetProcessById(pid);
			}
			catch
			{
				return null;
			}
		}

		private string GetDevicePathFromInfo(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX info)
		{
			if (_pidsToIgnore.Contains(info.UniqueProcessId))
			{
				return null;
			}

			using (DuplicatedObjectHandle handle = new DuplicatedObjectHandle(info.HandleValue, info.UniqueProcessId))
			{
				if (handle.ErrorMessage != null)
				{
					if (handle.ErrorMessage == "OpenProcess")
					{
						_pidsToIgnore.Add(info.UniqueProcessId);
					}
					return null;
				}
				try
				{
					return handle.GetObjectNameFromHandle();
				}
				catch (NtStatusException)
				{
					return null;
				}
			}
		}

	}
}