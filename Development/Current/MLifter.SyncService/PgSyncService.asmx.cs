using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Services;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.Server;
using Npgsql;
using NpgsqlTypes;
using MLifter.DAL.Interfaces;
using MLifter.DAL.Interfaces.DB;

namespace MLifterSyncService
{
    /// <summary>
    /// Summary Description for PgSyncService
    /// </summary>
    [WebService(Namespace = "http://www.memorylifter.com/sync/service/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class PgSyncService : System.Web.Services.WebService
    {
        private static string LogFile;
        private static DbServerSyncProvider serverSyncProvider = null;

        public PgSyncService()
        {
            if (serverSyncProvider == null)
            {
                LogFile = Server.MapPath(ConfigurationManager.AppSettings["LogPath"]);
                // Enable Npg logging.
                LogLevel logNpg = (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["NpgsqlEventLogLevel"]);
                if (logNpg != LogLevel.None)
                {
                    NpgsqlEventLog.Level = logNpg;
                    NpgsqlEventLog.LogName = Server.MapPath(ConfigurationManager.AppSettings["NpgsqlEventLogName"]);
                    NpgsqlEventLog.EchoMessages = false;
                }

                serverSyncProvider = new DbServerSyncProvider();
                NpgsqlConnection serverConnection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString);
                serverSyncProvider.Connection = serverConnection;
                serverSyncProvider.BatchSize = Convert.ToInt32(ConfigurationManager.AppSettings["BatchSize"]);
                serverSyncProvider.ApplyChangeFailed += new EventHandler<ApplyChangeFailedEventArgs>(serverSyncProvider_ApplyChangeFailed);
                PrepareAdapters();
                //PrepareAnchor();
                PrepareBatchingAnchor();
                PrepareSchema();
            }
        }

        void serverSyncProvider_ApplyChangeFailed(object sender, ApplyChangeFailedEventArgs e)
        {
            string line = "\n\r\n\r-----------------------------------------------------------------------------------------------------\n\r";
            File.AppendAllText(LogFile, line + e.Conflict.ClientChange.ToString() + line + e.Error.ToString() + line);
        }

        private void PrepareSchema()
        {
            SyncSchema syncSchema = new SyncSchema();
            serverSyncProvider.Schema = syncSchema;

            # region QueryDirections
            syncSchema.Tables.Add("QueryDirections");
            syncSchema.Tables["QueryDirections"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["QueryDirections"].Columns["id"].AllowNull = false;
            syncSchema.Tables["QueryDirections"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["QueryDirections"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["QueryDirections"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["QueryDirections"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["QueryDirections"].Columns.Add("question2answer").DataType = typeof(bool);
            syncSchema.Tables["QueryDirections"].Columns.Add("answer2question").DataType = typeof(bool);
            syncSchema.Tables["QueryDirections"].Columns.Add("mixed").DataType = typeof(bool);
            # endregion
            # region SynonymGradings
            syncSchema.Tables.Add("SynonymGradings");
            syncSchema.Tables["SynonymGradings"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["SynonymGradings"].Columns["id"].AllowNull = false;
            syncSchema.Tables["SynonymGradings"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["SynonymGradings"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["SynonymGradings"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["SynonymGradings"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["SynonymGradings"].Columns.Add("all_known").DataType = typeof(bool);
            syncSchema.Tables["SynonymGradings"].Columns.Add("half_known").DataType = typeof(bool);
            syncSchema.Tables["SynonymGradings"].Columns.Add("one_known").DataType = typeof(bool);
            syncSchema.Tables["SynonymGradings"].Columns.Add("first_known").DataType = typeof(bool);
            syncSchema.Tables["SynonymGradings"].Columns.Add("prompt").DataType = typeof(bool);
            # endregion
            # region QueryTypes
            syncSchema.Tables.Add("QueryTypes");
            syncSchema.Tables["QueryTypes"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["QueryTypes"].Columns["id"].AllowNull = false;
            syncSchema.Tables["QueryTypes"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["QueryTypes"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["QueryTypes"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["QueryTypes"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["QueryTypes"].Columns.Add("image_recognition").DataType = typeof(bool);
            syncSchema.Tables["QueryTypes"].Columns.Add("listening_comprehension").DataType = typeof(bool);
            syncSchema.Tables["QueryTypes"].Columns.Add("multiple_choice").DataType = typeof(bool);
            syncSchema.Tables["QueryTypes"].Columns.Add("sentence").DataType = typeof(bool);
            syncSchema.Tables["QueryTypes"].Columns.Add("word").DataType = typeof(bool);
            # endregion
            # region MultipleChoiceOptions
            syncSchema.Tables.Add("MultipleChoiceOptions");
            syncSchema.Tables["MultipleChoiceOptions"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["MultipleChoiceOptions"].Columns["id"].AllowNull = false;
            syncSchema.Tables["MultipleChoiceOptions"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["MultipleChoiceOptions"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["MultipleChoiceOptions"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["MultipleChoiceOptions"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["MultipleChoiceOptions"].Columns.Add("allow_multiple_correct_answers").DataType = typeof(bool);
            syncSchema.Tables["MultipleChoiceOptions"].Columns.Add("allow_random_distractors").DataType = typeof(bool);
            syncSchema.Tables["MultipleChoiceOptions"].Columns.Add("max_correct_answers").DataType = typeof(int);
            syncSchema.Tables["MultipleChoiceOptions"].Columns.Add("number_of_choices").DataType = typeof(int);
            # endregion
            # region TypeGradings
            syncSchema.Tables.Add("TypeGradings");
            syncSchema.Tables["TypeGradings"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["TypeGradings"].Columns["id"].AllowNull = false;
            syncSchema.Tables["TypeGradings"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["TypeGradings"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["TypeGradings"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["TypeGradings"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["TypeGradings"].Columns.Add("all_correct").DataType = typeof(bool);
            syncSchema.Tables["TypeGradings"].Columns.Add("half_correct").DataType = typeof(bool);
            syncSchema.Tables["TypeGradings"].Columns.Add("none_correct").DataType = typeof(bool);
            syncSchema.Tables["TypeGradings"].Columns.Add("prompt").DataType = typeof(bool);
            # endregion
            # region CardStyles
            syncSchema.Tables.Add("CardStyles");
            syncSchema.Tables["CardStyles"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["CardStyles"].Columns["id"].AllowNull = false;
            syncSchema.Tables["CardStyles"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["CardStyles"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["CardStyles"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["CardStyles"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["CardStyles"].Columns.Add("value").DataType = typeof(string);
            syncSchema.Tables["CardStyles"].Columns["value"].MaxLength = 8000;
            # endregion
            # region StyleSheets
            syncSchema.Tables.Add("StyleSheets");
            syncSchema.Tables["StyleSheets"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["StyleSheets"].Columns["id"].AllowNull = false;
            syncSchema.Tables["StyleSheets"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["StyleSheets"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["StyleSheets"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["StyleSheets"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["StyleSheets"].Columns.Add("value").DataType = typeof(string);
            syncSchema.Tables["StyleSheets"].Columns["value"].AllowNull = false;
            syncSchema.Tables["StyleSheets"].Columns["value"].MaxLength = 8000;
            # endregion
            # region SnoozeOptions
            syncSchema.Tables.Add("SnoozeOptions");
            syncSchema.Tables["SnoozeOptions"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["SnoozeOptions"].Columns["id"].AllowNull = false;
            syncSchema.Tables["SnoozeOptions"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["SnoozeOptions"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["SnoozeOptions"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["SnoozeOptions"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["SnoozeOptions"].Columns.Add("cards_enabled").DataType = typeof(bool);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("rights_enabled").DataType = typeof(bool);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("time_enabled").DataType = typeof(bool);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_cards").DataType = typeof(int);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_high").DataType = typeof(int);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_low").DataType = typeof(int);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_mode").DataType = typeof(string);
            syncSchema.Tables["SnoozeOptions"].Columns["snooze_mode"].MaxLength = 100;
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_rights").DataType = typeof(int);
            syncSchema.Tables["SnoozeOptions"].Columns.Add("snooze_time").DataType = typeof(int);
            # endregion
            # region Boxes
            syncSchema.Tables.Add("Boxes");
            syncSchema.Tables["Boxes"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Boxes"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Boxes"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Boxes"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Boxes"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Boxes"].Columns.Add("box1_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box2_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box3_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box4_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box5_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box6_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box7_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box8_size").DataType = typeof(int);
            syncSchema.Tables["Boxes"].Columns.Add("box9_size").DataType = typeof(int);
            # endregion
            # region MediaContent
            syncSchema.Tables.Add("MediaContent");
            syncSchema.Tables["MediaContent"].Columns.Add("id");
            syncSchema.Tables["MediaContent"].Columns["id"].AllowNull = false;
            syncSchema.Tables["MediaContent"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["MediaContent"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["MediaContent"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["MediaContent"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["MediaContent"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["MediaContent"].Columns.Add("data");
            syncSchema.Tables["MediaContent"].Columns["data"].AllowNull = true;
            syncSchema.Tables["MediaContent"].Columns["data"].ProviderDataType = "Image";
            syncSchema.Tables["MediaContent"].Columns.Add("media_type");
            syncSchema.Tables["MediaContent"].Columns["media_type"].AllowNull = false;
            syncSchema.Tables["MediaContent"].Columns["media_type"].DataType = typeof(string);
            syncSchema.Tables["MediaContent"].Columns["media_type"].MaxLength = 100;
            # endregion
            # region MediaProperties
            syncSchema.Tables.Add("MediaProperties");
            syncSchema.Tables["MediaProperties"].Columns.Add("media_id");
            syncSchema.Tables["MediaProperties"].Columns["media_id"].AllowNull = false;
            syncSchema.Tables["MediaProperties"].Columns["media_id"].DataType = typeof(int);
            syncSchema.Tables["MediaProperties"].Columns.Add("property");
            syncSchema.Tables["MediaProperties"].Columns["property"].AllowNull = false;
            syncSchema.Tables["MediaProperties"].Columns["property"].DataType = typeof(string);
            syncSchema.Tables["MediaProperties"].Columns["property"].MaxLength = 100;
            syncSchema.Tables["MediaProperties"].Columns.Add("value").DataType = typeof(string);
            syncSchema.Tables["MediaProperties"].Columns["value"].MaxLength = 100;
            syncSchema.Tables["MediaProperties"].PrimaryKey = new string[] { "media_id", "property" };
            # endregion
            # region Settings
            syncSchema.Tables.Add("Settings");
            syncSchema.Tables["Settings"].Columns.Add("id");
            syncSchema.Tables["Settings"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Settings"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Settings"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Settings"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Settings"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["Settings"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Settings"].Columns.Add("autoplay_audio").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("case_sensitive").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("confirm_demote").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("enable_commentary").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("correct_on_the_fly").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("enable_timer").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("synonym_gradings").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("type_gradings").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("multiple_choice_options").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("query_directions").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("query_types").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("random_pool").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("self_assessment").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("show_images").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("stripchars").DataType = typeof(string);
            syncSchema.Tables["Settings"].Columns["stripchars"].MaxLength = 100;
            syncSchema.Tables["Settings"].Columns.Add("question_culture").DataType = typeof(string);
            syncSchema.Tables["Settings"].Columns["question_culture"].MaxLength = 100;
            syncSchema.Tables["Settings"].Columns.Add("answer_culture").DataType = typeof(string);
            syncSchema.Tables["Settings"].Columns["answer_culture"].MaxLength = 100;
            syncSchema.Tables["Settings"].Columns.Add("question_caption").DataType = typeof(string);
            syncSchema.Tables["Settings"].Columns["question_caption"].MaxLength = 100;
            syncSchema.Tables["Settings"].Columns.Add("answer_caption").DataType = typeof(string);
            syncSchema.Tables["Settings"].Columns["answer_caption"].MaxLength = 100;
            syncSchema.Tables["Settings"].Columns.Add("logo").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("question_stylesheet").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("answer_stylesheet").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("auto_boxsize").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("pool_empty_message_shown").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("show_statistics").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("skip_correct_answers").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("snooze_options").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("use_lm_stylesheets").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns.Add("cardstyle").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("boxes").DataType = typeof(int);
            syncSchema.Tables["Settings"].Columns.Add("isCached").DataType = typeof(bool);
            syncSchema.Tables["Settings"].Columns["isCached"].DefaultValue = "1";
            syncSchema.Tables["Settings"].ForeignKeys.Add("synonym_gradings_fk", "SynonymGradings", "id", "Settings", "synonym_gradings");
            syncSchema.Tables["Settings"].ForeignKeys.Add("type_gradings_fk", "TypeGradings", "id", "Settings", "type_gradings");
            syncSchema.Tables["Settings"].ForeignKeys.Add("multiple_choice_options_fk", "MultipleChoiceOptions", "id", "Settings", "multiple_choice_options");
            syncSchema.Tables["Settings"].ForeignKeys.Add("query_directions_fk", "QueryDirections", "id", "Settings", "query_directions");
            syncSchema.Tables["Settings"].ForeignKeys.Add("query_types_fk", "QueryTypes", "id", "Settings", "query_types");
            syncSchema.Tables["Settings"].ForeignKeys.Add("logo_fk", "MediaContent", "id", "Settings", "logo");
            syncSchema.Tables["Settings"].ForeignKeys.Add("question_stylesheet_fk", "StyleSheets", "id", "Settings", "question_stylesheet");
            syncSchema.Tables["Settings"].ForeignKeys.Add("answer_stylesheet_fk", "StyleSheets", "id", "Settings", "answer_stylesheet");
            syncSchema.Tables["Settings"].ForeignKeys.Add("snooze_options_fk", "SnoozeOptions", "id", "Settings", "snooze_options");
            syncSchema.Tables["Settings"].ForeignKeys.Add("cardstyle_fk", "CardStyles", "id", "Settings", "cardstyle");
            syncSchema.Tables["Settings"].ForeignKeys.Add("boxes_fk", "Boxes", "id", "Settings", "boxes");
            # endregion
            # region Categories
            syncSchema.Tables.Add("Categories");
            syncSchema.Tables["Categories"].Columns.Add("id");
            syncSchema.Tables["Categories"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Categories"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Categories"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Categories"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Categories"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["Categories"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Categories"].Columns.Add("global_id").DataType = typeof(int);
            syncSchema.Tables["Categories"].Columns.Add("name").DataType = typeof(string);
            syncSchema.Tables["Categories"].Columns["name"].MaxLength = 100;
            # endregion
            # region LearningModules
            syncSchema.Tables.Add("LearningModules");
            syncSchema.Tables["LearningModules"].Columns.Add("id");
            syncSchema.Tables["LearningModules"].Columns["id"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["LearningModules"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["LearningModules"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["LearningModules"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["LearningModules"].Columns.Add("guid");
            syncSchema.Tables["LearningModules"].Columns["guid"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["guid"].DataType = typeof(string);
            syncSchema.Tables["LearningModules"].Columns["guid"].MaxLength = 36;
            syncSchema.Tables["LearningModules"].Columns.Add("categories_id");
            syncSchema.Tables["LearningModules"].Columns["categories_id"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["categories_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules"].Columns.Add("default_settings_id");
            syncSchema.Tables["LearningModules"].Columns["default_settings_id"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["default_settings_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules"].Columns.Add("allowed_settings_id");
            syncSchema.Tables["LearningModules"].Columns["allowed_settings_id"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["allowed_settings_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules"].Columns.Add("creator_id");
            syncSchema.Tables["LearningModules"].Columns["creator_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules"].Columns.Add("author");
            syncSchema.Tables["LearningModules"].Columns["author"].DataType = typeof(string);
            syncSchema.Tables["LearningModules"].Columns["author"].MaxLength = 8000;
            syncSchema.Tables["LearningModules"].Columns.Add("title");
            syncSchema.Tables["LearningModules"].Columns["title"].AllowNull = false;
            syncSchema.Tables["LearningModules"].Columns["title"].DataType = typeof(string);
            syncSchema.Tables["LearningModules"].Columns["title"].MaxLength = 8000;
            syncSchema.Tables["LearningModules"].Columns.Add("description");
            syncSchema.Tables["LearningModules"].Columns["description"].DataType = typeof(string);
            syncSchema.Tables["LearningModules"].Columns["description"].MaxLength = 8000;
            syncSchema.Tables["LearningModules"].Columns.Add("licence_key", typeof(string));
            syncSchema.Tables["LearningModules"].Columns["licence_key"].MaxLength = 100;
            syncSchema.Tables["LearningModules"].Columns.Add("content_protected", typeof(bool));
            syncSchema.Tables["LearningModules"].Columns.Add("cal_count", typeof(int));
            syncSchema.Tables["LearningModules"].ForeignKeys.Add("category_fk", "Categories", "id", "LearningModules", "categories_id");
            syncSchema.Tables["LearningModules"].ForeignKeys.Add("default_settings_fk", "Settings", "id", "LearningModules", "default_settings_id");
            syncSchema.Tables["LearningModules"].ForeignKeys.Add("allowed_settings_fk", "Settings", "id", "LearningModules", "allowed_settings_id");
            # endregion
            # region Chapters
            syncSchema.Tables.Add("Chapters");
            syncSchema.Tables["Chapters"].Columns.Add("id");
            syncSchema.Tables["Chapters"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Chapters"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Chapters"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Chapters"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Chapters"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["Chapters"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Chapters"].Columns.Add("lm_id").AllowNull = false;
            syncSchema.Tables["Chapters"].Columns["lm_id"].DataType = typeof(int);
            syncSchema.Tables["Chapters"].Columns.Add("title").AllowNull = false;
            syncSchema.Tables["Chapters"].Columns["title"].DataType = typeof(string);
            syncSchema.Tables["Chapters"].Columns["title"].MaxLength = 8000;
            syncSchema.Tables["Chapters"].Columns.Add("description").DataType = typeof(string);
            syncSchema.Tables["Chapters"].Columns["description"].MaxLength = 8000;
            syncSchema.Tables["Chapters"].Columns.Add("position").AllowNull = false;
            syncSchema.Tables["Chapters"].Columns["position"].DataType = typeof(int);
            syncSchema.Tables["Chapters"].Columns.Add("settings_id").DataType = typeof(int);
            syncSchema.Tables["Chapters"].ForeignKeys.Add("lm_id_fk", "LearningModules", "id", "Chapters", "lm_id");
            syncSchema.Tables["Chapters"].ForeignKeys.Add("settings_id_fk", "Settings", "id", "Chapters", "settings_id");
            # endregion
            # region Cards
            syncSchema.Tables.Add("Cards");
            syncSchema.Tables["Cards"].Columns.Add("id");
            syncSchema.Tables["Cards"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Cards"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Cards"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Cards"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Cards"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["Cards"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Cards"].Columns.Add("chapters_id");
            syncSchema.Tables["Cards"].Columns["chapters_id"].AllowNull = false;
            syncSchema.Tables["Cards"].Columns["chapters_id"].DataType = typeof(int);
            syncSchema.Tables["Cards"].Columns["chapters_id"].DefaultValue = "0";
            syncSchema.Tables["Cards"].Columns.Add("lm_id");
            syncSchema.Tables["Cards"].Columns["lm_id"].AllowNull = false;
            syncSchema.Tables["Cards"].Columns["lm_id"].DataType = typeof(int);
            syncSchema.Tables["Cards"].Columns["lm_id"].DefaultValue = "0";
            syncSchema.Tables["Cards"].Columns.Add("settings_id");
            syncSchema.Tables["Cards"].Columns["settings_id"].AllowNull = false;
            syncSchema.Tables["Cards"].Columns["settings_id"].DataType = typeof(int);
            syncSchema.Tables["Cards"].ForeignKeys.Add("settings_fk", "Settings", "id", "Cards", "settings_id");
            # endregion
            # region Chapters_Cards
            syncSchema.Tables.Add("Chapters_Cards");
            syncSchema.Tables["Chapters_Cards"].Columns.Add("chapters_id");
            syncSchema.Tables["Chapters_Cards"].Columns["chapters_id"].AllowNull = false;
            syncSchema.Tables["Chapters_Cards"].Columns["chapters_id"].DataType = typeof(int);
            syncSchema.Tables["Chapters_Cards"].Columns.Add("cards_id");
            syncSchema.Tables["Chapters_Cards"].Columns["cards_id"].AllowNull = false;
            syncSchema.Tables["Chapters_Cards"].Columns["cards_id"].DataType = typeof(int);
            syncSchema.Tables["Chapters_Cards"].PrimaryKey = new string[] { "chapters_id", "cards_id" };
            syncSchema.Tables["Chapters_Cards"].ForeignKeys.Add("chapters_fk", "Chapters", "id", "Chapters_Cards", "chapters_id");
            syncSchema.Tables["Chapters_Cards"].ForeignKeys.Add("cards_fk", "Cards", "id", "Chapters_Cards", "cards_id");
            # endregion
            # region LearningModules_Cards
            syncSchema.Tables.Add("LearningModules_Cards");
            syncSchema.Tables["LearningModules_Cards"].Columns.Add("lm_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["LearningModules_Cards"].Columns.Add("cards_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["LearningModules_Cards"].PrimaryKey = new string[] { "lm_id", "cards_id" };
            syncSchema.Tables["LearningModules_Cards"].ForeignKeys.Add("lm_fk", "LearningModules", "id", "LearningModules_Cards", "lm_id");
            syncSchema.Tables["LearningModules_Cards"].ForeignKeys.Add("cards_fk", "Cards", "id", "LearningModules_Cards", "cards_id");
            # endregion
            # region TextContent
            syncSchema.Tables.Add("TextContent");
            syncSchema.Tables["TextContent"].Columns.Add("id");
            syncSchema.Tables["TextContent"].Columns["id"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["TextContent"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["TextContent"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["TextContent"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["TextContent"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["TextContent"].Columns.Add("cards_id");
            syncSchema.Tables["TextContent"].Columns["cards_id"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["cards_id"].DataType = typeof(int);
            syncSchema.Tables["TextContent"].Columns.Add("text");
            syncSchema.Tables["TextContent"].Columns["text"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["text"].DataType = typeof(string);
            syncSchema.Tables["TextContent"].Columns["text"].MaxLength = 4000;
            syncSchema.Tables["TextContent"].Columns.Add("side");
            syncSchema.Tables["TextContent"].Columns["side"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["side"].DataType = typeof(string);
            syncSchema.Tables["TextContent"].Columns["side"].MaxLength = 100;
            syncSchema.Tables["TextContent"].Columns.Add("type");
            syncSchema.Tables["TextContent"].Columns["type"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["type"].DataType = typeof(string);
            syncSchema.Tables["TextContent"].Columns["type"].MaxLength = 100;
            syncSchema.Tables["TextContent"].Columns.Add("position");
            syncSchema.Tables["TextContent"].Columns["position"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["position"].DataType = typeof(int);
            syncSchema.Tables["TextContent"].Columns.Add("is_default");
            syncSchema.Tables["TextContent"].Columns["is_default"].AllowNull = false;
            syncSchema.Tables["TextContent"].Columns["is_default"].DataType = typeof(bool);
            # endregion
            # region Cards_MediaContent
            syncSchema.Tables.Add("Cards_MediaContent");
            syncSchema.Tables["Cards_MediaContent"].Columns.Add("media_id");
            syncSchema.Tables["Cards_MediaContent"].Columns["media_id"].AllowNull = false;
            syncSchema.Tables["Cards_MediaContent"].Columns["media_id"].DataType = typeof(int);
            syncSchema.Tables["Cards_MediaContent"].Columns.Add("cards_id");
            syncSchema.Tables["Cards_MediaContent"].Columns["cards_id"].AllowNull = false;
            syncSchema.Tables["Cards_MediaContent"].Columns["cards_id"].DataType = typeof(int);
            syncSchema.Tables["Cards_MediaContent"].Columns.Add("side");
            syncSchema.Tables["Cards_MediaContent"].Columns["side"].AllowNull = false;
            syncSchema.Tables["Cards_MediaContent"].Columns["side"].DataType = typeof(string);
            syncSchema.Tables["Cards_MediaContent"].Columns["side"].MaxLength = 100;
            syncSchema.Tables["Cards_MediaContent"].Columns.Add("type");
            syncSchema.Tables["Cards_MediaContent"].Columns["type"].AllowNull = false;
            syncSchema.Tables["Cards_MediaContent"].Columns["type"].DataType = typeof(string);
            syncSchema.Tables["Cards_MediaContent"].Columns["type"].MaxLength = 100;
            syncSchema.Tables["Cards_MediaContent"].PrimaryKey = new string[] { "media_id", "cards_id", "side", "type" };
            syncSchema.Tables["Cards_MediaContent"].Columns.Add("is_default");
            syncSchema.Tables["Cards_MediaContent"].Columns["is_default"].AllowNull = false;
            syncSchema.Tables["Cards_MediaContent"].Columns["is_default"].DataType = typeof(bool);
            syncSchema.Tables["Cards_MediaContent"].ForeignKeys.Add("media_fk", "MediaContent", "id", "Cards_MediaContent", "media_id");
            syncSchema.Tables["Cards_MediaContent"].ForeignKeys.Add("cards_fk", "Cards", "id", "Cards_MediaContent", "cards_id");
            # endregion
            # region Tags
            syncSchema.Tables.Add("Tags");
            syncSchema.Tables["Tags"].Columns.Add("id");
            syncSchema.Tables["Tags"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Tags"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Tags"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Tags"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Tags"].Columns["id"].DataType = typeof(int);
            syncSchema.Tables["Tags"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Tags"].Columns.Add("text");
            syncSchema.Tables["Tags"].Columns["text"].AllowNull = false;
            syncSchema.Tables["Tags"].Columns["text"].DataType = typeof(string);
            syncSchema.Tables["Tags"].Columns["text"].MaxLength = 8000;
            # endregion
            # region MediaContent_Tags
            syncSchema.Tables.Add("MediaContent_Tags");
            syncSchema.Tables["MediaContent_Tags"].Columns.Add("media_id");
            syncSchema.Tables["MediaContent_Tags"].Columns["media_id"].AllowNull = false;
            syncSchema.Tables["MediaContent_Tags"].Columns["media_id"].DataType = typeof(int);
            syncSchema.Tables["MediaContent_Tags"].Columns.Add("tags_id");
            syncSchema.Tables["MediaContent_Tags"].Columns["tags_id"].AllowNull = false;
            syncSchema.Tables["MediaContent_Tags"].Columns["tags_id"].DataType = typeof(int);
            syncSchema.Tables["MediaContent_Tags"].PrimaryKey = new string[] { "media_id", "tags_id" };
            syncSchema.Tables["MediaContent_Tags"].ForeignKeys.Add("media_fk", "MediaContent", "id", "MediaContent_Tags", "media_id");
            syncSchema.Tables["MediaContent_Tags"].ForeignKeys.Add("tags_fk", "Tags", "id", "MediaContent_Tags", "tags_id");
            # endregion
            # region MediaContent_CardStyles
            syncSchema.Tables.Add("MediaContent_CardStyles");
            syncSchema.Tables["MediaContent_CardStyles"].Columns.Add("media_id");
            syncSchema.Tables["MediaContent_CardStyles"].Columns["media_id"].AllowNull = false;
            syncSchema.Tables["MediaContent_CardStyles"].Columns["media_id"].DataType = typeof(int);
            syncSchema.Tables["MediaContent_CardStyles"].Columns.Add("cardstyles_id");
            syncSchema.Tables["MediaContent_CardStyles"].Columns["cardstyles_id"].AllowNull = false;
            syncSchema.Tables["MediaContent_CardStyles"].Columns["cardstyles_id"].DataType = typeof(int);
            syncSchema.Tables["MediaContent_CardStyles"].PrimaryKey = new string[] { "media_id", "cardstyles_id" };
            syncSchema.Tables["MediaContent_CardStyles"].ForeignKeys.Add("media_fk", "MediaContent", "id", "MediaContent_CardStyles", "media_id");
            syncSchema.Tables["MediaContent_CardStyles"].ForeignKeys.Add("cardstyles_fk", "CardStyles", "id", "MediaContent_CardStyles", "cardstyles_id");
            # endregion
            # region LearningModules_Tags
            syncSchema.Tables.Add("LearningModules_Tags");
            syncSchema.Tables["LearningModules_Tags"].Columns.Add("lm_id");
            syncSchema.Tables["LearningModules_Tags"].Columns["lm_id"].AllowNull = false;
            syncSchema.Tables["LearningModules_Tags"].Columns["lm_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules_Tags"].Columns.Add("tags_id");
            syncSchema.Tables["LearningModules_Tags"].Columns["tags_id"].AllowNull = false;
            syncSchema.Tables["LearningModules_Tags"].Columns["tags_id"].DataType = typeof(int);
            syncSchema.Tables["LearningModules_Tags"].PrimaryKey = new string[] { "lm_id", "tags_id" };
            syncSchema.Tables["LearningModules_Tags"].ForeignKeys.Add("lm_fk", "LearningModules", "id", "LearningModules_Tags", "lm_id");
            syncSchema.Tables["LearningModules_Tags"].ForeignKeys.Add("tags_fk", "Tags", "id", "LearningModules_Tags", "tags_id");
            # endregion
            # region CommentarySounds
            syncSchema.Tables.Add("CommentarySounds");
            syncSchema.Tables["CommentarySounds"].Columns.Add("media_id").DataType = typeof(int);
            syncSchema.Tables["CommentarySounds"].Columns["media_id"].AllowNull = false;
            syncSchema.Tables["CommentarySounds"].Columns.Add("settings_id").DataType = typeof(int);
            syncSchema.Tables["CommentarySounds"].Columns["settings_id"].AllowNull = false;
            syncSchema.Tables["CommentarySounds"].Columns.Add("side").DataType = typeof(string);
            syncSchema.Tables["CommentarySounds"].Columns["side"].AllowNull = false;
            syncSchema.Tables["CommentarySounds"].Columns["side"].MaxLength = 100;
            syncSchema.Tables["CommentarySounds"].Columns.Add("type").DataType = typeof(string);
            syncSchema.Tables["CommentarySounds"].Columns["type"].AllowNull = false;
            syncSchema.Tables["CommentarySounds"].Columns["type"].MaxLength = 100;
            syncSchema.Tables["CommentarySounds"].PrimaryKey = new string[] { "media_id", "settings_id", "side", "type" };
            syncSchema.Tables["CommentarySounds"].ForeignKeys.Add("media_fk", "MediaContent", "id", "CommentarySounds", "media_id");
            syncSchema.Tables["CommentarySounds"].ForeignKeys.Add("settings_fk", "Settings", "id", "CommentarySounds", "settings_id");
            # endregion
            # region SelectedLearnChapters
            syncSchema.Tables.Add("SelectedLearnChapters");
            syncSchema.Tables["SelectedLearnChapters"].Columns.Add("chapters_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["SelectedLearnChapters"].Columns.Add("settings_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["SelectedLearnChapters"].PrimaryKey = new string[] { "chapters_id", "settings_id" };
            syncSchema.Tables["SelectedLearnChapters"].ForeignKeys.Add("chapters_fk", "Chapters", "id", "SelectedLearnChapters", "chapters_id");
            syncSchema.Tables["SelectedLearnChapters"].ForeignKeys.Add("settings_fk", "Settings", "id", "SelectedLearnChapters", "settings_id");
            # endregion
            # region UserProfiles
            syncSchema.Tables.Add("UserProfiles");
            syncSchema.Tables["UserProfiles"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["UserProfiles"].Columns["id"].AllowNull = false;
            syncSchema.Tables["UserProfiles"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["UserProfiles"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["UserProfiles"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["UserProfiles"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["UserProfiles"].Columns.Add("username").DataType = typeof(string);
            syncSchema.Tables["UserProfiles"].Columns["username"].AllowNull = false;
            syncSchema.Tables["UserProfiles"].Columns["username"].MaxLength = 100;
            syncSchema.Tables["UserProfiles"].Columns.Add("password").DataType = typeof(string);
            syncSchema.Tables["UserProfiles"].Columns["password"].MaxLength = 100;
            syncSchema.Tables["UserProfiles"].Columns.Add("local_directory_id").DataType = typeof(string);
            syncSchema.Tables["UserProfiles"].Columns["local_directory_id"].MaxLength = 100;
            syncSchema.Tables["UserProfiles"].Columns.Add("user_type").DataType = typeof(string);
            syncSchema.Tables["UserProfiles"].Columns["user_type"].AllowNull = false;
            syncSchema.Tables["UserProfiles"].Columns["user_type"].MaxLength = 100;
            syncSchema.Tables["UserProfiles"].Columns.Add("enabled");
            syncSchema.Tables["UserProfiles"].Columns["enabled"].AllowNull = false;
            syncSchema.Tables["UserProfiles"].Columns["enabled"].DataType = typeof(bool);
            # endregion
            # region UserGroups
            syncSchema.Tables.Add("UserGroups");
            syncSchema.Tables["UserGroups"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["UserGroups"].Columns["id"].AllowNull = false;
            syncSchema.Tables["UserGroups"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["UserGroups"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["UserGroups"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["UserGroups"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["UserGroups"].Columns.Add("name").DataType = typeof(string);
            syncSchema.Tables["UserGroups"].Columns["name"].AllowNull = false;
            syncSchema.Tables["UserGroups"].Columns["name"].MaxLength = 100;
            # endregion
            # region UserProfiles_UserGroups
            syncSchema.Tables.Add("UserProfiles_UserGroups");
            syncSchema.Tables["UserProfiles_UserGroups"].Columns.Add("users_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserProfiles_UserGroups"].Columns.Add("groups_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserProfiles_UserGroups"].PrimaryKey = new string[] { "users_id", "groups_id" };
            syncSchema.Tables["UserProfiles_UserGroups"].ForeignKeys.Add("users_id_fk", "UserProfiles", "id", "UserProfiles_UserGroups", "users_id");
            syncSchema.Tables["UserProfiles_UserGroups"].ForeignKeys.Add("groups_id_fk", "UserGroups", "id", "UserProfiles_UserGroups", "groups_id");
            # endregion
            # region TypeDefinitions
            syncSchema.Tables.Add("TypeDefinitions");
            syncSchema.Tables["TypeDefinitions"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["TypeDefinitions"].Columns["id"].AllowNull = false;
            syncSchema.Tables["TypeDefinitions"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["TypeDefinitions"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["TypeDefinitions"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["TypeDefinitions"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["TypeDefinitions"].Columns.Add("clr_name").DataType = typeof(string);
            syncSchema.Tables["TypeDefinitions"].Columns["clr_name"].AllowNull = false;
            syncSchema.Tables["TypeDefinitions"].Columns["clr_name"].MaxLength = 1000;
            syncSchema.Tables["TypeDefinitions"].Columns.Add("parent_id").DataType = typeof(int);
            # endregion
            # region Permissions
            syncSchema.Tables.Add("Permissions");
            syncSchema.Tables["Permissions"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["Permissions"].Columns["id"].AllowNull = false;
            syncSchema.Tables["Permissions"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["Permissions"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["Permissions"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["Permissions"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["Permissions"].Columns.Add("types_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["Permissions"].Columns.Add("name").DataType = typeof(string);
            syncSchema.Tables["Permissions"].Columns["name"].AllowNull = true;
            syncSchema.Tables["Permissions"].Columns["name"].MaxLength = 100;
            syncSchema.Tables["Permissions"].Columns.Add("default");
            syncSchema.Tables["Permissions"].Columns["default"].AllowNull = false;
            syncSchema.Tables["Permissions"].Columns["default"].DataType = typeof(bool);
            syncSchema.Tables["Permissions"].ForeignKeys.Add("types_id_fk", "TypeDefinitions", "id", "Permissions", "types_id");
            # endregion
            # region ObjectList
            syncSchema.Tables.Add("ObjectList");
            syncSchema.Tables["ObjectList"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["ObjectList"].Columns["id"].AllowNull = false;
            syncSchema.Tables["ObjectList"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["ObjectList"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["ObjectList"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["ObjectList"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["ObjectList"].Columns.Add("locator").DataType = typeof(string);
            syncSchema.Tables["ObjectList"].Columns["locator"].AllowNull = false;
            syncSchema.Tables["ObjectList"].Columns["locator"].MaxLength = 100;
            syncSchema.Tables["ObjectList"].Columns.Add("parent_id").DataType = typeof(int);
            # endregion
            # region AccessControlList
            syncSchema.Tables.Add("AccessControlList");
            syncSchema.Tables["AccessControlList"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["AccessControlList"].Columns["id"].AllowNull = false;
            syncSchema.Tables["AccessControlList"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["AccessControlList"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["AccessControlList"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["AccessControlList"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["AccessControlList"].Columns.Add("object_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["AccessControlList"].Columns.Add("permissions_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["AccessControlList"].Columns.Add("access");
            syncSchema.Tables["AccessControlList"].Columns["access"].AllowNull = false;
            syncSchema.Tables["AccessControlList"].Columns["access"].DataType = typeof(bool);
            syncSchema.Tables["AccessControlList"].ForeignKeys.Add("object_id_fk", "ObjectList", "id", "AccessControlList", "object_id");
            syncSchema.Tables["AccessControlList"].ForeignKeys.Add("permissions_id_fk", "Permissions", "id", "AccessControlList", "permissions_id");
            # endregion
            # region UserProfiles_AccessControlList
            syncSchema.Tables.Add("UserProfiles_AccessControlList");
            syncSchema.Tables["UserProfiles_AccessControlList"].Columns.Add("users_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserProfiles_AccessControlList"].Columns.Add("acl_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserProfiles_AccessControlList"].PrimaryKey = new string[] { "users_id", "acl_id" };
            syncSchema.Tables["UserProfiles_AccessControlList"].ForeignKeys.Add("users_id_fk", "UserProfiles", "id", "UserProfiles_AccessControlList", "users_id");
            syncSchema.Tables["UserProfiles_AccessControlList"].ForeignKeys.Add("acl_id_fk", "AccessControlList", "id", "UserProfiles_AccessControlList", "acl_id");
            # endregion
            # region UserGroups_AccessControlList
            syncSchema.Tables.Add("UserGroups_AccessControlList");
            syncSchema.Tables["UserGroups_AccessControlList"].Columns.Add("groups_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserGroups_AccessControlList"].Columns.Add("acl_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["UserGroups_AccessControlList"].PrimaryKey = new string[] { "groups_id", "acl_id" };
            syncSchema.Tables["UserGroups_AccessControlList"].ForeignKeys.Add("groups_id_fk", "UserGroups", "id", "UserGroups_AccessControlList", "groups_id");
            syncSchema.Tables["UserGroups_AccessControlList"].ForeignKeys.Add("acl_id_fk", "AccessControlList", "id", "UserGroups_AccessControlList", "acl_id");
            # endregion
            # region UserCardState
            syncSchema.Tables.Add("UserCardState");
            syncSchema.Tables["UserCardState"].Columns.Add("user_id").DataType = typeof(int);
            syncSchema.Tables["UserCardState"].Columns["user_id"].AllowNull = false;
            syncSchema.Tables["UserCardState"].Columns.Add("cards_id").DataType = typeof(int);
            syncSchema.Tables["UserCardState"].Columns["cards_id"].AllowNull = false;
            syncSchema.Tables["UserCardState"].PrimaryKey = new string[] { "user_id", "cards_id" };
            syncSchema.Tables["UserCardState"].Columns.Add("box").DataType = typeof(int);
            syncSchema.Tables["UserCardState"].Columns.Add("active").DataType = typeof(bool);
            syncSchema.Tables["UserCardState"].Columns.Add("timestamp").DataType = typeof(DateTime);
            syncSchema.Tables["UserCardState"].ForeignKeys.Add("user_fk", "UserProfiles", "id", "UserCardState", "user_id");
            syncSchema.Tables["UserCardState"].ForeignKeys.Add("cards_fk", "Cards", "id", "UserCardState", "cards_id");
            # endregion
            # region UserProfileLearningModulesSettings
            syncSchema.Tables.Add("UserProfilesLearningModulesSettings");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns.Add("user_id");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["user_id"].DataType = typeof(int);
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["user_id"].AllowNull = false;
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns.Add("lm_id");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["lm_id"].DataType = typeof(int);
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["lm_id"].AllowNull = false;
            syncSchema.Tables["UserProfilesLearningModulesSettings"].PrimaryKey = new string[] { "user_id", "lm_id" };
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns.Add("settings_id");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["settings_id"].DataType = typeof(int);
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["settings_id"].AllowNull = false;
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns.Add("highscore");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["highscore"].DataType = typeof(decimal);
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["highscore"].ProviderDataType = NpgsqlDbType.Numeric.ToString();
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["highscore"].NumericPrecision = 18;
            syncSchema.Tables["UserProfilesLearningModulesSettings"].Columns["highscore"].NumericScale = 10;
            syncSchema.Tables["UserProfilesLearningModulesSettings"].ForeignKeys.Add("user_fk", "UserProfiles", "id", "UserProfilesLearningModulesSettings", "user_id");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].ForeignKeys.Add("lm_fk", "LearningModules", "id", "UserProfilesLearningModulesSettings", "lm_id");
            syncSchema.Tables["UserProfilesLearningModulesSettings"].ForeignKeys.Add("settings_fk", "Settings", "id", "UserProfilesLearningModulesSettings", "settings_id");
            # endregion
            # region LearningSessions
            syncSchema.Tables.Add("LearningSessions");
            syncSchema.Tables["LearningSessions"].Columns.Add("id", typeof(int)).AllowNull = false;
            syncSchema.Tables["LearningSessions"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["LearningSessions"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["LearningSessions"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["LearningSessions"].Columns.Add("user_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["LearningSessions"].Columns.Add("lm_id", typeof(int)).AllowNull = false;
            syncSchema.Tables["LearningSessions"].PrimaryKey = new string[] { "id", "user_id", "lm_id" };
            syncSchema.Tables["LearningSessions"].Columns.Add("starttime", typeof(DateTime)).AllowNull = false;
            syncSchema.Tables["LearningSessions"].Columns.Add("endtime").DataType = typeof(DateTime);
            syncSchema.Tables["LearningSessions"].Columns.Add("sum_right", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("sum_wrong", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("pool_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box1_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box2_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box3_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box4_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box5_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box6_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box7_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box8_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box9_content", typeof(int));
            syncSchema.Tables["LearningSessions"].Columns.Add("box10_content", typeof(int));
            syncSchema.Tables["LearningSessions"].ForeignKeys.Add("user_fk", "UserProfiles", "id", "LearningSessions", "user_id");
            syncSchema.Tables["LearningSessions"].ForeignKeys.Add("lm_fk", "LearningModules", "id", "LearningSessions", "lm_id");
            # endregion
            # region LearnLog
            syncSchema.Tables.Add("LearnLog");
            syncSchema.Tables["LearnLog"].Columns.Add("id").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns["id"].AllowNull = false;
            syncSchema.Tables["LearnLog"].Columns["id"].AutoIncrement = true;
            syncSchema.Tables["LearnLog"].Columns["id"].AutoIncrementSeed = 1;
            syncSchema.Tables["LearnLog"].Columns["id"].AutoIncrementStep = 1;
            syncSchema.Tables["LearnLog"].PrimaryKey = new string[] { "id" };
            syncSchema.Tables["LearnLog"].Columns.Add("session_id").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns["session_id"].AllowNull = false;
            syncSchema.Tables["LearnLog"].Columns.Add("user_id").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns["user_id"].AllowNull = false;
            syncSchema.Tables["LearnLog"].Columns.Add("lm_id").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns["lm_id"].AllowNull = false;
            syncSchema.Tables["LearnLog"].Columns.Add("cards_id").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns["cards_id"].AllowNull = false;
            syncSchema.Tables["LearnLog"].Columns.Add("old_box").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns.Add("new_box").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns.Add("timestamp").DataType = typeof(DateTime);
            syncSchema.Tables["LearnLog"].Columns.Add("duration").DataType = typeof(long);
            syncSchema.Tables["LearnLog"].Columns.Add("learn_mode").DataType = typeof(string);
            syncSchema.Tables["LearnLog"].Columns["learn_mode"].MaxLength = 100;
            syncSchema.Tables["LearnLog"].Columns.Add("move_type").DataType = typeof(string);
            syncSchema.Tables["LearnLog"].Columns["move_type"].MaxLength = 100;
            syncSchema.Tables["LearnLog"].Columns.Add("answer").DataType = typeof(string);
            syncSchema.Tables["LearnLog"].Columns["answer"].MaxLength = 8000;
            syncSchema.Tables["LearnLog"].Columns.Add("direction").DataType = typeof(string);
            syncSchema.Tables["LearnLog"].Columns["direction"].MaxLength = 100;
            syncSchema.Tables["LearnLog"].Columns.Add("case_sensitive").DataType = typeof(bool);
            syncSchema.Tables["LearnLog"].Columns.Add("correct_on_the_fly").DataType = typeof(bool);
            syncSchema.Tables["LearnLog"].Columns.Add("percentage_known").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].Columns.Add("percentage_required").DataType = typeof(int);
            syncSchema.Tables["LearnLog"].ForeignKeys.Add("session_fk", "LearningSessions", "id", "LearnLog", "session_id");
            //ToDo: Necessary because the sync framework creates an unique column otherwise
            //syncSchema.Tables["LearnLog"].ForeignKeys.Add("user_fk", "LearningSessions", "user_id", "LearnLog", "user_id");
            //syncSchema.Tables["LearnLog"].ForeignKeys.Add("lm_fk", "LearningSessions", "lm_id", "LearnLog", "lm_id");
            syncSchema.Tables["LearnLog"].ForeignKeys.Add("cards_fk", "Cards", "id", "LearnLog", "cards_id");
            # endregion
            # region DatabaseInformation
            syncSchema.Tables.Add("DatabaseInformation");
            syncSchema.Tables["DatabaseInformation"].Columns.Add("property");
            syncSchema.Tables["DatabaseInformation"].Columns["property"].AllowNull = false;
            syncSchema.Tables["DatabaseInformation"].Columns["property"].AutoIncrement = false;
            syncSchema.Tables["DatabaseInformation"].Columns["property"].DataType = typeof(string);
            syncSchema.Tables["DatabaseInformation"].Columns["property"].MaxLength = 100;
            syncSchema.Tables["DatabaseInformation"].PrimaryKey = new string[] { "property" };
            syncSchema.Tables["DatabaseInformation"].Columns.Add("value", typeof(string)).MaxLength = 100;
            # endregion
            #region Extensions
            syncSchema.Tables.Add("Extensions");

            syncSchema.Tables["Extensions"].Columns.Add("guid");
            syncSchema.Tables["Extensions"].Columns["guid"].DataType = typeof(string);
            syncSchema.Tables["Extensions"].Columns["guid"].MaxLength = 36;
            syncSchema.Tables["Extensions"].Columns["guid"].AllowNull = false;
            syncSchema.Tables["Extensions"].PrimaryKey = new string[] { "guid" };

            syncSchema.Tables["Extensions"].Columns.Add("lm_id");
            syncSchema.Tables["Extensions"].Columns["lm_id"].DataType = typeof(int);
            syncSchema.Tables["Extensions"].ForeignKeys.Add("lm_id_fk", "LearningModules", "id", "Extensions", "lm_id");

            syncSchema.Tables["Extensions"].Columns.Add("name").AllowNull = false;
            syncSchema.Tables["Extensions"].Columns["name"].DataType = typeof(string);
            syncSchema.Tables["Extensions"].Columns["name"].MaxLength = 8000;

            syncSchema.Tables["Extensions"].Columns.Add("version").AllowNull = false;
            syncSchema.Tables["Extensions"].Columns["version"].DataType = typeof(string);
            syncSchema.Tables["Extensions"].Columns["version"].MaxLength = 10;

            syncSchema.Tables["Extensions"].Columns.Add("type").AllowNull = false;
            syncSchema.Tables["Extensions"].Columns["type"].DataType = typeof(string);
            syncSchema.Tables["Extensions"].Columns["type"].MaxLength = 100;

            syncSchema.Tables["Extensions"].Columns.Add("data").AllowNull = true;
            syncSchema.Tables["Extensions"].Columns["data"].ProviderDataType = "Image";

            syncSchema.Tables["Extensions"].Columns.Add("startfile").DataType = typeof(string);
            syncSchema.Tables["Extensions"].Columns["startfile"].MaxLength = 8000;
            #endregion
            #region ExtensionActions
            syncSchema.Tables.Add("ExtensionActions");

            syncSchema.Tables["ExtensionActions"].Columns.Add("guid");
            syncSchema.Tables["ExtensionActions"].Columns["guid"].DataType = typeof(string);
            syncSchema.Tables["ExtensionActions"].Columns["guid"].MaxLength = 36;
            syncSchema.Tables["ExtensionActions"].Columns["guid"].AllowNull = false;

            syncSchema.Tables["ExtensionActions"].Columns.Add("action");
            syncSchema.Tables["ExtensionActions"].Columns["action"].DataType = typeof(string);
            syncSchema.Tables["ExtensionActions"].Columns["action"].MaxLength = 100;
            syncSchema.Tables["ExtensionActions"].Columns["action"].AllowNull = false;
            syncSchema.Tables["ExtensionActions"].PrimaryKey = new string[] { "guid", "action" };
            syncSchema.Tables["ExtensionActions"].ForeignKeys.Add("guid_fk", "Extensions", "guid", "ExtensionActions", "guid");

            syncSchema.Tables["ExtensionActions"].Columns.Add("execution").DataType = typeof(string);
            syncSchema.Tables["ExtensionActions"].Columns["execution"].MaxLength = 100;
            syncSchema.Tables["ExtensionActions"].Columns["execution"].AllowNull = false;
            #endregion
        }
        private void PrepareAdapters()
        {
            # region LearningModules
            SyncAdapter adapterLearningModules = new SyncAdapter("LearningModules");
            # region incremental insert command
            NpgsqlCommand incInsLearningModulesCmd = new NpgsqlCommand();
            incInsLearningModulesCmd.CommandType = CommandType.Text;
            incInsLearningModulesCmd.CommandText = "SELECT id, guid, categories_id, default_settings_id, allowed_settings_id, creator_id, author, title, description, content_protected " +
                                          "FROM \"LearningModules\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND id=:session_lm_id;";
            incInsLearningModulesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModulesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModulesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsLearningModulesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterLearningModules.SelectIncrementalInsertsCommand = incInsLearningModulesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdLearningModulesCmd = incInsLearningModulesCmd.Clone();
            incUpdLearningModulesCmd.CommandText = "SELECT id, guid, categories_id, default_settings_id, allowed_settings_id, creator_id, author, title, description, content_protected " +
                                          "FROM \"LearningModules\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND id=:session_lm_id;";
            incUpdLearningModulesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearningModulesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearningModulesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdLearningModulesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterLearningModules.SelectIncrementalUpdatesCommand = incUpdLearningModulesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelLearningModulesCmd = incInsLearningModulesCmd.Clone();
            incDelLearningModulesCmd.CommandText = "SELECT id FROM \"LearningModules_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelLearningModulesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModulesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModulesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelLearningModulesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterLearningModules.SelectIncrementalDeletesCommand = incDelLearningModulesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterLearningModules);
            # endregion
            # region Categories
            SyncAdapter adapterCategories = new SyncAdapter("Categories");
            # region incremental insert command
            NpgsqlCommand incInsCategoriesCmd = new NpgsqlCommand();
            incInsCategoriesCmd.CommandType = CommandType.Text;
            incInsCategoriesCmd.CommandText = "SELECT id, global_id, name " +
                                          "FROM \"Categories\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT categories_id FROM \"LearningModules\" WHERE id=:session_lm_id);";
            incInsCategoriesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCategoriesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCategoriesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsCategoriesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCategories.SelectIncrementalInsertsCommand = incInsCategoriesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdCategoriesCmd = incInsCategoriesCmd.Clone();
            incUpdCategoriesCmd.CommandText = "SELECT id,  global_id, name " +
                                          "FROM \"Categories\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT categories_id FROM \"LearningModules\" WHERE id=:session_lm_id);";
            incUpdCategoriesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCategoriesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCategoriesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdCategoriesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCategories.SelectIncrementalUpdatesCommand = incUpdCategoriesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelCategoriesCmd = incInsCategoriesCmd.Clone();
            incDelCategoriesCmd.CommandText = "SELECT id FROM \"Categories_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelCategoriesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCategoriesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCategoriesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelCategoriesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterCategories.SelectIncrementalDeletesCommand = incDelCategoriesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterCategories);
            # endregion
            # region Chapters
            SyncAdapter adapterChapters = new SyncAdapter("Chapters");
            # region incremental insert command
            NpgsqlCommand incInsChaptersCmd = new NpgsqlCommand();
            incInsChaptersCmd.CommandType = CommandType.Text;
            incInsChaptersCmd.CommandText = "SELECT id, lm_id, title, description, position, settings_id " +
                                          "FROM \"Chapters\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND lm_id=:session_lm_id;";
            incInsChaptersCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsChaptersCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsChaptersCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterChapters.SelectIncrementalInsertsCommand = incInsChaptersCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdChaptersCmd = incInsChaptersCmd.Clone();
            incUpdChaptersCmd.CommandText = "SELECT id, lm_id, title, description, position, settings_id " +
                                          "FROM \"Chapters\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND lm_id=:session_lm_id;";
            incUpdChaptersCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdChaptersCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdChaptersCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterChapters.SelectIncrementalUpdatesCommand = incUpdChaptersCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelChaptersCmd = incInsChaptersCmd.Clone();
            incDelChaptersCmd.CommandText = "SELECT id FROM \"Chapters_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelChaptersCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelChaptersCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelChaptersCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterChapters.SelectIncrementalDeletesCommand = incDelChaptersCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterChapters);
            # endregion
            # region Chapters_Cards
            SyncAdapter adapterChapters_Cards = new SyncAdapter("Chapters_Cards");
            # region incremental insert command
            NpgsqlCommand incInsChapters_CardsCmd = new NpgsqlCommand();
            incInsChapters_CardsCmd.CommandType = CommandType.Text;
            incInsChapters_CardsCmd.CommandText = "SELECT chapters_id, cards_id " +
                                          "FROM \"Chapters_Cards\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND chapters_id IN (SELECT id FROM \"Chapters\" WHERE lm_id=:session_lm_id);";
            incInsChapters_CardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsChapters_CardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsChapters_CardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsChapters_CardsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterChapters_Cards.SelectIncrementalInsertsCommand = incInsChapters_CardsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelChapters_CardsCmd = incInsChapters_CardsCmd.Clone();
            incDelChapters_CardsCmd.CommandText = "SELECT chapters_id, cards_id FROM \"Chapters_Cards_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelChapters_CardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelChapters_CardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelChapters_CardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelChapters_CardsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterChapters_Cards.SelectIncrementalDeletesCommand = incDelChapters_CardsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterChapters_Cards);
            # endregion
            # region Cards
            SyncAdapter adapterCards = new SyncAdapter("Cards");
            # region incremental insert command
            NpgsqlCommand incInsCardsCmd = new NpgsqlCommand();
            incInsCardsCmd.CommandType = CommandType.Text;
            incInsCardsCmd.CommandText = "SELECT id, chapters_id, lm_id, settings_id " +
                                          "FROM \"Cards\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incInsCardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsCardsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCards.SelectIncrementalInsertsCommand = incInsCardsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdCardsCmd = incInsCardsCmd.Clone();
            incUpdCardsCmd.CommandText = "SELECT id, chapters_id, lm_id, settings_id " +
                                          "FROM \"Cards\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incUpdCardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdCardsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCards.SelectIncrementalUpdatesCommand = incUpdCardsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelCardsCmd = incInsCardsCmd.Clone();
            incDelCardsCmd.CommandText = "SELECT id FROM \"Cards_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelCardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelCardsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterCards.SelectIncrementalDeletesCommand = incDelCardsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterCards);
            # endregion
            # region LearningModules_Cards
            SyncAdapter adapterLearningModules_Cards = new SyncAdapter("LearningModules_Cards");
            # region incremental insert command
            NpgsqlCommand incInsLearningModules_CardsCmd = new NpgsqlCommand();
            incInsLearningModules_CardsCmd.CommandType = CommandType.Text;
            incInsLearningModules_CardsCmd.CommandText = "SELECT lm_id, cards_id " +
                                          "FROM \"LearningModules_Cards\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND lm_id=:session_lm_id;";
            incInsLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsLearningModules_CardsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterLearningModules_Cards.SelectIncrementalInsertsCommand = incInsLearningModules_CardsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelLearningModules_CardsCmd = incInsLearningModules_CardsCmd.Clone();
            incDelLearningModules_CardsCmd.CommandText = "SELECT lm_id, cards_id FROM \"LearningModules_Cards_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModules_CardsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelLearningModules_CardsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterLearningModules_Cards.SelectIncrementalDeletesCommand = incDelLearningModules_CardsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterLearningModules_Cards);
            # endregion
            # region TextContent
            SyncAdapter adapterTextContent = new SyncAdapter("TextContent");
            # region incremental insert command
            NpgsqlCommand incInsTextContentCmd = new NpgsqlCommand();
            incInsTextContentCmd.CommandType = CommandType.Text;
            incInsTextContentCmd.CommandText = "SELECT id, cards_id, text, side, type, position, is_default " +
                                          "FROM \"TextContent\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incInsTextContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTextContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTextContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsTextContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterTextContent.SelectIncrementalInsertsCommand = incInsTextContentCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdTextContentCmd = incInsTextContentCmd.Clone();
            incUpdTextContentCmd.CommandText = "SELECT id, cards_id, text, side, type, position, is_default " +
                                          "FROM \"TextContent\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incUpdTextContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTextContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTextContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdTextContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterTextContent.SelectIncrementalUpdatesCommand = incUpdTextContentCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelTextContentCmd = incInsTextContentCmd.Clone();
            incDelTextContentCmd.CommandText = "SELECT id FROM \"TextContent_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelTextContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTextContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTextContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelTextContentCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterTextContent.SelectIncrementalDeletesCommand = incDelTextContentCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterTextContent);
            # endregion
            # region Cards_MediaContent
            SyncAdapter adapterCards_MediaContent = new SyncAdapter("Cards_MediaContent");
            # region incremental insert command
            NpgsqlCommand incInsCards_MediaContentCmd = new NpgsqlCommand();
            incInsCards_MediaContentCmd.CommandType = CommandType.Text;
            incInsCards_MediaContentCmd.CommandText = "SELECT media_id, cards_id, side, type, is_default " +
                                          "FROM \"Cards_MediaContent\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incInsCards_MediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCards_MediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCards_MediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsCards_MediaContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCards_MediaContent.SelectIncrementalInsertsCommand = incInsCards_MediaContentCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdCards_MediaContentCmd = incInsCards_MediaContentCmd.Clone();
            incUpdCards_MediaContentCmd.CommandText = "SELECT media_id, cards_id, side, type, is_default " +
                                          "FROM \"Cards_MediaContent\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incUpdCards_MediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCards_MediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCards_MediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdCards_MediaContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterCards_MediaContent.SelectIncrementalUpdatesCommand = incUpdCards_MediaContentCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelCards_MediaContentCmd = incInsCards_MediaContentCmd.Clone();
            incDelCards_MediaContentCmd.CommandText = "SELECT media_id, cards_id, side, type FROM \"Cards_MediaContent_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelCards_MediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCards_MediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCards_MediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelCards_MediaContentCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterCards_MediaContent.SelectIncrementalDeletesCommand = incDelCards_MediaContentCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterCards_MediaContent);
            # endregion
            # region MediaContent
            SyncAdapter adapterMediaContent = new SyncAdapter("MediaContent");
            # region incremental insert command
            NpgsqlCommand incInsMediaContentCmd = new NpgsqlCommand();
            incInsMediaContentCmd.CommandType = CommandType.Text;
            incInsMediaContentCmd.CommandText = "SELECT id, media_type " +
                                          "FROM \"MediaContent\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (id IN " +
                                            "(" +
                                                     "(SELECT media_id FROM \"Cards_MediaContent\" WHERE cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                               "UNION (SELECT media_id FROM \"CommentarySounds\" WHERE settings_id IN " +
                                                           "(" +
                                                                   "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                                             "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                             "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)" +
                                                           ") " +
                                                      ") " +
                                               "UNION (SELECT logo FROM \"Settings\" WHERE \"Settings\".id IN " +
                                                           "(" +
                                                                   "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                                             "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                             "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)" +
                                                           ") " +
                                                      ") " +
                                            ")" +
                                           ");";
            incInsMediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsMediaContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsMediaContentCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMediaContent.SelectIncrementalInsertsCommand = incInsMediaContentCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdMediaContentCmd = incInsMediaContentCmd.Clone();
            incUpdMediaContentCmd.CommandText = "SELECT id, media_type " +
                                          "FROM \"MediaContent\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (id IN " +
                                            "(" +
                                                     "(SELECT media_id FROM \"Cards_MediaContent\" WHERE cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                               "UNION (SELECT media_id FROM \"CommentarySounds\" WHERE settings_id IN " +
                                                           "(" +
                                                                   "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                                             "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                             "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)" +
                                                           ") " +
                                                      ") " +
                                               "UNION (SELECT logo FROM \"Settings\" WHERE \"Settings\".id IN " +
                                                           "(" +
                                                                   "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                                             "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                             "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                             "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)" +
                                                           ") " +
                                                      ") " +
                                            ")" +
                                           ");";
            incUpdMediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdMediaContentCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdMediaContentCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMediaContent.SelectIncrementalUpdatesCommand = incUpdMediaContentCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelMediaContentCmd = incInsMediaContentCmd.Clone();
            incDelMediaContentCmd.CommandText = "SELECT id FROM \"MediaContent_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelMediaContentCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContentCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContentCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelMediaContentCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterMediaContent.SelectIncrementalDeletesCommand = incDelMediaContentCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterMediaContent);
            # endregion
            # region MediaProperties
            SyncAdapter adapterMediaProperties = new SyncAdapter("MediaProperties");
            # region incremental insert command
            NpgsqlCommand incInsMediaPropertiesCmd = new NpgsqlCommand();
            incInsMediaPropertiesCmd.CommandType = CommandType.Text;
            incInsMediaPropertiesCmd.CommandText = "SELECT media_id, property, value " +
                                          "FROM \"MediaProperties\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND media_id IN (SELECT media_id FROM \"Cards_MediaContent\" WHERE cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +

                                            "UNION (SELECT media_id FROM \"CommentarySounds\" WHERE settings_id IN " +
                                                "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +
                                                "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)))" +

                                            "UNION (SELECT media_id FROM \"MediaContent_CardStyles\" WHERE cardstyles_id IN " +
                                                "(SELECT cardstyle FROM \"Settings\" WHERE id IN " +
                                                    "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +
                                                    "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                    "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                    "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                    "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)))))";
            incInsMediaPropertiesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaPropertiesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaPropertiesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsMediaPropertiesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsMediaPropertiesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMediaProperties.SelectIncrementalInsertsCommand = incInsMediaPropertiesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdMediaPropertiesCmd = incInsMediaPropertiesCmd.Clone();
            incUpdMediaPropertiesCmd.CommandText = "SELECT media_id, property, value " +
                                          "FROM \"MediaProperties\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND media_id IN (SELECT media_id FROM \"Cards_MediaContent\" WHERE cards_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +

                                            "UNION (SELECT media_id FROM \"CommentarySounds\" WHERE settings_id IN " +
                                                "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +
                                                "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)))" +

                                            "UNION (SELECT media_id FROM \"MediaContent_CardStyles\" WHERE cardstyles_id IN " +
                                                "(SELECT cardstyle FROM \"Settings\" WHERE id IN " +
                                                    "(SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id) " +
                                                    "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                                    "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                    "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                                    "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)))))";
            incUpdMediaPropertiesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMediaPropertiesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMediaPropertiesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdMediaPropertiesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdMediaPropertiesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMediaProperties.SelectIncrementalUpdatesCommand = incUpdMediaPropertiesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelMediaPropertiesCmd = incInsMediaPropertiesCmd.Clone();
            incDelMediaPropertiesCmd.CommandText = "SELECT media_id, property FROM \"MediaProperties_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelMediaPropertiesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaPropertiesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaPropertiesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelMediaPropertiesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterMediaProperties.SelectIncrementalDeletesCommand = incDelMediaPropertiesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterMediaProperties);
            # endregion
            # region MediaContent_Tags
            SyncAdapter adapterMediaContent_Tags = new SyncAdapter("MediaContent_Tags");
            # region incremental insert command
            NpgsqlCommand incInsMediaContent_TagsCmd = new NpgsqlCommand();
            incInsMediaContent_TagsCmd.CommandType = CommandType.Text;
            incInsMediaContent_TagsCmd.CommandText = "SELECT media_id, tags_id " +
                                          "FROM \"MediaContent_Tags\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND media_id IN (SELECT media_id FROM \"Cards_MediaContent\" WHERE media_id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id));";
            incInsMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsMediaContent_TagsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterMediaContent_Tags.SelectIncrementalInsertsCommand = incInsMediaContent_TagsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelMediaContent_TagsCmd = incInsMediaContent_TagsCmd.Clone();
            incDelMediaContent_TagsCmd.CommandText = "SELECT media_id, tags_id FROM \"MediaContent_Tags_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContent_TagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelMediaContent_TagsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterMediaContent_Tags.SelectIncrementalDeletesCommand = incDelMediaContent_TagsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterMediaContent_Tags);
            # endregion
            # region MediaContent_CardStyles
            SyncAdapter adapterMediaContent_CardStyles = new SyncAdapter("MediaContent_CardStyles");
            # region incremental insert command
            NpgsqlCommand incInsMediaContent_CardStylesCmd = new NpgsqlCommand();
            incInsMediaContent_CardStylesCmd.CommandType = CommandType.Text;
            incInsMediaContent_CardStylesCmd.CommandText = "SELECT media_id, cardstyles_id " +
                                            "FROM \"MediaContent_CardStyles\" AS mc INNER JOIN \"CardStyles\" AS cs ON mc.cardstyles_id = cs.id " +
                                            "INNER JOIN \"Settings\" AS s ON cs.id = s.cardstyle " +
                                            "WHERE mc.create_timestamp is null OR (  mc.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                            "AND mc.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                            "AND mc.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                            "AND (s.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                            "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsMediaContent_CardStylesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsMediaContent_CardStylesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMediaContent_CardStyles.SelectIncrementalInsertsCommand = incInsMediaContent_CardStylesCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelMediaContent_CardStylesCmd = incInsMediaContent_CardStylesCmd.Clone();
            incDelMediaContent_CardStylesCmd.CommandText = "SELECT media_id, cardstyles_id FROM \"MediaContent_CardStyles_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMediaContent_CardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelMediaContent_CardStylesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterMediaContent_CardStyles.SelectIncrementalDeletesCommand = incDelMediaContent_CardStylesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterMediaContent_CardStyles);
            # endregion
            # region Tags
            SyncAdapter adapterTags = new SyncAdapter("Tags");
            # region incremental insert command
            NpgsqlCommand incInsTagsCmd = new NpgsqlCommand();
            incInsTagsCmd.CommandType = CommandType.Text;
            incInsTagsCmd.CommandText = "SELECT id, text " +
                                          "FROM \"Tags\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (id IN (SELECT tags_id FROM \"MediaContent_Tags\" WHERE media_id IN (SELECT media_id FROM \"Cards_MediaContent\" " +
                                          "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                          "OR id IN (SELECT tags_id FROM \"LearningModules_Tags\" WHERE lm_id=:session_lm_id));";
            incInsTagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsTagsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterTags.SelectIncrementalInsertsCommand = incInsTagsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdTagsCmd = incInsTagsCmd.Clone();
            incUpdTagsCmd.CommandText = "SELECT id, text " +
                                          "FROM \"Tags\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (id IN (SELECT tags_id FROM \"MediaContent_Tags\" WHERE media_id IN (SELECT media_id FROM \"Cards_MediaContent\" " +
                                          "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                          "OR id IN (SELECT tags_id FROM \"LearningModules_Tags\" WHERE lm_id=:session_lm_id));";
            incUpdTagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdTagsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterTags.SelectIncrementalUpdatesCommand = incUpdTagsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelTagsCmd = incInsTagsCmd.Clone();
            incDelTagsCmd.CommandText = "SELECT id FROM \"Tags_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelTagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelTagsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterTags.SelectIncrementalDeletesCommand = incDelTagsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterTags);
            # endregion
            # region LearningModules_Tags
            SyncAdapter adapterLearningModules_Tags = new SyncAdapter("LearningModules_Tags");
            # region incremental insert command
            NpgsqlCommand incInsLearningModules_TagsCmd = new NpgsqlCommand();
            incInsLearningModules_TagsCmd.CommandType = CommandType.Text;
            incInsLearningModules_TagsCmd.CommandText = "SELECT lm_id, tags_id " +
                                          "FROM \"LearningModules_Tags\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND lm_id=:session_lm_id;";
            incInsLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsLearningModules_TagsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterLearningModules_Tags.SelectIncrementalInsertsCommand = incInsLearningModules_TagsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelLearningModules_TagsCmd = incInsLearningModules_TagsCmd.Clone();
            incDelLearningModules_TagsCmd.CommandText = "SELECT lm_id, tags_id FROM \"LearningModules_Tags_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningModules_TagsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelLearningModules_TagsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterLearningModules_Tags.SelectIncrementalDeletesCommand = incDelLearningModules_TagsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterLearningModules_Tags);
            # endregion
            # region Settings
            SyncAdapter adapterSettings = new SyncAdapter("Settings");
            # region incremental insert command
            NpgsqlCommand incInsSettingsCmd = new NpgsqlCommand();
            incInsSettingsCmd.CommandType = CommandType.Text;
            incInsSettingsCmd.CommandText = "SELECT id, autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, synonym_gradings, " +
                                                "type_gradings, multiple_choice_options, query_directions, query_types, random_pool, self_assessment, show_images, stripchars, " +
                                                "question_culture, answer_culture, question_caption, answer_caption, logo, question_stylesheet, answer_stylesheet, auto_boxsize, " +
                                                "pool_empty_message_shown, show_statistics, skip_correct_answers, snooze_options, use_lm_stylesheets, cardstyle, boxes, isCached " +
                                              "FROM \"Settings\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                              "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                              "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                              "AND ((id IN (SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                                "OR (id IN (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id)) " +
                                                "OR (id IN (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                                "OR (id IN (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                                "OR (id IN (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsSettingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsSettingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSettings.SelectIncrementalInsertsCommand = incInsSettingsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdSettingsCmd = incInsSettingsCmd.Clone();
            incUpdSettingsCmd.CommandText = "SELECT id, autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, synonym_gradings, " +
                                                "type_gradings, multiple_choice_options, query_directions, query_types, random_pool, self_assessment, show_images, stripchars, " +
                                                "question_culture, answer_culture, question_caption, answer_caption, logo, question_stylesheet, answer_stylesheet, auto_boxsize, " +
                                                "pool_empty_message_shown, show_statistics, skip_correct_answers, snooze_options, use_lm_stylesheets, cardstyle, boxes, isCached " +
                                              "FROM \"Settings\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                              "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                              "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                              "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                              "AND ((id IN (SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                                "OR (id IN (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id)) " +
                                                "OR (id IN (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                                "OR (id IN (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                                "OR (id IN (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdSettingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdSettingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSettings.SelectIncrementalUpdatesCommand = incUpdSettingsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelSettingsCmd = incInsSettingsCmd.Clone();
            incDelSettingsCmd.CommandText = "SELECT id FROM \"Settings_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelSettingsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterSettings.SelectIncrementalDeletesCommand = incDelSettingsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insSettingsCmd = incInsSettingsCmd.Clone();
            insSettingsCmd.CommandText = "INSERT INTO \"Settings\" (id, autoplay_audio, case_sensitive, confirm_demote, enable_commentary, correct_on_the_fly, enable_timer, synonym_gradings, " +
                                          "type_gradings, multiple_choice_options, query_directions, query_types, random_pool, self_assessment, show_images, stripchars, " +
                                          "question_culture, answer_culture, question_caption, answer_caption, logo, question_stylesheet, answer_stylesheet, auto_boxsize, " +
                                          "pool_empty_message_shown, show_statistics, skip_correct_answers, snooze_options, use_lm_stylesheets, cardstyle, boxes, isCached, update_originator_id) " +
                                        "VALUES (:id, :autoplay_audio, :case_sensitive, :confirm_demote, :enable_commentary, :correct_on_the_fly, :enable_timer, :synonym_gradings, " +
                                          ":type_gradings, :multiple_choice_options, :query_directions, :query_types, :random_pool, :self_assessment, :show_images, :stripchars, " +
                                          ":question_culture, :answer_culture, :question_caption, :answer_caption, :logo, :question_stylesheet, :answer_stylesheet, :auto_boxsize, " +
                                          ":pool_empty_message_shown, :show_statistics, :skip_correct_answers, :snooze_options, :use_lm_stylesheets, :cardstyle, :boxes, isCached, :" + SyncSession.SyncClientIdHash + ");";
            insSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("autoplay_audio", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("case_sensitive", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("confirm_demote", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("enable_commentary", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("correct_on_the_fly", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("enable_timer", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("synonym_gradings", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("type_gradings", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("multiple_choice_options", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("query_directions", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("query_types", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("random_pool", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("self_assessment", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("show_images", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("stripchars", NpgsqlDbType.Varchar);
            insSettingsCmd.Parameters.Add("question_culture", NpgsqlDbType.Varchar);
            insSettingsCmd.Parameters.Add("answer_culture", NpgsqlDbType.Varchar);
            insSettingsCmd.Parameters.Add("question_caption", NpgsqlDbType.Varchar);
            insSettingsCmd.Parameters.Add("answer_caption", NpgsqlDbType.Varchar);
            insSettingsCmd.Parameters.Add("logo", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("question_stylesheet", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("answer_stylesheet", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("auto_boxsize", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("pool_empty_message_shown", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("show_statistics", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("skip_correct_answers", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("snooze_options", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("use_lm_stylesheets", NpgsqlDbType.Boolean);
            insSettingsCmd.Parameters.Add("cardstyle", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("boxes", NpgsqlDbType.Integer);
            insSettingsCmd.Parameters.Add("isCached", NpgsqlDbType.Boolean);
            adapterSettings.InsertCommand = insSettingsCmd;
            # endregion
            # region update command
            NpgsqlCommand updSettingsCmd = incInsSettingsCmd.Clone();
            updSettingsCmd.CommandText = "UPDATE \"Settings\" SET autoplay_audio=:autoplay_audio, case_sensitive=:case_sensitive, confirm_demote=:confirm_demote, enable_commentary=:enable_commentary, " +
                                            "correct_on_the_fly=:correct_on_the_fly, enable_timer=:enable_timer, synonym_gradings=:synonym_gradings, type_gradings=:type_gradings, " +
                                            "multiple_choice_options=:multiple_choice_options, query_directions=:query_directions, query_types=:query_types, random_pool=:random_pool, " +
                                            "random_pool=:random_pool, show_images=:show_images, stripchars=:stripchars, question_culture:=question_culture, answer_culture:=answer_culture, " +
                                            "question_caption=:question_caption, answer_caption=:answer_caption, logo=:logo, question_stylesheet=:question_stylesheet, answer_stylesheet=:answer_stylesheet, " +
                                            "auto_boxsize=:auto_boxsize, pool_empty_message_shown=:pool_empty_message_shown, show_statistics=:show_statistics, skip_correct_answers=:skip_correct_answers, " +
                                            "snooze_options=:snooze_options, use_lm_stylesheets=:use_lm_stylesheets, cardstyle=:cardstyle, boxes=:boxes, isCached=:isCached" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("autoplay_audio", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("case_sensitive", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("confirm_demote", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("enable_commentary", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("correct_on_the_fly", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("enable_timer", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("synonym_gradings", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("type_gradings", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("multiple_choice_options", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("query_directions", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("query_types", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("random_pool", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("self_assessment", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("show_images", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("stripchars", NpgsqlDbType.Varchar);
            updSettingsCmd.Parameters.Add("question_culture", NpgsqlDbType.Varchar);
            updSettingsCmd.Parameters.Add("answer_culture", NpgsqlDbType.Varchar);
            updSettingsCmd.Parameters.Add("question_caption", NpgsqlDbType.Varchar);
            updSettingsCmd.Parameters.Add("answer_caption", NpgsqlDbType.Varchar);
            updSettingsCmd.Parameters.Add("logo", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("question_stylesheet", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("answer_stylesheet", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("auto_boxsize", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("pool_empty_message_shown", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("show_statistics", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("skip_correct_answers", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("snooze_options", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("use_lm_stylesheets", NpgsqlDbType.Boolean);
            updSettingsCmd.Parameters.Add("cardstyle", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("boxes", NpgsqlDbType.Integer);
            updSettingsCmd.Parameters.Add("isCached", NpgsqlDbType.Boolean);
            adapterSettings.UpdateCommand = updSettingsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delSettingsCmd = incInsSettingsCmd.Clone();
            delSettingsCmd.CommandText = "DELETE FROM \"Settings\" WHERE id=:id; " +
                                        "UPDATE \"Settings_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delSettingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterSettings.DeleteCommand = delSettingsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterSettings);
            # endregion
            # region CommentarySounds
            SyncAdapter adapterCommentarySounds = new SyncAdapter("CommentarySounds");
            # region incremental insert command
            NpgsqlCommand incInsCommentarySoundsCmd = new NpgsqlCommand();
            incInsCommentarySoundsCmd.CommandType = CommandType.Text;
            incInsCommentarySoundsCmd.CommandText = "SELECT media_id, settings_id, side, type " +
                                          "FROM \"CommentarySounds\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND ((settings_id IN (SELECT settings_id FROM \"Cards\" WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                           "OR (settings_id IN (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" WHERE lm_id=:session_lm_id AND user_id=:session_user_id)) " +
                                           "OR (settings_id IN (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (settings_id IN (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (settings_id IN (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsCommentarySoundsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCommentarySoundsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCommentarySoundsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsCommentarySoundsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsCommentarySoundsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterCommentarySounds.SelectIncrementalInsertsCommand = incInsCommentarySoundsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelCommentarySoundsCmd = incInsCommentarySoundsCmd.Clone();
            incDelCommentarySoundsCmd.CommandText = "SELECT media_id, settings_id, side, type FROM \"CommentarySounds_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelCommentarySoundsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCommentarySoundsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCommentarySoundsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelCommentarySoundsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterCommentarySounds.SelectIncrementalDeletesCommand = incDelCommentarySoundsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterCommentarySounds);
            # endregion
            # region QueryDirections
            SyncAdapter adapterQueryDirections = new SyncAdapter("QueryDirections");
            # region incremental insert command
            NpgsqlCommand incInsQueryDirectionsCmd = new NpgsqlCommand();
            incInsQueryDirectionsCmd.CommandType = CommandType.Text;
            incInsQueryDirectionsCmd.CommandText = "SELECT QD.id, QD.question2answer, QD.answer2question, QD.mixed " +
                                          "FROM \"QueryDirections\" AS QD INNER JOIN \"Settings\" AS S ON QD.id=S.query_directions " +
                                          "WHERE QD.create_timestamp is null OR (  QD.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and QD.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND QD.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND ((S.id IN (SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                           "OR (S.id IN (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id)) " +
                                           "OR (S.id IN (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (S.id IN (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (S.id IN (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsQueryDirectionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsQueryDirectionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsQueryDirectionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsQueryDirectionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterQueryDirections.SelectIncrementalInsertsCommand = incInsQueryDirectionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdQueryDirectionsCmd = incInsQueryDirectionsCmd.Clone();
            incUpdQueryDirectionsCmd.CommandText = "SELECT QD.id, QD.question2answer, QD.answer2question, QD.mixed " +
                                          "FROM \"QueryDirections\" AS QD INNER JOIN \"Settings\" AS S ON QD.id=S.query_directions " +
                                          "WHERE (QD.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (QD.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (QD.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND QD.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND ((S.id IN (SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id))) " +
                                           "OR (S.id IN (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id)) " +
                                           "OR (S.id IN (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (S.id IN (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id)) " +
                                           "OR (S.id IN (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdQueryDirectionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdQueryDirectionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdQueryDirectionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdQueryDirectionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterQueryDirections.SelectIncrementalUpdatesCommand = incUpdQueryDirectionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelQueryDirectionsCmd = incInsQueryDirectionsCmd.Clone();
            incDelQueryDirectionsCmd.CommandText = "SELECT id FROM \"QueryDirections_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelQueryDirectionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelQueryDirectionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelQueryDirectionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterQueryDirections.SelectIncrementalDeletesCommand = incDelQueryDirectionsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insQueryDirectionsCmd = incInsQueryDirectionsCmd.Clone();
            insQueryDirectionsCmd.CommandText = "INSERT INTO \"QueryDirections\" (id, question2answer, answer2question, mixed, update_originator_id) " +
                                        "VALUES (:id, :question2answer, :answer2question, :mixed, :" + SyncSession.SyncClientIdHash + ");";
            insQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insQueryDirectionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insQueryDirectionsCmd.Parameters.Add("question2answer", NpgsqlDbType.Boolean);
            insQueryDirectionsCmd.Parameters.Add("answer2question", NpgsqlDbType.Boolean);
            insQueryDirectionsCmd.Parameters.Add("mixed", NpgsqlDbType.Boolean);
            adapterQueryDirections.InsertCommand = insQueryDirectionsCmd;
            # endregion
            # region update command
            NpgsqlCommand updQueryDirectionsCmd = incInsQueryDirectionsCmd.Clone();
            updQueryDirectionsCmd.CommandText = "UPDATE \"QueryDirections\" SET question2answer=:question2answer, answer2question=:answer2question, mixed=:mixed" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updQueryDirectionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updQueryDirectionsCmd.Parameters.Add("question2answer", NpgsqlDbType.Boolean);
            updQueryDirectionsCmd.Parameters.Add("answer2question", NpgsqlDbType.Boolean);
            updQueryDirectionsCmd.Parameters.Add("mixed", NpgsqlDbType.Boolean);
            adapterQueryDirections.UpdateCommand = updQueryDirectionsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delQueryDirectionsCmd = incInsQueryDirectionsCmd.Clone();
            delQueryDirectionsCmd.CommandText = "DELETE FROM \"QueryDirections\" WHERE id=:id; " +
                                        "UPDATE \"QueryDirections_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delQueryDirectionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delQueryDirectionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterQueryDirections.DeleteCommand = delQueryDirectionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterQueryDirections);
            # endregion
            # region SynonymGradings
            SyncAdapter adapterSynonymGradings = new SyncAdapter("SynonymGradings");
            # region incremental insert command
            NpgsqlCommand incInsSynonymGradingsCmd = new NpgsqlCommand();
            incInsSynonymGradingsCmd.CommandType = CommandType.Text;
            incInsSynonymGradingsCmd.CommandText = "SELECT ST.id, all_known, half_known, one_known, first_known, prompt " +
                                          "FROM \"SynonymGradings\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.synonym_gradings " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsSynonymGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSynonymGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsSynonymGradingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsSynonymGradingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSynonymGradings.SelectIncrementalInsertsCommand = incInsSynonymGradingsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdSynonymGradingsCmd = incInsSynonymGradingsCmd.Clone();
            incUpdSynonymGradingsCmd.CommandText = "SELECT ST.id, all_known, half_known, one_known, first_known, prompt " +
                                          "FROM \"SynonymGradings\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.synonym_gradings " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdSynonymGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSynonymGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdSynonymGradingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdSynonymGradingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSynonymGradings.SelectIncrementalUpdatesCommand = incUpdSynonymGradingsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelSynonymGradingsCmd = incInsSynonymGradingsCmd.Clone();
            incDelSynonymGradingsCmd.CommandText = "SELECT id FROM \"SynonymGradings_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelSynonymGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSynonymGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelSynonymGradingsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterSynonymGradings.SelectIncrementalDeletesCommand = incDelSynonymGradingsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insSynonymGradingsCmd = incInsSynonymGradingsCmd.Clone();
            insSynonymGradingsCmd.CommandText = "INSERT INTO \"SynonymGradings\" (id, all_known, half_known, one_known, first_known, prompt, update_originator_id) " +
                                        "VALUES (:id, :all_known, :half_known, :one_known, :first_known, :prompt, :" + SyncSession.SyncClientIdHash + ");";
            insSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insSynonymGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insSynonymGradingsCmd.Parameters.Add("all_known", NpgsqlDbType.Boolean);
            insSynonymGradingsCmd.Parameters.Add("half_known", NpgsqlDbType.Boolean);
            insSynonymGradingsCmd.Parameters.Add("one_known", NpgsqlDbType.Boolean);
            insSynonymGradingsCmd.Parameters.Add("first_known", NpgsqlDbType.Boolean);
            insSynonymGradingsCmd.Parameters.Add("prompt", NpgsqlDbType.Boolean);
            adapterSynonymGradings.InsertCommand = insSynonymGradingsCmd;
            # endregion
            # region update command
            NpgsqlCommand updSynonymGradingsCmd = incInsSynonymGradingsCmd.Clone();
            updSynonymGradingsCmd.CommandText = "UPDATE \"SynonymGradings\" SET all_known=:all_known, half_known=:half_known, one_known=:one_known, first_known=:first_known, prompt=:prompt" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updSynonymGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updSynonymGradingsCmd.Parameters.Add("all_known", NpgsqlDbType.Boolean);
            updSynonymGradingsCmd.Parameters.Add("half_known", NpgsqlDbType.Boolean);
            updSynonymGradingsCmd.Parameters.Add("one_known", NpgsqlDbType.Boolean);
            updSynonymGradingsCmd.Parameters.Add("first_known", NpgsqlDbType.Boolean);
            updSynonymGradingsCmd.Parameters.Add("prompt", NpgsqlDbType.Boolean);
            adapterSynonymGradings.UpdateCommand = updSynonymGradingsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delSynonymGradingsCmd = incInsSynonymGradingsCmd.Clone();
            delSynonymGradingsCmd.CommandText = "DELETE FROM \"SynonymGradings\" WHERE id=:id; " +
                                        "UPDATE \"SynonymGradings_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delSynonymGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delSynonymGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterSynonymGradings.DeleteCommand = delSynonymGradingsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterSynonymGradings);
            # endregion
            # region QueryTypes
            SyncAdapter adapterQueryTypes = new SyncAdapter("QueryTypes");
            # region incremental insert command
            NpgsqlCommand incInsQueryTypesCmd = new NpgsqlCommand();
            incInsQueryTypesCmd.CommandType = CommandType.Text;
            incInsQueryTypesCmd.CommandText = "SELECT ST.id, image_recognition, listening_comprehension, multiple_choice, sentence, word " +
                                          "FROM \"QueryTypes\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.query_types " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsQueryTypesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsQueryTypesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsQueryTypesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsQueryTypesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterQueryTypes.SelectIncrementalInsertsCommand = incInsQueryTypesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdQueryTypesCmd = incInsQueryTypesCmd.Clone();
            incUpdQueryTypesCmd.CommandText = "SELECT ST.id, image_recognition, listening_comprehension, multiple_choice, sentence, word " +
                                          "FROM \"QueryTypes\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.query_types " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdQueryTypesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdQueryTypesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdQueryTypesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdQueryTypesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterQueryTypes.SelectIncrementalUpdatesCommand = incUpdQueryTypesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelQueryTypesCmd = incInsQueryTypesCmd.Clone();
            incDelQueryTypesCmd.CommandText = "SELECT id FROM \"QueryTypes_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelQueryTypesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelQueryTypesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelQueryTypesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterQueryTypes.SelectIncrementalDeletesCommand = incDelQueryTypesCmd;
            # endregion
            # region insert command
            NpgsqlCommand insQueryTypesCmd = incInsQueryTypesCmd.Clone();
            insQueryTypesCmd.CommandText = "INSERT INTO \"QueryTypes\" (id, image_recognition, listening_comprehension, multiple_choice, sentence, word, update_originator_id) " +
                                        "VALUES (:id, :image_recognition, :listening_comprehension, :multiple_choice, :sentence, :word, :" + SyncSession.SyncClientIdHash + ");";
            insQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insQueryTypesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insQueryTypesCmd.Parameters.Add("image_recognition", NpgsqlDbType.Boolean);
            insQueryTypesCmd.Parameters.Add("listening_comprehension", NpgsqlDbType.Boolean);
            insQueryTypesCmd.Parameters.Add("multiple_choice", NpgsqlDbType.Boolean);
            insQueryTypesCmd.Parameters.Add("sentence", NpgsqlDbType.Boolean);
            insQueryTypesCmd.Parameters.Add("word", NpgsqlDbType.Boolean);
            adapterQueryTypes.InsertCommand = insQueryTypesCmd;
            # endregion
            # region update command
            NpgsqlCommand updQueryTypesCmd = incInsQueryTypesCmd.Clone();
            updQueryTypesCmd.CommandText = "UPDATE \"QueryTypes\" SET image_recognition=:image_recognition, listening_comprehension=:listening_comprehension, multiple_choice=:multiple_choice, " +
                                        "sentence=:sentence, word=:word" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updQueryTypesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updQueryTypesCmd.Parameters.Add("image_recognition", NpgsqlDbType.Boolean);
            updQueryTypesCmd.Parameters.Add("listening_comprehension", NpgsqlDbType.Boolean);
            updQueryTypesCmd.Parameters.Add("multiple_choice", NpgsqlDbType.Boolean);
            updQueryTypesCmd.Parameters.Add("sentence", NpgsqlDbType.Boolean);
            updQueryTypesCmd.Parameters.Add("word", NpgsqlDbType.Boolean);
            adapterQueryTypes.UpdateCommand = updQueryTypesCmd;
            # endregion
            # region delete command
            NpgsqlCommand delQueryTypesCmd = incInsQueryTypesCmd.Clone();
            delQueryTypesCmd.CommandText = "DELETE FROM \"QueryTypes\" WHERE id=:id; " +
                                        "UPDATE \"QueryTypes_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delQueryTypesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delQueryTypesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterQueryTypes.DeleteCommand = delQueryTypesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterQueryTypes);
            # endregion
            # region MultipleChoiceOptions
            SyncAdapter adapterMultipleChoiceOptions = new SyncAdapter("MultipleChoiceOptions");
            # region incremental insert command
            NpgsqlCommand incInsMultipleChoiceOptionsCmd = new NpgsqlCommand();
            incInsMultipleChoiceOptionsCmd.CommandType = CommandType.Text;
            incInsMultipleChoiceOptionsCmd.CommandText = "SELECT ST.id, allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices " +
                                          "FROM \"MultipleChoiceOptions\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.multiple_choice_options " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsMultipleChoiceOptionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsMultipleChoiceOptionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMultipleChoiceOptions.SelectIncrementalInsertsCommand = incInsMultipleChoiceOptionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdMultipleChoiceOptionsCmd = incInsMultipleChoiceOptionsCmd.Clone();
            incUpdMultipleChoiceOptionsCmd.CommandText = "SELECT ST.id, allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices " +
                                          "FROM \"MultipleChoiceOptions\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.multiple_choice_options " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdMultipleChoiceOptionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdMultipleChoiceOptionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterMultipleChoiceOptions.SelectIncrementalUpdatesCommand = incUpdMultipleChoiceOptionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelMultipleChoiceOptionsCmd = incInsMultipleChoiceOptionsCmd.Clone();
            incDelMultipleChoiceOptionsCmd.CommandText = "SELECT id FROM \"MultipleChoiceOptions_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelMultipleChoiceOptionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterMultipleChoiceOptions.SelectIncrementalDeletesCommand = incDelMultipleChoiceOptionsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insMultipleChoiceOptionsCmd = incInsMultipleChoiceOptionsCmd.Clone();
            insMultipleChoiceOptionsCmd.CommandText = "INSERT INTO \"MultipleChoiceOptions\" (id, allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices, update_originator_id) " +
                                        "VALUES (:id, :allow_multiple_correct_answers, :allow_random_distractors, :max_correct_answers, :number_of_choices, :" + SyncSession.SyncClientIdHash + ");";
            insMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insMultipleChoiceOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insMultipleChoiceOptionsCmd.Parameters.Add("allow_multiple_correct_answers", NpgsqlDbType.Boolean);
            insMultipleChoiceOptionsCmd.Parameters.Add("allow_random_distractors", NpgsqlDbType.Boolean);
            insMultipleChoiceOptionsCmd.Parameters.Add("max_correct_answers", NpgsqlDbType.Integer);
            insMultipleChoiceOptionsCmd.Parameters.Add("number_of_choices", NpgsqlDbType.Integer);
            adapterMultipleChoiceOptions.InsertCommand = insMultipleChoiceOptionsCmd;
            # endregion
            # region update command
            NpgsqlCommand updMultipleChoiceOptionsCmd = incInsMultipleChoiceOptionsCmd.Clone();
            updMultipleChoiceOptionsCmd.CommandText = "UPDATE \"MultipleChoiceOptions\" SET allow_multiple_correct_answers=:allow_multiple_correct_answers, allow_random_distractors=:allow_random_distractors, " +
                                        "max_correct_answers=:max_correct_answers, number_of_choices=:number_of_choices" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updMultipleChoiceOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updMultipleChoiceOptionsCmd.Parameters.Add("allow_multiple_correct_answers", NpgsqlDbType.Boolean);
            updMultipleChoiceOptionsCmd.Parameters.Add("allow_random_distractors", NpgsqlDbType.Boolean);
            updMultipleChoiceOptionsCmd.Parameters.Add("max_correct_answers", NpgsqlDbType.Integer);
            updMultipleChoiceOptionsCmd.Parameters.Add("number_of_choices", NpgsqlDbType.Integer);
            adapterMultipleChoiceOptions.UpdateCommand = updMultipleChoiceOptionsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delMultipleChoiceOptionsCmd = incInsMultipleChoiceOptionsCmd.Clone();
            delMultipleChoiceOptionsCmd.CommandText = "DELETE FROM \"MultipleChoiceOptions\" WHERE id=:id; " +
                                        "UPDATE \"MultipleChoiceOptions_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delMultipleChoiceOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delMultipleChoiceOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterMultipleChoiceOptions.DeleteCommand = delMultipleChoiceOptionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterMultipleChoiceOptions);
            # endregion
            # region TypeGradings
            SyncAdapter adapterTypeGradings = new SyncAdapter("TypeGradings");
            # region incremental insert command
            NpgsqlCommand incInsTypeGradingsCmd = new NpgsqlCommand();
            incInsTypeGradingsCmd.CommandType = CommandType.Text;
            incInsTypeGradingsCmd.CommandText = "SELECT ST.id, all_correct, half_correct, none_correct, prompt " +
                                          "FROM \"TypeGradings\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.type_gradings " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsTypeGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTypeGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsTypeGradingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsTypeGradingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterTypeGradings.SelectIncrementalInsertsCommand = incInsTypeGradingsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdTypeGradingsCmd = incInsTypeGradingsCmd.Clone();
            incUpdTypeGradingsCmd.CommandText = "SELECT ST.id, all_correct, half_correct, none_correct, prompt " +
                                          "FROM \"TypeGradings\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.type_gradings " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdTypeGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTypeGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdTypeGradingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdTypeGradingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterTypeGradings.SelectIncrementalUpdatesCommand = incUpdTypeGradingsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelTypeGradingsCmd = incInsTypeGradingsCmd.Clone();
            incDelTypeGradingsCmd.CommandText = "SELECT id FROM \"TypeGradings_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelTypeGradingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTypeGradingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelTypeGradingsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterTypeGradings.SelectIncrementalDeletesCommand = incDelTypeGradingsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insTypeGradingsCmd = incInsTypeGradingsCmd.Clone();
            insTypeGradingsCmd.CommandText = "INSERT INTO \"TypeGradings\" (id, all_correct, half_correct, none_correct, prompt, update_originator_id) " +
                                        "VALUES (:id, :all_correct, :half_correct, :none_correct, :prompt, :" + SyncSession.SyncClientIdHash + ");";
            insTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insTypeGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insTypeGradingsCmd.Parameters.Add("all_correct", NpgsqlDbType.Boolean);
            insTypeGradingsCmd.Parameters.Add("half_correct", NpgsqlDbType.Boolean);
            insTypeGradingsCmd.Parameters.Add("none_correct", NpgsqlDbType.Boolean);
            insTypeGradingsCmd.Parameters.Add("prompt", NpgsqlDbType.Boolean);
            adapterTypeGradings.InsertCommand = insTypeGradingsCmd;
            # endregion
            # region update command
            NpgsqlCommand updTypeGradingsCmd = incInsTypeGradingsCmd.Clone();
            updTypeGradingsCmd.CommandText = "UPDATE \"TypeGradings\" SET all_correct=:all_correct, half_correct=:half_correct, none_correct=:none_correct, prompt=:prompt" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updTypeGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updTypeGradingsCmd.Parameters.Add("all_correct", NpgsqlDbType.Boolean);
            updTypeGradingsCmd.Parameters.Add("half_correct", NpgsqlDbType.Boolean);
            updTypeGradingsCmd.Parameters.Add("none_correct", NpgsqlDbType.Boolean);
            updTypeGradingsCmd.Parameters.Add("prompt", NpgsqlDbType.Boolean);
            adapterTypeGradings.UpdateCommand = updTypeGradingsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delTypeGradingsCmd = incInsTypeGradingsCmd.Clone();
            delTypeGradingsCmd.CommandText = "DELETE FROM \"TypeGradings\" WHERE id=:id; " +
                                        "UPDATE \"TypeGradings_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delTypeGradingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delTypeGradingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterTypeGradings.DeleteCommand = delTypeGradingsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterTypeGradings);
            # endregion
            # region CardStyles
            SyncAdapter adapterCardStyles = new SyncAdapter("CardStyles");
            # region incremental insert command
            NpgsqlCommand incInsCardStylesCmd = new NpgsqlCommand();
            incInsCardStylesCmd.CommandType = CommandType.Text;
            incInsCardStylesCmd.CommandText = "SELECT ST.id, value " +
                                          "FROM \"CardStyles\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.cardstyle " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsCardStylesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCardStylesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsCardStylesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsCardStylesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterCardStyles.SelectIncrementalInsertsCommand = incInsCardStylesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdCardStylesCmd = incInsCardStylesCmd.Clone();
            incUpdCardStylesCmd.CommandText = "SELECT ST.id, value " +
                                          "FROM \"CardStyles\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.cardstyle " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdCardStylesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCardStylesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdCardStylesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdCardStylesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterCardStyles.SelectIncrementalUpdatesCommand = incUpdCardStylesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelCardStylesCmd = incInsCardStylesCmd.Clone();
            incDelCardStylesCmd.CommandText = "SELECT id FROM \"CardStyles_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelCardStylesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCardStylesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelCardStylesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterCardStyles.SelectIncrementalDeletesCommand = incDelCardStylesCmd;
            # endregion
            # region insert command
            NpgsqlCommand insCardStylesCmd = incInsCardStylesCmd.Clone();
            insCardStylesCmd.CommandText = "INSERT INTO \"CardStyles\" (id, value, update_originator_id) " +
                                        "VALUES (:id, :value, :" + SyncSession.SyncClientIdHash + ");";
            insCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insCardStylesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insCardStylesCmd.Parameters.Add("value", NpgsqlDbType.Text);
            adapterCardStyles.InsertCommand = insCardStylesCmd;
            # endregion
            # region update command
            NpgsqlCommand updCardStylesCmd = incInsCardStylesCmd.Clone();
            updCardStylesCmd.CommandText = "UPDATE \"CardStyles\" SET value=:value" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updCardStylesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updCardStylesCmd.Parameters.Add("value", NpgsqlDbType.Text);
            adapterCardStyles.UpdateCommand = updCardStylesCmd;
            # endregion
            # region delete command
            NpgsqlCommand delCardStylesCmd = incInsCardStylesCmd.Clone();
            delCardStylesCmd.CommandText = "DELETE FROM \"CardStyles\" WHERE id=:id; " +
                                        "UPDATE \"CardStyles_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delCardStylesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delCardStylesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterCardStyles.DeleteCommand = delCardStylesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterCardStyles);
            # endregion
            # region StyleSheets
            SyncAdapter adapterStyleSheets = new SyncAdapter("StyleSheets");
            # region incremental insert command
            NpgsqlCommand incInsStyleSheetsCmd = new NpgsqlCommand();
            incInsStyleSheetsCmd.CommandType = CommandType.Text;
            incInsStyleSheetsCmd.CommandText = "SELECT ST.id, value " +
                                          "FROM \"StyleSheets\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.question_stylesheet OR ST.id=S.answer_stylesheet " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsStyleSheetsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsStyleSheetsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsStyleSheetsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsStyleSheetsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterStyleSheets.SelectIncrementalInsertsCommand = incInsStyleSheetsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdStyleSheetsCmd = incInsStyleSheetsCmd.Clone();
            incUpdStyleSheetsCmd.CommandText = "SELECT ST.id, value " +
                                          "FROM \"StyleSheets\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.question_stylesheet OR ST.id=S.answer_stylesheet " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdStyleSheetsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdStyleSheetsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdStyleSheetsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdStyleSheetsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterStyleSheets.SelectIncrementalUpdatesCommand = incUpdStyleSheetsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelStyleSheetsCmd = incInsStyleSheetsCmd.Clone();
            incDelStyleSheetsCmd.CommandText = "SELECT id FROM \"StyleSheets_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelStyleSheetsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelStyleSheetsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelStyleSheetsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterStyleSheets.SelectIncrementalDeletesCommand = incDelStyleSheetsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insStyleSheetsCmd = incInsStyleSheetsCmd.Clone();
            insStyleSheetsCmd.CommandText = "INSERT INTO \"StyleSheets\" (id, value, update_originator_id) " +
                                        "VALUES (:id, :value, :" + SyncSession.SyncClientIdHash + ");";
            insStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insStyleSheetsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insStyleSheetsCmd.Parameters.Add("value", NpgsqlDbType.Text);
            adapterStyleSheets.InsertCommand = insStyleSheetsCmd;
            # endregion
            # region update command
            NpgsqlCommand updStyleSheetsCmd = incInsStyleSheetsCmd.Clone();
            updStyleSheetsCmd.CommandText = "UPDATE \"StyleSheets\" SET value=:value" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updStyleSheetsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updStyleSheetsCmd.Parameters.Add("value", NpgsqlDbType.Text);
            adapterStyleSheets.UpdateCommand = updStyleSheetsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delStyleSheetsCmd = incInsStyleSheetsCmd.Clone();
            delStyleSheetsCmd.CommandText = "DELETE FROM \"StyleSheets\" WHERE id=:id; " +
                                        "UPDATE \"StyleSheets_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delStyleSheetsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delStyleSheetsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterStyleSheets.DeleteCommand = delStyleSheetsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterStyleSheets);
            # endregion
            # region SnoozeOptions
            SyncAdapter adapterSnoozeOptions = new SyncAdapter("SnoozeOptions");
            # region incremental insert command
            NpgsqlCommand incInsSnoozeOptionsCmd = new NpgsqlCommand();
            incInsSnoozeOptionsCmd.CommandType = CommandType.Text;
            incInsSnoozeOptionsCmd.CommandText = "SELECT ST.id, cards_enabled, rights_enabled, time_enabled, snooze_cards, snooze_high, snooze_low, snooze_mode, snooze_rights, snooze_time " +
                                          "FROM \"SnoozeOptions\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.snooze_options " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsSnoozeOptionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsSnoozeOptionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSnoozeOptions.SelectIncrementalInsertsCommand = incInsSnoozeOptionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdSnoozeOptionsCmd = incInsSnoozeOptionsCmd.Clone();
            incUpdSnoozeOptionsCmd.CommandText = "SELECT ST.id, cards_enabled, rights_enabled, time_enabled, snooze_cards, snooze_high, snooze_low, snooze_mode, snooze_rights, snooze_time " +
                                          "FROM \"SnoozeOptions\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.snooze_options " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdSnoozeOptionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdSnoozeOptionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSnoozeOptions.SelectIncrementalUpdatesCommand = incUpdSnoozeOptionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelSnoozeOptionsCmd = incInsSnoozeOptionsCmd.Clone();
            incDelSnoozeOptionsCmd.CommandText = "SELECT id FROM \"SnoozeOptions_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelSnoozeOptionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterSnoozeOptions.SelectIncrementalDeletesCommand = incDelSnoozeOptionsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insSnoozeOptionsCmd = incInsSnoozeOptionsCmd.Clone();
            insSnoozeOptionsCmd.CommandText = "INSERT INTO \"SnoozeOptions\" (id, cards_enabled, rights_enabled, time_enabled, snooze_cards, snooze_high, snooze_low, snooze_mode, snooze_rights, snooze_time, update_originator_id) " +
                                        "VALUES (:id, :cards_enabled, :rights_enabled, :time_enabled, :snooze_cards, :snooze_high, :snooze_low, :snooze_mode, :snooze_rights, :snooze_time, :" + SyncSession.SyncClientIdHash + ");";
            insSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("cards_enabled", NpgsqlDbType.Boolean);
            insSnoozeOptionsCmd.Parameters.Add("rights_enabled", NpgsqlDbType.Boolean);
            insSnoozeOptionsCmd.Parameters.Add("time_enabled", NpgsqlDbType.Boolean);
            insSnoozeOptionsCmd.Parameters.Add("snooze_cards", NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("snooze_high", NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("snooze_low", NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("snooze_mode", NpgsqlDbType.Text);
            insSnoozeOptionsCmd.Parameters.Add("snooze_rights", NpgsqlDbType.Integer);
            insSnoozeOptionsCmd.Parameters.Add("snooze_time", NpgsqlDbType.Integer);
            adapterSnoozeOptions.InsertCommand = insSnoozeOptionsCmd;
            # endregion
            # region update command
            NpgsqlCommand updSnoozeOptionsCmd = incInsSnoozeOptionsCmd.Clone();
            updSnoozeOptionsCmd.CommandText = "UPDATE \"SnoozeOptions\" SET cards_enabled=:cards_enabled, rights_enabled=:rights_enabled, time_enabled=:time_enabled, snooze_cards=:snooze_cards, " +
                                        "snooze_high=:snooze_high, snooze_low=:snooze_low, snooze_mode=:snooze_mode, snooze_rights=:snooze_rights, snooze_time=:snooze_time" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("cards_enabled", NpgsqlDbType.Boolean);
            updSnoozeOptionsCmd.Parameters.Add("rights_enabled", NpgsqlDbType.Boolean);
            updSnoozeOptionsCmd.Parameters.Add("time_enabled", NpgsqlDbType.Boolean);
            updSnoozeOptionsCmd.Parameters.Add("snooze_cards", NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("snooze_high", NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("snooze_low", NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("snooze_mode", NpgsqlDbType.Text);
            updSnoozeOptionsCmd.Parameters.Add("snooze_rights", NpgsqlDbType.Integer);
            updSnoozeOptionsCmd.Parameters.Add("snooze_time", NpgsqlDbType.Integer);
            adapterSnoozeOptions.UpdateCommand = updSnoozeOptionsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delSnoozeOptionsCmd = incInsSnoozeOptionsCmd.Clone();
            delSnoozeOptionsCmd.CommandText = "DELETE FROM \"SnoozeOptions\" WHERE id=:id; " +
                                        "UPDATE \"SnoozeOptions_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delSnoozeOptionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delSnoozeOptionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterSnoozeOptions.DeleteCommand = delSnoozeOptionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterSnoozeOptions);
            # endregion
            # region Boxes
            SyncAdapter adapterBoxes = new SyncAdapter("Boxes");
            # region incremental insert command
            NpgsqlCommand incInsBoxesCmd = new NpgsqlCommand();
            incInsBoxesCmd.CommandType = CommandType.Text;
            incInsBoxesCmd.CommandText = "SELECT ST.id, box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size " +
                                          "FROM \"Boxes\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.boxes " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsBoxesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsBoxesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsBoxesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsBoxesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterBoxes.SelectIncrementalInsertsCommand = incInsBoxesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdBoxesCmd = incInsBoxesCmd.Clone();
            incUpdBoxesCmd.CommandText = "SELECT ST.id, box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size " +
                                          "FROM \"Boxes\" AS ST INNER JOIN \"Settings\" AS S ON ST.id=S.boxes " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incUpdBoxesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdBoxesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdBoxesCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdBoxesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterBoxes.SelectIncrementalUpdatesCommand = incUpdBoxesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelBoxesCmd = incInsBoxesCmd.Clone();
            incDelBoxesCmd.CommandText = "SELECT id FROM \"Boxes_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelBoxesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelBoxesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelBoxesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterBoxes.SelectIncrementalDeletesCommand = incDelBoxesCmd;
            # endregion
            # region insert command
            NpgsqlCommand insBoxesCmd = incInsBoxesCmd.Clone();
            insBoxesCmd.CommandText = "INSERT INTO \"Boxes\" (id, box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size, update_originator_id) " +
                                        "VALUES (:id, :box1_size, :box2_size, :box3_size, :box4_size, :box5_size, :box6_size, :box7_size, :box8_size, :box9_size, :" + SyncSession.SyncClientIdHash + ");";
            insBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box1_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box2_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box3_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box4_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box5_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box6_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box7_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box8_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box9_size", NpgsqlDbType.Integer);
            adapterBoxes.InsertCommand = insBoxesCmd;
            # endregion
            # region update command
            NpgsqlCommand updBoxesCmd = incInsBoxesCmd.Clone();
            updBoxesCmd.CommandText = "UPDATE \"Boxes\" SET box1_size=:box1_size, box2_size=:box2_size, box3_size=:box3_size, box4_size=:box4_size, " +
                                        "box5_size=:box5_size, box6_size=:box6_size, box7_size=:box7_size, box8_size=:box8_size, box9_size=:box9_size" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updBoxesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box1_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box2_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box3_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box4_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box5_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box6_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box7_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box8_size", NpgsqlDbType.Integer);
            insBoxesCmd.Parameters.Add("box9_size", NpgsqlDbType.Integer);
            adapterBoxes.UpdateCommand = updBoxesCmd;
            # endregion
            # region delete command
            NpgsqlCommand delBoxesCmd = incInsBoxesCmd.Clone();
            delBoxesCmd.CommandText = "DELETE FROM \"Boxes\" WHERE id=:id; " +
                                        "UPDATE \"Boxes_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delBoxesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delBoxesCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterBoxes.DeleteCommand = delBoxesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterBoxes);
            # endregion
            # region SelectedLearnChapters
            SyncAdapter adapterSelectedLearnChapters = new SyncAdapter("SelectedLearnChapters");
            # region incremental insert command
            NpgsqlCommand incInsSelectedLearnChaptersCmd = new NpgsqlCommand();
            incInsSelectedLearnChaptersCmd.CommandType = CommandType.Text;
            incInsSelectedLearnChaptersCmd.CommandText = "SELECT ST.chapters_id, ST.settings_id " +
                                          "FROM \"SelectedLearnChapters\" AS ST INNER JOIN \"Settings\" AS S ON ST.settings_id=S.id " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND (S.id IN ((SELECT settings_id FROM \"Cards\" " +
                                            "WHERE id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id)) " +
                                           "UNION (SELECT settings_id FROM \"UserProfilesLearningModulesSettings\" " +
                                            "WHERE lm_id=:session_lm_id AND user_id=:session_user_id) " +
                                           "UNION (SELECT default_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT allowed_settings_id FROM \"LearningModules\" WHERE id=:session_lm_id) " +
                                           "UNION (SELECT settings_id FROM \"Chapters\" WHERE lm_id=:session_lm_id)));";
            incInsSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsSelectedLearnChaptersCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsSelectedLearnChaptersCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterSelectedLearnChapters.SelectIncrementalInsertsCommand = incInsSelectedLearnChaptersCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelSelectedLearnChaptersCmd = incInsSelectedLearnChaptersCmd.Clone();
            incDelSelectedLearnChaptersCmd.CommandText = "SELECT chapters_id, settings_id FROM \"SelectedLearnChapters_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelSelectedLearnChaptersCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterSelectedLearnChapters.SelectIncrementalDeletesCommand = incDelSelectedLearnChaptersCmd;
            # endregion
            # region insert command
            NpgsqlCommand insSelectedLearnChaptersCmd = incInsSelectedLearnChaptersCmd.Clone();
            insSelectedLearnChaptersCmd.CommandText = "INSERT INTO \"SelectedLearnChapters\" (chapters_id, settings_id, update_originator_id) " +
                                        "VALUES (:chapters_id, :settings_id, :" + SyncSession.SyncClientIdHash + ");";
            insSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insSelectedLearnChaptersCmd.Parameters.Add("chapters_id", NpgsqlDbType.Integer);
            insSelectedLearnChaptersCmd.Parameters.Add("settings_id", NpgsqlDbType.Integer);
            adapterSelectedLearnChapters.InsertCommand = insSelectedLearnChaptersCmd;
            # endregion
            # region update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region delete command
            NpgsqlCommand delSelectedLearnChaptersCmd = incInsSelectedLearnChaptersCmd.Clone();
            delSelectedLearnChaptersCmd.CommandText = "DELETE FROM \"SelectedLearnChapters\" WHERE chapters_id=:chapters_id; " +
                                        "UPDATE \"SelectedLearnChapters_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE chapters_id=:chapters_id;";
            delSelectedLearnChaptersCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delSelectedLearnChaptersCmd.Parameters.Add("chapters_id", NpgsqlDbType.Integer);
            adapterSelectedLearnChapters.DeleteCommand = delSelectedLearnChaptersCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterSelectedLearnChapters);
            # endregion
            # region UserProfiles
            SyncAdapter adapterUserProfiles = new SyncAdapter("UserProfiles");
            # region incremental insert command
            NpgsqlCommand incInsUserProfilesCmd = new NpgsqlCommand();
            incInsUserProfilesCmd.CommandType = CommandType.Text;
            incInsUserProfilesCmd.CommandText = "SELECT id, username, user_type, enabled " +
                                          "FROM \"UserProfiles\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id=:session_user_id;";
            incInsUserProfilesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfilesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfilesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserProfilesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfiles.SelectIncrementalInsertsCommand = incInsUserProfilesCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdUserProfilesCmd = incInsUserProfilesCmd.Clone();
            incUpdUserProfilesCmd.CommandText = "SELECT id, username, user_type, enabled " +
                                          "FROM \"UserProfiles\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id=:session_user_id;";
            incUpdUserProfilesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserProfilesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserProfilesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdUserProfilesCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfiles.SelectIncrementalUpdatesCommand = incUpdUserProfilesCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserProfilesCmd = incInsUserProfilesCmd.Clone();
            incDelUserProfilesCmd.CommandText = "SELECT id FROM \"UserProfiles_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserProfilesCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfilesCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfilesCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserProfilesCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserProfiles.SelectIncrementalDeletesCommand = incDelUserProfilesCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserProfiles);
            # endregion
            # region UserGroups
            SyncAdapter adapterUserGroups = new SyncAdapter("UserGroups");
            # region incremental insert command
            NpgsqlCommand incInsUserGroupsCmd = new NpgsqlCommand();
            incInsUserGroupsCmd.CommandType = CommandType.Text;
            incInsUserGroupsCmd.CommandText = "SELECT id, name " +
                                          "FROM \"UserGroups\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT groups_id FROM \"UserProfiles_UserGroups\" WHERE users_id=:session_user_id);";
            incInsUserGroupsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserGroupsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserGroupsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserGroupsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserGroups.SelectIncrementalInsertsCommand = incInsUserGroupsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdUserGroupsCmd = incInsUserGroupsCmd.Clone();
            incUpdUserGroupsCmd.CommandText = "SELECT id, name " +
                                          "FROM \"UserGroups\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN (SELECT groups_id FROM \"UserProfiles_UserGroups\" WHERE users_id=:session_user_id);";
            incUpdUserGroupsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserGroupsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserGroupsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdUserGroupsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserGroups.SelectIncrementalUpdatesCommand = incUpdUserGroupsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserGroupsCmd = incInsUserGroupsCmd.Clone();
            incDelUserGroupsCmd.CommandText = "SELECT id FROM \"UserGroups_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserGroupsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserGroupsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserGroupsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserGroupsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserGroups.SelectIncrementalDeletesCommand = incDelUserGroupsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserGroups);
            # endregion
            # region UserProfiles_UserGroups
            SyncAdapter adapterUserProfiles_UserGroups = new SyncAdapter("UserProfiles_UserGroups");
            # region incremental insert command
            NpgsqlCommand incInsUserProfiles_UserGroupsCmd = new NpgsqlCommand();
            incInsUserProfiles_UserGroupsCmd.CommandType = CommandType.Text;
            incInsUserProfiles_UserGroupsCmd.CommandText = "SELECT users_id, groups_id " +
                                          "FROM \"UserProfiles_UserGroups\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND users_id=:session_user_id;";
            incInsUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserProfiles_UserGroupsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfiles_UserGroups.SelectIncrementalInsertsCommand = incInsUserProfiles_UserGroupsCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserProfiles_UserGroupsCmd = incInsUserProfiles_UserGroupsCmd.Clone();
            incDelUserProfiles_UserGroupsCmd.CommandText = "SELECT users_id FROM \"UserProfiles_UserGroups_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfiles_UserGroupsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserProfiles_UserGroupsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserProfiles_UserGroups.SelectIncrementalDeletesCommand = incDelUserProfiles_UserGroupsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserProfiles_UserGroups);
            # endregion
            # region TypeDefinitions
            SyncAdapter adapterTypeDefinitions = new SyncAdapter("TypeDefinitions");
            # region incremental insert command
            NpgsqlCommand incInsTypeDefinitionsCmd = new NpgsqlCommand();
            incInsTypeDefinitionsCmd.CommandType = CommandType.Text;
            incInsTypeDefinitionsCmd.CommandText = "SELECT id, clr_name, parent_id " +
                                          "FROM \"TypeDefinitions\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + ";";
            incInsTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterTypeDefinitions.SelectIncrementalInsertsCommand = incInsTypeDefinitionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdTypeDefinitionsCmd = incInsTypeDefinitionsCmd.Clone();
            incUpdTypeDefinitionsCmd.CommandText = "SELECT id, clr_name, parent_id " +
                                          "FROM \"TypeDefinitions\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + ";";
            incUpdTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterTypeDefinitions.SelectIncrementalUpdatesCommand = incUpdTypeDefinitionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelTypeDefinitionsCmd = incInsTypeDefinitionsCmd.Clone();
            incDelTypeDefinitionsCmd.CommandText = "SELECT id FROM \"TypeDefinitions_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelTypeDefinitionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelTypeDefinitionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterTypeDefinitions.SelectIncrementalDeletesCommand = incDelTypeDefinitionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterTypeDefinitions);
            # endregion
            # region Permissions
            SyncAdapter adapterPermissions = new SyncAdapter("Permissions");
            # region incremental insert command
            NpgsqlCommand incInsPermissionsCmd = new NpgsqlCommand();
            incInsPermissionsCmd.CommandType = CommandType.Text;
            incInsPermissionsCmd.CommandText = "SELECT id, types_id, name, \"Permissions\".default " +
                                          "FROM \"Permissions\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + ";";
            incInsPermissionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsPermissionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsPermissionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterPermissions.SelectIncrementalInsertsCommand = incInsPermissionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdPermissionsCmd = incInsPermissionsCmd.Clone();
            incUpdPermissionsCmd.CommandText = "SELECT id, types_id, name, \"Permissions\".default " +
                                          "FROM \"Permissions\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + ";";
            incUpdPermissionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdPermissionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdPermissionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterPermissions.SelectIncrementalUpdatesCommand = incUpdPermissionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelPermissionsCmd = incInsPermissionsCmd.Clone();
            incDelPermissionsCmd.CommandText = "SELECT id FROM \"Permissions_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelPermissionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelPermissionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelPermissionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelPermissionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterPermissions.SelectIncrementalDeletesCommand = incDelPermissionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterPermissions);
            # endregion
            # region ObjectList
            SyncAdapter adapterObjectList = new SyncAdapter("ObjectList");
            # region incremental insert command
            NpgsqlCommand incInsObjectListCmd = new NpgsqlCommand();
            incInsObjectListCmd.CommandType = CommandType.Text;
            incInsObjectListCmd.CommandText = "SELECT id, locator, parent_id " +
                                          "FROM \"ObjectList\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN ((SELECT object_id FROM \"AccessControlList\" AS acl INNER JOIN \"UserProfiles_AccessControlList\" AS ua ON acl.id = ua.acl_id " +
                                          "WHERE users_id=:session_user_id) " +
                                          "UNION " +
                                          "(SELECT object_id FROM \"AccessControlList\" AS acl INNER JOIN \"UserGroups_AccessControlList\" AS ua ON acl.id = ua.acl_id " +
                                          "INNER JOIN \"UserGroups\" AS g ON ua.groups_id = g.id " +
                                          "INNER JOIN \"UserProfiles_UserGroups\" AS uu ON g.id = uu.groups_id WHERE uu.users_id=:session_user_id));";
            incInsObjectListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsObjectListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsObjectListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsObjectListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterObjectList.SelectIncrementalInsertsCommand = incInsObjectListCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdObjectListCmd = incInsObjectListCmd.Clone();
            incUpdObjectListCmd.CommandText = "SELECT id, locator, parent_id " +
                                          "FROM \"ObjectList\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN ((SELECT object_id FROM \"AccessControlList\" AS acl INNER JOIN \"UserProfiles_AccessControlList\" AS ua ON acl.id = ua.acl_id " +
                                          "WHERE users_id=:session_user_id) " +
                                          "UNION " +
                                          "(SELECT object_id FROM \"AccessControlList\" AS acl INNER JOIN \"UserGroups_AccessControlList\" AS ua ON acl.id = ua.acl_id " +
                                          "INNER JOIN \"UserGroups\" AS g ON ua.groups_id = g.id " +
                                          "INNER JOIN \"UserProfiles_UserGroups\" AS uu ON g.id = uu.groups_id WHERE uu.users_id=:session_user_id));";
            incUpdObjectListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdObjectListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdObjectListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdObjectListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterObjectList.SelectIncrementalUpdatesCommand = incUpdObjectListCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelObjectListCmd = incInsObjectListCmd.Clone();
            incDelObjectListCmd.CommandText = "SELECT id FROM \"ObjectList_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelObjectListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelObjectListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelObjectListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelObjectListCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterObjectList.SelectIncrementalDeletesCommand = incDelObjectListCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterObjectList);
            # endregion
            # region AccessControlList
            SyncAdapter adapterAccessControlList = new SyncAdapter("AccessControlList");
            # region incremental insert command
            NpgsqlCommand incInsAccessControlListCmd = new NpgsqlCommand();
            incInsAccessControlListCmd.CommandType = CommandType.Text;
            incInsAccessControlListCmd.CommandText = "SELECT id, object_id, permissions_id, access " +
                                          "FROM \"AccessControlList\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN ((SELECT acl_id FROM \"UserProfiles_AccessControlList\" WHERE users_id=:session_user_id) " +
                                          "UNION " +
                                          "(SELECT acl_id FROM \"UserGroups_AccessControlList\" AS ua INNER JOIN \"UserGroups\" AS g ON ua.groups_id = g.id " +
                                          "INNER JOIN \"UserProfiles_UserGroups\" AS uu ON g.id = uu.groups_id WHERE uu.users_id=:session_user_id));";
            incInsAccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsAccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsAccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsAccessControlListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterAccessControlList.SelectIncrementalInsertsCommand = incInsAccessControlListCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdAccessControlListCmd = incInsAccessControlListCmd.Clone();
            incUpdAccessControlListCmd.CommandText = "SELECT id, object_id, permissions_id, access " +
                                          "FROM \"AccessControlList\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND id IN ((SELECT acl_id FROM \"UserProfiles_AccessControlList\" WHERE users_id=:session_user_id) " +
                                          "UNION " +
                                          "(SELECT acl_id FROM \"UserGroups_AccessControlList\" AS ua INNER JOIN \"UserGroups\" AS g ON ua.groups_id = g.id " +
                                          "INNER JOIN \"UserProfiles_UserGroups\" AS uu ON g.id = uu.groups_id WHERE uu.users_id=:session_user_id));";
            incUpdAccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdAccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdAccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdAccessControlListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterAccessControlList.SelectIncrementalUpdatesCommand = incUpdAccessControlListCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelAccessControlListCmd = incInsAccessControlListCmd.Clone();
            incDelAccessControlListCmd.CommandText = "SELECT id FROM \"AccessControlList_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelAccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelAccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelAccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelAccessControlListCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterAccessControlList.SelectIncrementalDeletesCommand = incDelAccessControlListCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterAccessControlList);
            # endregion
            # region UserProfiles_AccessControlList
            SyncAdapter adapterUserProfiles_AccessControlList = new SyncAdapter("UserProfiles_AccessControlList");
            # region incremental insert command
            NpgsqlCommand incInsUserProfiles_AccessControlListCmd = new NpgsqlCommand();
            incInsUserProfiles_AccessControlListCmd.CommandType = CommandType.Text;
            incInsUserProfiles_AccessControlListCmd.CommandText = "SELECT users_id, acl_id " +
                                          "FROM \"UserProfiles_AccessControlList\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND users_id=:session_user_id;";
            incInsUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserProfiles_AccessControlListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfiles_AccessControlList.SelectIncrementalInsertsCommand = incInsUserProfiles_AccessControlListCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserProfiles_AccessControlListCmd = incInsUserProfiles_AccessControlListCmd.Clone();
            incDelUserProfiles_AccessControlListCmd.CommandText = "SELECT users_id FROM \"UserProfiles_AccessControlList_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfiles_AccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserProfiles_AccessControlListCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserProfiles_AccessControlList.SelectIncrementalDeletesCommand = incDelUserProfiles_AccessControlListCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserProfiles_AccessControlList);
            # endregion
            # region UserGroups_AccessControlList
            SyncAdapter adapterUserGroups_AccessControlList = new SyncAdapter("UserGroups_AccessControlList");
            # region incremental insert command
            NpgsqlCommand incInsUserGroups_AccessControlListCmd = new NpgsqlCommand();
            incInsUserGroups_AccessControlListCmd.CommandType = CommandType.Text;
            incInsUserGroups_AccessControlListCmd.CommandText = "SELECT groups_id, acl_id " +
                                          "FROM \"UserGroups_AccessControlList\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND groups_id IN (SELECT groups_id FROM \"UserProfiles_UserGroups\" WHERE users_id=:session_user_id);";
            incInsUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserGroups_AccessControlListCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserGroups_AccessControlList.SelectIncrementalInsertsCommand = incInsUserGroups_AccessControlListCmd;
            # endregion
            # region incremental update command
            //NOT applicable if only PK-Table!!!
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserGroups_AccessControlListCmd = incInsUserGroups_AccessControlListCmd.Clone();
            incDelUserGroups_AccessControlListCmd.CommandText = "SELECT groups_id FROM \"UserGroups_AccessControlList_tombstone\" " +
                                          "WHERE ((update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ")) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserGroups_AccessControlListCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserGroups_AccessControlListCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserGroups_AccessControlList.SelectIncrementalDeletesCommand = incDelUserGroups_AccessControlListCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserGroups_AccessControlList);
            # endregion
            # region UserCardState
            SyncAdapter adapterUserCardState = new SyncAdapter("UserCardState");
            # region incremental insert command
            NpgsqlCommand incInsUserCardStateCmd = new NpgsqlCommand();
            incInsUserCardStateCmd.CommandType = CommandType.Text;
            incInsUserCardStateCmd.CommandText = "SELECT ST.user_id, cards_id, box, active, timestamp " +
                                          "FROM \"UserCardState\" AS ST INNER JOIN \"Cards\" AS S ON ST.cards_id=S.id " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND user_id=:session_user_id " +
                                          "AND S.id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incInsUserCardStateCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserCardStateCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserCardStateCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsUserCardStateCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserCardState.SelectIncrementalInsertsCommand = incInsUserCardStateCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdUserCardStateCmd = incInsUserCardStateCmd.Clone();
            incUpdUserCardStateCmd.CommandText = "SELECT ST.user_id, cards_id, box, active, timestamp " +
                                          "FROM \"UserCardState\" AS ST INNER JOIN \"Cards\" AS S ON ST.cards_id=S.id " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND user_id=:session_user_id " +
                                          "AND S.id IN (SELECT cards_id FROM \"LearningModules_Cards\" WHERE lm_id=:session_lm_id);";
            incUpdUserCardStateCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserCardStateCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdUserCardStateCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdUserCardStateCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserCardState.SelectIncrementalUpdatesCommand = incUpdUserCardStateCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserCardStateCmd = incInsUserCardStateCmd.Clone();
            incDelUserCardStateCmd.CommandText = "SELECT user_id, cards_id FROM \"UserCardState_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelUserCardStateCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserCardStateCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelUserCardStateCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserCardState.SelectIncrementalDeletesCommand = incDelUserCardStateCmd;
            # endregion
            # region insert command
            NpgsqlCommand insUserCardStateCmd = incInsUserCardStateCmd.Clone();
            insUserCardStateCmd.CommandText = "INSERT INTO \"UserCardState\" (user_id, cards_id, box, active, timestamp, update_originator_id) " +
                                        "VALUES (:user_id, :cards_id, :box, :active, :timestamp, :" + SyncSession.SyncClientIdHash + ");";
            insUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insUserCardStateCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            insUserCardStateCmd.Parameters.Add("cards_id", NpgsqlDbType.Integer);
            insUserCardStateCmd.Parameters.Add("box", NpgsqlDbType.Integer);
            insUserCardStateCmd.Parameters.Add("active", NpgsqlDbType.Boolean);
            insUserCardStateCmd.Parameters.Add("timestamp", NpgsqlDbType.Timestamp);
            adapterUserCardState.InsertCommand = insUserCardStateCmd;
            # endregion
            # region update command
            NpgsqlCommand updUserCardStateCmd = incInsUserCardStateCmd.Clone();
            updUserCardStateCmd.CommandText = "UPDATE \"UserCardState\" SET box=:box, active=:active, timestamp=:timestamp" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE user_id=:user_id AND cards_id=:cards_id;";
            updUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updUserCardStateCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            updUserCardStateCmd.Parameters.Add("cards_id", NpgsqlDbType.Integer);
            updUserCardStateCmd.Parameters.Add("box", NpgsqlDbType.Integer);
            updUserCardStateCmd.Parameters.Add("active", NpgsqlDbType.Boolean);
            updUserCardStateCmd.Parameters.Add("timestamp", NpgsqlDbType.Timestamp);
            adapterUserCardState.UpdateCommand = updUserCardStateCmd;
            # endregion
            # region delete command
            NpgsqlCommand delUserCardStateCmd = incInsUserCardStateCmd.Clone();
            delUserCardStateCmd.CommandText = "DELETE FROM \"UserCardState\" WHERE user_id=:user_id AND cards_id=:cards_id; " +
                                        "UPDATE \"UserCardState_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE user_id=:user_id AND cards_id=:cards_id;";
            delUserCardStateCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delUserCardStateCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            delUserCardStateCmd.Parameters.Add("cards_id", NpgsqlDbType.Integer);
            adapterUserCardState.DeleteCommand = delUserCardStateCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserCardState);
            # endregion
            # region UserProfilesLearningModulesSettings
            SyncAdapter adapterUserProfilesLearningModulesSettings = new SyncAdapter("UserProfilesLearningModulesSettings");
            # region incremental insert command
            NpgsqlCommand incInsUserProfilesLearningModulesSettingsCmd = new NpgsqlCommand();
            incInsUserProfilesLearningModulesSettingsCmd.CommandType = CommandType.Text;
            incInsUserProfilesLearningModulesSettingsCmd.CommandText = "SELECT user_id, lm_id, settings_id, highscore " +
                                          "FROM \"UserProfilesLearningModulesSettings\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND user_id=:session_user_id AND lm_id=:session_lm_id;";
            incInsUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfilesLearningModulesSettings.SelectIncrementalInsertsCommand = incInsUserProfilesLearningModulesSettingsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdUserProfilesLearningModulesSettingsCmd = incInsUserProfilesLearningModulesSettingsCmd.Clone();
            incUpdUserProfilesLearningModulesSettingsCmd.CommandText = "SELECT user_id, lm_id, settings_id, highscore " +
                                          "FROM \"UserProfilesLearningModulesSettings\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND user_id=:session_user_id AND lm_id=:session_lm_id;";
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterUserProfilesLearningModulesSettings.SelectIncrementalUpdatesCommand = incUpdUserProfilesLearningModulesSettingsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelUserProfilesLearningModulesSettingsCmd = incInsUserProfilesLearningModulesSettingsCmd.Clone();
            incDelUserProfilesLearningModulesSettingsCmd.CommandText = "SELECT user_id, lm_id FROM \"UserProfilesLearningModulesSettings_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND user_id=:session_user_id AND lm_id=:session_lm_id " +
                                          "AND NOT :isNewDb;";
            incDelUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdUserProfilesLearningModulesSettingsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            incDelUserProfilesLearningModulesSettingsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterUserProfilesLearningModulesSettings.SelectIncrementalDeletesCommand = incDelUserProfilesLearningModulesSettingsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insUserProfilesLearningModulesSettingsCmd = incInsUserProfilesLearningModulesSettingsCmd.Clone();
            insUserProfilesLearningModulesSettingsCmd.CommandText = "INSERT INTO \"UserProfilesLearningModulesSettings\" (user_id, lm_id, settings_id, highscore) " +
                                        "VALUES (:user_id, :lm_id, :settings_id, :highscore, :" + SyncSession.SyncClientIdHash + ");";
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("settings_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("highscore", NpgsqlDbType.Numeric);
            adapterUserProfilesLearningModulesSettings.InsertCommand = insUserProfilesLearningModulesSettingsCmd;
            # endregion
            # region update command
            NpgsqlCommand updUserProfilesLearningModulesSettingsCmd = incInsUserProfilesLearningModulesSettingsCmd.Clone();
            updUserProfilesLearningModulesSettingsCmd.CommandText = "UPDATE \"UserProfilesLearningModulesSettings\" SET settings_id=:settings_id, highscore=:highscore" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE user_id=:user_id AND lm_id=:lm_id;";
            updUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("settings_id", NpgsqlDbType.Integer);
            insUserProfilesLearningModulesSettingsCmd.Parameters.Add("highscore", NpgsqlDbType.Numeric);
            adapterUserProfilesLearningModulesSettings.UpdateCommand = updUserProfilesLearningModulesSettingsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delUserProfilesLearningModulesSettingsCmd = incInsUserProfilesLearningModulesSettingsCmd.Clone();
            updUserProfilesLearningModulesSettingsCmd.CommandText = "DELETE FROM \"UserProfilesLearningModulesSettings\" WHERE user_id=:user_id AND lm_id=:lm_id; " +
                                        "UPDATE \"UserProfilesLearningModulesSettings_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE user_id=:user_id AND lm_id=:lm_id;";
            updUserProfilesLearningModulesSettingsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updUserProfilesLearningModulesSettingsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterUserProfilesLearningModulesSettings.DeleteCommand = updUserProfilesLearningModulesSettingsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterUserProfilesLearningModulesSettings);
            # endregion
            # region LearningSessions
            SyncAdapter adapterLearningSessions = new SyncAdapter("LearningSessions");
            # region incremental insert command
            NpgsqlCommand incInsLearningSessionsCmd = new NpgsqlCommand();
            incInsLearningSessionsCmd.CommandType = CommandType.Text;
            incInsLearningSessionsCmd.CommandText = "SELECT id, user_id, lm_id, starttime, endtime, sum_right, sum_wrong, pool_content, " +
                                          "box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content " +
                                          "FROM \"LearningSessions\" " +
                                          "WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND lm_id=:session_lm_id AND user_id=:session_user_id;";
            incInsLearningSessionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningSessionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsLearningSessionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsLearningSessionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterLearningSessions.SelectIncrementalInsertsCommand = incInsLearningSessionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdLearningSessionsCmd = incInsLearningSessionsCmd.Clone();
            incUpdLearningSessionsCmd.CommandText = "SELECT id, user_id, lm_id, starttime, endtime, sum_right, sum_wrong, pool_content, " +
                                          "box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content " +
                                          "FROM \"LearningSessions\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND lm_id=:session_lm_id AND user_id=:session_user_id;";
            incUpdLearningSessionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearningSessionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdLearningSessionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdLearningSessionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterLearningSessions.SelectIncrementalUpdatesCommand = incUpdLearningSessionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelLearningSessionsCmd = incInsLearningSessionsCmd.Clone();
            incDelLearningSessionsCmd.CommandText = "SELECT id FROM \"LearningSessions_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND lm_id=:session_lm_id AND user_id=:session_user_id " +
                                          "AND NOT :isNewDb;";
            incDelLearningSessionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningSessionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdLearningSessionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdLearningSessionsCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            incDelLearningSessionsCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterLearningSessions.SelectIncrementalDeletesCommand = incDelLearningSessionsCmd;
            # endregion
            # region insert command
            NpgsqlCommand insLearningSessionsCmd = incInsLearningSessionsCmd.Clone();
            insLearningSessionsCmd.CommandText = "INSERT INTO \"LearningSessions\" (id, user_id, lm_id, starttime, endtime, sum_right, sum_wrong, pool_content, " +
                                        "box1_content, box2_content, box3_content, box4_content, box5_content, box6_content, box7_content, box8_content, box9_content, box10_content, " +
                                        "update_originator_id) " +
                                        "VALUES (:id, :user_id, :lm_id, :starttime, :endtime, :sum_right, :sum_wrong, :pool_content, " +
                                        ":box1_content, :box2_content, :box3_content, :box4_content, :box5_content, :box6_content, :box7_content, :box8_content, :box9_content, :box10_content, " +
                                        ":" + SyncSession.SyncClientIdHash + ");";
            insLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("starttime", NpgsqlDbType.Timestamp);
            insLearningSessionsCmd.Parameters.Add("endtime", NpgsqlDbType.Timestamp);
            insLearningSessionsCmd.Parameters.Add("sum_right", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("sum_wrong", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("pool_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box1_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box2_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box3_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box4_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box5_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box6_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box7_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box8_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box9_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box10_content", NpgsqlDbType.Integer);
            adapterLearningSessions.InsertCommand = insLearningSessionsCmd;
            # endregion
            # region update command
            NpgsqlCommand updLearningSessionsCmd = incInsLearningSessionsCmd.Clone();
            updLearningSessionsCmd.CommandText = "UPDATE \"LearningSessions\" SET user_id=:user_id, lm_id=:lm_id, starttime=:starttime, endtime=:endtime" +
                                        "sum_right=:sum_right, sum_wrong=:sum_wrong, pool_content=:pool_content, " +
                                        "box1_content=:box1_content, box2_content=:box2_content, box3_content=:box3_content, box4_content=:box4_content, " +
                                        "box5_content=:box5_content, box6_content=:box6_content, box7_content=:box7_content, box8_content=:box8_content, " +
                                        "box9_content=:box9_content, box10_content=:box10_content, " +
                                        "update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updLearningSessionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updLearningSessionsCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            updLearningSessionsCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            updLearningSessionsCmd.Parameters.Add("starttime", NpgsqlDbType.Timestamp);
            updLearningSessionsCmd.Parameters.Add("endtime", NpgsqlDbType.Timestamp);
            insLearningSessionsCmd.Parameters.Add("sum_right", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("sum_wrong", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("pool_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box1_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box2_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box3_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box4_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box5_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box6_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box7_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box8_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box9_content", NpgsqlDbType.Integer);
            insLearningSessionsCmd.Parameters.Add("box10_content", NpgsqlDbType.Integer);
            adapterLearningSessions.UpdateCommand = updLearningSessionsCmd;
            # endregion
            # region delete command
            NpgsqlCommand delLearningSessionsCmd = incInsLearningSessionsCmd.Clone();
            delLearningSessionsCmd.CommandText = "DELETE FROM \"LearningSessions\" WHERE id=:id; " +
                                        "UPDATE \"LearningSessions_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delLearningSessionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delLearningSessionsCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterLearningSessions.DeleteCommand = delLearningSessionsCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterLearningSessions);
            # endregion
            # region LearnLog
            SyncAdapter adapterLearnLog = new SyncAdapter("LearnLog");
            # region incremental insert command
            NpgsqlCommand incInsLearnLogCmd = new NpgsqlCommand();
            incInsLearnLogCmd.CommandType = CommandType.Text;
            incInsLearnLogCmd.CommandText = "SELECT ST.id, ST.user_id, ST.lm_id, session_id, cards_id, old_box, new_box, timestamp, duration, learn_mode, move_type, " +
                                          "answer, direction, case_sensitive, correct_on_the_fly, percentage_known, percentage_required " +
                                          "FROM \"LearnLog\" AS ST INNER JOIN \"LearningSessions\" AS S ON ST.session_id=S.id " +
                                          "WHERE ST.create_timestamp is null OR (  ST.create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and ST.create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND S.lm_id=:session_lm_id AND S.user_id=:session_user_id;";
            incInsLearnLogCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearnLogCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsLearnLogCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incInsLearnLogCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterLearnLog.SelectIncrementalInsertsCommand = incInsLearnLogCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdLearnLogCmd = incInsLearnLogCmd.Clone();
            incUpdLearnLogCmd.CommandText = "SELECT ST.id, ST.user_id, ST.lm_id, session_id, cards_id, old_box, new_box, timestamp, duration, learn_mode, move_type, " +
                                          "answer, direction, case_sensitive, correct_on_the_fly, percentage_known, percentage_required " +
                                          "FROM \"LearnLog\" AS ST INNER JOIN \"LearningSessions\" AS S ON ST.session_id=S.id " +
                                          "WHERE (ST.update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (ST.update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (ST.create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND ST.update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND S.lm_id=:session_lm_id AND S.user_id=:session_user_id;";
            incUpdLearnLogCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearnLogCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdLearnLogCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            incUpdLearnLogCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            adapterLearnLog.SelectIncrementalUpdatesCommand = incUpdLearnLogCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelLearnLogCmd = incInsLearnLogCmd.Clone();
            incDelLearnLogCmd.CommandText = "SELECT id FROM \"LearnLog_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelLearnLogCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearnLogCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelLearnLogCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterLearnLog.SelectIncrementalDeletesCommand = incDelLearnLogCmd;
            # endregion
            # region insert command
            NpgsqlCommand insLearnLogCmd = incInsLearnLogCmd.Clone();
            insLearnLogCmd.CommandText = "INSERT INTO \"LearnLog\" (session_id, user_id, lm_id, cards_id, old_box, new_box, timestamp, duration, learn_mode, move_type, " +
                                            "answer, direction, case_sensitive, correct_on_the_fly, percentage_known, percentage_required, update_originator_id) " +
                                        "VALUES (:session_id, :user_id, :lm_id, :cards_id, :old_box, :new_box, :timestamp, :duration, :learn_mode, :move_type, " +
                                            ":answer, :direction, :case_sensitive, :correct_on_the_fly, :percentage_known, :percentage_required, :" + SyncSession.SyncClientIdHash + ");";
            insLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("session_id", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("cards_id", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("old_box", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("new_box", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("timestamp", NpgsqlDbType.Timestamp);
            insLearnLogCmd.Parameters.Add("duration", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("learn_mode", EQueryType.ListeningComprehension.ToString());
            insLearnLogCmd.Parameters.Add("move_type", MoveType.CanceledDemote.ToString());
            insLearnLogCmd.Parameters.Add("answer", NpgsqlDbType.Text);
            insLearnLogCmd.Parameters.Add("direction", EQueryDirection.Answer2Question.ToString());
            insLearnLogCmd.Parameters.Add("case_sensitive", NpgsqlDbType.Boolean);
            insLearnLogCmd.Parameters.Add("correct_on_the_fly", NpgsqlDbType.Boolean);
            insLearnLogCmd.Parameters.Add("percentage_known", NpgsqlDbType.Integer);
            insLearnLogCmd.Parameters.Add("percentage_required", NpgsqlDbType.Integer);
            adapterLearnLog.InsertCommand = insLearnLogCmd;
            # endregion
            # region update command
            NpgsqlCommand updLearnLogCmd = incInsLearnLogCmd.Clone();
            updLearnLogCmd.CommandText = "UPDATE \"LearnLog\" SET session_id=:session_id, user_id=:user_id, lm_id=:lm_id, cards_id=:cards_id, old_box=:old_box, new_box=:new_box, " +
                                        "timestamp=:timestamp, duration=:duration, learn_mode=:learn_mode, move_type=:move_type, answer=:answer, direction=:direction, case_sensitive=:case_sensitive, " +
                                        "correct_on_the_fly=:correct_on_the_fly, percentage_known=:percentage_known, percentage_required=:percentage_required" +
                                        ", update_originator_id=:" + SyncSession.SyncClientIdHash + " WHERE id=:id;";
            updLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("session_id", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("user_id", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("lm_id", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("cards_id", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("old_box", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("new_box", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("timestamp", NpgsqlDbType.Timestamp);
            updLearnLogCmd.Parameters.Add("duration", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("learn_mode", EQueryType.ListeningComprehension.ToString());
            updLearnLogCmd.Parameters.Add("move_type", MoveType.CanceledDemote.ToString());
            updLearnLogCmd.Parameters.Add("answer", NpgsqlDbType.Text);
            updLearnLogCmd.Parameters.Add("direction", EQueryDirection.Answer2Question.ToString());
            updLearnLogCmd.Parameters.Add("case_sensitive", NpgsqlDbType.Boolean);
            updLearnLogCmd.Parameters.Add("correct_on_the_fly", NpgsqlDbType.Boolean);
            updLearnLogCmd.Parameters.Add("percentage_known", NpgsqlDbType.Integer);
            updLearnLogCmd.Parameters.Add("percentage_required", NpgsqlDbType.Integer);
            adapterLearnLog.UpdateCommand = updLearnLogCmd;
            # endregion
            # region delete command
            NpgsqlCommand delLearnLogCmd = incInsLearnLogCmd.Clone();
            delLearnLogCmd.CommandText = "DELETE FROM \"LearnLog\" WHERE id=:id; " +
                                        "UPDATE \"LearnLog_tombstone\" SET update_originator_id=:" + SyncSession.SyncClientIdHash + " " +
                                        "WHERE id=:id;";
            delLearnLogCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            delLearnLogCmd.Parameters.Add("id", NpgsqlDbType.Integer);
            adapterLearnLog.DeleteCommand = delLearnLogCmd;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterLearnLog);
            # endregion
            # region DatabaseInformation
            SyncAdapter adapterDatabaseInformation = new SyncAdapter("DatabaseInformation");
            # region incremental insert command
            NpgsqlCommand incInsDatabaseInformationCmd = new NpgsqlCommand();
            incInsDatabaseInformationCmd.CommandType = CommandType.Text;
            incInsDatabaseInformationCmd.CommandText = "SELECT property, value " +
                                          "FROM \"DatabaseInformation\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "and create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " ;";
            incInsDatabaseInformationCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsDatabaseInformationCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsDatabaseInformationCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterDatabaseInformation.SelectIncrementalInsertsCommand = incInsDatabaseInformationCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdDatabaseInformationCmd = incInsDatabaseInformationCmd.Clone();
            incUpdDatabaseInformationCmd.CommandText = "SELECT property, value " +
                                          "FROM \"DatabaseInformation\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " ;";
            incUpdDatabaseInformationCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdDatabaseInformationCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdDatabaseInformationCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            adapterDatabaseInformation.SelectIncrementalUpdatesCommand = incUpdDatabaseInformationCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelDatabaseInformation = incInsDatabaseInformationCmd.Clone();
            incDelDatabaseInformation.CommandText = "SELECT property FROM \"DatabaseInformation_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "and (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "and (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelDatabaseInformation.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelDatabaseInformation.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelDatabaseInformation.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelDatabaseInformation.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterDatabaseInformation.SelectIncrementalDeletesCommand = incDelDatabaseInformation;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterDatabaseInformation);
            # endregion
            #region Extensions
            SyncAdapter adapterExtensions = new SyncAdapter("Extensions");
            # region incremental insert command
            NpgsqlCommand incInsExtensionsCmd = new NpgsqlCommand();
            incInsExtensionsCmd.CommandType = CommandType.Text;
            incInsExtensionsCmd.CommandText = "SELECT guid, lm_id, name, version, type, startfile " +
                                          "FROM \"Extensions\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "AND create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND (lm_id=:session_lm_id OR lm_id=NULL);";
            incInsExtensionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsExtensionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsExtensionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsExtensionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterExtensions.SelectIncrementalInsertsCommand = incInsExtensionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdExtensionsCmd = incInsExtensionsCmd.Clone();
            incUpdExtensionsCmd.CommandText = "SELECT guid, lm_id, name, version, type, startfile " +
                                          "FROM \"Extensions\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "AND (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " AND (lm_id=:session_lm_id OR lm_id=NULL);";
            incUpdExtensionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdExtensionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdExtensionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdExtensionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterExtensions.SelectIncrementalUpdatesCommand = incUpdExtensionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelExceptions = incInsExtensionsCmd.Clone();
            incDelExceptions.CommandText = "SELECT guid FROM \"Extensions_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "AND (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelExceptions.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelExceptions.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelExceptions.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelExceptions.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterExtensions.SelectIncrementalDeletesCommand = incDelExceptions;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterExtensions);
            #endregion
            #region ExtensionActions
            SyncAdapter adapterExtensionActions = new SyncAdapter("ExtensionActions");
            # region incremental insert command
            NpgsqlCommand incInsExtensionActionsCmd = new NpgsqlCommand();
            incInsExtensionActionsCmd.CommandType = CommandType.Text;
            incInsExtensionActionsCmd.CommandText = "SELECT guid, action, execution " +
                                          "FROM \"ExtensionActions\" WHERE create_timestamp is null OR (  create_timestamp > :" + SyncSession.SyncLastReceivedAnchor + " " +
                                          "AND create_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + " ) " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND guid IN (SELECT guid FROM \"Extensions\" WHERE lm_id=:session_lm_id OR lm_id=NULL);";
            incInsExtensionActionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsExtensionActionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incInsExtensionActionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incInsExtensionActionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterExtensionActions.SelectIncrementalInsertsCommand = incInsExtensionActionsCmd;
            # endregion
            # region incremental update command
            NpgsqlCommand incUpdExtensionActionsCmd = incInsExtensionActionsCmd.Clone();
            incUpdExtensionActionsCmd.CommandText = "SELECT guid, action, execution " +
                                          "FROM \"ExtensionActions\" WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "AND (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND guid IN (SELECT guid FROM \"Extensions\" WHERE lm_id=:session_lm_id OR lm_id=NULL);";
            incUpdExtensionActionsCmd.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdExtensionActionsCmd.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incUpdExtensionActionsCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incUpdExtensionActionsCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            adapterExtensionActions.SelectIncrementalUpdatesCommand = incUpdExtensionActionsCmd;
            # endregion
            # region incremental delete command
            NpgsqlCommand incDelExceptionActions = incInsExtensionActionsCmd.Clone();
            incDelExceptionActions.CommandText = "SELECT guid, action FROM \"ExtensionActions_tombstone\" " +
                                          "WHERE (update_timestamp > :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND (update_timestamp <= :" + SyncSession.SyncNewReceivedAnchor + ") " +
                                          "AND (create_timestamp <= :" + SyncSession.SyncLastReceivedAnchor + ") " +
                                          "AND update_originator_id <> :" + SyncSession.SyncClientIdHash + " " +
                                          "AND NOT :isNewDb;";
            incDelExceptionActions.Parameters.Add(SyncSession.SyncLastReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelExceptionActions.Parameters.Add(SyncSession.SyncNewReceivedAnchor, NpgsqlTypes.NpgsqlDbType.Timestamp);
            incDelExceptionActions.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            incDelExceptionActions.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            adapterExtensionActions.SelectIncrementalDeletesCommand = incDelExceptionActions;
            # endregion
            serverSyncProvider.SyncAdapters.Add(adapterExtensionActions);
            #endregion
        }
        private void PrepareAnchor()
        {
            // select new anchor command                
            NpgsqlCommand anchorCmd = new NpgsqlCommand();
            anchorCmd.CommandType = CommandType.Text;
            anchorCmd.CommandText = "SELECT * FROM now();";
            NpgsqlParameter param = new NpgsqlParameter(SyncSession.SyncNewReceivedAnchor, NpgsqlDbType.Timestamp, 20);
            param.Direction = ParameterDirection.Output;
            anchorCmd.Parameters.Clear();
            anchorCmd.Parameters.Add(param);
            serverSyncProvider.SelectNewAnchorCommand = anchorCmd;
        }
        private void PrepareBatchingAnchor()
        {
            // select new anchor command                
            NpgsqlCommand anchorCmd = new NpgsqlCommand();
            anchorCmd.CommandType = CommandType.StoredProcedure;
            anchorCmd.CommandText = "\"GetNewBatchAnchor\"";
            anchorCmd.Parameters.Clear();
            anchorCmd.Parameters.Add(new NpgsqlParameter(SyncSession.SyncLastReceivedAnchor, NpgsqlDbType.Timestamp));
            anchorCmd.Parameters[SyncSession.SyncLastReceivedAnchor].Direction = ParameterDirection.Input;

            anchorCmd.Parameters.Add(new NpgsqlParameter(SyncSession.SyncBatchSize, NpgsqlDbType.Bigint));
            anchorCmd.Parameters[SyncSession.SyncBatchSize].Direction = ParameterDirection.Input;

            anchorCmd.Parameters.Add(new NpgsqlParameter(SyncSession.SyncMaxReceivedAnchor, NpgsqlDbType.Timestamp));
            anchorCmd.Parameters[SyncSession.SyncMaxReceivedAnchor].Direction = ParameterDirection.Output;

            anchorCmd.Parameters.Add(new NpgsqlParameter(SyncSession.SyncNewReceivedAnchor, NpgsqlDbType.Timestamp));
            anchorCmd.Parameters[SyncSession.SyncNewReceivedAnchor].Direction = ParameterDirection.Output;

            anchorCmd.Parameters.Add(new NpgsqlParameter(SyncSession.SyncBatchCount, NpgsqlDbType.Integer));
            anchorCmd.Parameters[SyncSession.SyncBatchCount].Direction = ParameterDirection.InputOutput;

            anchorCmd.Parameters.Add(SyncSession.SyncClientIdHash, NpgsqlDbType.Integer);
            anchorCmd.Parameters[SyncSession.SyncClientIdHash].Direction = ParameterDirection.Input;

            anchorCmd.Parameters.Add("session_lm_id", NpgsqlDbType.Integer);
            anchorCmd.Parameters["session_lm_id"].Direction = ParameterDirection.Input;

            anchorCmd.Parameters.Add("session_user_id", NpgsqlDbType.Integer);
            anchorCmd.Parameters["session_user_id"].Direction = ParameterDirection.Input;

            anchorCmd.Parameters.Add("isNewDb", NpgsqlDbType.Boolean);
            anchorCmd.Parameters["isNewDb"].Direction = ParameterDirection.Input;

            serverSyncProvider.SelectNewAnchorCommand = anchorCmd;
        }

        [WebMethod(true)]
        public SyncServerInfo GetServerInfo(Microsoft.Synchronization.Data.SyncSession syncSession)
        {
            return serverSyncProvider.GetServerInfo(syncSession);
        }

        [WebMethod(true)]
        public SyncSchema GetSchema(Collection<string> tableNames, Microsoft.Synchronization.Data.SyncSession syncSession)
        {
            return serverSyncProvider.GetSchema(tableNames, syncSession);
        }

        [WebMethod(true)]
        public SyncContext GetChanges(SyncGroupMetadata groupMetadata, Microsoft.Synchronization.Data.SyncSession syncSession)
        {
            return serverSyncProvider.GetChanges(groupMetadata, syncSession);
        }

        [WebMethod(true)]
        public SyncContext ApplyChanges(SyncGroupMetadata groupMetadata, DataSet dataSet, Microsoft.Synchronization.Data.SyncSession syncSession)
        {
            return serverSyncProvider.ApplyChanges(groupMetadata, dataSet, syncSession);
        }
    }
}
