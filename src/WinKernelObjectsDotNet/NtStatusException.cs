using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// The exception that is thrown when a call to an NT-API did not return 'STATUS_SUCCESS'
	/// </summary>
	[Serializable]
	public class NtStatusException : Exception, ISerializable
	{
		#region private fields...
		private NT_STATUS status;
		#endregion

		// constructors...
		#region NtStatusException()
		/// <summary>
		/// Constructs a new NtStatusException.
		/// </summary>
		public NtStatusException() { }
		#endregion
		#region NtStatusException(string message, NT_STATUS status)
		/// <summary>
		/// Constructs a new NtStatusException.
		/// </summary>
		/// <param name="message">The exception message</param>
		/// <param name="status">The value for the Status property.</param>
		public NtStatusException(string message, NT_STATUS status) : base(message)
		{
			this.status = status;
		}
		#endregion
		#region NtStatusException(string message, NT_STATUS status, Exception innerException)
		/// <summary>
		/// Constructs a new NtStatusException.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="status">The value for the Status property.</param>
		/// <param name="innerException">The inner exception.</param>
		public NtStatusException(string message, NT_STATUS status, Exception innerException) : base(message, innerException)
		{
			this.status = status;
		}
		#endregion
		#region NtStatusException(SerializationInfo info, StreamingContext context)
		/// <summary>
		/// Serialization constructor.
		/// </summary>
		protected NtStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.status = (NT_STATUS)info.GetValue("Status", typeof(NT_STATUS));
		}
		#endregion

		// public methods...
		#region GetObjectData
		/// <summary>
		/// Overridden method from the ISerializable interface, to include the additional fields in serialization.
		/// </summary>
		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Status", Status);
		}
		#endregion

		// public properties...
		#region Message
		/// <summary>
		/// Overridden property from System.Exception, to include the additional fields in the message.
		/// </summary>
		public override string Message
		{
			get { return String.Format(CultureInfo.CurrentCulture, "{0}, Status: {1}", base.Message, Status); }
		}
		#endregion
		#region Status
		/// <summary>
		/// The returned error code.
		/// </summary>
		public NT_STATUS Status
		{
			get { return status; }
		}
		#endregion
	}
}