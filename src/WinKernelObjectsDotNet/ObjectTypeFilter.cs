using System;
using System.Collections.Generic;
using System.Linq;

namespace WinKernelObjectsDotNet
{
	/// <summary>
	/// This class allows to filter a sequence of SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX instances by their type name,
	/// even though this struct stores only the type index.
	/// </summary>
	public class ObjectTypeFilter
	{
		private readonly int[] _objectTypeIndices;

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectTypeFilter"/> class.
		/// </summary>
		/// <param name="objectTypeNames">The object type names that should pass the filter e.g. 'File' or 'Key'.</param>
		public ObjectTypeFilter(params string[] objectTypeNames) : this((IEnumerable<string>)objectTypeNames)
		{
		}

		public ObjectTypeFilter(IEnumerable<string> objectTypeNames)
		{
			if (objectTypeNames == null)
			{
				throw new ArgumentNullException(nameof(objectTypeNames), $"{nameof(objectTypeNames)} is null.");
			}
			string[] allObjectTypesArr = KernelObject.GetObjectTypeNames();
			_objectTypeIndices = objectTypeNames.Select(item => FindTypeIndex(item, allObjectTypesArr)).ToArray();
		}

		private static int FindTypeIndex(string objectTypeName, string[] allObjectTypesArr)
		{
			int index = Array.IndexOf(allObjectTypesArr, objectTypeName);
			if (index == -1)
			{
				throw new ArgumentException("objectTypeName not found: " + objectTypeName);
			}
			return index;
		}

		public bool IsMatch(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX info)
		{
			return _objectTypeIndices.Contains(info.ObjectTypeIndex);
		}
	}
}