using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbUserConnector
    {
        IList<UserStruct> GetUserList();
        UserAuthenticationTyp? GetAllowedAuthenticationModes();
        LocalDirectoryType? GetLocalDirectoryType();

        string GetLdapServer();
        int GetLdapPort();
        string GetLdapUser();
        string GetLdapPassword();
        string GetLdapContext();
        bool GetLdapUseSSL();

        int GetUserLearningModuleSettings(int id);

        int LoginListUser(string username, Guid sid, bool closeOpenSessions, bool standAlone);
        int LoginFormsUser(string username, string password, Guid sid, bool closeOpenSessions, bool standAlone);
        int LoginLocalDirectoryUser(string username, string localDirectoryIdentifier, Guid sid, bool closeOpenSessions, bool standAlone);
        void LogoutUserSession(Guid sid);
    }
}
