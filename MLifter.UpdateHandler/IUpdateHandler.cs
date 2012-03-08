using System;
using System.Collections.Generic;
using System.Text;

namespace MLifterUpdateHandler
{
    public sealed class UpdateHandler : Attribute { }

    public interface IUpdateHandler
    {
        void StartUpdateProcess(Version newVersion);
    }
}
