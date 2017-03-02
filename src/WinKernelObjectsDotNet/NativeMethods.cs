using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace WinKernelObjectsDotNet
{
	internal static class NativeMethods
	{

		const uint HANDLE_FLAG_INHERIT = 0x00000001;
		const uint HANDLE_FLAG_PROTECT_FROM_CLOSE = 0x00000002;

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern bool GetHandleInformation(IntPtr hObject, out uint lpdwFlags);


		[DllImport("ntdll.dll")]
		internal static extern NT_STATUS NtQuerySystemInformation(
			[In] SYSTEM_INFORMATION_CLASS SystemInformationClass,
			[In] IntPtr SystemInformation,
			[In] int SystemInformationLength,
			[Out] out int ReturnLength);

		[DllImport("ntdll.dll")]
		internal static extern NT_STATUS NtQueryObject(
			[In] IntPtr Handle,
			[In] OBJECT_INFORMATION_CLASS ObjectInformationClass,
			[In] IntPtr ObjectInformation,
			[In] int ObjectInformationLength,
			[Out] out int ReturnLength);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern SafeProcessHandle OpenProcess(
			[In] ProcessAccessRights dwDesiredAccess,
			[In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
			[In] int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateHandle(
			[In] IntPtr hSourceProcessHandle,
			[In] IntPtr hSourceHandle,
			[In] IntPtr hTargetProcessHandle,
			[Out] out SafeObjectHandle lpTargetHandle,
			[In] int dwDesiredAccess,
			[In, MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
			[In] DuplicateHandleOptions dwOptions);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int GetProcessId([In] IntPtr Process);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle([In] IntPtr hObject);

		/// <summary>
		/// Retrieves information about MS-DOS device names. The function can obtain the current mapping for a particular MS-DOS device name.
		/// The function can also obtain a list of all existing MS-DOS device names.
		/// </summary>
		/// <param name="lpDeviceName">Name of the lp device.</param>
		/// <param name="lpTargetPath">The lp target path.</param>
		/// <param name="ucchMax">The ucch maximum.</param>
		/// <returns>
		/// If the function succeeds, the return value is the number of TCHARs stored into the buffer pointed to by lpTargetPath.
		/// If the function fails, the return value is zero. To get extended error information, call GetLastError.
		/// If the buffer is too small, the function fails and the last error code is ERROR_INSUFFICIENT_BUFFER.
		/// </returns>
		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int QueryDosDevice([In] string lpDeviceName, [Out] StringBuilder lpTargetPath, [In] int ucchMax);
	}


	//see  http://www.exploit-monday.com/2013/06/undocumented-ntquerysysteminformation.html
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct SYSTEM_HANDLE_TABLE_ENTRY_INFO // Size32bit=16(0x10) Size64bit=24(0x18)
	{
		private IntPtr _dummy; // Size=4bytes in 32 bit, 8bytes in 64bit! - in 64bit-mode the actual data seems to be preceded by 4 bytes that are always zero 
		public ushort UniqueProcessId
		{
			get { return (ushort)Marshal.ReadInt16(this, IntPtr.Size == 4 ? 0 : 4); }
		}
		public ushort CreatorBackTraceIndex
		{
			get { return (ushort)Marshal.ReadInt16(this, IntPtr.Size == 4 ? 2 : 6); }
		}

		public byte ObjectTypeIndex; // Size=1 ObjectTypeNumber
		public byte HandleAttributes; // Size=1 (SYSTEM_HANDLE_FLAGS)
		public ushort HandleValue; // Size=2
		public IntPtr Object; // Size=4bytes in 32 bit, 8bytes in 64bit!
		public UInt32 GrantedAccess; // Size=4 AccessMask
	}


	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX
	{
		public IntPtr Object;
		private IntPtr _uniqueProcessId; // Size=4bytes in 32 bit, 8bytes in 64bit!
		public int UniqueProcessId
		{
			get { return _uniqueProcessId.ToInt32(); }
		}
		public IntPtr HandleValue; // Size=4bytes in 32 bit, 8bytes in 64bit!
		public uint GrantedAccess;
		public ushort CreatorBackTraceIndex;
		public ushort ObjectTypeIndex;
		public uint HandleAttributes;
		public uint Reserved; // seems to be always zero
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct UNICODE_STRING
	{
		private IntPtr _dummy; // the two ushorts seem to be padded with 4 bytes in 64bit mode only

		/// <summary>
		/// The length, in bytes, of the string stored in Buffer. If the string is null-terminated, Length does not include the trailing null character.
		/// </summary>
		public ushort Length
		{
			get { return (ushort)Marshal.ReadInt16(this, 0); }
		}

		/// <summary>
		/// The length, in bytes, of Buffer.
		/// </summary>
		public ushort MaximumLength
		{
			get { return (ushort)Marshal.ReadInt16(this, 2); }
		}

		public IntPtr Buffer;
	}


	[StructLayout(LayoutKind.Sequential)]
	public struct GENERIC_MAPPING
	{
		public int GenericRead;
		public int GenericWrite;
		public int GenericExecute;
		public int GenericAll;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct OBJECT_NAME_INFORMATION
	{
		public UNICODE_STRING Name;
	}

	// according to microsoft: a UNICODE_STRING and 88(0x58)bytes of undocumented data
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct OBJECT_TYPE_INFORMATION
	{
		public UNICODE_STRING TypeName;
		public uint TotalNumberOfObjects;
		public uint TotalNumberOfHandles;
		public uint TotalPagedPoolUsage;
		public uint TotalNonPagedPoolUsage;
		public uint TotalNamePoolUsage;
		public uint TotalHandleTableUsage;
		public uint HighWaterNumberOfObjects;// PeakObjectCount;
		public uint HighWaterNumberOfHandles;// PeakHandleCount;
		public uint HighWaterPagedPoolUsage;
		public uint HighWaterNonPagedPoolUsage;
		public uint HighWaterNamePoolUsage;
		public uint HighWaterHandleTableUsage;
		public uint InvalidAttributes;
		public GENERIC_MAPPING GenericMapping;
		public uint ValidAccessMask;
		public byte SecurityRequired;//bool
		public byte MaintainHandleCount;//bool
		public ushort MaintainTypeList;//see https://github.com/adobe/chromium/blob/master/sandbox/src/nt_internals.h - in http://processhacker.sourceforge.net/doc/struct___o_b_j_e_c_t___t_y_p_e___i_n_f_o_r_m_a_t_i_o_n.html this was UCHAR TypeIndex and CHAR ReservedByte => so definitely 2 byte!
		public uint PoolType;
		public uint DefaultPagedPoolCharge;// PagedPoolUsage;
		public uint DefaultNonPagedPoolCharge;//NonPagedPoolUsage;
	}

	// winsdk: ntstatus.h
	public enum NT_STATUS
	{
		STATUS_SUCCESS = 0x00000000,
		STATUS_TIMEOUT = unchecked((int)0x00000102L),
		STATUS_BUFFER_OVERFLOW = unchecked((int)0x80000005L),
		STATUS_INFO_LENGTH_MISMATCH = unchecked((int)0xC0000004L),  // The specified information record length does not match the length required for the specified information class.
		STATUS_ACCESS_VIOLATION = unchecked((int)0xC0000005L),      // The instruction at 0x%p referenced memory at 0x%p. The memory could not be %s.
		STATUS_INVALID_HANDLE = unchecked((int)0xC0000008L),        // An invalid HANDLE was specified.
		STATUS_NO_MEMORY = unchecked((int)0xC0000017L),             // Not enough virtual memory or paging file quota is available to complete the specified operation.
		STATUS_ACCESS_DENIED = unchecked((int)0xC0000022L),         // A process has requested access to an object, but has not been granted those access rights.
		STATUS_OBJECT_PATH_INVALID = unchecked((int)0xC0000039L),   // Object Path Component was not a directory object.
		STATUS_NOT_SUPPORTED = unchecked((int)0xC00000BBL),         // The request is not supported.
		STATUS_KEY_DELETED = unchecked((int)0xC000017CL),           // Illegal operation attempted on a registry key which has been marked for deletion.
		STATUS_HIVE_UNLOADED = unchecked((int)0xC0000425L),         // Illegal operation attempted on a registry key which has already been unloaded.
	}

	internal enum SYSTEM_INFORMATION_CLASS
	{
		SystemBasicInformation = 0,
		SystemPerformanceInformation = 2,
		SystemTimeOfDayInformation = 3,
		SystemProcessInformation = 5,
		SystemProcessorPerformanceInformation = 8,
		SystemHandleInformation = 16,
		SystemInterruptInformation = 23,
		SystemExceptionInformation = 33,
		SystemRegistryQuotaInformation = 37,
		SystemLookasideInformation = 45,
		SystemExtendedHandleInformation = 64,
	}

	internal enum OBJECT_INFORMATION_CLASS
	{
		ObjectBasicInformation = 0,
		ObjectNameInformation = 1,
		ObjectTypeInformation = 2,
		ObjectAllTypesInformation = 3,
		ObjectHandleInformation = 4
	}

	[Flags]
	internal enum ProcessAccessRights
	{
		PROCESS_DUP_HANDLE = 0x00000040
	}

	[Flags]
	internal enum DuplicateHandleOptions
	{
		DUPLICATE_CLOSE_SOURCE = 0x1,
		DUPLICATE_SAME_ACCESS = 0x2
	}

	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal sealed class SafeObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeObjectHandle()
			: base(true)
		{ }

		internal SafeObjectHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.CloseHandle(base.handle);
		}
	}

	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeProcessHandle()
			: base(true)
		{ }

		internal SafeProcessHandle(IntPtr preexistingHandle, bool ownsHandle)
			: base(ownsHandle)
		{
			base.SetHandle(preexistingHandle);
		}

		protected override bool ReleaseHandle()
		{
			return NativeMethods.CloseHandle(base.handle);
		}
	}
}