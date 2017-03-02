using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class allows to retrieve information about existing kernel objects (respectively their handles)
	/// </summary>
	public class KernelObject
	{

		/// <summary>
		/// Enumerates all kernel objects.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// For an unknown reason NtQuerySystemInformation returns a lot of invalid handles (actually all multiples of 4 to the last valid handle)
		/// It also returns handles for processes that the current process doesn't have the privileges to duplicate the handle into the current process.
		/// The SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX struct doesn't seem to indicate, if a handle is invalid, but there are some strong indications:
		/// GetObjectNameFromHandle() fails or returns an empty string or a string that doesn't start with '\' 
		/// In this case SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX.ObjectTypeIndex often is an arbitrary number that does not match the 
		/// type that GetHandleType will return.
		/// </remarks>
		public static IEnumerable<SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX> EnumerateAllObjects()
		{
			NT_STATUS ret;
			int length = 0x10000;
			// Loop, probing for required memory.
			do
			{
				IntPtr ptr = IntPtr.Zero;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try { }
					finally
					{
						// CER guarantees that the address of the allocated memory is actually assigned to ptr if an asynchronous exception occurs.
						ptr = Marshal.AllocHGlobal(length);
					}
					int returnLength;
					ret = NativeMethods.NtQuerySystemInformation(SYSTEM_INFORMATION_CLASS.SystemExtendedHandleInformation, ptr, length, out returnLength);
					if (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
					{
						// Round required memory up to the nearest 64KB boundary.
						length = ((returnLength + 0xffff) & ~0xffff);
					}
					else if (ret == NT_STATUS.STATUS_SUCCESS)
					{
						int handleCount = Marshal.ReadInt32(ptr); // NtQuerySystemInformation returns SYSTEM_HANDLE_INFORMATION which only consists of ULONG NumberOfHandles and an array of SYSTEM_HANDLE_TABLE_ENTRY_INFO
						int infoSize = Marshal.SizeOf(typeof(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX));
						int currentOffset = Marshal.SizeOf<int>() * (IntPtr.Size == 4 ? 2 : 4); // the int32 we just read and one(32bit) or three(64bit) reserved uint for _ex version (non-_ex version, does not have any reserved uints)
						for (int i = 0; i < handleCount; i++)
						{
							SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX handleEntry = Marshal.PtrToStructure<SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX>(IntPtr.Add(ptr, currentOffset));
							yield return handleEntry;
							currentOffset += infoSize;
						}
					}
				}
				finally
				{
					// CER guarantees that the allocated memory is freed, if an asynchronous exception occurs. 
					Marshal.FreeHGlobal(ptr);
				}
			}
			while (ret == NT_STATUS.STATUS_INFO_LENGTH_MISMATCH);
		}


		/// <summary>
		/// Enumerates all kernel object type-infos available in the operating system.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NtStatusException">NtQueryObject failed</exception>
		public static IEnumerable<ObjectTypeInfo> EnumerateAllObjectTypes()
		{
			int nLength = 0x1000;
			IntPtr ipBufferObjectType = IntPtr.Zero;
			ipBufferObjectType = Marshal.AllocHGlobal(nLength);
			while (true)
			{
				NT_STATUS res1 = NativeMethods.NtQueryObject(IntPtr.Zero, OBJECT_INFORMATION_CLASS.ObjectAllTypesInformation, ipBufferObjectType, nLength, out nLength);
				if (res1 == NT_STATUS.STATUS_SUCCESS)
				{
					break;
				}
				if (res1 != NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
				{
					throw new NtStatusException("NtQueryObject failed", res1);
				}
				Marshal.FreeHGlobal(ipBufferObjectType);
				ipBufferObjectType = Marshal.AllocHGlobal(nLength);
			}

			int typeInfoCount = Marshal.ReadInt32(ipBufferObjectType); // actually uint! C++: ULONG NumberOfTypes;
			IntPtr ipTypeInfo = IntPtr.Add(ipBufferObjectType, IntPtr.Size); // the int that we just read + padding (only in 64bit!)

			for (int nIndex = 0; nIndex < typeInfoCount; nIndex++)
			{
				OBJECT_TYPE_INFORMATION otiTemp = Marshal.PtrToStructure<OBJECT_TYPE_INFORMATION>(ipTypeInfo);
				string strType = Helpers.MarshalUnicodeString(otiTemp.TypeName);
				yield return new ObjectTypeInfo(otiTemp, strType);
				int currentSize = Marshal.SizeOf<OBJECT_TYPE_INFORMATION>() + otiTemp.TypeName.MaximumLength;
				int offsetToNext = Helpers.RoundUp(currentSize, IntPtr.Size); // padding depends on 32/64bit
				ipTypeInfo = IntPtr.Add(ipTypeInfo, offsetToNext);
			}
		}

		/// <summary>
		/// Gets the kernel object type-names array. Use SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX.ObjectTypeIndex as index for the returned array to retrieve the type-name.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Object-type "Directory" does not mean a file-system folder! It refers to a Directory for Kernel-Objects.
		/// </remarks>
		public static string[] GetObjectTypeNames()
		{
			var objectTypes = KernelObject.EnumerateAllObjectTypes();
			string[] objectTypesArr = Enumerable.Repeat("<unknown>", 2) // not sure why we need this offset - was determined through experiments
										.Concat(objectTypes.Select(item => item.TypeName)).ToArray();
			return objectTypesArr;
		}
	}
}