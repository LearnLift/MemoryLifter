using System;
using System.Data.SqlServerCe;
using System.IO;
using Microsoft.Synchronization;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Data.SqlServerCe;
using MLifter.DAL.DB.MsSqlCe;
using System.Diagnostics;

namespace MLifter.BusinessLayer
{
    public class SyncClient
    {
        private static readonly string paramIsNewDb = "isNewDb";

        /// <summary>
        /// Gets the agent.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <param name="learningModuleId">The learning module id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="path">The path.</param>
        /// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
        /// <param name="password">The password.</param>
        /// <param name="createNew">if set to <c>true</c> [create new].</param>
        /// <param name="overwrite">if set to <c>true</c> [overwrite].</param>
        /// <returns></returns>
        /// <remarks>Documented by Dev05, 2009-02-13</remarks>
        public static SyncAgent GetAgent(string serverUri, int learningModuleId, int userId, ref string path, bool contentProtected, string password, bool createNew, bool overwrite)
        {
            # region 1. Create instance of the sync components (client, agent, server)
            // In this demo the sync agent will connect to the server through WebService.
            // A web reference is added to the project and I created a thin wrapper SyncServerProviderProxy
            // That inherits from SyncServerProvider base class and redirect the calls to the web service.                
            //
            //                

            SyncAgent syncAgent = new SyncAgent();
            SyncWebService.PgSyncService syncWebService = new SyncWebService.PgSyncService();
            syncWebService.Url = serverUri;
            syncAgent.RemoteProvider = new ServerSyncProviderProxy(syncWebService);

            # endregion
            # region 2. Prepare client db connection and attach it to the sync provider

            if (createNew)
            {
                if (overwrite && File.Exists(path))
                    File.Delete(path);
                else
                {
                    int i = 1;
                    while (File.Exists(path.Replace(MLifter.DAL.Helper.EmbeddedDbExtension, "_" + i + MLifter.DAL.Helper.EmbeddedDbExtension))) i++;

                    path = path.Replace(MLifter.DAL.Helper.EmbeddedDbExtension, "_" + i + MLifter.DAL.Helper.EmbeddedDbExtension);
                }
                CreateNewDb(contentProtected ? MSSQLCEConn.GetFullConnectionString(path, password) : MSSQLCEConn.GetFullConnectionString(path));
            }
            else if (!File.Exists(path))
            {
                createNew = true;
                CreateNewDb(contentProtected ? MSSQLCEConn.GetFullConnectionString(path, password) : MSSQLCEConn.GetFullConnectionString(path));
            }

            SqlCeClientSyncProvider clientSyncProvider = new SqlCeClientSyncProvider(contentProtected ? MSSQLCEConn.GetFullConnectionString(path, password) : MSSQLCEConn.GetFullConnectionString(path));
            syncAgent.LocalProvider = clientSyncProvider;

            # endregion
            # region 3. Create SyncTables and SyncGroups
            // To sync a table, a SyncTable object needs to be created and setup with desired properties:
            // TableCreationOption tells the agent how to initialize the new table in the local database
            // SyncDirection is how changes from with respect to client {Download, Upload, Bidirectional or Snapshot}
            # region SyncTables
            SyncTable tableLearningModules = new SyncTable("LearningModules");
            tableLearningModules.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableLearningModules.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableCategories = new SyncTable("Categories");
            tableCategories.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableCategories.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableChapters = new SyncTable("Chapters");
            tableChapters.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableChapters.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableCards = new SyncTable("Cards");
            tableCards.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableCards.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableChapters_Cards = new SyncTable("Chapters_Cards");
            tableChapters_Cards.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableChapters_Cards.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableLearningModules_Cards = new SyncTable("LearningModules_Cards");
            tableLearningModules_Cards.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableLearningModules_Cards.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableTextContent = new SyncTable("TextContent");
            tableTextContent.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableTextContent.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableMediaContent = new SyncTable("MediaContent");
            tableMediaContent.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableMediaContent.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableMediaProperties = new SyncTable("MediaProperties");
            tableMediaProperties.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableMediaProperties.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableCards_MediaContent = new SyncTable("Cards_MediaContent");
            tableCards_MediaContent.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableCards_MediaContent.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableTags = new SyncTable("Tags");
            tableTags.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableTags.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableMediaContent_Tags = new SyncTable("MediaContent_Tags");
            tableMediaContent_Tags.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableMediaContent_Tags.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableLearningModules_Tags = new SyncTable("LearningModules_Tags");
            tableLearningModules_Tags.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableLearningModules_Tags.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableSettings = new SyncTable("Settings");
            tableSettings.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableSettings.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableSelectedLearnChapters = new SyncTable("SelectedLearnChapters");
            tableSelectedLearnChapters.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableSelectedLearnChapters.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableCommentarySounds = new SyncTable("CommentarySounds");
            tableCommentarySounds.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableCommentarySounds.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableQueryDirections = new SyncTable("QueryDirections");
            tableQueryDirections.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableQueryDirections.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableSynonymGradings = new SyncTable("SynonymGradings");
            tableSynonymGradings.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableSynonymGradings.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableQueryTypes = new SyncTable("QueryTypes");
            tableQueryTypes.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableQueryTypes.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableMultipleChoiceOptions = new SyncTable("MultipleChoiceOptions");
            tableMultipleChoiceOptions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableMultipleChoiceOptions.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableTypeGradings = new SyncTable("TypeGradings");
            tableTypeGradings.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableTypeGradings.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableCardStyles = new SyncTable("CardStyles");
            tableCardStyles.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableCardStyles.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableMediaContent_CardStyles = new SyncTable("MediaContent_CardStyles");
            tableMediaContent_CardStyles.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableMediaContent_CardStyles.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableStyleSheets = new SyncTable("StyleSheets");
            tableStyleSheets.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableStyleSheets.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableSnoozeOptions = new SyncTable("SnoozeOptions");
            tableSnoozeOptions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableSnoozeOptions.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableBoxes = new SyncTable("Boxes");
            tableBoxes.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableBoxes.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableUserProfilesLearningModulesSettings = new SyncTable("UserProfilesLearningModulesSettings");
            tableUserProfilesLearningModulesSettings.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserProfilesLearningModulesSettings.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserProfiles = new SyncTable("UserProfiles");
            tableUserProfiles.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserProfiles.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserGroups = new SyncTable("UserGroups");
            tableUserGroups.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserGroups.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserProfiles_UserGroups = new SyncTable("UserProfiles_UserGroups");
            tableUserProfiles_UserGroups.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserProfiles_UserGroups.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableTypeDefinitions = new SyncTable("TypeDefinitions");
            tableTypeDefinitions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableTypeDefinitions.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tablePermissions = new SyncTable("Permissions");
            tablePermissions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tablePermissions.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableObjectList = new SyncTable("ObjectList");
            tableObjectList.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableObjectList.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableAccessControlList = new SyncTable("AccessControlList");
            tableAccessControlList.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableAccessControlList.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserProfiles_AccessControlList = new SyncTable("UserProfiles_AccessControlList");
            tableUserProfiles_AccessControlList.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserProfiles_AccessControlList.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserGroups_AccessControlListt = new SyncTable("UserGroups_AccessControlList");
            tableUserGroups_AccessControlListt.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserGroups_AccessControlListt.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableUserCardState = new SyncTable("UserCardState");
            tableUserCardState.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableUserCardState.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableLearningSessions = new SyncTable("LearningSessions");
            tableLearningSessions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableLearningSessions.SyncDirection = SyncDirection.Bidirectional;

            SyncTable tableLearnLog = new SyncTable("LearnLog");
            tableLearnLog.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableLearnLog.SyncDirection = SyncDirection.UploadOnly;

            SyncTable tableDatabaseInformation = new SyncTable("DatabaseInformation");
            tableDatabaseInformation.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableDatabaseInformation.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableExtensions = new SyncTable("Extensions");
            tableExtensions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableExtensions.SyncDirection = SyncDirection.DownloadOnly;

            SyncTable tableExtensionActions = new SyncTable("ExtensionActions");
            tableExtensionActions.CreationOption = TableCreationOption.DropExistingOrCreateNewTable;
            tableExtensionActions.SyncDirection = SyncDirection.DownloadOnly;
            # endregion
            // Sync changes for both tables as one bunch, using SyncGroup object
            // This is important if the tables has PK-FK relationship, grouping will ensure that 
            // and FK change won't be applied before its PK is applied
            # region SyncGroup
            SyncGroup memorylifterGroup = new SyncGroup("AllChanges");
            tableLearningModules.SyncGroup = memorylifterGroup;
            tableCategories.SyncGroup = memorylifterGroup;
            tableChapters.SyncGroup = memorylifterGroup;
            tableCards.SyncGroup = memorylifterGroup;
            tableChapters_Cards.SyncGroup = memorylifterGroup;
            tableLearningModules_Cards.SyncGroup = memorylifterGroup;
            tableTextContent.SyncGroup = memorylifterGroup;
            tableMediaContent.SyncGroup = memorylifterGroup;
            tableMediaProperties.SyncGroup = memorylifterGroup;
            tableCards_MediaContent.SyncGroup = memorylifterGroup;
            tableTags.SyncGroup = memorylifterGroup;
            tableMediaContent_Tags.SyncGroup = memorylifterGroup;
            tableLearningModules_Tags.SyncGroup = memorylifterGroup;
            tableSettings.SyncGroup = memorylifterGroup;
            tableSelectedLearnChapters.SyncGroup = memorylifterGroup;
            tableCommentarySounds.SyncGroup = memorylifterGroup;
            tableQueryDirections.SyncGroup = memorylifterGroup;
            tableSynonymGradings.SyncGroup = memorylifterGroup;
            tableQueryTypes.SyncGroup = memorylifterGroup;
            tableMultipleChoiceOptions.SyncGroup = memorylifterGroup;
            tableTypeGradings.SyncGroup = memorylifterGroup;
            tableCardStyles.SyncGroup = memorylifterGroup;
            tableMediaContent_CardStyles.SyncGroup = memorylifterGroup;
            tableStyleSheets.SyncGroup = memorylifterGroup;
            tableSnoozeOptions.SyncGroup = memorylifterGroup;
            tableBoxes.SyncGroup = memorylifterGroup;
            tableUserProfilesLearningModulesSettings.SyncGroup = memorylifterGroup;
            tableUserProfiles.SyncGroup = memorylifterGroup;
            tableUserGroups.SyncGroup = memorylifterGroup;
            tableUserProfiles_UserGroups.SyncGroup = memorylifterGroup;
            tableTypeDefinitions.SyncGroup = memorylifterGroup;
            tablePermissions.SyncGroup = memorylifterGroup;
            tableObjectList.SyncGroup = memorylifterGroup;
            tableAccessControlList.SyncGroup = memorylifterGroup;
            tableUserProfiles_AccessControlList.SyncGroup = memorylifterGroup;
            tableUserGroups_AccessControlListt.SyncGroup = memorylifterGroup;
            tableUserCardState.SyncGroup = memorylifterGroup;
            tableLearningSessions.SyncGroup = memorylifterGroup;
            tableLearnLog.SyncGroup = memorylifterGroup;
            tableDatabaseInformation.SyncGroup = memorylifterGroup;
            tableExtensions.SyncGroup = memorylifterGroup;
            tableExtensionActions.SyncGroup = memorylifterGroup;
            # endregion
            # region Adding to SyncAgent
            syncAgent.Configuration.SyncTables.Add(tableLearningModules);
            syncAgent.Configuration.SyncTables.Add(tableCategories);
            syncAgent.Configuration.SyncTables.Add(tableChapters);
            syncAgent.Configuration.SyncTables.Add(tableCards);
            syncAgent.Configuration.SyncTables.Add(tableChapters_Cards);
            syncAgent.Configuration.SyncTables.Add(tableLearningModules_Cards);
            syncAgent.Configuration.SyncTables.Add(tableTextContent);
            syncAgent.Configuration.SyncTables.Add(tableMediaContent);
            syncAgent.Configuration.SyncTables.Add(tableMediaProperties);
            syncAgent.Configuration.SyncTables.Add(tableCards_MediaContent);
            syncAgent.Configuration.SyncTables.Add(tableTags);
            syncAgent.Configuration.SyncTables.Add(tableMediaContent_Tags);
            syncAgent.Configuration.SyncTables.Add(tableLearningModules_Tags);
            syncAgent.Configuration.SyncTables.Add(tableSettings);
            syncAgent.Configuration.SyncTables.Add(tableSelectedLearnChapters);
            syncAgent.Configuration.SyncTables.Add(tableCommentarySounds);
            syncAgent.Configuration.SyncTables.Add(tableQueryDirections);
            syncAgent.Configuration.SyncTables.Add(tableSynonymGradings);
            syncAgent.Configuration.SyncTables.Add(tableQueryTypes);
            syncAgent.Configuration.SyncTables.Add(tableMultipleChoiceOptions);
            syncAgent.Configuration.SyncTables.Add(tableTypeGradings);
            syncAgent.Configuration.SyncTables.Add(tableCardStyles);
            syncAgent.Configuration.SyncTables.Add(tableMediaContent_CardStyles);
            syncAgent.Configuration.SyncTables.Add(tableStyleSheets);
            syncAgent.Configuration.SyncTables.Add(tableSnoozeOptions);
            syncAgent.Configuration.SyncTables.Add(tableBoxes);
            syncAgent.Configuration.SyncTables.Add(tableUserProfilesLearningModulesSettings);
            syncAgent.Configuration.SyncTables.Add(tableUserProfiles);
            syncAgent.Configuration.SyncTables.Add(tableUserGroups);
            syncAgent.Configuration.SyncTables.Add(tableUserProfiles_UserGroups);
            syncAgent.Configuration.SyncTables.Add(tableTypeDefinitions);
            syncAgent.Configuration.SyncTables.Add(tablePermissions);
            syncAgent.Configuration.SyncTables.Add(tableObjectList);
            syncAgent.Configuration.SyncTables.Add(tableAccessControlList);
            syncAgent.Configuration.SyncTables.Add(tableUserProfiles_AccessControlList);
            syncAgent.Configuration.SyncTables.Add(tableUserGroups_AccessControlListt);
            syncAgent.Configuration.SyncTables.Add(tableUserCardState);
            syncAgent.Configuration.SyncTables.Add(tableLearningSessions);
            syncAgent.Configuration.SyncTables.Add(tableLearnLog);
            syncAgent.Configuration.SyncTables.Add(tableDatabaseInformation);
            syncAgent.Configuration.SyncTables.Add(tableExtensions);
            syncAgent.Configuration.SyncTables.Add(tableExtensionActions);
            # endregion
            # region SyncParameters
            syncAgent.Configuration.SyncParameters.Add("session_lm_id", learningModuleId);
            syncAgent.Configuration.SyncParameters.Add("session_user_id", userId);
            syncAgent.Configuration.SyncParameters.Add(paramIsNewDb, createNew);
            syncAgent.Configuration.SyncParameters.Add("id", -1);
            syncAgent.Configuration.SyncParameters.Add("user_id", -1);
            syncAgent.Configuration.SyncParameters.Add("lm_id", -1);
            syncAgent.Configuration.SyncParameters.Add("starttime", DateTime.MinValue);
            syncAgent.Configuration.SyncParameters.Add("endtime", DateTime.MinValue);
            syncAgent.Configuration.SyncParameters.Add("sum_right", -1);
            syncAgent.Configuration.SyncParameters.Add("sum_wrong", -1);
            syncAgent.Configuration.SyncParameters.Add("pool_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box1_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box2_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box3_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box4_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box5_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box6_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box7_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box8_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box9_content", -1);
            syncAgent.Configuration.SyncParameters.Add("box10_content", -1);
            syncAgent.Configuration.SyncParameters.Add("session_id", -1);
            syncAgent.Configuration.SyncParameters.Add("cards_id", -1);
            syncAgent.Configuration.SyncParameters.Add("old_box", -1);
            syncAgent.Configuration.SyncParameters.Add("new_box", -1);
            syncAgent.Configuration.SyncParameters.Add("timestamp", DateTime.MinValue);
            syncAgent.Configuration.SyncParameters.Add("duration", -1);
            syncAgent.Configuration.SyncParameters.Add("learn_mode", -1);
            syncAgent.Configuration.SyncParameters.Add("move_type", -1);
            syncAgent.Configuration.SyncParameters.Add("answer", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("direction", -1);
            syncAgent.Configuration.SyncParameters.Add("case_sensitive", false);
            syncAgent.Configuration.SyncParameters.Add("correct_on_the_fly", false);
            syncAgent.Configuration.SyncParameters.Add("percentage_known", -1);
            syncAgent.Configuration.SyncParameters.Add("percentage_required", -1);
            syncAgent.Configuration.SyncParameters.Add("box", -1);
            syncAgent.Configuration.SyncParameters.Add("active", false);
            syncAgent.Configuration.SyncParameters.Add("chapters_id", -1);
            syncAgent.Configuration.SyncParameters.Add("settings_id", -1);
            syncAgent.Configuration.SyncParameters.Add("autoplay_audio", false);
            syncAgent.Configuration.SyncParameters.Add("case_sensetive", false);
            syncAgent.Configuration.SyncParameters.Add("confirm_demote", false);
            syncAgent.Configuration.SyncParameters.Add("enable_commentary", false);
            syncAgent.Configuration.SyncParameters.Add("enable_timer", false);
            syncAgent.Configuration.SyncParameters.Add("synonym_gradings", -1);
            syncAgent.Configuration.SyncParameters.Add("type_gradings", -1);
            syncAgent.Configuration.SyncParameters.Add("multiple_choice_options", -1);
            syncAgent.Configuration.SyncParameters.Add("query_directions", -1);
            syncAgent.Configuration.SyncParameters.Add("query_types", -1);
            syncAgent.Configuration.SyncParameters.Add("random_pool", false);
            syncAgent.Configuration.SyncParameters.Add("self_assessment", false);
            syncAgent.Configuration.SyncParameters.Add("show_images", false);
            syncAgent.Configuration.SyncParameters.Add("stripchars", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("question_culture", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("answer_culture", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("question_caption", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("answer_caption", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("logo", -1);
            syncAgent.Configuration.SyncParameters.Add("question_stylesheet", -1);
            syncAgent.Configuration.SyncParameters.Add("answer_stylesheet", -1);
            syncAgent.Configuration.SyncParameters.Add("auto_boxsize", false);
            syncAgent.Configuration.SyncParameters.Add("pool_empty_message_shown", false);
            syncAgent.Configuration.SyncParameters.Add("show_statistics", false);
            syncAgent.Configuration.SyncParameters.Add("skip_correct_answers", false);
            syncAgent.Configuration.SyncParameters.Add("snooze_options", -1);
            syncAgent.Configuration.SyncParameters.Add("use_lm_stylesheets", false);
            syncAgent.Configuration.SyncParameters.Add("cardstyle", -1);
            syncAgent.Configuration.SyncParameters.Add("boxes", -1);
            syncAgent.Configuration.SyncParameters.Add("isCached", false);
            syncAgent.Configuration.SyncParameters.Add("question2answer", false);
            syncAgent.Configuration.SyncParameters.Add("answer2question", false);
            syncAgent.Configuration.SyncParameters.Add("mixed", false);
            syncAgent.Configuration.SyncParameters.Add("all_known", false);
            syncAgent.Configuration.SyncParameters.Add("half_known", false);
            syncAgent.Configuration.SyncParameters.Add("one_known", false);
            syncAgent.Configuration.SyncParameters.Add("first_known", false);
            syncAgent.Configuration.SyncParameters.Add("prompt", false);
            syncAgent.Configuration.SyncParameters.Add("image_recognition", false);
            syncAgent.Configuration.SyncParameters.Add("listening_comprehension", false);
            syncAgent.Configuration.SyncParameters.Add("multiple_choice", false);
            syncAgent.Configuration.SyncParameters.Add("sentence", false);
            syncAgent.Configuration.SyncParameters.Add("word", false);
            syncAgent.Configuration.SyncParameters.Add("allow_multiple_correct_answers", false);
            syncAgent.Configuration.SyncParameters.Add("allow_random_distractors", false);
            syncAgent.Configuration.SyncParameters.Add("max_correct_answers", -1);
            syncAgent.Configuration.SyncParameters.Add("number_of_choices", -1);
            syncAgent.Configuration.SyncParameters.Add("all_correct", false);
            syncAgent.Configuration.SyncParameters.Add("half_correct", false);
            syncAgent.Configuration.SyncParameters.Add("none_correct", false);
            syncAgent.Configuration.SyncParameters.Add("value", string.Empty);
            syncAgent.Configuration.SyncParameters.Add("cards_enabled", false);
            syncAgent.Configuration.SyncParameters.Add("rights_enabled", false);
            syncAgent.Configuration.SyncParameters.Add("time_enabled", false);
            syncAgent.Configuration.SyncParameters.Add("snooze_cards", -1);
            syncAgent.Configuration.SyncParameters.Add("snooze_high", -1);
            syncAgent.Configuration.SyncParameters.Add("snooze_low", -1);
            syncAgent.Configuration.SyncParameters.Add("snooze_mode", -1);
            syncAgent.Configuration.SyncParameters.Add("snooze_rights", -1);
            syncAgent.Configuration.SyncParameters.Add("snooze_time", -1);
            syncAgent.Configuration.SyncParameters.Add("box1_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box2_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box3_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box4_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box5_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box6_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box7_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box8_size", -1);
            syncAgent.Configuration.SyncParameters.Add("box9_size", -1);
            syncAgent.Configuration.SyncParameters.Add("highscore", -1);
            # endregion
            # endregion

            return syncAgent;
        }

        /// <summary>
        /// Determines whether [is new db] [the specified sync agent].
        /// </summary>
        /// <param name="syncAgent">The sync agent.</param>
        /// <returns>
        /// 	<c>true</c> if [is new db] [the specified sync agent]; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        public static bool IsNewDb(SyncAgent syncAgent)
        {
            if (syncAgent.Configuration.SyncParameters.Contains(paramIsNewDb))
                return (bool)syncAgent.Configuration.SyncParameters[paramIsNewDb].Value;
            else
                return false;
        }

        /// <summary>
        /// Creates the new db.
        /// </summary>
        /// <param name="connString">The conn string.</param>
        /// <remarks>Documented by Dev05, 2009-04-28</remarks>
        private static void CreateNewDb(string connString)
        {
            using (SqlCeEngine clientEngine = new SqlCeEngine(connString))
            {
                clientEngine.CreateDatabase();
            }
        }

        /// <summary>
        /// Applies the indices to database.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
        /// <param name="password">The password.</param>
        /// <remarks>Documented by Dev03, 2009-04-30</remarks>
        public static void ApplyIndicesToDatabase(string path, bool contentProtected, string password)
        {
            try
            {
                MSSQLCEConn.ApplyIndicesToDatabase(path, contentProtected, password);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to create indices for database! (" + ex.ToString() + ")", "SyncClient");
            }
        }

        /// <summary>
        /// Verifies the data base and repairs if necessary (to avoid IDENTITY problems after sync).
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contentProtected">if set to <c>true</c> [content protected].</param>
        /// <param name="password">The password.</param>
        /// <remarks>Documented by Dev05, 2009-04-28</remarks>
        public static void VerifyDataBase(string path, bool contentProtected, string password)
        {
            try
            {
                MSSQLCEConn.VerifyDataBase(path, contentProtected, password);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to verufy and repair the database! (" + ex.ToString() + ")", "SyncClient");
            }
        }
    }
}
