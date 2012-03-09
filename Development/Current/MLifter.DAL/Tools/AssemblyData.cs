using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MLifter.DAL.Tools
{
	/// <summary>
	/// Helper class to get some assembly informations.
	/// </summary>
	public sealed class AssemblyData
	{
		private static Assembly m_Assembly;
		private static AssemblyName m_AssemblyName;
		private static AssemblyTitleAttribute m_AssemblyTitleAttribute;
		private static AssemblyCompanyAttribute m_AssemblyCompanyAttribute;
		private static AssemblyDescriptionAttribute m_AssemblyDescriptionAttribute;

		private AssemblyData() { }

		internal static Assembly Assembly
		{
			get
			{
				if (object.ReferenceEquals(m_Assembly, null))
				{
					m_Assembly = System.Reflection.Assembly.GetExecutingAssembly();
				}
				return m_Assembly;
			}
		}

		internal static AssemblyName AssemblyName
		{
			get
			{
				if (object.ReferenceEquals(m_AssemblyName, null))
				{
					m_AssemblyName = Assembly.GetName();
				}
				return m_AssemblyName;
			}
		}

		internal static AssemblyTitleAttribute AssemblyTitleAttribute
		{
			get
			{
				if (object.ReferenceEquals(m_AssemblyTitleAttribute, null))
				{
					m_AssemblyTitleAttribute = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly, typeof(AssemblyTitleAttribute));
				}
				return m_AssemblyTitleAttribute;
			}
		}

		internal static AssemblyCompanyAttribute AssemblyCompanyAttribute
		{
			get
			{
				if (object.ReferenceEquals(m_AssemblyCompanyAttribute, null))
				{
					m_AssemblyCompanyAttribute = (AssemblyCompanyAttribute)AssemblyCompanyAttribute.GetCustomAttribute(Assembly, typeof(AssemblyCompanyAttribute));
				}
				return m_AssemblyCompanyAttribute;
			}
		}

		internal static AssemblyDescriptionAttribute AssemblyDescriptionAttribute
		{
			get
			{
				if (object.ReferenceEquals(m_AssemblyDescriptionAttribute, null))
				{
					m_AssemblyDescriptionAttribute = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly, typeof(AssemblyDescriptionAttribute));
				}
				return m_AssemblyDescriptionAttribute;
			}
		}

		/// <summary>
		/// Gets the version.
		/// </summary>
		public static Version Version
		{
			get
			{
				return AssemblyName.Version;
			}
		}

		/// <summary>
		/// Gets the assembly version.
		/// </summary>
		public static string AssemblyVersion
		{
			get
			{
				return AssemblyName.Version.ToString(4);
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public static string Name
		{
			get
			{
				return Assembly.FullName;
			}
		}

		/// <summary>
		/// Gets the company.
		/// </summary>
		public static string Company
		{
			get
			{
				return AssemblyCompanyAttribute.Company;
			}
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		public static string Description
		{
			get
			{
				return AssemblyDescriptionAttribute.Description;
			}
		}

		/// <summary>
		/// Gets the title.
		/// </summary>
		public static string Title
		{
			get
			{
				return AssemblyTitleAttribute.Title;
			}
		}

	}
}
