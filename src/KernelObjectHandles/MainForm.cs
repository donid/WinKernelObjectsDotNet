using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinKernelObjectsDotNet;


namespace KernelObjectHandles
{
	public partial class MainForm : Form
	{
		private readonly bool _isElevated;

		public MainForm()
		{
			InitializeComponent();
			_isElevated = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (_isElevated)
			{
				base.Text += " (Administrator)";
			}

			RefreshData();
		}

		private void simpleButtonRefresh_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				RefreshData();
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void RefreshData()
		{
			//handles without name seem to be always invalid
			Func<RowViewModel, bool> invalidFilter = item => !string.IsNullOrEmpty(item.Name) && !item.Name.StartsWith("<");
			Func<RowViewModel, bool> standardFilter = item => true;
			gridControl1.DataSource = KernelObject.EnumerateAllObjects().Select(item => new RowViewModel(item)).Where(checkEditHideInvalidHandles.Checked ? invalidFilter : standardFilter).ToList();
			/*.Where(handleEntry => handleEntry.UniqueProcessId == processId).Where(fileOrDirectoryFilter.FilterMethod)*/
		}

		private void ShowMessageBox(string text)
		{
			MessageBox.Show(text, "KernelObjectHandles");
		}

		private void simpleButtonShowShareFlags_Click(object sender, EventArgs e)
		{
			if (!_isElevated)
			{
				ShowMessageBox("This feature is only available when running in elevated mode!"); // ShareFlags in Handle.exe are only visible when runasadministrator
				return;
			}

			RowViewModel row = (RowViewModel)gridView1.GetFocusedRow();
			if (row == null)
			{
				ShowMessageBox("No focused row!");
				return;
			}
			if (row.ObjectTypeString != "File")
			{
				ShowMessageBox("Focused row is not of type'File'!");
				return;
			}

			const int ERROR_CANCELLED = 1223; //The operation was canceled by the user.
			ProcessStartInfo info = new ProcessStartInfo(@"Handle.exe"); // tool has to be in the same folder as this program
			info.RedirectStandardOutput = true;
			info.UseShellExecute = false; // 'UseShellExecute = true' does not work with redirect
			info.Arguments = "-p " + row.PID;
			info.CreateNoWindow = true;
			//info.Verb = "runas"; needs UseShellExecute = true
			Process p = null;
			try
			{
				p = Process.Start(info);
			}
			catch (Win32Exception ex)
			{
				if (ex.NativeErrorCode == ERROR_CANCELLED)
				{
					ShowMessageBox("Feature cannot run without admin privileges!");
				}
				else
				{
					ShowMessageBox(ex.Message);
				}
			}
			Task<string> resultt = p.StandardOutput.ReadToEndAsync();
			p.WaitForExit();
			string es = resultt.Result;
			var lines = es.Split(new[] { "\r\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			string expectedLineStart = $"{row.Handle,5:X}: File  ";
			string foundLine = lines.FirstOrDefault(item => item.StartsWith(expectedLineStart));
			if (foundLine == null)
			{
				ShowMessageBox("Handle not found!");
			}
			else
			{
				ShowMessageBox(foundLine);
			}
		}
	}

	class RowViewModel
	{
		static string cusid = WindowsIdentity.GetCurrent().User.Value;

		static readonly TimeSpan timeout = TimeSpan.FromMilliseconds(500);
		static Dictionary<int, string> _pids = Process.GetProcesses().ToDictionary(item => item.Id, item => item.ProcessName);

		private static string[] objectTypesArr = KernelObject.GetObjectTypeNames();

		private SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX _info;
		public RowViewModel(SYSTEM_HANDLE_TABLE_ENTRY_INFO_EX info)
		{
			_info = info;

			using (DuplicatedObjectHandle dhandle = new DuplicatedObjectHandle(_info.HandleValue, _info.UniqueProcessId))
			{
				if (dhandle.ErrorMessage != null)
				{
					_name = "<" + dhandle.ErrorMessage + ">";
					return;
				}
				string name;
				NT_STATUS status = dhandle.TryGetObjectNameFromHandle(out name, timeout);
				if (status != NT_STATUS.STATUS_SUCCESS)
				{
					_name = "<" + status.ToString() + ">";
					return;
				}
				_name = name;
			}

			if (_name != null && !_name.StartsWith("<"))
			{
				if (this.ObjectTypeString == "File")
				{
					_prettyName = DevicePathConverter.ConvertDevicePathToDosPath(_name);
				}
				if (this.ObjectTypeString == "Key")
				{
					_prettyName = _name.Replace(@"\REGISTRY\MACHINE", @"HKLM").Replace(@"\REGISTRY\USER\" + cusid, @"HKCU").Replace(@"\REGISTRY\USER", @"HKU");// todo: do not use replace, use code similar to DevicePathConverter.IsMatch
				}
			}

		}

		public int PID
		{
			get { return _info.UniqueProcessId; }
		}
		public long Handle
		{
			get { return _info.HandleValue.ToInt64(); }
		}
		public uint HandleAttributes
		{
			get { return _info.HandleAttributes; }
		}
		public uint GrantedAccess
		{
			get { return _info.GrantedAccess; }
		}
		public ushort ObjectTypeIndex
		{
			get { return _info.ObjectTypeIndex; }
		}
		public string ObjectTypeString
		{
			get
			{
				if (_info.ObjectTypeIndex >= objectTypesArr.Length)
				{
					return _info.ObjectTypeIndex.ToString();
				}
				return objectTypesArr[_info.ObjectTypeIndex];
			}
		}
		public long Object
		{
			get { return _info.Object.ToInt64(); }
		}

		private string _name;
		public string Name
		{
			get { return _name; }
		}
		private string _prettyName;
		public string PrettyName
		{
			get { return _prettyName; }
		}

		public string ProcessName
		{
			get { return _pids[_info.UniqueProcessId]; }
		}

	}
}
