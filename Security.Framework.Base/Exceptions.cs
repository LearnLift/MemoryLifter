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
using System.Text;

namespace SecurityFramework
{
    public class FrameworkException : Exception
    {
        public FrameworkException(string message):base(message)
        {

        }
    }

    public class AccountAlreadyDefinedException:FrameworkException
    {
        public AccountAlreadyDefinedException():base("Account already defined")
        {

        }
    }
    public class GroupAlreadyDefinedException : FrameworkException
    {
        public GroupAlreadyDefinedException()
            : base("Group already defined")
        {

        }
    }

    public class AccountWrongException : FrameworkException
    {
        public AccountWrongException()
            : base("Account  wrong")
        {

        }
    }

    public class ObjectNotLocatedException : FrameworkException
    {
        public ObjectNotLocatedException()
            : base("Object cannot be located")
        {

        }
    }

    public class TypeNotFoundException : FrameworkException
    {
        public TypeNotFoundException()
            : base("Type cannot be found")
        {

        }
    }

    public class TypeAlreadyDefinedException : FrameworkException
    {
        public TypeAlreadyDefinedException()
            : base("Type already defined")
        {

        }
    }
    public class PermissionAlreadyDefinedException : FrameworkException
    {
        public PermissionAlreadyDefinedException()
            : base("Permission already defined")
        {

        }
    }
    public class TypeOrPermissionNotFoundException : FrameworkException
    {
        public TypeOrPermissionNotFoundException()
            : base("Type or permission cannot be found")
        {

        }
    }
}
