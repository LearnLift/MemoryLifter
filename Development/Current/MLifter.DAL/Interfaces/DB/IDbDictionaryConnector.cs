using System;
using System.Collections.Generic;
using System.Text;
using MLifter.DAL.DB;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbDictionaryConnector
    {
        string GetDbVersion();

        bool GetContentProtected(int id);

        string GetTitle(int id);
        void SetTitle(int id, string title);

        string GetAuthor(int id);
        void SetAuthor(int id, string author);

        string GetDescription(int id);
        void SetDescription(int id, string description);

        string GetGuid(int id);
        void SetGuid(int id, string guid);

        ISettings GetDefaultSettings(int id);
        void SetDefaultSettings(int id, int settingsId);

        ISettings GetAllowedSettings(int id);
        void SetAllowedSettings(int id, int settingsId);

        ISettings GetUserSettings(int id);
        void SetUserSettings(int id, int settingsId);

        ISettings CreateSettings();

        double GetScore(int id);

        double GetHighscore(int id);

        void SetHighscore(int id, double Highscore);

        long GetDictionarySize(int id, int defaultCardSizeValue);

        int GetDictionaryMediaObjectsCount(int id);

        Category GetCategoryId(int id);

        void SetCategory(int id, int catId);

        void SetCategory(int id, Category cat);

        bool CheckUserSession();

        void PreloadCardCache(int id);

        void ClearUnusedMedia(int id);

        IList<Guid> GetExtensions(int id);
    }
}
