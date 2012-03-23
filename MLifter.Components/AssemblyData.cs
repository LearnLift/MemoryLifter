/***************************************************************************************************************************************
 * Copyright (C) 2001-2012 LearnLift USA																	*
 * Contact: Learnlift USA, 12 Greenway Plaza, Suite 1510, Houston, Texas 77046, support@memorylifter.com					*
 *																								*
 * This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License	*
 * as published by the Free Software Foundation; either version 2.1 of the License, or (at your option) any later version.			*
 *																								*
 * This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty	*
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU Lesser General Public License for more details.	*
 *																								*
 * You should have received a copy of the GNU Lesser General Public License along with this library; if not,					*
 * write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA					*
 ***************************************************************************************************************************************/
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
