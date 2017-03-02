using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.Serialization;
using System.Globalization;
using System.Security.Permissions;

namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class wraps Handles of foreign processes, they have to be duplicated into the current process, to query information about them.
	/// If you forget to duplicate the handle, you often don't receive an error as query result - the Windows API just returns wrong data!
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	/// <remarks>
	/// The handle will only be duplicated when the processId provided to the constructor doesn't belong to the current process.
	/// </remarks>
	public class DuplicatedObjectHandle : IDisposable
	{
		private readonly Win32Exception _exception;
		private SafeObjectHandle _objectHandle = null;

		private readonly string _errorMessage;

		/// <summary>
		/// Gets the error message, if an error occurred during construction.
		/// </summary>
		/// <value>
		/// The error message - return null, if no error occurred.
		/// </value>
		public string ErrorMessage
		{
			get
			{
				if (_errorMessage == null && _exception == null)
				{
					return null;
				}
				return _errorMessage + ": " + _exception.Message;
			}
		}


		private IntPtr _duplicatedObjectHandle;
		internal IntPtr Handle
		{
			get
			{
				if (_exception != null)
				{
					throw _exception;
				}
				return _duplicatedObjectHandle;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DuplicatedObjectHandle"/> class.
		/// </summary>
		/// <param name="handle">The handle (will not be closed, when the DuplicatedObjectHandle-instance is disposed).</param>
		/// <param name="processId">The process ID.</param>
		public DuplicatedObjectHandle(IntPtr handle, int processId)
		{
			_duplicatedObjectHandle = handle;
			IntPtr currentProcess = NativeMethods.GetCurrentProcess();
			bool remote = (processId != NativeMethods.GetProcessId(currentProcess));
			SafeProcessHandle processHandle = null;
			try
			{
				if (remote)
				{
					processHandle = NativeMethods.OpenProcess(ProcessAccessRights.PROCESS_DUP_HANDLE, true, processId);
					if (processHandle.IsInvalid)
					{
						_exception = new Win32Exception();
						_errorMessage = "OpenProcess";
						return;
					}
					bool success = NativeMethods.DuplicateHandle(processHandle.DangerousGetHandle(), handle, currentProcess, out _objectHandle, 0, false, DuplicateHandleOptions.DUPLICATE_SAME_ACCESS);
					if (!success)
					{
						_exception = new Win32Exception();
						_errorMessage = "DuplicateHandle";
						return;
					}
					_duplicatedObjectHandle = _objectHandle.DangerousGetHandle();
				}
			}
			finally
			{
				if (processHandle != null)
				{
					processHandle.Close();
				}
			}

		}


		#region IDisposable Members and finalizer

		private bool _isDisposedFlag;   //Compiler initializes this to false

		/// <summary>
		/// dispose managed and unmanaged resources
		/// </summary>
		/// <param name="isDisposing">true if called via IDisposable.Dispose, false if called via finalizer</param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (_isDisposedFlag)
			{
				return;
			}
			if (isDisposing)
			{
				// free managed resources here
				if (_objectHandle != null)
				{
					_objectHandle.Close();
				}
			}
			// free unmanaged resources here

			// set mark this object as disposed
			_isDisposedFlag = true;
		}

		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			// If you implement a Close-Method: use the same code
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Call this method in every public method of this class as first statement
		/// </summary>
		private void ThrowIfDisposed()
		{
			if (_isDisposedFlag)
			{
				throw (new ObjectDisposedException("DuplicatedObjectHandle", "Cannot access a disposed object."));
			}
		}
		#endregion


		/// <summary>
		/// Gets the object name from the wrapped handle.
		/// </summary>
		/// <returns>
		/// For object-type 'File': the device path (serial-ports also have the type 'file') - starts always with '\Device\', if it doesn't the handle is invalid.
		/// For object-type 'Key': the registry Path - starts always with '\REGISTRY\', if it doesn't the handle is invalid.
		/// </returns>
		/// <exception cref="System.Exception"></exception>
		public string GetObjectNameFromHandle()
		{
			string name;
			NT_STATUS status = TryGetObjectNameFromHandle(out name);
			if (status != NT_STATUS.STATUS_SUCCESS)
			{
				throw new NtStatusException("TryGetObjectNameFromHandle failed", status);
			}
			return name;
		}

		/// <summary>
		/// Tries the get object name from the wrapped handle.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		/// <remarks>
		/// This method might hang infinitely in NtQueryObject, so it is safer to use the overload with the timeout parameter.
		/// The hangs have been observed for handles with GrantedAccess == 0x00120189 and 0x0012019f all of them were for type 'file' 
		/// and process explorer showed that the handle names all were '\Device\NamedPipe' 
		/// This was tested on win10 64bit 1511 and 1607.
		/// The problem seems only to appear when 'Prefer 32bit' is disabled
		/// </remarks>
		public NT_STATUS TryGetObjectNameFromHandle(out string name)
		{
			name = null;
			IntPtr ptr = IntPtr.Zero;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				int length = 0x200;  // 512 bytes
				RuntimeHelpers.PrepareConstrainedRegions();
				try { }
				finally
				{
					// CER guarantees the assignment of the allocated memory address to ptr, if an asynchronous exception occurs.
					ptr = Marshal.AllocHGlobal(length);
				}
				NT_STATUS ret = NativeMethods.NtQueryObject(_duplicatedObjectHandle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);
				if (ret == NT_STATUS.STATUS_BUFFER_OVERFLOW)
				{
					RuntimeHelpers.PrepareConstrainedRegions();
					try { }
					finally
					{
						// CER guarantees that the previous allocation is freed, and that the newly allocated memory address is 
						// assigned to ptr if an asynchronous exception occurs.
						Marshal.FreeHGlobal(ptr);
						ptr = Marshal.AllocHGlobal(length);
					}
					ret = NativeMethods.NtQueryObject(_duplicatedObjectHandle, OBJECT_INFORMATION_CLASS.ObjectNameInformation, ptr, length, out length);
				}
				if (ret == NT_STATUS.STATUS_SUCCESS)
				{
					OBJECT_NAME_INFORMATION nameInfo = Marshal.PtrToStructure<OBJECT_NAME_INFORMATION>(ptr);
					name = Helpers.MarshalUnicodeString(nameInfo.Name);
				}
				return ret;
			}
			finally
			{
				// CER guarantees that the allocated memory is freed, if an asynchronous exception occurs.
				Marshal.FreeHGlobal(ptr);
			}

		}

		/// <summary>
		/// Tries the get object name from the wrapped handle.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns></returns>
		/// <remarks>
		/// Since the call to NtQueryObject might hang infinitely, this method uses a separate thread with a timeout to call the API
		/// If the timeout expires the status 'NT_STATUS.STATUS_TIMEOUT' will be returned.
		/// </remarks>
		public NT_STATUS TryGetObjectNameFromHandle(out string fileName, TimeSpan timeout)
		{
			string name = null;
			NT_STATUS result = NT_STATUS.STATUS_SUCCESS;

			AutoResetEvent signal = new AutoResetEvent(false);
			Thread workerThread = null;

			ThreadPool.QueueUserWorkItem((o) =>
				{
					workerThread = Thread.CurrentThread;
					result = TryGetObjectNameFromHandle(out name);
					signal.Set();
				});

			bool waitres = signal.WaitOne(timeout);

			fileName = name;
			if (workerThread != null && workerThread.IsAlive && waitres == false)
			{
				workerThread.Abort();
				return NT_STATUS.STATUS_TIMEOUT;
			}
			return result;
		}

		/// <summary>
		/// Gets the type-info for the handle.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NtStatusException">
		/// NtQueryObject failed
		/// </exception>
		public ObjectTypeInfo GetHandleType()
		{
			int length;
			NT_STATUS res1 = NativeMethods.NtQueryObject(_duplicatedObjectHandle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, IntPtr.Zero, 0, out length);
			if (res1 != NT_STATUS.STATUS_SUCCESS && res1 != NT_STATUS.STATUS_INFO_LENGTH_MISMATCH)
			{
				throw new NtStatusException("NtQueryObject call1 failed", res1);
			}
			IntPtr ptr = IntPtr.Zero;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try { }
				finally
				{
					ptr = Marshal.AllocHGlobal(length);
				}
				NT_STATUS res2 = NativeMethods.NtQueryObject(_duplicatedObjectHandle, OBJECT_INFORMATION_CLASS.ObjectTypeInformation, ptr, length, out length);
				if (res2 != NT_STATUS.STATUS_SUCCESS)
				{
					throw new NtStatusException("NtQueryObject call2 failed", res2);
				}
				OBJECT_TYPE_INFORMATION objectType = Marshal.PtrToStructure<OBJECT_TYPE_INFORMATION>(ptr);
				string typeName = Helpers.MarshalUnicodeString(objectType.TypeName);
				return new ObjectTypeInfo(objectType, typeName);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
		}

	}
}