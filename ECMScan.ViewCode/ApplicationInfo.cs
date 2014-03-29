using System;
using System.Reflection;

namespace ISBLScan.ViewCode
{
	/// <summary>
	/// Application info.
	/// </summary>
	/// <see cref="http://stackoverflow.com/questions/909555/how-can-i-get-the-assembly-file-version/10723966#10723966"/>
	/// <see cref="http://stackoverflow.com/users/196919/clark-kent"/>
	public class ApplicationInfo
	{
		public ApplicationInfo()
		{
			CallingAssembly = Assembly.GetCallingAssembly ();
		}
		Assembly CallingAssembly
		{
			get; set;
		}
		
		public Version Version { get { return CallingAssembly.GetName ().Version; } }

		public string Title
		{
			get
			{
				object[] attributes = CallingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
				if (attributes.Length > 0)
				{
					AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
					if (titleAttribute.Title.Length > 0) return titleAttribute.Title;
				}
				return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
			}
		}

		public string ProductName
		{
			get
			{
				object[] attributes = CallingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
				return attributes.Length == 0 ? "" : ((AssemblyProductAttribute)attributes[0]).Product;
			}
		}

		public string Description
		{
			get
			{
				object[] attributes = CallingAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
				return attributes.Length == 0 ? "" : ((AssemblyDescriptionAttribute)attributes[0]).Description;
			}
		}

		public string CopyrightHolder
		{
			get
			{
				object[] attributes = CallingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
				return attributes.Length == 0 ? "" : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
			}
		}

		public string CompanyName
		{
			get
			{
				object[] attributes = CallingAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				return attributes.Length == 0 ? "" : ((AssemblyCompanyAttribute)attributes[0]).Company;
			}
		}
	}
}

