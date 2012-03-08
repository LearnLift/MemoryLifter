using System;
using System.Collections.Generic;
using System.Text;

namespace MLifter.DAL.Interfaces.DB
{
    interface IDbSnoozeOptionsConnector
    {
        void CheckId(int id);

        bool? GetCardsEnabled(int id);
        void SetCardsEnabled(int id, bool? CardsEnabled);

        bool? GetRightsEnabled(int id);
        void SetRightsEnabled(int id, bool? RightsEnabled);

        bool? GetTimeEnabled(int id);
        void SetTimeEnabled(int id, bool? TimeEnabled);

        int? GetSnoozeCards(int id);
        void SetSnoozeCards(int id, int? SnoozeCards);

        int? GetSnoozeHigh(int id);
        void SetSnoozeHigh(int id, int? SnoozeHigh);

        int? GetSnoozeLow(int id);
        void SetSnoozeLow(int id, int? SnoozeLow);

        ESnoozeMode? GetSnoozeMode(int id);
        void SetSnoozeMode(int id, ESnoozeMode? SnoozeMode);

        int? GetSnoozeRights(int id);
        void SetSnoozeRights(int id, int? SnoozeRights);

        int? GetSnoozeTime(int id);
        void SetSnoozeTime(int id, int? SnoozeTime);
    }
}
