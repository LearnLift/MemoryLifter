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
