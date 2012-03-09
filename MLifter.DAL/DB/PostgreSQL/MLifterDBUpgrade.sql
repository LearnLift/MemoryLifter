CREATE OR REPLACE FUNCTION "UpdateDatabase"() RETURNS void AS
$BODY$
	DECLARE
		current_version VARCHAR(20);
		new_line TEXT;
	BEGIN	
		SELECT value INTO current_version FROM "DatabaseInformation" WHERE property='Version';
		
		new_line := E'\r\n';
		RAISE INFO 'Current version: % %', current_version, new_line;
	
		-- update 1.0.1 to 1.0.2
		IF current_version = '1.0.1' THEN
		
			RAISE INFO 'Updating V% to V1.0.2 %', current_version, new_line;
		
			-- drop exisitng tables
			DROP TABLE IF EXISTS "ExtensionActions";
			DROP TABLE IF EXISTS "Extensions";
		
			-- table Extensions
			CREATE TABLE "Extensions" (
				guid char(36) PRIMARY KEY NOT NULL,
				lm_id integer,
				CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
				name text NOT NULL,
				version varchar(10) NOT NULL,
				type varchar(100) NOT NULL,
				data oid NOT NULL,
				startfile text,
				update_originator_id integer default 0,
				update_timestamp timestamp,
				create_timestamp timestamp default now());
				
			DROP TABLE IF EXISTS "Extensions_tombstone";
			CREATE TABLE "Extensions_tombstone" (
				guid char(36) PRIMARY KEY NOT NULL,
				update_originator_id integer default 0,
				update_timestamp timestamp default now(),
				create_timestamp timestamp);
			    
			       
			CREATE OR REPLACE FUNCTION "Extensions_delete_trigger"() RETURNS trigger AS $$
			BEGIN
				INSERT INTO "Extensions_tombstone" (guid, create_timestamp, update_originator_id) 
					VALUES (OLD.guid, OLD.create_timestamp, 0);
				RETURN OLD;
			END;
			$$ LANGUAGE 'plpgsql';

			CREATE TRIGGER "Extensions_update_trigger" BEFORE UPDATE ON "Extensions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
			CREATE TRIGGER "Extensions_delete_trigger" BEFORE DELETE ON "Extensions" FOR EACH ROW EXECUTE PROCEDURE "Extensions_delete_trigger"();
				
			-- table ExtensionActions
			CREATE TABLE "ExtensionActions" (
				guid char(36) NOT NULL,
				action varchar(100) NOT NULL,
				PRIMARY KEY (guid, action),
				CONSTRAINT guid_fk FOREIGN KEY (guid) REFERENCES "Extensions"(guid) ON DELETE CASCADE,
				execution varchar(100) NOT NULL,
				update_originator_id integer default 0,
				update_timestamp timestamp,
				create_timestamp timestamp default now());
				
			DROP TABLE IF EXISTS "ExtensionActions_tombstone";
			CREATE TABLE "ExtensionActions_tombstone" (
				guid char(36) NOT NULL,
				action varchar(100) NOT NULL,
				PRIMARY KEY (guid, action),
				update_originator_id integer default 0,
				update_timestamp timestamp default now(),
				create_timestamp timestamp);
			    
			CREATE OR REPLACE FUNCTION "ExtensionActions_delete_trigger"() RETURNS trigger AS $$
			BEGIN
				INSERT INTO "ExtensionActions_tombstone" (guid, action, create_timestamp, update_originator_id) 
					VALUES (OLD.guid, OLD.action, OLD.create_timestamp, 0);
				RETURN OLD;
			END;
			$$ LANGUAGE 'plpgsql';

			CREATE TRIGGER "ExtensionActions_update_trigger" BEFORE UPDATE ON "ExtensionActions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
			CREATE TRIGGER "ExtensionActions_delete_trigger" BEFORE DELETE ON "ExtensionActions" FOR EACH ROW EXECUTE PROCEDURE "ExtensionActions_delete_trigger"();
			
			-- update batching method
			CREATE OR REPLACE FUNCTION "RenderBatchAnchors"(IN sync_min_received_anchor timestamp with time zone, IN sync_max_received_anchor timestamp with time zone, IN sync_batch_size BIGINT, IN sync_client_id_hash INTEGER, IN session_lm_id INTEGER, IN session_user_id INTEGER, IN isNewDb BOOLEAN, OUT sync_row_count INTEGER, OUT sync_batch_count INTEGER)
			  RETURNS SETOF record AS $$
			DECLARE
				tablename character varying;
				row_number integer;
			BEGIN
				CREATE TEMPORARY TABLE "RowTimestamps" (
					create_timestamp timestamp without time zone NOT NULL,
					update_timestamp timestamp without time zone,
					updated timestamp without time zone
					) ON COMMIT DROP;

				-- caching of complex query results
				CREATE TEMPORARY TABLE "SyncSettings" ON COMMIT DROP
					AS SELECT id, synonym_gradings, type_gradings, multiple_choice_options, query_directions, query_types, logo, question_stylesheet, answer_stylesheet, snooze_options, cardstyle, boxes 
						FROM "Settings" 
						WHERE id IN 
							(SELECT id FROM "Syncview_Settings" AS S WHERE S.session_lm_id=session_lm_id OR S.session_user_id=session_user_id);

				CREATE TEMPORARY TABLE "SyncMediaContent" ON COMMIT DROP
					AS SELECT id FROM "Syncview_MediaContent" AS S 
						WHERE S.session_lm_id=session_lm_id OR S.session_user_id=session_user_id;

				-- collecting row timestamps from every table
				INSERT INTO "RowTimestamps" 
					SELECT create_timestamp, update_timestamp FROM "DatabaseInformation" UNION
					SELECT create_timestamp, update_timestamp FROM "Categories" UNION
					SELECT create_timestamp, update_timestamp FROM "LearningModules";
					
				INSERT INTO "RowTimestamps"
					SELECT create_timestamp, update_timestamp FROM "Extensions"
						WHERE lm_id=session_lm_id UNION
					SELECT create_timestamp, update_timestamp FROM "ExtensionActions"
						WHERE guid IN (SELECT guid FROM "Extensions" WHERE lm_id=session_lm_id);
				
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps" 
						SELECT create_timestamp, update_timestamp FROM "DatabaseInformation_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Categories_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "LearningModules_tombstone";
				END IF;
				
				IF NOT isNewDb THEN
					 INSERT INTO "RowTimestamps" 
						SELECT create_timestamp, update_timestamp FROM "Extensions_tombstone"
							WHERE guid IN (SELECT guid FROM "Extensions" WHERE lm_id=session_lm_id) UNION
						SELECT create_timestamp, update_timestamp FROM "ExtensionActions_tombstone"
							WHERE guid IN (SELECT guid FROM "Extensions" WHERE lm_id=session_lm_id);
				END IF;

				INSERT INTO "RowTimestamps" -- user profile
					SELECT create_timestamp, update_timestamp FROM "UserProfiles" 
						WHERE id=session_user_id UNION
					SELECT create_timestamp, update_timestamp FROM "UserCardState"
						WHERE user_id=session_user_id AND cards_id IN 
							(SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "LearningSessions"
						WHERE user_id=session_user_id AND lm_id=session_lm_id UNION
					SELECT create_timestamp, update_timestamp FROM "LearnLog"
						WHERE session_id IN 
							(SELECT id FROM "LearningSessions" WHERE user_id=session_user_id AND lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "UserProfilesLearningModulesSettings"
						WHERE user_id=session_user_id AND lm_id=session_lm_id;
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps"
						SELECT create_timestamp, update_timestamp FROM "UserProfiles_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "UserCardState_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "LearningSessions_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "LearnLog_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "UserProfilesLearningModulesSettings_tombstone";
				END IF;

				INSERT INTO "RowTimestamps" -- user security stuff
					SELECT create_timestamp, update_timestamp FROM "Permissions" UNION
					SELECT create_timestamp, update_timestamp FROM "TypeDefinitions" UNION
					SELECT create_timestamp, update_timestamp FROM "UserProfiles_UserGroups" 
						WHERE users_id=session_user_id UNION
					SELECT create_timestamp, update_timestamp FROM "UserGroups" 
						WHERE id IN (SELECT groups_id FROM "UserProfiles_UserGroups" WHERE users_id=session_user_id) UNION
					SELECT create_timestamp, update_timestamp FROM "UserProfiles_AccessControlList" 
						WHERE users_id=session_user_id  UNION
					SELECT create_timestamp, update_timestamp FROM "UserGroups_AccessControlList" 
						WHERE groups_id IN (SELECT groups_id FROM "UserProfiles_UserGroups" WHERE users_id=session_user_id) UNION
					SELECT create_timestamp, update_timestamp FROM "AccessControlList" 
						WHERE id IN (SELECT id FROM "Syncview_AccessControlList" AS s 
							WHERE s.session_lm_id=session_lm_id OR s.session_user_id=session_user_id) UNION
					SELECT create_timestamp, update_timestamp FROM "ObjectList" 
						WHERE id IN (SELECT a.object_id FROM "AccessControlList" AS a JOIN "Syncview_AccessControlList" AS s ON a.id=s.id 
							WHERE s.session_lm_id=session_lm_id OR s.session_user_id=session_user_id);
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps"
						SELECT create_timestamp, update_timestamp FROM "Permissions_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "TypeDefinitions_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "UserGroups_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "UserProfiles_UserGroups_tombstone"UNION			
						SELECT create_timestamp, update_timestamp FROM "UserProfiles_AccessControlList_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "UserGroups_AccessControlList_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "AccessControlList_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "ObjectList_tombstone";
				END IF;

				INSERT INTO "RowTimestamps" -- learn content
					SELECT create_timestamp, update_timestamp FROM "LearningModules_Cards"
						WHERE lm_id=session_lm_id UNION
					SELECT create_timestamp, update_timestamp FROM "Chapters"
						WHERE lm_id=session_lm_id UNION
					SELECT create_timestamp, update_timestamp FROM "SelectedLearnChapters" 
						WHERE settings_id IN (SELECT id FROM "SyncSettings") 
						AND chapters_id IN (SELECT id FROM "Chapters" WHERE lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "Cards" 
						WHERE id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "TextContent"
						WHERE cards_id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "Chapters_Cards"
						WHERE chapters_id IN (SELECT id FROM "Chapters" WHERE lm_id=session_lm_id);	
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps"
						SELECT create_timestamp, update_timestamp FROM "LearningModules_Cards_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Chapters_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "SelectedLearnChapters_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Cards_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "TextContent_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Chapters_Cards_tombstone";
				END IF;	
				
				INSERT INTO "RowTimestamps" -- settings
					SELECT create_timestamp, update_timestamp FROM "Settings" 
						WHERE id IN (SELECT id FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "SynonymGradings"
						WHERE id IN (SELECT synonym_gradings FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "TypeGradings"
						WHERE id IN (SELECT type_gradings FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "MultipleChoiceOptions"
						WHERE id IN (SELECT multiple_choice_options FROm "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "QueryDirections"
						WHERE id IN (SELECT query_directions FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "QueryTypes"
						WHERE id IN (SELECT query_types FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "StyleSheets"
						WHERE id IN (SELECT question_stylesheet FROM "SyncSettings" UNION SELECT answer_stylesheet FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "SnoozeOptions"
						WHERE id IN (SELECT snooze_options FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "CardStyles"
						WHERE id IN (SELECT cardstyle FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "MediaContent_CardStyles"
						WHERE cardstyles_id IN (SELECT cardstyle FROM "SyncSettings") UNION
					SELECT create_timestamp, update_timestamp FROM "Boxes"
						WHERE id IN (SELECT boxes FROM "SyncSettings");
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps" 
						SELECT create_timestamp, update_timestamp FROM "Settings_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "SynonymGradings_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "TypeGradings_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "MultipleChoiceOptions_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "QueryDirections_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "QueryTypes_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "StyleSheets_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "SnoozeOptions_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "CardStyles_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "MediaContent_CardStyles_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Boxes_tombstone";
				END IF;	

				INSERT INTO "RowTimestamps" -- media stuff
					SELECT create_timestamp, update_timestamp FROM "MediaContent" 
						WHERE id IN (SELECT id FROM "SyncMediaContent") UNION
					SELECT create_timestamp, update_timestamp FROM "Cards_MediaContent"
						WHERE media_id IN (SELECT id FROM "SyncMediaContent")
						OR cards_id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=session_lm_id) UNION
					SELECT create_timestamp, update_timestamp FROM "CommentarySounds"
						WHERE settings_id IN (SELECT logo FROM "SyncSettings") 
						OR media_id IN (SELECT id FROM "SyncMediaContent") UNION
					SELECT create_timestamp, update_timestamp FROM "MediaProperties"
						WHERE media_id IN (SELECT id FROM "SyncMediaContent");
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps" 
						SELECT create_timestamp, update_timestamp FROM "MediaContent_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Cards_MediaContent_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "CommentarySounds_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "MediaProperties_tombstone";
				END IF;	

				INSERT INTO "RowTimestamps" -- tags
					SELECT create_timestamp, update_timestamp FROM "MediaContent_Tags"
						WHERE media_id IN (SELECT id FROM "SyncMediaContent") UNION
					SELECT create_timestamp, update_timestamp FROM "LearningModules_Tags"
						WHERE lm_id=session_lm_id UNION
					SELECT create_timestamp, update_timestamp FROM "Tags"
						WHERE id IN (SELECT tags_id FROM "MediaContent_Tags" AS M JOIN "SyncMediaContent" AS S ON M.media_id=S.id)
						OR id IN (SELECT tags_id FROM "LearningModules_Tags" WHERE lm_id=session_lm_id);
					
				IF NOT isNewDb THEN
					INSERT INTO "RowTimestamps" 
						SELECT create_timestamp, update_timestamp FROM "MediaContent_Tags_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "LearningModules_Tags_tombstone" UNION
						SELECT create_timestamp, update_timestamp FROM "Tags_tombstone";
				END IF;	

				-- calculate updated value
				UPDATE "RowTimestamps" SET updated=GREATEST(create_timestamp, update_timestamp);

				-- clean up timestamps
				DELETE FROM "RowTimestamps"
					WHERE updated<sync_min_received_anchor
					OR updated>sync_max_received_anchor;

				-- calculations
				sync_row_count := count(*) FROM "RowTimestamps";
				sync_batch_count := CEILING(sync_row_count::real/sync_batch_size);

				-- insert timestamps into session table
				DELETE FROM "BatchSessions" WHERE client_id=sync_client_id_hash;
				INSERT INTO "BatchSessions" (client_id, batch_size, batch_count)
					VALUES (sync_client_id_hash, sync_batch_size, sync_batch_count);

				INSERT INTO "BatchTimestamps" SELECT sync_client_id_hash, updated FROM "RowTimestamps" ORDER BY updated ASC;		
				
				RETURN NEXT;
			END;
			$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

			-- update current version
			UPDATE "DatabaseInformation" SET value='1.0.2' WHERE property='Version';
			SELECT '1.0.2' INTO current_version;
			
			RAISE INFO 'Current version: % %', current_version, new_line;
		END IF;
		IF current_version = '1.0.2' THEN
			RAISE INFO 'Updating V% to V1.0.3 %', current_version, new_line;
			
			DROP TABLE IF EXISTS "ClientAccessList" CASCADE;

			-- update current version
			UPDATE "DatabaseInformation" SET value='1.0.3' WHERE property='Version';
			UPDATE "DatabaseInformation" SET value='2.4.0,>2.4.0' WHERE property='SupportedDataLayerVersions';
			SELECT '1.0.3' INTO current_version;
			
			RAISE INFO 'Current version: % %', current_version, new_line;
		END IF;
	END;
$BODY$
 LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "ApplyHotfixes"() RETURNS void AS
$BODY$
	DECLARE
		current_version VARCHAR(20);
		new_line TEXT;
	BEGIN	
		SELECT value INTO current_version FROM "DatabaseInformation" WHERE property='Version';
		
		new_line := E'\r\n';
		RAISE INFO 'Apply general hotfixes for version % %', current_version, new_line;
	
		-- function deletes a learning module
		CREATE OR REPLACE FUNCTION "DeleteLearningModule"(integer) RETURNS void AS $$
			DECLARE
				p_lmid ALIAS FOR $1;
				mytables RECORD;
				foundTrigger BOOLEAN;
				foundTriggerVariant_1 BOOLEAN;
				foundTriggerVariant_2 BOOLEAN;
				triggerCmd TEXT;
			BEGIN
				-- [ML-2511] Postgre migration from version 8.3 to 8.4 cause LM delete to fail 
				-- check if the attribute 'reltriggers' or 'relhastriggers' exists (changed from ver 8.3 to 8.4)
				SELECT count(*) > 0 INTO foundTriggerVariant_1 FROM pg_attribute WHERE attrelid = (SELECT oid FROM pg_class WHERE relname = 'pg_class') AND attname = 'reltriggers';
				SELECT count(*) > 0 INTO foundTriggerVariant_2 FROM pg_attribute WHERE attrelid = (SELECT oid FROM pg_class WHERE relname = 'pg_class') AND attname = 'relhastriggers'; 
				SELECT foundTriggerVariant_1 OR foundTriggerVariant_2 INTO foundTrigger;
				IF foundTriggerVariant_1 THEN
					SELECT 'SELECT relname FROM pg_class WHERE reltriggers > 0 AND NOT relname LIKE ''pg_%''' INTO triggerCmd;
				ELSIF foundTriggerVariant_2 THEN
					SELECT 'SELECT relname FROM pg_class WHERE relhastriggers = true AND NOT relname LIKE ''pg_%''' INTO triggerCmd;
				END IF;
				IF foundTrigger THEN
					-- disable all user triggers
					FOR mytables IN EXECUTE triggerCmd
					LOOP
						EXECUTE 'LOCK TABLE "' || mytables.relname || '" IN ROW EXCLUSIVE MODE';
						EXECUTE 'ALTER TABLE "' || mytables.relname || '" DISABLE TRIGGER USER';
					END LOOP;
				END IF;
				-- doing the delete now (delete and release "LearningSessions" and "UserCardState" first)
				DELETE FROM "LearningSessions" WHERE lm_id = p_lmid;
				ALTER TABLE "LearningSessions" ENABLE TRIGGER USER;
				DELETE FROM "UserCardState" WHERE "UserCardState".cards_id IN (SELECT "LearningModules_Cards".cards_id FROM "LearningModules_Cards" WHERE lm_id = p_lmid);
				ALTER TABLE "UserCardState" ENABLE TRIGGER USER;
				DELETE FROM "Cards" WHERE "Cards".id IN (SELECT "LearningModules_Cards".cards_id FROM "LearningModules_Cards" WHERE lm_id = p_lmid);
				DELETE FROM "LearningModules" WHERE id = p_lmid;
				IF foundTrigger THEN
					-- re-enable all user triggers
					FOR mytables IN EXECUTE triggerCmd
					LOOP
						EXECUTE 'ALTER TABLE "' || mytables.relname || '" ENABLE TRIGGER USER';
					END LOOP;
				END IF;
			END;
		$$ LANGUAGE 'plpgsql';
			
		RAISE INFO 'Applied general hotfixes for version % %', current_version, new_line;
	END;
$BODY$
 LANGUAGE 'plpgsql' VOLATILE SECURITY DEFINER;
 
 SELECT * FROM "ApplyHotfixes"();
 SELECT * FROM "UpdateDatabase"();
 
 DROP FUNCTION "ApplyHotfixes"();
 DROP FUNCTION "UpdateDatabase"();