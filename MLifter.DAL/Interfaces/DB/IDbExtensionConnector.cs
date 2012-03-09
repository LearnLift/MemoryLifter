using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MLifter.DAL.Interfaces;
using MLifter.Generics;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbExtensionConnector
    {
        Guid AddNewExtension();
        Guid AddNewExtension(Guid guid);

        void SetExtensionLM(Guid guid, int lmid);

        void DeleteExtension(Guid guid);

        string GetExtensionName(Guid guid);
        void SetExtensionName(Guid guid, string extensionName);

        Version GetExtensionVersion(Guid guid);
        void SetExtensionVersion(Guid guid, Version versionName);

        ExtensionType GetExtensionType(Guid guid);
        void SetExtensionType(Guid guid, ExtensionType extensionType);

        string GetExtensionStartFile(Guid guid);
        void SetExtensionStartFile(Guid guid, string startFile);

        Stream GetExtensionStream(Guid guid);
        void SetExtensionStream(Guid guid, Stream extensionStream);

        bool IsStreamAvailable(Guid guid);

        IList<ExtensionAction> GetExtensionActions(Guid guid);
        void SetExtensionActions(Guid guid, IList<ExtensionAction> extensionActions);
    }
}
