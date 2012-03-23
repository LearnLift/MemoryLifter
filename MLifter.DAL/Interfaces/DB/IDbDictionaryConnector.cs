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
