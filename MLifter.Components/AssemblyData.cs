using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MLifter.Components
{
    public sealed class AssemblyData
    {
        private static Assembly m_Assembly;
        private static AssemblyName m_AssemblyName;
        private static AssemblyTitleAttribute m_AssemblyTitleAttribute;
        private static AssemblyCompanyAttribute m_AssemblyCompanyAttribute;
        private static AssemblyDescriptionAttribute m_AssemblyDescriptionAttribute;

        private AssemblyData()
        {
        }

        internal static Assembly Assembly
        {
            get
            {
                if (object.ReferenceEquals(m_Assembly, null))
                {
                    m_Assembly = System.Reflection.Assembly.GetEntryAssembly();
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

        public static string Version
        {
            get
            {
                return AssemblyName.Version.ToString(2);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return AssemblyName.Version.ToString(4);
            }
        }

        public static string Name
        {
            get
            {
                return Assembly.FullName;
            }
        }

        public static string Company
        {
            get
            {
                return AssemblyCompanyAttribute.Company;
            }
        }

        public static string Description
        {
            get
            {
                return AssemblyDescriptionAttribute.Description;
            }
        }

        public static string Title
        {
            get
            {
                return AssemblyTitleAttribute.Title;
            }
        }

    }
}
