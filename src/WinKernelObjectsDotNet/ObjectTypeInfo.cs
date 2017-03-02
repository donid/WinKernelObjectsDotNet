using System;
using System.Linq;


namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class wraps the OBJECT_TYPE_INFORMATION struct.
	/// </summary>
	public class ObjectTypeInfo
	{
		private readonly OBJECT_TYPE_INFORMATION _raw;
		internal OBJECT_TYPE_INFORMATION Raw
		{
			get { return _raw; }
		}

		private readonly string _typeName;
		public string TypeName
		{
			get { return _typeName; }
		}

		public ObjectTypeInfo(OBJECT_TYPE_INFORMATION raw, string typeName)
		{
			_raw = raw;
			_typeName = typeName;
		}
	}
}