-- to ensure to get a clean database to first manualy delete and recreate the "memorylifter" database:
-- DROP DATABASE IF EXISTS memorylifter;
-- CREATE DATABASE memorylifter;
 
-- should clean the whole database
DROP SCHEMA IF EXISTS public CASCADE;
CREATE SCHEMA public;

-- update trigger function
CREATE OR REPLACE FUNCTION update_trigger() RETURNS trigger AS $$
BEGIN
	NEW.update_timestamp := now();
	If NEW.update_originator_id = OLD.update_originator_id THEN
		NEW.update_originator_id := 0;
	END IF;
	RETURN NEW;
END;
$$ LANGUAGE 'plpgsql';

-- function to create a new setting
CREATE OR REPLACE FUNCTION "CreateNewSetting"()
  RETURNS integer AS $$
  
	DECLARE
		result INTEGER;
	
	BEGIN
		
		INSERT INTO "SnoozeOptions"
			(cards_enabled,rights_enabled,time_enabled)
			VALUES
			(null,null,null);
			
		INSERT INTO "MultipleChoiceOptions"
			(allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices)
			VALUES
			(null, null, null, null);
			
		INSERT INTO "QueryTypes"
			(image_recognition, listening_comprehension, multiple_choice, sentence, word)
			VALUES
			(null,null,null,null,null);
			
		INSERT INTO "TypeGradings"
			(all_correct, half_correct, none_correct, prompt)
			VALUES
			(null,null,null,null);

		INSERT INTO "SynonymGradings"
			(all_known, half_known, one_known, first_known, prompt)
			VALUES
			(null,null,null,null,null);
			
		INSERT INTO "QueryDirections"
			(question2answer, answer2question, mixed)
			VALUES
			(null,null,null);
			
		INSERT INTO "CardStyles"
			(id)
			VALUES
			(DEFAULT);
			
		INSERT INTO "Boxes"
			(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size)
			VALUES
			(null, null, null, null, null, null, null, null, null);

		INSERT INTO "Settings"
			(snooze_options, query_types, query_directions, multiple_choice_options, synonym_gradings, type_gradings, cardstyle, boxes, autoplay_audio, case_sensitive, confirm_demote, 
			enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, show_images, stripchars, auto_boxsize, pool_empty_message_shown, 
			show_statistics, skip_correct_answers, use_lm_stylesheets, question_culture, answer_culture)
			VALUES
			(
				currval('"SnoozeOptions_id_seq"'), 
				currval('"QueryTypes_id_seq"'), 
				currval('"QueryDirections_id_seq"'), 
				currval('"MultipleChoiceOptions_id_seq"'),
				currval('"SynonymGradings_id_seq"'),
				currval('"TypeGradings_id_seq"'),
				currval('"CardStyles_id_seq"'),
				currval('"Boxes_id_seq"'),
				null, null , null,
				null, null, null, null, null, null, null, null, null,
				null, null, null, 'en', 'en'
			);
			
		SELECT INTO result CAST(currval('"Settings_id_seq"') AS integer);

		RETURN result;
	END;
$$ LANGUAGE 'plpgsql';

-- table DatabaseInformation: create a table for storing general information about the database like the version
DROP TABLE IF EXISTS "DatabaseInformation" CASCADE;	-- CASCADE --> delete related tables too
CREATE TABLE "DatabaseInformation" (
	property varchar(100) PRIMARY KEY NOT NULL,
	value varchar(100),
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "DatabaseInformation_tombstone";
CREATE TABLE "DatabaseInformation_tombstone" (
	property varchar(100) PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "DatabaseInformation_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "DatabaseInformation_tombstone" (property, create_timestamp, update_originator_id) 
		VALUES (OLD.property, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "DatabaseInformation_update_trigger" BEFORE UPDATE ON "DatabaseInformation" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "DatabaseInformation_delete_trigger" BEFORE DELETE ON "DatabaseInformation" FOR EACH ROW EXECUTE PROCEDURE "DatabaseInformation_delete_trigger"();

-- insert the current version into the table
INSERT INTO "DatabaseInformation" VALUES ('Version','1.0.3');
INSERT INTO "DatabaseInformation" VALUES ('SupportedDataLayerVersions','2.4.0,>2.4.0');

-- table SynonymGradings
DROP TABLE IF EXISTS "SynonymGradings" CASCADE;
CREATE TABLE "SynonymGradings" (
	id serial PRIMARY KEY NOT NULL,
	all_known boolean,
	half_known boolean,
	one_known boolean,
	first_known boolean,
	prompt boolean,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
    
DROP TABLE IF EXISTS "SynonymGradings_tombstone";
CREATE TABLE "SynonymGradings_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "SynonymGradings_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "SynonymGradings_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "SynonymGradings_update_trigger" BEFORE UPDATE ON "SynonymGradings" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "SynonymGradings_delete_trigger" BEFORE DELETE ON "SynonymGradings" FOR EACH ROW EXECUTE PROCEDURE "SynonymGradings_delete_trigger"();

-- table TypeGradings
DROP TABLE IF EXISTS "TypeGradings" CASCADE;
CREATE TABLE "TypeGradings" (
	id serial PRIMARY KEY NOT NULL,
	all_correct boolean,
	half_correct boolean,
	none_correct boolean,
	prompt boolean,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "TypeGradings_tombstone";
CREATE TABLE "TypeGradings_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "TypeGradings_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "TypeGradings_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "TypeGradings_update_trigger" BEFORE UPDATE ON "TypeGradings" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "TypeGradings_delete_trigger" BEFORE DELETE ON "TypeGradings" FOR EACH ROW EXECUTE PROCEDURE "TypeGradings_delete_trigger"();

--table MultipleChoiceOptions
DROP TABLE IF EXISTS "MultipleChoiceOptions" CASCADE;
CREATE TABLE "MultipleChoiceOptions" (
	id serial PRIMARY KEY NOT NULL,
	allow_multiple_correct_answers boolean,
	allow_random_distractors boolean,
	max_correct_answers integer,
	number_of_choices integer,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "MultipleChoiceOptions_tombstone";
CREATE TABLE "MultipleChoiceOptions_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "MultipleChoiceOptions_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "MultipleChoiceOptions_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "MultipleChoiceOptions_update_trigger" BEFORE UPDATE ON "MultipleChoiceOptions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "MultipleChoiceOptions_delete_trigger" BEFORE DELETE ON "MultipleChoiceOptions" FOR EACH ROW EXECUTE PROCEDURE "MultipleChoiceOptions_delete_trigger"();

-- table QueryDirections
DROP TABLE IF EXISTS "QueryDirections" CASCADE;
CREATE TABLE "QueryDirections" (
	id serial PRIMARY KEY NOT NULL,
	question2answer boolean,
	answer2question boolean,
	mixed boolean,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "QueryDirections_tombstone";
CREATE TABLE "QueryDirections_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "QueryDirections_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "QueryDirections_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "QueryDirections_update_trigger" BEFORE UPDATE ON "QueryDirections" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "QueryDirections_delete_trigger" BEFORE DELETE ON "QueryDirections" FOR EACH ROW EXECUTE PROCEDURE "QueryDirections_delete_trigger"();

--table QueryTypes
DROP TABLE IF EXISTS "QueryTypes" CASCADE;
CREATE TABLE "QueryTypes" (
	id serial PRIMARY KEY NOT NULL,
	image_recognition boolean,
	listening_comprehension boolean,
	multiple_choice boolean,
	sentence boolean,
	word boolean,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "QueryTypes_tombstone";
CREATE TABLE "QueryTypes_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "QueryTypes_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "QueryTypes_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "QueryTypes_update_trigger" BEFORE UPDATE ON "QueryTypes" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "QueryTypes_delete_trigger" BEFORE DELETE ON "QueryTypes" FOR EACH ROW EXECUTE PROCEDURE "QueryTypes_delete_trigger"();

--table StyleSheets
DROP TABLE IF EXISTS "StyleSheets" CASCADE;
CREATE TABLE "StyleSheets" (
	id serial PRIMARY KEY NOT NULL,
	value text NOT NULL,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "StyleSheets_tombstone";
CREATE TABLE "StyleSheets_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "StyleSheets_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "StyleSheets_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "StyleSheets_update_trigger" BEFORE UPDATE ON "StyleSheets" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "StyleSheets_delete_trigger" BEFORE DELETE ON "StyleSheets" FOR EACH ROW EXECUTE PROCEDURE "StyleSheets_delete_trigger"();

DROP TYPE IF EXISTS "SnoozeMode" CASCADE;
CREATE TYPE "SnoozeMode" AS ENUM ('SendToTray', 'QuitProgram');

-- table SnoozeOptions
DROP TABLE IF EXISTS "SnoozeOptions" CASCADE;
CREATE TABLE "SnoozeOptions" (
	id serial PRIMARY KEY NOT NULL,
	cards_enabled boolean,
	rights_enabled boolean,
	time_enabled boolean,
	snooze_cards integer,
	snooze_high integer,
	snooze_low integer,
	snooze_mode "SnoozeMode",
	snooze_rights integer,
	snooze_time integer,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "SnoozeOptions_tombstone";
CREATE TABLE "SnoozeOptions_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "SnoozeOptions_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "SnoozeOptions_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "SnoozeOptions_update_trigger" BEFORE UPDATE ON "SnoozeOptions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "SnoozeOptions_delete_trigger" BEFORE DELETE ON "SnoozeOptions" FOR EACH ROW EXECUTE PROCEDURE "SnoozeOptions_delete_trigger"();
	
-- table CardStyles
DROP TABLE IF EXISTS "CardStyles" CASCADE;
CREATE TABLE "CardStyles" (
	id serial PRIMARY KEY NOT NULL,
	value text,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "CardStyles_tombstone";
CREATE TABLE "CardStyles_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "CardStyles_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "CardStyles_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "CardStyles_update_trigger" BEFORE UPDATE ON "CardStyles" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "CardStyles_delete_trigger" BEFORE DELETE ON "CardStyles" FOR EACH ROW EXECUTE PROCEDURE "CardStyles_delete_trigger"();

-- define custom types to store the text and media content
DROP TYPE IF EXISTS "side" CASCADE;
DROP TYPE IF EXISTS "type" CASCADE;
DROP TYPE IF EXISTS "mediatype" CASCADE;
DROP TYPE IF EXISTS "commentarytype" CASCADE;
CREATE TYPE "side" AS ENUM ('Answer', 'Question');
CREATE TYPE "type" AS ENUM ('Word', 'Sentence', 'Distractor');
CREATE TYPE "mediatype" AS ENUM ('Audio', 'Video', 'Image', 'Unknown');
CREATE TYPE "commentarytype" AS ENUM ('RightStandAlone', 'WrongStandAlone', 'AlmostStandAlone', 'Right', 'Wrong', 'Almost');


-- table MediaContent: stores the media objects	
DROP TABLE IF EXISTS "MediaContent" CASCADE;
CREATE TABLE "MediaContent" (
	id serial PRIMARY KEY NOT NULL,
	data oid NOT NULL,
	media_type mediatype NOT NULL,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "MediaContent_tombstone";
CREATE TABLE "MediaContent_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	data oid NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "MediaContent_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "MediaContent_tombstone" (id, data, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.data, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "MediaContent_update_trigger" BEFORE UPDATE ON "MediaContent" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "MediaContent_delete_trigger" BEFORE DELETE ON "MediaContent" FOR EACH ROW EXECUTE PROCEDURE "MediaContent_delete_trigger"();

-- table "MediaContent_CardStyles"
DROP TABLE IF EXISTS "MediaContent_CardStyles" CASCADE;
CREATE TABLE "MediaContent_CardStyles"
(
  media_id integer NOT NULL,
  cardstyles_id integer NOT NULL,
  CONSTRAINT "MediaContent_CardStyles_pkey" PRIMARY KEY (media_id, cardstyles_id),
  CONSTRAINT cardstyles_id_pk FOREIGN KEY (cardstyles_id)
      REFERENCES "CardStyles" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT media_id_fk FOREIGN KEY (media_id)
      REFERENCES "MediaContent" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());

DROP TABLE IF EXISTS "MediaContent_CardStyles_tombstone";
CREATE TABLE "MediaContent_CardStyles_tombstone" (
	media_id integer NOT NULL,
	cardstyles_id integer NOT NULL,
	PRIMARY KEY (media_id, cardstyles_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "MediaContent_CardStyles_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "MediaContent_CardStyles_tombstone" WHERE media_id = OLD.media_id AND cardstyles_id = OLD.cardstyles_id;
	IF c > 0 THEN
		UPDATE "MediaContent_CardStyles_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE media_id = OLD.media_id AND cardstyles_id = OLD.cardstyles_id;
	ELSE
		INSERT INTO "MediaContent_CardStyles_tombstone" (media_id, cardstyles_id, create_timestamp, update_originator_id) 
			VALUES (OLD.media_id, OLD.cardstyles_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "MediaContent_CardStyles_update_trigger" BEFORE UPDATE ON "MediaContent_CardStyles" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "MediaContent_CardStyles_delete_trigger" BEFORE DELETE ON "MediaContent_CardStyles" FOR EACH ROW EXECUTE PROCEDURE "MediaContent_CardStyles_delete_trigger"();



-- table Boxes
DROP TABLE IF EXISTS "Boxes" CASCADE;
CREATE TABLE "Boxes" (
	id serial PRIMARY KEY NOT NULL,
	box1_size integer,
	box2_size integer,
	box3_size integer,
	box4_size integer,
	box5_size integer,
	box6_size integer,
	box7_size integer,
	box8_size integer,
	box9_size integer,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Boxes_tombstone";
CREATE TABLE "Boxes_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Boxes_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Boxes_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Boxes_update_trigger" BEFORE UPDATE ON "Boxes" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Boxes_delete_trigger" BEFORE DELETE ON "Boxes" FOR EACH ROW EXECUTE PROCEDURE "Boxes_delete_trigger"();	

-- table Settings
DROP TABLE IF EXISTS "Settings" CASCADE;
CREATE TABLE "Settings" (
	id serial PRIMARY KEY NOT NULL,
	autoplay_audio boolean,
	case_sensitive boolean,
	confirm_demote boolean,
	enable_commentary boolean,
	correct_on_the_fly boolean,
	enable_timer boolean,
	synonym_gradings integer,
	type_gradings integer,
	multiple_choice_options integer,
	query_directions integer,
	query_types integer,
	random_pool boolean,
	self_assessment boolean,
	show_images boolean,
	stripchars varchar(100),
	question_culture varchar(100),
	answer_culture varchar(100),
	question_caption varchar(100),
	answer_caption varchar(100),
	logo integer,
	question_stylesheet integer,
	answer_stylesheet integer,
	auto_boxsize boolean,
	pool_empty_message_shown boolean,
	show_statistics boolean,
	skip_correct_answers boolean,
	snooze_options integer,
	use_lm_stylesheets boolean,
	cardstyle integer,
	boxes integer,
	isCached boolean NOT NULL DEFAULT TRUE,
	CONSTRAINT synonym_gradings_fk FOREIGN KEY (synonym_gradings) REFERENCES "SynonymGradings"(id) ON DELETE CASCADE,
	CONSTRAINT type_gradings_fk FOREIGN KEY (type_gradings) REFERENCES "TypeGradings"(id) ON DELETE CASCADE,
	CONSTRAINT mc_options_fk FOREIGN KEY (multiple_choice_options) REFERENCES "MultipleChoiceOptions"(id) ON DELETE CASCADE,
	CONSTRAINT query_directions_fk FOREIGN KEY (query_directions) REFERENCES "QueryDirections"(id) ON DELETE CASCADE,
	CONSTRAINT query_types_fk FOREIGN KEY (query_types) REFERENCES "QueryTypes"(id) ON DELETE CASCADE,
	CONSTRAINT logo_fk FOREIGN KEY (logo) REFERENCES "MediaContent"(id) ON DELETE CASCADE,
	CONSTRAINT question_stylesheet_fk FOREIGN KEY (question_stylesheet) REFERENCES "StyleSheets"(id),
	CONSTRAINT answer_stylesheet_fk FOREIGN KEY (answer_stylesheet) REFERENCES "StyleSheets"(id),
	CONSTRAINT snooze_options_fk FOREIGN KEY (snooze_options) REFERENCES "SnoozeOptions"(id) ON DELETE CASCADE,
	CONSTRAINT cardstyle_fk FOREIGN KEY (cardstyle) REFERENCES "CardStyles"(id),
	CONSTRAINT boxes_fk FOREIGN KEY (boxes) REFERENCES "Boxes"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Settings_tombstone";
CREATE TABLE "Settings_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Settings_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Settings_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Settings_update_trigger" BEFORE UPDATE ON "Settings" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Settings_delete_trigger" BEFORE DELETE ON "Settings" FOR EACH ROW EXECUTE PROCEDURE "Settings_delete_trigger"();

--table Categories
DROP TABLE IF EXISTS "Categories" CASCADE;
CREATE TABLE "Categories" (
	id serial PRIMARY KEY NOT NULL,
	global_id integer,
	name varchar(100),
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Categories_tombstone";
CREATE TABLE "Categories_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Categories_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Categories_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Categories_update_trigger" BEFORE UPDATE ON "Categories" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Categories_delete_trigger" BEFORE DELETE ON "Categories" FOR EACH ROW EXECUTE PROCEDURE "Categories_delete_trigger"();

-- table LearningModules
DROP TABLE IF EXISTS "LearningModules" CASCADE;
CREATE TABLE "LearningModules" (
	id serial PRIMARY KEY NOT NULL,
	guid char(36) NOT NULL,
	categories_id integer NOT NULL,
	default_settings_id integer NOT NULL,
	allowed_settings_id integer NOT NULL,
	creator_id int,
	author text,
	title text NOT NULL,
	description text,
	licence_key varchar(100),
	content_protected boolean default false,
	cal_count integer default 1,
	CONSTRAINT categories_id_fk FOREIGN KEY (categories_id) REFERENCES "Categories"(id) ON DELETE CASCADE,
	CONSTRAINT default_settings_id_fk FOREIGN KEY (default_settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	CONSTRAINT allowed_settings_id_fk FOREIGN KEY (allowed_settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "LearningModules_tombstone";
CREATE TABLE "LearningModules_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "LearningModules_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "LearningModules_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "LearningModules_update_trigger" BEFORE UPDATE ON "LearningModules" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "LearningModules_delete_trigger" BEFORE DELETE ON "LearningModules" FOR EACH ROW EXECUTE PROCEDURE "LearningModules_delete_trigger"();

-- table Extensions
DROP TABLE IF EXISTS "Extensions";
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
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "Extensions_tombstone" WHERE guid = OLD.guid;
	IF c > 0 THEN
		UPDATE "Extensions_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE guid = OLD.guid;
	ELSE
		INSERT INTO "Extensions_tombstone" (guid, create_timestamp, update_originator_id) 
			VALUES (OLD.guid, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Extensions_update_trigger" BEFORE UPDATE ON "Extensions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Extensions_delete_trigger" BEFORE DELETE ON "Extensions" FOR EACH ROW EXECUTE PROCEDURE "Extensions_delete_trigger"();
	
-- table ExtensionActions
DROP TABLE IF EXISTS "ExtensionActions";
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
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "ExtensionActions_tombstone" WHERE guid = OLD.guid AND action = OLD.action;
	IF c > 0 THEN
		UPDATE "ExtensionActions_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE guid = OLD.guid AND action = OLD.action;
	ELSE
		INSERT INTO "ExtensionActions_tombstone" (guid, action, create_timestamp, update_originator_id) 
			VALUES (OLD.guid, OLD.action, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "ExtensionActions_update_trigger" BEFORE UPDATE ON "ExtensionActions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "ExtensionActions_delete_trigger" BEFORE DELETE ON "ExtensionActions" FOR EACH ROW EXECUTE PROCEDURE "ExtensionActions_delete_trigger"();

-- table Chapters
DROP TABLE IF EXISTS "Chapters" CASCADE;
CREATE TABLE "Chapters" (
	id serial PRIMARY KEY NOT NULL,
	lm_id integer NOT NULL,
	title text,
	description text,
	position integer NOT NULL,
	settings_id integer NOT NULL,
	CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
	CONSTRAINT settings_id_fk FOREIGN KEY (settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
    
CREATE OR REPLACE FUNCTION "Chapters_insert_trigger"() RETURNS trigger AS $$
BEGIN
	NEW.settings_id := "CreateNewSetting"();
	RETURN NEW;
END;
$$ LANGUAGE 'plpgsql';
	
DROP TABLE IF EXISTS "Chapters_tombstone";
CREATE TABLE "Chapters_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Chapters_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Chapters_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Chapters_insert_trigger" BEFORE INSERT ON "Chapters" FOR EACH ROW EXECUTE PROCEDURE "Chapters_insert_trigger"();
CREATE TRIGGER "Chapters_update_trigger" BEFORE UPDATE ON "Chapters" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Chapters_delete_trigger" BEFORE DELETE ON "Chapters" FOR EACH ROW EXECUTE PROCEDURE "Chapters_delete_trigger"();

-- table Cards
DROP TABLE IF EXISTS "Cards" CASCADE;
CREATE TABLE "Cards" (
	id serial PRIMARY KEY NOT NULL,
	chapters_id integer NOT NULL default 0,
	lm_id integer NOT NULL default 0,
	settings_id integer NOT NULL,
	CONSTRAINT settings_id_fk FOREIGN KEY (settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
    
CREATE OR REPLACE FUNCTION "Cards_insert_trigger"() RETURNS trigger AS $$
BEGIN
	NEW.settings_id := "CreateNewSetting"();
	RETURN NEW;
END;
$$ LANGUAGE 'plpgsql';
	
DROP TABLE IF EXISTS "Cards_tombstone";
CREATE TABLE "Cards_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Cards_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Cards_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Cards_insert_trigger" BEFORE INSERT ON "Cards" FOR EACH ROW EXECUTE PROCEDURE "Cards_insert_trigger"();
CREATE TRIGGER "Cards_update_trigger" BEFORE UPDATE ON "Cards" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Cards_delete_trigger" BEFORE DELETE ON "Cards" FOR EACH ROW EXECUTE PROCEDURE "Cards_delete_trigger"();

-- table TextContent: stores the text content of the cards (words, sentences, distractors)	
DROP TABLE IF EXISTS "TextContent" CASCADE;
CREATE TABLE "TextContent" (
	id serial PRIMARY KEY NOT NULL,
	cards_id integer NOT NULL,
	text text NOT NULL,
	side side NOT NULL,
	type type NOT NULL,
	position integer NOT NULL,
	is_default boolean NOT NULL DEFAULT FALSE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "TextContent_tombstone";
CREATE TABLE "TextContent_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "TextContent_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "TextContent_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "TextContent_update_trigger" BEFORE UPDATE ON "TextContent" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "TextContent_delete_trigger" BEFORE DELETE ON "TextContent" FOR EACH ROW EXECUTE PROCEDURE "TextContent_delete_trigger"();
	
-- table MediaProperties: stores the text properties for media objects (e.g. mime-type, width, height, size, lenght, etc.)
DROP TABLE IF EXISTS "MediaProperties" CASCADE;
CREATE TABLE "MediaProperties" (
	media_id integer NOT NULL,
	property varchar(100) NOT NULL,
	value varchar(100),
	PRIMARY KEY (media_id, property),
	CONSTRAINT media_id_fk FOREIGN KEY (media_id) REFERENCES "MediaContent"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "MediaProperties_tombstone";
CREATE TABLE "MediaProperties_tombstone" (
	media_id integer NOT NULL,
	property varchar(100) NOT NULL,
	PRIMARY KEY (media_id, property),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "MediaProperties_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "MediaProperties_tombstone" WHERE media_id = OLD.media_id AND property = OLD.property;
	IF c > 0 THEN
		UPDATE "MediaProperties_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE media_id = OLD.media_id AND property = OLD.property;
	ELSE
		INSERT INTO "MediaProperties_tombstone" (media_id, property, create_timestamp, update_originator_id) 
			VALUES (OLD.media_id, OLD.property, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "MediaProperties_update_trigger" BEFORE UPDATE ON "MediaProperties" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "MediaProperties_delete_trigger" BEFORE DELETE ON "MediaProperties" FOR EACH ROW EXECUTE PROCEDURE "MediaProperties_delete_trigger"();
	
-- table Cards_MediaContent: junction table to connect media with cards
DROP TABLE IF EXISTS "Cards_MediaContent" CASCADE;
CREATE TABLE "Cards_MediaContent" (
	media_id integer NOT NULL,
	cards_id integer NOT NULL,
	side side NOT NULL,
	type type NOT NULL,
	is_default boolean NOT NULL DEFAULT FALSE,
	PRIMARY KEY (media_id, cards_id, side, type),
	CONSTRAINT media_id_fk FOREIGN KEY (media_id) REFERENCES "MediaContent"(id) ON DELETE CASCADE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Cards_MediaContent_tombstone";
CREATE TABLE "Cards_MediaContent_tombstone" (
	media_id integer NOT NULL,
	cards_id integer NOT NULL,
	side side NOT NULL,
	type type NOT NULL,
	PRIMARY KEY (media_id, cards_id, side, type),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Cards_MediaContent_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "Cards_MediaContent_tombstone"
		WHERE media_id = OLD.media_id AND cards_id = OLD.cards_id AND side = OLD.side AND type = OLD.type;
	IF c > 0 THEN
		UPDATE "Cards_MediaContent_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE media_id = OLD.media_id AND cards_id = OLD.cards_id AND side = OLD.side AND type = OLD.type;
	ELSE
		INSERT INTO "Cards_MediaContent_tombstone" (media_id, cards_id, side, type, create_timestamp, update_originator_id) 
			VALUES (OLD.media_id, OLD.cards_id, OLD.side, OLD.type, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Cards_MediaContent_update_trigger" BEFORE UPDATE ON "Cards_MediaContent" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Cards_MediaContent_delete_trigger" BEFORE DELETE ON "Cards_MediaContent" FOR EACH ROW EXECUTE PROCEDURE "Cards_MediaContent_delete_trigger"();

-- table CommentarySounds: junction table to connect media (=commentary sounds) with settings
DROP TABLE IF EXISTS "CommentarySounds" CASCADE;
CREATE TABLE "CommentarySounds" (
	media_id integer NOT NULL,
	settings_id integer NOT NULL,
	side side NOT NULL,
	type commentarytype NOT NULL,
	PRIMARY KEY (media_id, settings_id, side, type),
	CONSTRAINT media_id_fk FOREIGN KEY (media_id) REFERENCES "MediaContent"(id) ON DELETE CASCADE,
	CONSTRAINT settings_id_fk FOREIGN KEY (settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "CommentarySounds_tombstone";
CREATE TABLE "CommentarySounds_tombstone" (
	media_id integer NOT NULL,
	settings_id integer NOT NULL,
	side side NOT NULL,
	type type NOT NULL,
	PRIMARY KEY (media_id, settings_id, side, type),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "CommentarySounds_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "CommentarySounds_tombstone" WHERE media_id = OLD.media_id AND settings_id = OLD.settings_id;
	IF c > 0 THEN
		UPDATE "CommentarySounds_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE media_id = OLD.media_id AND settings_id = OLD.settings_id;
	ELSE
		INSERT INTO "CommentarySounds_tombstone" (media_id, settings_id, side, type, create_timestamp, update_originator_id) 
			VALUES (OLD.media_id, OLD.settings_id, OLD.side, OLD.type, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "CommentarySounds_update_trigger" BEFORE UPDATE ON "CommentarySounds" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "CommentarySounds_delete_trigger" BEFORE DELETE ON "CommentarySounds" FOR EACH ROW EXECUTE PROCEDURE "CommentarySounds_delete_trigger"();
	
-- table Chapters_Cards: junction table to connect chapters with cards
DROP TABLE IF EXISTS "Chapters_Cards" CASCADE;
CREATE TABLE "Chapters_Cards" (
	chapters_id integer NOT NULL,
	cards_id integer NOT NULL,
	PRIMARY KEY (chapters_id, cards_id),
	CONSTRAINT chapters_id_fk FOREIGN KEY (chapters_id) REFERENCES "Chapters"(id) ON DELETE CASCADE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Chapters_Cards_tombstone";
CREATE TABLE "Chapters_Cards_tombstone" (
	chapters_id integer NOT NULL,
	cards_id integer NOT NULL,
	PRIMARY KEY (chapters_id, cards_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Chapters_Cards_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "Chapters_Cards_tombstone" WHERE chapters_id = OLD.chapters_id AND cards_id = OLD.cards_id;
	IF c > 0 THEN
		UPDATE "Chapters_Cards_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE chapters_id = OLD.chapters_id AND cards_id = OLD.cards_id;
	ELSE
		INSERT INTO "Chapters_Cards_tombstone" (chapters_id, cards_id, create_timestamp, update_originator_id) 
			VALUES (OLD.chapters_id, OLD.cards_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Chapters_Cards_update_trigger" BEFORE UPDATE ON "Chapters_Cards" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Chapters_Cards_delete_trigger" BEFORE DELETE ON "Chapters_Cards" FOR EACH ROW EXECUTE PROCEDURE "Chapters_Cards_delete_trigger"();
	
-- table SelectedLearnChapters: junction table to connect chapters with settings (=learning chapters/querychapters)
DROP TABLE IF EXISTS "SelectedLearnChapters" CASCADE;
CREATE TABLE "SelectedLearnChapters" (
	chapters_id integer NOT NULL,
	settings_id integer NOT NULL,
	PRIMARY KEY (chapters_id, settings_id),
	CONSTRAINT chapters_id_fk FOREIGN KEY (chapters_id) REFERENCES "Chapters"(id) ON DELETE CASCADE,
	CONSTRAINT settings_id_fk FOREIGN KEY (settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "SelectedLearnChapters_tombstone";
CREATE TABLE "SelectedLearnChapters_tombstone" (
	chapters_id integer NOT NULL,
	settings_id integer NOT NULL,
	PRIMARY KEY (chapters_id, settings_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "SelectedLearnChapters_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "SelectedLearnChapters_tombstone" WHERE chapters_id = OLD.chapters_id AND settings_id = OLD.settings_id;
	IF c > 0 THEN
		UPDATE "SelectedLearnChapters_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE chapters_id = OLD.chapters_id AND settings_id = OLD.settings_id;
	ELSE
		INSERT INTO "SelectedLearnChapters_tombstone" (chapters_id, settings_id, create_timestamp, update_originator_id) 
			VALUES (OLD.chapters_id, OLD.settings_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "SelectedLearnChapters_update_trigger" BEFORE UPDATE ON "SelectedLearnChapters" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "SelectedLearnChapters_delete_trigger" BEFORE DELETE ON "SelectedLearnChapters" FOR EACH ROW EXECUTE PROCEDURE "SelectedLearnChapters_delete_trigger"();

-- table LearningModules_Cards: junction tables to connect learning modules with cards
DROP TABLE IF EXISTS "LearningModules_Cards" CASCADE;
CREATE TABLE "LearningModules_Cards" (
	lm_id integer NOT NULL,
	cards_id integer NOT NULL,
	PRIMARY KEY (lm_id, cards_id),
	CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "LearningModules_Cards_tombstone";
CREATE TABLE "LearningModules_Cards_tombstone" (
	lm_id integer NOT NULL,
	cards_id integer NOT NULL,
	PRIMARY KEY (lm_id, cards_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "LearningModules_Cards_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "LearningModules_Cards_tombstone" WHERE lm_id = OLD.lm_id AND cards_id = OLD.cards_id;
	IF c > 0 THEN
		UPDATE "LearningModules_Cards_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE lm_id = OLD.lm_id AND cards_id = OLD.cards_id;
	ELSE
		INSERT INTO "LearningModules_Cards_tombstone" (lm_id, cards_id, create_timestamp, update_originator_id) 
			VALUES (OLD.lm_id, OLD.cards_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "LearningModules_Cards_update_trigger" BEFORE UPDATE ON "LearningModules_Cards" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "LearningModules_Cards_delete_trigger" BEFORE DELETE ON "LearningModules_Cards" FOR EACH ROW EXECUTE PROCEDURE "LearningModules_Cards_delete_trigger"();

-- table Tags
DROP TABLE IF EXISTS "Tags" CASCADE;
CREATE TABLE "Tags" (
	id serial PRIMARY KEY NOT NULL,
	text text,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "Tags_tombstone";
CREATE TABLE "Tags_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Tags_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Tags_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Tags_update_trigger" BEFORE UPDATE ON "Tags" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Tags_delete_trigger" BEFORE DELETE ON "Tags" FOR EACH ROW EXECUTE PROCEDURE "Tags_delete_trigger"();

-- table LearningModules_Tags: junction table to connect tags with learning modules
DROP TABLE IF EXISTS "LearningModules_Tags" CASCADE;
CREATE TABLE "LearningModules_Tags" (
	lm_id integer NOT NULL,
	tags_id integer NOT NULL,
	PRIMARY KEY (lm_id, tags_id),
	CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
	CONSTRAINT tags_id_fk FOREIGN KEY (tags_id) REFERENCES "Tags"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "LearningModules_Tags_tombstone";
CREATE TABLE "LearningModules_Tags_tombstone" (
	lm_id integer NOT NULL,
	tags_id integer NOT NULL,
	PRIMARY KEY (lm_id, tags_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "LearningModules_Tags_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "LearningModules_Tags_tombstone" WHERE lm_id = OLD.lm_id AND tags_id = OLD.tags_id;
	IF c > 0 THEN
		UPDATE "LearningModules_Tags_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE lm_id = OLD.lm_id AND tags_id = OLD.tags_id;
	ELSE
		INSERT INTO "LearningModules_Tags_tombstone" (lm_id, tags_id, create_timestamp, update_originator_id) 
			VALUES (OLD.lm_id, OLD.tags_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "LearningModules_Tags_update_trigger" BEFORE UPDATE ON "LearningModules_Tags" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "LearningModules_Tags_delete_trigger" BEFORE DELETE ON "LearningModules_Tags" FOR EACH ROW EXECUTE PROCEDURE "LearningModules_Tags_delete_trigger"();

-- table MediaContent_Tags: junction table to connect tags with media content
DROP TABLE IF EXISTS "MediaContent_Tags" CASCADE;
CREATE TABLE "MediaContent_Tags" (
	media_id integer NOT NULL,
	tags_id integer NOT NULL,
	PRIMARY KEY (media_id, tags_id),
	CONSTRAINT media_id_fk FOREIGN KEY (media_id) REFERENCES "MediaContent"(id) ON DELETE CASCADE,
	CONSTRAINT tags_id_fk FOREIGN KEY (tags_id) REFERENCES "Tags"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "MediaContent_Tags_tombstone";
CREATE TABLE "MediaContent_Tags_tombstone" (
	media_id integer NOT NULL,
	tags_id integer NOT NULL,
	PRIMARY KEY (media_id, tags_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "MediaContent_Tags_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "MediaContent_Tags_tombstone" WHERE media_id = OLD.media_id AND tags_id = OLD.tags_id;
	IF c > 0 THEN
		UPDATE "MediaContent_Tags_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE media_id = OLD.media_id AND tags_id = OLD.tags_id;
	ELSE
		INSERT INTO "MediaContent_Tags_tombstone" (media_id, tags_id, create_timestamp, update_originator_id) 
			VALUES (OLD.media_id, OLD.tags_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "MediaContent_Tags_update_trigger" BEFORE UPDATE ON "MediaContent_Tags" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "MediaContent_Tags_delete_trigger" BEFORE DELETE ON "MediaContent_Tags" FOR EACH ROW EXECUTE PROCEDURE "MediaContent_Tags_delete_trigger"();
	
DROP TYPE IF EXISTS "usertype" CASCADE;
CREATE TYPE "usertype" AS ENUM ('ListAuthentication', 'FormsAuthentication', 'LocalDirectoryAuthentication');

-- table UserProfiles
DROP TABLE IF EXISTS "UserProfiles" CASCADE;
CREATE TABLE "UserProfiles" (
	id serial PRIMARY KEY NOT NULL,
	username varchar(100) NOT NULL,
	password varchar(100),
	local_directory_id varchar(100),
	user_type usertype NOT NULL,
	enabled boolean NOT NULL default false,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "UserProfiles_tombstone";
CREATE TABLE "UserProfiles_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserProfiles_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "UserProfiles_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserProfiles_update_trigger" BEFORE UPDATE ON "UserProfiles" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserProfiles_delete_trigger" BEFORE DELETE ON "UserProfiles" FOR EACH ROW EXECUTE PROCEDURE "UserProfiles_delete_trigger"();
COMMENT ON TABLE "UserProfiles" IS 'This table stores the basic user profiles.';

DROP TABLE IF EXISTS "UserGroups" CASCADE;
-- table UserGroups
CREATE TABLE "UserGroups"
(
	id serial PRIMARY KEY NOT NULL,
	"name" character varying(100),
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());

DROP TABLE IF EXISTS "UserGroups_tombstone";
CREATE TABLE "UserGroups_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserGroups_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "UserGroups_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserGroups_update_trigger" BEFORE UPDATE ON "UserGroups" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserGroups_delete_trigger" BEFORE DELETE ON "UserGroups" FOR EACH ROW EXECUTE PROCEDURE "UserGroups_delete_trigger"();

DROP TABLE IF EXISTS "UserProfiles_UserGroups" CASCADE;
-- table UserProfiles_UserGroups
CREATE TABLE "UserProfiles_UserGroups"
(
  users_id integer NOT NULL,
  groups_id integer NOT NULL,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now(),
  CONSTRAINT "UserProfiles_UserGroups_PK" PRIMARY KEY (users_id, groups_id),
  CONSTRAINT "groups_id_FK" FOREIGN KEY (groups_id)
      REFERENCES "UserGroups" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "users_id_FK" FOREIGN KEY (users_id)
      REFERENCES "UserProfiles" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE
);

DROP TABLE IF EXISTS "UserProfiles_UserGroups_tombstone" CASCADE;
CREATE TABLE "UserProfiles_UserGroups_tombstone" (
	users_id integer NOT NULL,
	groups_id integer NOT NULL,
	PRIMARY KEY (users_id, groups_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserProfiles_UserGroups_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserProfiles_UserGroups_tombstone" WHERE users_id = OLD.users_id AND groups_id = OLD.groups_id;
	IF c > 0 THEN
		UPDATE "UserProfiles_UserGroups_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE users_id = OLD.users_id AND groups_id = OLD.groups_id;
	ELSE
		INSERT INTO "UserProfiles_UserGroups_tombstone" (users_id, groups_id, create_timestamp, update_originator_id) 
			VALUES (OLD.users_id, OLD.groups_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserProfiles_UserGroups_update_trigger" BEFORE UPDATE ON "UserProfiles_UserGroups" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserProfiles_UserGroups_delete_trigger" BEFORE DELETE ON "UserProfiles_UserGroups" FOR EACH ROW EXECUTE PROCEDURE "UserProfiles_UserGroups_delete_trigger"();

DROP TABLE IF EXISTS "TypeDefinitions" CASCADE;
-- table TypeDefinitions
CREATE TABLE "TypeDefinitions"
(
	id serial PRIMARY KEY NOT NULL,
  clr_name character varying(1000) NOT NULL,
  parent_id integer,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());

DROP TABLE IF EXISTS "TypeDefinitions_tombstone";
CREATE TABLE "TypeDefinitions_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "TypeDefinitions_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "TypeDefinitions_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "TypeDefinitions_update_trigger" BEFORE UPDATE ON "TypeDefinitions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "TypeDefinitions_delete_trigger" BEFORE DELETE ON "TypeDefinitions" FOR EACH ROW EXECUTE PROCEDURE "TypeDefinitions_delete_trigger"();

DROP TABLE IF EXISTS "Permissions" CASCADE;
-- table Permissions
CREATE TABLE "Permissions"
(
  id serial PRIMARY KEY NOT NULL,
  types_id integer NOT NULL,
  "name" character varying(100),
  "default" boolean NOT NULL,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now(),
  CONSTRAINT "typedef_id_FK" FOREIGN KEY (types_id)
      REFERENCES "TypeDefinitions" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE);

DROP TABLE IF EXISTS "Permissions_tombstone";
CREATE TABLE "Permissions_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "Permissions_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "Permissions_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "Permissions_update_trigger" BEFORE UPDATE ON "Permissions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "Permissions_delete_trigger" BEFORE DELETE ON "Permissions" FOR EACH ROW EXECUTE PROCEDURE "Permissions_delete_trigger"();

DROP TABLE IF EXISTS "ObjectList" CASCADE;
-- table ObjectList
CREATE TABLE "ObjectList"
(
	id serial PRIMARY KEY NOT NULL,
  locator character varying(100) NOT NULL,
  parent_id integer,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());

DROP TABLE IF EXISTS "ObjectList_tombstone";
CREATE TABLE "ObjectList_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "ObjectList_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "ObjectList_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "ObjectList_update_trigger" BEFORE UPDATE ON "ObjectList" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "ObjectList_delete_trigger" BEFORE DELETE ON "ObjectList" FOR EACH ROW EXECUTE PROCEDURE "ObjectList_delete_trigger"();

DROP TABLE IF EXISTS "AccessControlList" CASCADE;
-- table AccessControlList
CREATE TABLE "AccessControlList"
(
  id serial PRIMARY KEY NOT NULL,
  object_id integer NOT NULL,
  permissions_id integer NOT NULL,
  "access" boolean NOT NULL,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now(),
  CONSTRAINT "object_id_FK" FOREIGN KEY (object_id)
      REFERENCES "ObjectList" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "permission_id_FK" FOREIGN KEY (permissions_id)
      REFERENCES "Permissions" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE);

DROP TABLE IF EXISTS "AccessControlList_tombstone";
CREATE TABLE "AccessControlList_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "AccessControlList_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "AccessControlList_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "AccessControlList_update_trigger" BEFORE UPDATE ON "AccessControlList" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "AccessControlList_delete_trigger" BEFORE DELETE ON "AccessControlList" FOR EACH ROW EXECUTE PROCEDURE "AccessControlList_delete_trigger"();

DROP TABLE IF EXISTS "UserProfiles_AccessControlList" CASCADE;
-- table UserProfiles_AccessControlList
CREATE TABLE "UserProfiles_AccessControlList"
(
  users_id integer NOT NULL,
  acl_id integer NOT NULL,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now(),
  CONSTRAINT "UserProfiles_AccessControlList_PK" PRIMARY KEY (users_id, acl_id),
  CONSTRAINT "acl_id_FK" FOREIGN KEY (acl_id)
      REFERENCES "AccessControlList" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "users_id_FK" FOREIGN KEY (users_id)
      REFERENCES "UserProfiles" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE);

DROP TABLE IF EXISTS "UserProfiles_AccessControlList_tombstone";
CREATE TABLE "UserProfiles_AccessControlList_tombstone" (
	users_id integer NOT NULL,
	acl_id integer NOT NULL,
	PRIMARY KEY (users_id, acl_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserProfiles_AccessControlList_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserProfiles_AccessControlList_tombstone" WHERE users_id = OLD.users_id AND acl_id = OLD.acl_id;
	IF c > 0 THEN
		UPDATE "UserProfiles_AccessControlList_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE user_id = OLD.users_id AND acl_id = OLD.acl_id;
	ELSE
		INSERT INTO "UserProfiles_AccessControlList_tombstone" (users_id, acl_id, create_timestamp, update_originator_id) 
			VALUES (OLD.users_id, OLD.acl_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserProfiles_AccessControlList_update_trigger" BEFORE UPDATE ON "UserProfiles_AccessControlList" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserProfiles_AccessControlList_delete_trigger" BEFORE DELETE ON "UserProfiles_AccessControlList" FOR EACH ROW EXECUTE PROCEDURE "UserProfiles_AccessControlList_delete_trigger"();

DROP TABLE IF EXISTS "UserGroups_AccessControlList" CASCADE;
-- table UserGroups_AccessControlList
CREATE TABLE "UserGroups_AccessControlList"
(
  groups_id integer NOT NULL,
  acl_id integer NOT NULL,
  update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now(),
  CONSTRAINT "UserGroups_AccessControlList_PK" PRIMARY KEY (groups_id, acl_id),
  CONSTRAINT "acl_id_FK" FOREIGN KEY (acl_id)
      REFERENCES "AccessControlList" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE,
  CONSTRAINT "groups_id_FK" FOREIGN KEY (groups_id)
      REFERENCES "UserGroups" (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE CASCADE);

DROP TABLE IF EXISTS "UserGroups_AccessControlList_tombstone";
CREATE TABLE "UserGroups_AccessControlList_tombstone" (
	groups_id integer NOT NULL,
	acl_id integer NOT NULL,
	PRIMARY KEY (groups_id, acl_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserGroups_AccessControlList_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserGroups_AccessControlList_tombstone" WHERE groups_id = OLD.groups_id AND acl_id = OLD.acl_id;
	IF c > 0 THEN
		UPDATE "UserGroups_AccessControlList_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE groups_id = OLD.groups_id AND acl_id = OLD.acl_id;
	ELSE
		INSERT INTO "UserGroups_AccessControlList_tombstone" (groups_id, acl_id, create_timestamp, update_originator_id) 
			VALUES (OLD.groups_id, OLD.acl_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserGroups_AccessControlList_update_trigger" BEFORE UPDATE ON "UserGroups_AccessControlList" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserGroups_AccessControlList_delete_trigger" BEFORE DELETE ON "UserGroups_AccessControlList" FOR EACH ROW EXECUTE PROCEDURE "UserGroups_AccessControlList_delete_trigger"();

-- table UserSessions
DROP TABLE IF EXISTS "UserSessions" CASCADE;
CREATE TABLE "UserSessions" (
	id serial NOT NULL,
	sid char(36) NOT NULL,
	user_id integer NOT NULL,
	login_time timestamp NOT NULL,
	refresh_time timestamp,
	logout_time timestamp NOT NULL,
	PRIMARY KEY (id, sid),
	CONSTRAINT user_id_fk FOREIGN KEY (user_id) REFERENCES "UserProfiles"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "UserSessions_tombstone";
CREATE TABLE "UserSessions_tombstone" (
	id serial NOT NULL,
	sid char(36) NOT NULL,
	PRIMARY KEY (id, sid),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserSessions_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserSessions_tombstone" WHERE id = OLD.id AND sid = OLD.sid;
	IF c > 0 THEN
		UPDATE "UserSessions_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE id = OLD.id AND sid = OLD.sid;
	ELSE
		INSERT INTO "UserSessions_tombstone" (id, sid, create_timestamp, update_originator_id) 
			VALUES (OLD.id, OLD.sid, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserSessions_update_trigger" BEFORE UPDATE ON "UserSessions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserSessions_delete_trigger" BEFORE DELETE ON "UserSessions" FOR EACH ROW EXECUTE PROCEDURE "UserSessions_delete_trigger"();

-- table UserProfilesLearningModulesSettings
DROP TABLE IF EXISTS "UserProfilesLearningModulesSettings" CASCADE;
CREATE TABLE "UserProfilesLearningModulesSettings" (
	user_id	integer NOT NULL,
	lm_id integer NOT NULL,
	settings_id integer NOT NULL,
	highscore numeric(18,2) DEFAULT 0,
	PRIMARY KEY (user_id, lm_id),
	CONSTRAINT user_id_fk FOREIGN KEY (user_id) REFERENCES "UserProfiles"(id) ON DELETE CASCADE,	
	CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
	CONSTRAINT settings_id_fk FOREIGN KEY (settings_id) REFERENCES "Settings"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "UserProfilesLearningModulesSettings_tombstone";
CREATE TABLE "UserProfilesLearningModulesSettings_tombstone" (
	user_id	integer NOT NULL,
	lm_id integer NOT NULL,
	PRIMARY KEY (user_id, lm_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserProfilesLearningModulesSettings_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserProfilesLearningModulesSettings_tombstone" WHERE lm_id = OLD.lm_id AND user_id = OLD.user_id;
	IF c > 0 THEN
		UPDATE "UserProfilesLearningModulesSettings_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE lm_id = OLD.lm_id AND user_id = OLD.user_id;
	ELSE
		INSERT INTO "UserProfilesLearningModulesSettings_tombstone" (user_id, lm_id, create_timestamp, update_originator_id) 
			VALUES (OLD.user_id, OLD.lm_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserProfilesLearningModulesSettings_update_trigger" BEFORE UPDATE ON "UserProfilesLearningModulesSettings" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserProfilesLearningModulesSettings_delete_trigger" BEFORE DELETE ON "UserProfilesLearningModulesSettings" FOR EACH ROW EXECUTE PROCEDURE "UserProfilesLearningModulesSettings_delete_trigger"();

-- table UserCardState
DROP TABLE IF EXISTS "UserCardState" CASCADE;
CREATE TABLE "UserCardState" (
	user_id integer NOT NULL,
	cards_id integer NOT NULL,
	box integer,
	active boolean,
	timestamp timestamp,
	PRIMARY KEY (user_id, cards_id),
	CONSTRAINT user_id_fk FOREIGN KEY (user_id) REFERENCES "UserProfiles"(id) ON DELETE CASCADE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "UserCardState_tombstone";
CREATE TABLE "UserCardState_tombstone" (
	user_id integer NOT NULL,
	cards_id integer NOT NULL,
	PRIMARY KEY (user_id, cards_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "UserCardState_delete_trigger"() RETURNS trigger AS $$
	DECLARE
		c integer;
BEGIN
	SELECT INTO c count(*) FROM "UserCardState_tombstone" WHERE user_id = OLD.user_id AND cards_id = OLD.cards_id;
	IF c > 0 THEN
		UPDATE "UserCardState_tombstone"
			SET update_timestamp = now(), create_timestamp = OLD.create_timestamp, update_originator_id = 0
			WHERE user_id = OLD.user_id AND cards_id = OLD.cards_id;
	ELSE
		INSERT INTO "UserCardState_tombstone" (user_id, cards_id, create_timestamp, update_originator_id) 
			VALUES (OLD.user_id, OLD.cards_id, OLD.create_timestamp, 0);
	END IF;
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "UserCardState_update_trigger" BEFORE UPDATE ON "UserCardState" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "UserCardState_delete_trigger" BEFORE DELETE ON "UserCardState" FOR EACH ROW EXECUTE PROCEDURE "UserCardState_delete_trigger"();

-- table LearningSessions
DROP TABLE IF EXISTS "LearningSessions" CASCADE;
CREATE TABLE "LearningSessions" (
	id serial NOT NULL,
	user_id	integer NOT NULL,
	lm_id integer NOT NULL,
	starttime timestamp NOT NULL,
	endtime timestamp,
	sum_right integer default 0,
	sum_wrong integer default 0,
	pool_content integer,
	box1_content integer,
	box2_content integer,
	box3_content integer,
	box4_content integer,
	box5_content integer,
	box6_content integer,
	box7_content integer,
	box8_content integer,
	box9_content integer,
	box10_content integer,
	PRIMARY KEY (id, user_id, lm_id),
	CONSTRAINT user_id_fk FOREIGN KEY (user_id) REFERENCES "UserProfiles"(id) ON DELETE CASCADE,
	CONSTRAINT lm_id_fk FOREIGN KEY (lm_id) REFERENCES "LearningModules"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "LearningSessions_tombstone";
CREATE TABLE "LearningSessions_tombstone" (
	id integer NOT NULL,
	user_id	integer NOT NULL,
	lm_id integer NOT NULL,
	PRIMARY KEY (id, user_id, lm_id),
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "LearningSessions_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "LearningSessions_tombstone" (id, user_id, lm_id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.user_id, OLD.lm_id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "LearningSessions_update_trigger" BEFORE UPDATE ON "LearningSessions" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "LearningSessions_delete_trigger" BEFORE DELETE ON "LearningSessions" FOR EACH ROW EXECUTE PROCEDURE "LearningSessions_delete_trigger"();
	
DROP TYPE IF EXISTS "learnmode" CASCADE;
DROP TYPE IF EXISTS "movetype" CASCADE;
DROP TYPE IF EXISTS "direction" CASCADE;
CREATE TYPE "learnmode" AS ENUM ('ImageRecognition', 'ListeningComprehension', 'MultipleChoice', 'Sentences', 'Word');
CREATE TYPE "movetype" AS ENUM ('AutoPromote', 'AutoDemote', 'ManualPromote', 'ManualDemote', 'Manual', 'CanceledDemote');
CREATE TYPE "direction" AS ENUM ('Question2Answer', 'Answer2Question', 'Mixed');

-- table LearnLog
DROP TABLE IF EXISTS "LearnLog" CASCADE;
CREATE TABLE "LearnLog" (
	id serial PRIMARY KEY NOT NULL,
	session_id integer NOT NULL,
	user_id	integer NOT NULL,
	lm_id integer NOT NULL,
	cards_id integer NOT NULL,
	old_box integer,
	new_box integer,
	timestamp timestamp,
	duration bigint,
	learn_mode learnmode,
	move_type movetype,
	answer text,
	direction direction,
	case_sensitive boolean,
	correct_on_the_fly boolean,
	percentage_known integer,
	percentage_required integer,
	CONSTRAINT session_id_fk FOREIGN KEY (session_id, user_id, lm_id) REFERENCES "LearningSessions"(id, user_id, lm_id) ON DELETE CASCADE,
	CONSTRAINT cards_id_fk FOREIGN KEY (cards_id) REFERENCES "Cards"(id) ON DELETE CASCADE,
	update_originator_id integer default 0,
	update_timestamp timestamp,
	create_timestamp timestamp default now());
	
DROP TABLE IF EXISTS "LearnLog_tombstone";
CREATE TABLE "LearnLog_tombstone" (
	id integer PRIMARY KEY NOT NULL,
	update_originator_id integer default 0,
    update_timestamp timestamp default now(),
    create_timestamp timestamp);
    
CREATE OR REPLACE FUNCTION "LearnLog_delete_trigger"() RETURNS trigger AS $$
BEGIN
	INSERT INTO "LearnLog_tombstone" (id, create_timestamp, update_originator_id) 
		VALUES (OLD.id, OLD.create_timestamp, 0);
	RETURN OLD;
END;
$$ LANGUAGE 'plpgsql';

CREATE TRIGGER "LearnLog_update_trigger" BEFORE UPDATE ON "LearnLog" FOR EACH ROW EXECUTE PROCEDURE update_trigger();
CREATE TRIGGER "LearnLog_delete_trigger" BEFORE DELETE ON "LearnLog" FOR EACH ROW EXECUTE PROCEDURE "LearnLog_delete_trigger"();
	
-- views
-- vwGetCardsIndex - combines a cards text contents (question and answer) with chapter and box
CREATE OR REPLACE VIEW "vwGetCardsIndex" AS 
 SELECT "Cards".id, "Chapters_Cards".chapters_id, "Chapters".title, "UserCardState".box, "UserCardState".active, "UserCardState"."timestamp", "TextContent".text, "TextContent".side, "LearningModules_Cards".lm_id, "UserCardState".user_id
   FROM "Cards"
   JOIN "Chapters_Cards" ON "Chapters_Cards".cards_id = "Cards".id
   JOIN "Chapters" ON "Chapters_Cards".chapters_id = "Chapters".id
   LEFT JOIN "UserCardState" ON "Cards".id = "UserCardState".cards_id
   JOIN "TextContent" ON "Cards".id = "TextContent".cards_id AND "TextContent".type = 'Word'::type
   JOIN "LearningModules_Cards" ON "Cards".id = "LearningModules_Cards".cards_id
  ORDER BY "Cards".id, "TextContent".side, "TextContent"."position";
  
CREATE OR REPLACE VIEW "vwGetCardsForCache" AS
 SELECT "Cards".id AS cards_id, "Chapters_Cards".chapters_id, "Chapters".title AS chapters_title, "UserCardState".box, "UserCardState".active, "UserCardState"."timestamp",
	"TextContent".text, "TextContent".side, "LearningModules_Cards".lm_id, "UserCardState".user_id
   FROM "Cards"
   JOIN "Chapters_Cards" ON "Chapters_Cards".cards_id = "Cards".id
   JOIN "Chapters" ON "Chapters_Cards".chapters_id = "Chapters".id
   LEFT JOIN "UserCardState" ON "Cards".id = "UserCardState".cards_id
   JOIN "TextContent" ON "Cards".id = "TextContent".cards_id AND "TextContent".type = 'Word'::type
   JOIN "LearningModules_Cards" ON "Cards".id = "LearningModules_Cards".cards_id
  ORDER BY "Cards".id, "TextContent".side, "TextContent"."position";

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

 -- function to only insert words when they aren't already in the database
CREATE OR REPLACE FUNCTION "InsertWordIfNotExists"(integer,integer,boolean,text,side,type) RETURNS boolean AS $$
	DECLARE
		p_wordid ALIAS FOR $1;
		p_cardid ALIAS FOR $2;
		p_isdefault ALIAS FOR $3;
		p_word ALIAS FOR $4;
		p_side ALIAS FOR $5;
		p_typ ALIAS FOR $6;

		c INTEGER;

	BEGIN
		SELECT INTO c count(*) FROM "TextContent" WHERE id = p_wordid AND text = p_word AND type = p_typ AND is_default = p_isdefault;

		IF c < 0 THEN
			INSERT INTO "TextContent" (cards_id, text, side, type, position) VALUES (p_cardid, p_word, p_side, p_typ, (COALESCE((SELECT position FROM "TextContent" WHERE cards_id=p_cardid AND side=p_side AND type=p_typ ORDER BY position DESC LIMIT 1), 0) + 10));
			RETURN TRUE;
		ELSE
			RETURN FALSE;
		END IF;
	END;
$$ LANGUAGE 'plpgsql';

-- function to redistribute positions of the chapters
CREATE OR REPLACE FUNCTION "RedistributeChapterPositions"(integer) RETURNS void AS $$
	DECLARE
		lmid ALIAS FOR $1;
		rec RECORD;
	BEGIN
		DROP SEQUENCE IF EXISTS serialchapters; 
		CREATE TEMPORARY SEQUENCE serialchapters INCREMENT BY 10 START 10; 
		FOR rec IN  (SELECT * FROM "Chapters" WHERE lm_id=lmid ORDER BY position ASC)  LOOP
		UPDATE "Chapters" SET position = nextval('serialchapters') WHERE "Chapters".id=rec.id;
		END LOOP;
		DROP SEQUENCE serialchapters; 
	END;
$$ language 'plpgsql';

-- function to create a new learning module
CREATE OR REPLACE FUNCTION "CreateNewLearningModule"(character, integer, text)
  RETURNS integer AS $$
  
	DECLARE
		allowed_id INTEGER;
		default_id INTEGER;
		result INTEGER;
	
	BEGIN
		INSERT INTO "SnoozeOptions"
			(cards_enabled,rights_enabled,time_enabled)
			VALUES
			(TRUE,TRUE,TRUE);
			
		INSERT INTO "MultipleChoiceOptions"
			(allow_multiple_correct_answers, allow_random_distractors)
			VALUES
			(TRUE, TRUE);
			
		INSERT INTO "QueryTypes"
			(image_recognition, listening_comprehension, multiple_choice, sentence, word)
			VALUES
			(TRUE,TRUE,TRUE,TRUE,TRUE);
			
		INSERT INTO "TypeGradings"
			(all_correct, half_correct, none_correct, prompt)
			VALUES
			(TRUE,TRUE,TRUE,TRUE);

		INSERT INTO "SynonymGradings"
			(all_known, half_known, one_known, first_known, prompt)
			VALUES
			(TRUE,TRUE,TRUE,TRUE,TRUE);
			
		INSERT INTO "QueryDirections"
			(question2answer, answer2question, mixed)
			VALUES
			(TRUE,TRUE,TRUE);

		INSERT INTO "Settings"
			(snooze_options, query_types, query_directions, multiple_choice_options, synonym_gradings, type_gradings, question_culture, answer_culture)
			VALUES
			(
				currval('"SnoozeOptions_id_seq"'), 
				currval('"QueryTypes_id_seq"'), 
				currval('"QueryDirections_id_seq"'), 
				currval('"MultipleChoiceOptions_id_seq"'),
				currval('"SynonymGradings_id_seq"'),
				currval('"TypeGradings_id_seq"'),
				'en', 'en'
			);
			
		SELECT INTO allowed_id CAST(currval('"Settings_id_seq"') AS integer);
		
		INSERT INTO "SnoozeOptions"
			(cards_enabled,rights_enabled,time_enabled)
			VALUES
			(FALSE,FALSE,FALSE);
			
		INSERT INTO "MultipleChoiceOptions"
			(allow_multiple_correct_answers, allow_random_distractors, max_correct_answers, number_of_choices)
			VALUES
			(FALSE, TRUE, 1, 4);
			
		INSERT INTO "QueryTypes"
			(image_recognition, listening_comprehension, multiple_choice, sentence, word)
			VALUES
			(FALSE,FALSE,TRUE,FALSE,TRUE);
			
		INSERT INTO "TypeGradings"
			(all_correct, half_correct, none_correct, prompt)
			VALUES
			(FALSE,TRUE,FALSE,FALSE);

		INSERT INTO "SynonymGradings"
			(all_known, half_known, one_known, first_known, prompt)
			VALUES
			(FALSE,FALSE,TRUE,FALSE,FALSE);
			
		INSERT INTO "QueryDirections"
			(question2answer, answer2question, mixed)
			VALUES
			(TRUE,FALSE,FALSE);
			
		INSERT INTO "Boxes"
			(box1_size, box2_size, box3_size, box4_size, box5_size, box6_size, box7_size, box8_size, box9_size)
			VALUES
			(10, 20, 50, 100, 250, 500, 1000, 2000, 4000);

		INSERT INTO "Settings"
			(snooze_options, query_types, query_directions, multiple_choice_options, synonym_gradings, type_gradings, boxes, autoplay_audio, case_sensitive, confirm_demote, 
			enable_commentary, correct_on_the_fly, enable_timer, random_pool, self_assessment, show_images, stripchars, auto_boxsize, pool_empty_message_shown, 
			show_statistics, skip_correct_answers, use_lm_stylesheets, question_culture, answer_culture)
			VALUES
			(
				currval('"SnoozeOptions_id_seq"'), 
				currval('"QueryTypes_id_seq"'), 
				currval('"QueryDirections_id_seq"'), 
				currval('"MultipleChoiceOptions_id_seq"'),
				currval('"SynonymGradings_id_seq"'),
				currval('"TypeGradings_id_seq"'),
				currval('"Boxes_id_seq"'),
				TRUE, FALSE, FALSE,
				FALSE, FALSE, FALSE, TRUE, FALSE, TRUE, '!,.?;', FALSE, FALSE,
				TRUE, FALSE, TRUE, 'en', 'en'
			);
			
		SELECT INTO default_id CAST(currval('"Settings_id_seq"') AS integer);

		INSERT INTO "LearningModules" (guid, categories_id, allowed_settings_id, default_settings_id, title)
			VALUES	($1, $2, allowed_id, default_id, $3);

		SELECT INTO result CAST(currval('"LearningModules_id_seq"') AS integer);
		
		RETURN result;
	END;
$$ LANGUAGE 'plpgsql';

DROP TYPE IF EXISTS "CardState" CASCADE;
CREATE TYPE "CardState" AS (box integer, active boolean, timestamp timestamp);

CREATE OR REPLACE FUNCTION "GetCardState"(param_user_id integer, param_cards_id integer) RETURNS "CardState" AS $$
	DECLARE
		cnt integer;
		result "CardState";
	BEGIN
		SELECT count(*) INTO cnt FROM "UserCardState" WHERE user_id=param_user_id and cards_id=param_cards_id;
		
		If cnt < 1 THEN
			INSERT INTO "UserCardState" (user_id, cards_id, box, active) VALUES (param_user_id, param_cards_id, 0, true);
		END IF;
		-- DO NOT put the parameters of the result-type into brackets!!!
		SELECT box, active, timestamp INTO result FROM "UserCardState" WHERE user_id=param_user_id and cards_id=param_cards_id;
		RETURN result;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "SetCardState"(param_user_id integer, param_cards_id integer, param_box integer, param_active boolean, param_timestamp timestamp) RETURNS VOID AS $$
	DECLARE
		cnt integer;
		result "CardState";
	BEGIN
		SELECT count(*) INTO cnt FROM "UserCardState" WHERE user_id=param_user_id and cards_id=param_cards_id;
		
		If cnt < 1 THEN
			INSERT INTO "UserCardState" (user_id, cards_id, box, active, timestamp) VALUES (param_user_id, param_cards_id, param_box, param_active, param_timestamp);
		ELSE
			UPDATE "UserCardState" SET box=param_box, active=param_active, timestamp=param_timestamp WHERE user_id=param_user_id and cards_id=param_cards_id;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "GetUserSettings"(param_user_id integer, param_lm_id integer) RETURNS integer AS $$
	DECLARE
		cnt integer;
		cid integer;
		result integer;
	BEGIN
		SELECT count(*) INTO cnt FROM "UserProfilesLearningModulesSettings" WHERE user_id=param_user_id and lm_id=param_lm_id;
		
		IF cnt < 1 THEN
			INSERT INTO "UserProfilesLearningModulesSettings" VALUES (param_user_id, param_lm_id, "CreateNewSetting"());
		
			SELECT settings_id INTO result FROM "UserProfilesLearningModulesSettings" WHERE user_id=param_user_id and lm_id=param_lm_id;
			
			FOR cid IN SELECT id FROM "Chapters" WHERE lm_id=param_lm_id LOOP
				INSERT INTO "SelectedLearnChapters" VALUES (cid, result);
			END LOOP;
			
			RETURN result;
		ELSE		
			SELECT settings_id INTO result FROM "UserProfilesLearningModulesSettings" WHERE user_id=param_user_id and lm_id=param_lm_id;	
			RETURN result;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

-- This function returns the settings
DROP TYPE IF EXISTS "AllSettings" CASCADE;
CREATE TYPE "AllSettings" AS (autoplay_audio boolean, 
			      case_sensitive boolean,
			      confirm_demote boolean,
			      enable_commentary boolean,
			      correct_on_the_fly boolean,
			      enable_timer boolean,
			      synonymGradingsId integer,		--FK
			      typeGradingsId integer,			--FK
			      multipleChoiceOptionsId integer,	--FK
			      queryDirectionsId integer,		--FK
			      queryTypesId integer,				--FK
			      random_pool boolean,
			      self_assessment boolean,
			      show_images boolean,
			      stripchars varchar(100),
			      question_culture varchar(100),
			      answer_culture varchar(100),
			      question_caption varchar(100),
			      answer_caption varchar(100),
			      logo integer,
			      auto_boxsize boolean,
			      pool_empty_message_shown boolean,
			      show_statistics boolean,
			      skip_correct_answers boolean,
			      snoozeOptionsId integer,			--FK
			      use_lm_stylesheets boolean,
			      cardStyleId integer,				--FK
			      boxesId integer,					--FK
			      isCached boolean,

			      --SnoozeOptions
			      /*cards_enabled boolean,
			      rights_enabled boolean,
			      time_enabled boolean,
			      snooze_cards integer,
			      snooze_high integer,
			      snooze_low integer,
			      snooze_mode "SnoozeMode",
			      snooze_rights integer,
			      snooze_time integer,

			      --Boxes
			      box1_size integer,
			      box2_size integer,
			      box3_size integer,
			      box4_size integer,
			      box5_size integer,
			      box6_size integer,
			      box7_size integer,
			      box8_size integer,
			      box9_size integer,

			      --TypeGradings
			      all_correct boolean,
			      half_correct boolean,
			      none_correct boolean,
			      typing_prompt boolean,

			      --MultipleChoiceOptions
			      allow_multiple_correct_answers boolean,
			      allow_random_distractors boolean,
			      max_correct_answers integer,
			      number_of_choices integer,

			      --QueryTypes
			      image_recognition boolean,
			      listening_comprehension boolean,
			      multiple_choice boolean,
			      sentence boolean,
			      word boolean,

			      --SynonymGradings
			      all_known boolean,
			      half_known boolean,
			      one_known boolean,
			      first_known boolean,
			      synonyms_prompt boolean,

			      --QueryDirections
			      question2answer boolean,
			      answer2question boolean,
			      mixed boolean,*/

			      -- StyleSheets
			      question_stylesheet text,
			      answer_stylesheet text,

			      --CardStyles
			      cardstyle text
			      );

CREATE OR REPLACE FUNCTION "GetAllSettings"(settings_id integer) RETURNS "AllSettings" AS $$
	DECLARE
	   result "AllSettings";
	BEGIN
		SELECT 	"Settings".autoplay_audio, "Settings".case_sensitive, "Settings".confirm_demote, "Settings".enable_commentary, "Settings".correct_on_the_fly, "Settings".enable_timer, "Settings".synonym_gradings,
			"Settings".type_gradings, "Settings".multiple_choice_options, "Settings".query_directions, "Settings".query_types, 
			"Settings".random_pool,	"Settings".self_assessment, "Settings".show_images, "Settings".stripchars, "Settings".question_culture, "Settings".answer_culture, "Settings".question_caption,
			"Settings".answer_caption, "Settings".logo, "Settings".auto_boxsize, "Settings".pool_empty_message_shown, "Settings".show_statistics, "Settings".skip_correct_answers, "Settings".snooze_options, 
			"Settings".use_lm_stylesheets, "Settings".cardstyle, "Settings".boxes, "Settings".isCached,
			/*"SnoozeOptions".cards_enabled, "SnoozeOptions".rights_enabled, "SnoozeOptions".time_enabled, "SnoozeOptions".snooze_cards, "SnoozeOptions".snooze_high, "SnoozeOptions".snooze_low,
			"SnoozeOptions".snooze_mode, "SnoozeOptions".snooze_rights, "SnoozeOptions".snooze_time,
			"Boxes".box1_size, "Boxes".box2_size, "Boxes".box3_size, "Boxes".box4_size, "Boxes".box5_size, "Boxes".box6_size, "Boxes".box7_size, "Boxes".box8_size, "Boxes".box9_size,
			"TypeGradings".all_correct, "TypeGradings".half_correct, "TypeGradings".none_correct, "TypeGradings".prompt,
			"MultipleChoiceOptions".allow_multiple_correct_answers, "MultipleChoiceOptions".allow_random_distractors, "MultipleChoiceOptions".max_correct_answers, "MultipleChoiceOptions".number_of_choices,
			"QueryTypes".image_recognition, "QueryTypes".listening_comprehension, "QueryTypes".multiple_choice, "QueryTypes".sentence, "QueryTypes".word,
			"SynonymGradings".all_known, "SynonymGradings".half_known, "SynonymGradings".one_known, "SynonymGradings".first_known, "SynonymGradings".prompt,
			"QueryDirections".question2answer, "QueryDirections".answer2question, "QueryDirections".mixed,*/
	
		CASE WHEN "Settings".question_stylesheet > 0 THEN
			(SELECT "StyleSheets".value FROM "StyleSheets" WHERE "StyleSheets".id = "Settings".question_stylesheet )
		END AS question_stylesheet,
		CASE WHEN "Settings".answer_stylesheet > 0 THEN
			(SELECT "StyleSheets".value FROM "StyleSheets" WHERE "StyleSheets".id = "Settings".answer_stylesheet )
		END AS answer_stylesheet,
		CASE WHEN "Settings".cardstyle > 0 THEN
			(SELECT "CardStyles".value FROM "CardStyles" WHERE "CardStyles".id = "Settings".cardstyle )
		END AS cardstyle

		INTO result

		FROM "Settings"/*, "SnoozeOptions", "Boxes", "TypeGradings", "MultipleChoiceOptions", "QueryTypes", "SynonymGradings", "QueryDirections"*/
		WHERE "Settings".id = settings_id;
			/*AND "Settings".snooze_options = "SnoozeOptions".id
			AND "Settings".boxes = "Boxes".id
			AND "Settings".type_gradings = "TypeGradings".id
			AND "Settings".multiple_choice_options = "MultipleChoiceOptions".id
			AND "Settings".query_types = "QueryTypes".id
			AND "Settings".synonym_gradings = "SynonymGradings".id
			AND "Settings".query_directions = "QueryDirections".id;*/

		RETURN result;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;


DROP TYPE IF EXISTS "ScoreValues" CASCADE;
CREATE TYPE "ScoreValues" AS (sum numeric, total numeric);
DROP FUNCTION IF EXISTS "GetScore"(integer, integer) CASCADE;
CREATE OR REPLACE FUNCTION "GetScore"(param_user_id integer, param_lm_id integer) RETURNS "ScoreValues" AS $$
DECLARE
	ScoreValue "ScoreValues";
BEGIN
	SELECT (box1_content * 0 + box2_content * 1 + box3_content * 1.5 + box4_content * 1.83333333333333 + box5_content * 2.08333333333333 + box6_content * 2.28333333333333 + box7_content * 2.45 + box8_content * 2.59285714285714 + box9_content * 2.71785714285714 + box10_content * 2.82896825396825), ((pool_content + box1_content + box2_content + box3_content + box4_content + box5_content + box6_content + box7_content + box8_content + box9_content + box10_content) * 2.82896825396825) INTO ScoreValue FROM "LearningSessions" WHERE (user_id=param_user_id AND lm_id=param_lm_id) ORDER BY id DESC LIMIT 1;
	RETURN ScoreValue;
END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

DROP TYPE IF EXISTS "BoxSizes" CASCADE;
--DROP TYPE IF EXISTS "BoxCardsCount" CASCADE;
CREATE TYPE "BoxSizes" AS (box0 integer, box1 integer, box2 integer, box3 integer, box4 integer, box5 integer, box6 integer, box7 integer, box8 integer, box9 integer, box10 integer);
--CREATE TYPE "BoxCardsCount" AS (box0 integer, box1 integer, box2 integer, box3 integer, box4 integer, box5 integer, box6 integer, box7 integer, box8 integer, box9 integer, box10 integer);

CREATE OR REPLACE FUNCTION "GetBoxSizes"(param_user_id integer, param_lm_id integer) RETURNS "BoxSizes" AS $$
	DECLARE
		cnt integer;
		bid integer;
		result "BoxSizes";
		result_user "BoxSizes";
	BEGIN
		SELECT count(*) INTO cnt FROM "UserProfilesLearningModulesSettings" WHERE user_id=param_user_id and lm_id=param_lm_id;
		
		SELECT (SELECT count(*) FROM "Cards" WHERE id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=param_lm_id)), box1_size, box2_size, box3_size, box4_size, box5_size, 
			box6_size, box7_size, box8_size, box9_size, (SELECT count(*) FROM "Cards" WHERE id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=param_lm_id)) 
			INTO result FROM "Boxes" WHERE id=(SELECT boxes FROM "Settings" WHERE id=(SELECT default_settings_id FROM "LearningModules" WHERE id=param_lm_id));
			
		IF cnt > 0 THEN
			SELECT boxes INTO bid FROM "Settings" WHERE id=(SELECT settings_id FROM "UserProfilesLearningModulesSettings" WHERE user_id=param_user_id and lm_id=param_lm_id);
						
			SELECT count(*), 
				COALESCE((SELECT box1_size FROM "Boxes" WHERE id=bid), result.box1),
				COALESCE((SELECT box2_size FROM "Boxes" WHERE id=bid), result.box2),
				COALESCE((SELECT box3_size FROM "Boxes" WHERE id=bid), result.box3),
				COALESCE((SELECT box4_size FROM "Boxes" WHERE id=bid), result.box4),
				COALESCE((SELECT box5_size FROM "Boxes" WHERE id=bid), result.box5),
				COALESCE((SELECT box6_size FROM "Boxes" WHERE id=bid), result.box6),
				COALESCE((SELECT box7_size FROM "Boxes" WHERE id=bid), result.box7),
				COALESCE((SELECT box8_size FROM "Boxes" WHERE id=bid), result.box8),
				COALESCE((SELECT box9_size FROM "Boxes" WHERE id=bid), result.box9),
				count(*) INTO result_user FROM "Cards" WHERE id IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id=param_lm_id);
			RETURN result_user;
		ELSE
			RETURN result;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

-- This function returns one row and 11 colums with the current number of cards in each box from first start to endSessionId: eg.:
-- b0, b1, b2, b3, b4, b5, b6, b7, b8, b9, b10,
--   ,  9, 15,  7,  3,   ,   ,   ,   ,   ,    ,
CREATE OR REPLACE FUNCTION "GetBoxContent"(endSessionId integer) RETURNS "BoxSizes" AS $$
	DECLARE 
		counter integer;
		result "BoxSizes";
	BEGIN
		SELECT COUNT(*) INTO result.box1 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 1;
			
		SELECT COUNT(*) INTO result.box2 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 2;
			
		SELECT COUNT(*) INTO result.box3 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 3;
			
		SELECT COUNT(*) INTO result.box4 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 4;
			
		SELECT COUNT(*) INTO result.box5 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 5;
			
		SELECT COUNT(*) INTO result.box6 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 6;
			
		SELECT COUNT(*) INTO result.box7 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 7;
			
		SELECT COUNT(*) INTO result.box8 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 8;
			
		SELECT COUNT(*) INTO result.box9 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 9;
			
		SELECT COUNT(*) INTO result.box10 FROM
			(SELECT DISTINCT ON ("LearnLog".cards_id) "LearnLog".new_box FROM
				"LearnLog" WHERE "LearnLog".session_id IN (SELECT id FROM "LearningSessions" WHERE "LearningSessions".lm_id = 
				(SELECT lm_id FROM "LearningSessions" WHERE "LearningSessions".id = endSessionId)) AND "LearnLog".session_id <= endSessionId ORDER BY cards_id, timestamp DESC
			) AS boxes
			WHERE new_box = 10;

		return result;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "FillUpUserCardState"(param_user_id integer, param_lm_id integer) RETURNS void AS $$
	DECLARE
		cardcount integer;
		cardstatecount integer;
		cardid integer;
	BEGIN
		
		SELECT count(*) INTO cardstatecount FROM "UserCardState" WHERE user_id = param_user_id AND cards_id IN 
			(SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id = param_lm_id);
		SELECT count(*) INTO cardcount FROM "LearningModules_Cards" WHERE lm_id = param_lm_id;

		IF cardcount <> cardstatecount THEN
			FOR cardid IN (SELECT cards_id FROM "LearningModules_Cards" WHERE lm_id = param_lm_id AND cards_id NOT IN 
				(SELECT cards_id FROM "UserCardState" WHERE user_id = param_user_id)) LOOP
					INSERT INTO "UserCardState" (user_id, cards_id, box, active) VALUES (param_user_id, cardid, 0, TRUE);
			END LOOP;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

-- starts a new learning session
CREATE OR REPLACE FUNCTION "StartLearningSession"(p_user_id integer, p_lm_id integer, p_pool_content integer, p_box1_content integer, p_box2_content integer, p_box3_content integer, p_box4_content integer, p_box5_content integer, p_box6_content integer, p_box7_content integer, p_box8_content integer, p_box9_content integer, p_box10_content integer) RETURNS integer AS $$
	DECLARE
		result INTEGER;
		newid INTEGER;
		numberPreviousSessions INTEGER;
		lastEndTime TIMESTAMP;
		lastSessionId INTEGER;
	BEGIN
        SELECT count(*) INTO numberPreviousSessions FROM "LearningSessions" WHERE user_id = p_user_id AND lm_id = p_lm_id;
		IF numberPreviousSessions > 0 THEN
			SELECT "id" INTO lastSessionId FROM "LearningSessions" WHERE user_id = p_user_id AND lm_id = p_lm_id ORDER BY starttime DESC;
			SELECT "endtime" INTO lastEndTime FROM "LearningSessions" WHERE user_id = p_user_id AND lm_id = p_lm_id ORDER BY starttime DESC;
			IF lastEndTime IS NULL THEN
				UPDATE "LearningSessions" SET endtime=CURRENT_TIMESTAMP WHERE id = lastSessionId;
			END IF;
		END IF;
                
		SELECT INTO newid CAST(nextval('"LearningSessions_id_seq"') AS integer);
		INSERT INTO "LearningSessions" (id, user_id, lm_id, starttime, sum_right, sum_wrong, pool_content, box1_content,
					        box2_content, box3_content, box4_content, box5_content, box6_content, box7_content,
					        box8_content, box9_content, box10_content)
			VALUES(newid, p_user_id, p_lm_id, CURRENT_TIMESTAMP,
			       0, 0, p_pool_content, p_box1_content, p_box2_content,
			       p_box3_content, p_box4_content, p_box5_content, p_box6_content,
			       p_box7_content, p_box8_content, p_box9_content, p_box10_content);

		SELECT INTO result CAST(currval('"LearningSessions_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql';

-- functions for user login
DROP TYPE IF EXISTS "UserStruct" CASCADE;
CREATE TYPE "UserStruct" AS (username varchar(100), typ usertype);

CREATE OR REPLACE FUNCTION "GetUserList"() RETURNS SETOF "UserStruct" AS $$
		SELECT username, user_type FROM "UserProfiles" WHERE user_type != 'LocalDirectoryAuthentication';
$$ LANGUAGE 'SQL' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "CheckUserSession"(session_id varchar(36)) RETURNS varchar(100) AS $$
	DECLARE
    	validsession INTEGER;
    BEGIN
    	SELECT id INTO validsession FROM "UserSessions" WHERE sid=session_id AND logout_time > now() ORDER BY logout_time DESC LIMIT 1;
        
        IF COUNT(validsession)>0 THEN
        	UPDATE "UserSessions" SET refresh_time=now(), logout_time=now()+'7 days' WHERE sid=session_id;
        	RETURN NULL;
        ELSE
        	RETURN 'SessionInvalid';
        END IF;
    END;
$$ LANGUAGE 'plpgsql' SECURITY Definer;

CREATE OR REPLACE FUNCTION "LoginListUser"(param_username varchar(100), new_session_id VARCHAR(36), close_open_sessions BOOLEAN, stand_alone BOOLEAN) RETURNS integer AS $$
	DECLARE
		allowed BOOLEAN;
		user_count INTEGER;
		user_auth_type usertype;
		uid INTEGER;
        session_id varchar(36);
        isValide varchar(100);
	BEGIN
		SELECT value INTO allowed FROM "DatabaseInformation" WHERE property='ListAuthentication';
		
		IF NOT allowed THEN
			RETURN -4;
		ELSE
			SELECT count(*) INTO user_count FROM "UserProfiles" WHERE username=param_username;
			
			IF user_count < 1 THEN
				RETURN -1;
			ELSE
				SELECT user_type INTO user_auth_type FROM "UserProfiles" WHERE username=param_username;
				
				IF user_auth_type <> 'ListAuthentication' THEN
					RETURN -3;
				ELSE
					SELECT id INTO uid FROM "UserProfiles" WHERE username=param_username;
					
                    SELECT sid INTO session_id FROM "UserSessions" WHERE user_id=uid AND logout_time>now() ORDER BY logout_time DESC LIMIT 1;
                    
                    IF count(session_id)<1 OR stand_alone THEN
                    	INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                        	new_session_id,
                            uid,
                            now(),
                            now()+'7 days');
                            
						RETURN uid;
                    ELSE
                    	SELECT * INTO isValide FROM "CheckUserSession"(session_id);
                        
                        IF count(isValide)=1 THEN
                          INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                              new_session_id,
                              uid,
                              now(),
                              now()+'7 days');
                            
                        	RETURN uid;
                        ELSE
                        	IF close_open_sessions THEN
                            	PERFORM "LogoutUser"(uid);
                                RETURN "LoginListUser"(param_username, new_session_id, FALSE, stand_alone);
                            ELSE
	                        	RETURN -5;
                            END IF;
                        END IF;
                    END IF;
				END IF;
			END IF;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "LoginFormsUser"(param_username varchar(100), param_password varchar(100), new_session_id VARCHAR(36), close_open_sessions BOOLEAN, stand_alone BOOLEAN) RETURNS integer AS $$
	DECLARE
		allowed BOOLEAN;
		passed BOOLEAN;
		user_count INTEGER;
		user_auth_type usertype;
		uid INTEGER;
        session_id varchar(36);
        isValide varchar(100);
	BEGIN
		SELECT value INTO allowed FROM "DatabaseInformation" WHERE property='FormsAuthentication';
		
		IF NOT allowed THEN
			RETURN -4;
		ELSE
			SELECT count(*) INTO user_count FROM "UserProfiles" WHERE username=param_username;
			
			IF user_count < 1 THEN
				RETURN -1;
			ELSE
				SELECT user_type INTO user_auth_type FROM "UserProfiles" WHERE username=param_username;
				
				IF user_auth_type <> 'FormsAuthentication' THEN
					RETURN -3;
				ELSE
					SELECT (password = param_password) INTO passed FROM "UserProfiles" WHERE username=param_username;
					
					IF NOT passed THEN
						RETURN -2;
					ELSE
						SELECT id INTO uid FROM "UserProfiles" WHERE username=param_username;
					
                        SELECT sid INTO session_id FROM "UserSessions" WHERE user_id=uid AND logout_time>now() ORDER BY logout_time DESC LIMIT 1;
                        
                        IF count(session_id)<1 OR stand_alone THEN
                          INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                              new_session_id,
                              uid,
                              now(),
                              now()+'7 days');
                            
                            RETURN uid;
                        ELSE
                            SELECT * INTO isValide FROM "CheckUserSession"(session_id);
                            
                            IF count(isValide)=1 THEN
                            	INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                                    new_session_id,
                                    uid,
                                    now(),
                                    now()+'7 days');
                            
                            	RETURN uid;
                            ELSE
                              IF close_open_sessions THEN
                                  PERFORM "LogoutUser"(uid);
                                  RETURN "LoginFormsUser"(param_username, param_password, new_session_id, FALSE, stand_alone);
                              ELSE
                                  RETURN -5;
                              END IF;
                            END IF;
                        END IF;
					END IF;
				END IF;
			END IF;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "LoginLocalDirectoryUser"(param_username varchar(100), param_ld_id varchar(100), new_session_id VARCHAR(36), close_open_sessions BOOLEAN, stand_alone BOOLEAN) RETURNS integer AS $$
	DECLARE
		allowed BOOLEAN;
		passed BOOLEAN;
		user_count INTEGER;
		user_auth_type usertype;
		uid INTEGER;
        session_id varchar(36);
        isValide varchar(100);
	BEGIN
		SELECT value INTO allowed FROM "DatabaseInformation" WHERE property='LocalDirectoryAuthentication';
		
		IF NOT allowed THEN
			RETURN -4;
		ELSE
			SELECT count(*) INTO user_count FROM "UserProfiles" WHERE username=param_username;
			
			IF user_count < 1 THEN
				RETURN -1;
			ELSE
				SELECT user_type INTO user_auth_type FROM "UserProfiles" WHERE username=param_username;
				
				IF user_auth_type <> 'LocalDirectoryAuthentication' THEN
					RETURN -3;
				ELSE
					SELECT (local_directory_id = param_ld_id) INTO passed FROM "UserProfiles" WHERE username=param_username;
					
					IF NOT passed THEN
						RETURN -2;
					ELSE
						SELECT id INTO uid FROM "UserProfiles" WHERE username=param_username;
					
                        SELECT sid INTO session_id FROM "UserSessions" WHERE user_id=uid AND logout_time>now() ORDER BY logout_time DESC LIMIT 1;
                        
                        IF count(session_id)<1 OR stand_alone THEN
                            INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                                new_session_id,
                                uid,
                                now(),
                                now()+'7 days');
                            
                            RETURN uid;
                        ELSE
                            SELECT * INTO isValide FROM "CheckUserSession"(session_id);
                            
                            IF count(isValide)=1 THEN
                                INSERT INTO "UserSessions"(sid, user_id, login_time, logout_time) VALUES (
                                    new_session_id,
                                    uid,
                                    now(),
                                    now()+'7 days');
                            
                                RETURN uid;
                            ELSE
                              IF close_open_sessions THEN
                                  PERFORM "LogoutUser"(uid);
                                  RETURN "LoginLocalDirectoryUser"(param_username, param_ld_id, new_session_id, FALSE, stand_alone);
                              ELSE
                                  RETURN -5;
                              END IF;
                            END IF;
                        END IF;
					END IF;
				END IF;
			END IF;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "LogoutUser"(session_id varchar(36)) RETURNS void AS $$
	BEGIN
    	UPDATE "UserSessions" SET logout_time=now() WHERE sid=session_id;
    END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "LogoutUser"(uid INTEGER) RETURNS void AS $$
	BEGIN
    	UPDATE "UserSessions" SET logout_time=now() WHERE user_id=uid AND logout_time>now();
    END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;

--****************************************
-- user profile and permission related sps
--****************************************
CREATE OR REPLACE FUNCTION "InsertUserGroups"(group_name varchar(100)) RETURNS integer AS $$
	DECLARE
		result INTEGER;
	BEGIN
		INSERT INTO "UserGroups"("name") VALUES(group_name);
		SELECT INTO result CAST(currval('"UserGroups_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "InsertAclEntry"(p_object_id integer, p_permissions_id integer, p_access boolean) RETURNS integer AS $$
	DECLARE
		dummyo INTEGER;
		result INTEGER;
	BEGIN
		IF p_object_id = 0 THEN
			SELECT id INTO dummyo FROM "ObjectList" WHERE locator = 'DUMMYOBJECT';
			INSERT INTO "AccessControlList" (object_id,permissions_id,access) VALUES(dummyo,p_permissions_id,p_access);
		ELSE
			INSERT INTO "AccessControlList" (object_id,permissions_id,access) VALUES(p_object_id,p_permissions_id,p_access);
		END IF;
		SELECT INTO result CAST(currval('"AccessControlList_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "InsertObject"(p_locator varchar(100), p_parent varchar(100)) RETURNS integer AS $$
	DECLARE
		parent INTEGER;
		result INTEGER;
	BEGIN
		SELECT id INTO parent FROM "ObjectList" WHERE locator = p_parent;
		IF parent > 0 THEN
			INSERT INTO "ObjectList" (locator,parent_id) VALUES(p_locator,parent);
		ELSE
			INSERT INTO "ObjectList" (locator) VALUES(p_locator);
		END IF;
		SELECT INTO result CAST(currval('"ObjectList_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "InsertTypeDefinitions"(p_clr_name varchar(1000), p_parent varchar(1000)) RETURNS integer AS $$
	DECLARE
		parent INTEGER;
		result INTEGER;
	BEGIN
		SELECT id INTO parent FROM "TypeDefinitions" WHERE clr_name = p_parent;
		IF parent > 0 THEN
			INSERT INTO "TypeDefinitions" (clr_name,parent_id) VALUES(p_clr_name,parent);
		ELSE
			INSERT INTO "TypeDefinitions" (clr_name) VALUES(p_clr_name);
		END IF;
		SELECT INTO result CAST(currval('"TypeDefinitions_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "InsertPermissions"(p_name varchar(100), p_clr_name varchar(1000), p_default boolean) RETURNS integer AS $$
	DECLARE
		typeid INTEGER;
		result INTEGER;
	BEGIN
		SELECT id INTO typeid FROM "TypeDefinitions" WHERE clr_name = p_clr_name;
		IF typeid > 0 THEN
			INSERT INTO "Permissions"(types_id, "name", "default") VALUES (typeid, p_name, p_default);
			SELECT INTO result CAST(currval('"TypeDefinitions_id_seq"') AS integer);
			RETURN result;
		ELSE
			RETURN -1;
		END IF;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "AddGroupToUser"(uid integer, gid integer)
  RETURNS void AS
$BODY$
	BEGIN
		INSERT INTO "UserProfiles_UserGroups" (users_id, groups_id) VALUES (uid, gid);
	END;
$BODY$
  LANGUAGE 'plpgsql' SECURITY DEFINER;
  
CREATE OR REPLACE FUNCTION "AddGroupToUserByName"(p_username varchar(100), p_groupname varchar(100)) RETURNS INTEGER AS $$
	DECLARE
		userid INTEGER;
		groupid INTEGER;
	BEGIN
		SELECT id INTO userid FROM "UserProfiles" WHERE username = p_username;
		SELECT id INTO groupid FROM "UserGroups" WHERE name = p_groupname;
		IF userid > 0 AND groupid > 0 THEN
			INSERT INTO "UserProfiles_UserGroups" (users_id, groups_id) VALUES (userid, groupid);
			RETURN 1;
		ELSE
			RETURN -1;
		END IF;
	END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;
  
CREATE OR REPLACE FUNCTION "InsertGroupPermission"(p_groupname varchar(100), p_permission varchar(100), p_clr_name varchar(100), p_access boolean) RETURNS integer AS $$
	DECLARE
		groupid INTEGER;
		aclid INTEGER;
		permissionid INTEGER;
		objectid INTEGER;
	BEGIN
		SELECT id INTO groupid FROM "UserGroups" WHERE name = p_groupname;
		SELECT id INTO objectid FROM "ObjectList" WHERE locator = 'DUMMYOBJECT';
		SELECT p.id INTO permissionid FROM "TypeDefinitions" AS td INNER JOIN "Permissions" AS p ON td.id = p.types_id
			WHERE td.clr_name = p_clr_name AND p.name = p_permission;
		IF groupid IS NULL OR objectid IS NULL THEN
			RETURN -1;
		END IF;
		INSERT INTO "AccessControlList"(object_id, permissions_id, "access")
			VALUES (objectid, permissionid, p_access);
		SELECT INTO aclid CAST(currval('"AccessControlList_id_seq"') AS integer);

		IF groupid > 0 AND aclid > 0 THEN
			INSERT INTO "UserGroups_AccessControlList"(groups_id, acl_id)
				VALUES (groupid, aclid);
			RETURN 1;
		ELSE
			RETURN -1;
		END IF;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

CREATE OR REPLACE FUNCTION "InsertUserProfile"(p_username varchar(100), p_password varchar(100), p_local_directory_id varchar(100), p_user_type usertype) RETURNS integer AS $$
	DECLARE
		result INTEGER;
	BEGIN
		IF p_user_type = 'FormsAuthentication' THEN
			INSERT INTO "UserProfiles" (id, username, password, local_directory_id, user_type)
				VALUES (default, p_username, md5(p_password), p_local_directory_id, p_user_type);
		ELSE
			INSERT INTO "UserProfiles" (id, username, password, local_directory_id, user_type)
				VALUES (default, p_username, '', p_local_directory_id, p_user_type);
		END IF;
		SELECT INTO result CAST(currval('"UserProfiles_id_seq"') AS integer);
		RETURN result;
	END;
$$ language 'plpgsql' SECURITY DEFINER;

--****************************************
-- SYNCHRONIZATION
--****************************************
-- synchronisation helper views
CREATE OR REPLACE VIEW "Syncview_Settings" AS
	SELECT l.id AS session_lm_id, null::integer AS session_user_id, l.default_settings_id AS id FROM "LearningModules" AS l
	UNION
	SELECT l.id AS session_lm_id, null::integer AS session_user_id, l.allowed_settings_id AS id FROM "LearningModules" AS l
	UNION
	SELECT l.lm_id AS session_lm_id, null::integer AS session_user_id, c.settings_id AS id FROM "Cards" AS c JOIN "LearningModules_Cards" AS l ON c.id=l.cards_id WHERE settings_id IS NOT NULL
	UNION
	SELECT c.lm_id AS session_lm_id, null::integer AS session_user_id, c.settings_id AS id FROM "Chapters" AS c WHERE settings_id IS NOT NULL
	UNION
	SELECT u.lm_id AS session_lm_id, u.user_id AS session_user_id, u.settings_id AS id FROM "UserProfilesLearningModulesSettings" AS u WHERE settings_id IS NOT NULL;

CREATE OR REPLACE VIEW "Syncview_MediaContent" AS
	SELECT l.lm_id AS session_lm_id, null::integer AS session_user_id, c.media_id AS id FROM "Cards_MediaContent" AS c 
	JOIN "LearningModules_Cards" AS l ON c.cards_id=l.cards_id
	UNION 
	SELECT sy.session_lm_id AS session_lm_id, sy.session_user_id AS session_user_id, s.logo AS id FROM "Syncview_Settings" AS sy JOIN "Settings" AS s ON sy.id=s.id WHERE s.logo IS NOT NULL
	UNION
	SELECT sy.session_lm_id AS session_lm_id, sy.session_user_id AS session_user_id, c.media_id AS id FROM "CommentarySounds" AS c JOIN "Syncview_Settings" AS sy ON c.settings_id=sy.id;

CREATE OR REPLACE VIEW "Syncview_AccessControlList" AS
	SELECT null::integer AS session_lm_id, up.users_id AS session_user_id, up.acl_id AS id FROM "UserProfiles_AccessControlList" AS up
	UNION
	SELECT null::integer AS session_lm_id, uu.users_id AS session_user_id, ua.acl_id AS id FROM "UserGroups_AccessControlList" AS ua JOIN "UserProfiles_UserGroups" AS uu ON ua.groups_id=uu.groups_id;
	
-- synchronization helper tables
CREATE TABLE "BatchSessions"
(
	client_id integer primary key NOT NULL,
	created timestamp default now(),
	batch_size integer NOT NULL,
	batch_count integer NOT NULL
);

CREATE TABLE "BatchTimestamps"
(
	client_id integer NOT NULL,
	updated timestamp NOT NULL,
	CONSTRAINT "client_id_FK" FOREIGN KEY (client_id)
		REFERENCES "BatchSessions" (client_id) MATCH SIMPLE
		ON UPDATE NO ACTION ON DELETE CASCADE
);

-- synchronisation helper functions
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

CREATE OR REPLACE FUNCTION "GetNewBatchAnchor"(IN sync_last_received_anchor timestamp with time zone, IN sync_batch_size bigint, OUT sync_max_received_anchor timestamp with time zone, OUT sync_new_received_anchor timestamp with time zone, INOUT sync_batch_count INTEGER, IN sync_client_id_hash INTEGER, IN session_lm_id INTEGER, IN session_user_id INTEGER, IN isNewDb BOOLEAN)
  RETURNS SETOF record AS $$
DECLARE
	batch_size		BIGINT;
	last_received_anchor	TIMESTAMP WITH TIME ZONE;
	sync_row_count		INTEGER;
BEGIN
	-- Set a default batch size if a valid one is not passed in.
	IF sync_batch_size IS NULL OR sync_batch_size<=0 THEN
		batch_size := 1000;
	ELSE
		batch_size := sync_batch_size;
	END IF;

	-- Before selecting the first batch of changes,
	-- set the maximum anchor value for this synchronization session.
	sync_max_received_anchor := now();

	-- If this is the first synchronization session for a database,
	-- get the lowest possible timestamp value. 
	IF sync_last_received_anchor IS NULL THEN 
		last_received_anchor := '-infinity'::timestamp;
	ELSE
		last_received_anchor := sync_last_received_anchor;
	END IF;

	-- Calculate the batch anchors in case this is the first batch of a synchronization session
	IF (sync_batch_count <= 0) THEN
		SELECT r.sync_row_count, r.sync_batch_count INTO sync_row_count, sync_batch_count 
		FROM "RenderBatchAnchors"(last_received_anchor, sync_max_received_anchor, sync_batch_size, sync_client_id_hash, session_lm_id, session_user_id, isNewDb) AS r;
	END IF;

	-- Select the current anchor timestamp
	SELECT updated FROM "BatchTimestamps"
		WHERE client_id=sync_client_id_hash 
		LIMIT 1 OFFSET sync_batch_size
		INTO sync_new_received_anchor;

	-- If last batch: clean up whole session
	IF sync_new_received_anchor IS NULL THEN
		sync_new_received_anchor = sync_max_received_anchor;
		DELETE FROM "BatchSessions" WHERE client_id = sync_client_id_hash;
	END IF;

	-- Delete old Timestamps
	DELETE FROM "BatchTimestamps"
		WHERE client_id = sync_client_id_hash
		AND updated <= sync_new_received_anchor;

	RETURN NEXT;
END;
$$ LANGUAGE 'plpgsql' SECURITY DEFINER;
  
-- insert sample data (demo dictionary)
INSERT INTO "DatabaseInformation" VALUES ('ListAuthentication', 'true');
INSERT INTO "DatabaseInformation" VALUES ('FormsAuthentication', 'true');
INSERT INTO "DatabaseInformation" VALUES ('LocalDirectoryAuthentication', 'false');
INSERT INTO "DatabaseInformation" VALUES ('LocalDirectoryType', 'eDirectory');
INSERT INTO "DatabaseInformation" VALUES ('LdapServer', '172.22.100.10');
INSERT INTO "DatabaseInformation" VALUES ('LdapPort', '389');
INSERT INTO "DatabaseInformation" VALUES ('LdapUser', null);
INSERT INTO "DatabaseInformation" VALUES ('LdapPassword', null);
INSERT INTO "DatabaseInformation" VALUES ('LdapContext', 'OU=AT,O=ON');
INSERT INTO "DatabaseInformation" VALUES ('LdapUseSsl', 'false');

INSERT INTO "Categories" (global_id, name) VALUES (0, 'Applied Sciences');
INSERT INTO "Categories" (global_id, name) VALUES (1, 'Arts');
INSERT INTO "Categories" (global_id, name) VALUES (2, 'Languages');
INSERT INTO "Categories" (global_id, name) VALUES (3, 'Miscellaneous');
INSERT INTO "Categories" (global_id, name) VALUES (4, 'Natural Sciences');
INSERT INTO "Categories" (global_id, name) VALUES (5, 'Social Sciences');
	
--SELECT "CreateNewLearningModule"
	--(CAST('-' AS character), CAST(currval('"Categories_id_seq"') AS integer), CAST('Test-LM' AS text));
	
--UPDATE "LearningModules" SET
	--author='LearnLift', description='Test-Description'
	--WHERE id=currval('"LearningModules_id_seq"');
	
--UPDATE "Settings" SET
	--question_culture='en-us', answer_culture='de-at', question_caption='English', answer_caption='German'
	--WHERE id=(
		--SELECT default_settings_id 
		--FROM "LearningModules" 
		--WHERE id=currval('"LearningModules_id_seq"')
	--);
	
--INSERT INTO "Chapters"
	--(lm_id,title,description,position)
	--VALUES
	--(currval('"LearningModules_id_seq"'),'First Chapter','Chapter Description',10);
	
--INSERT INTO "SelectedLearnChapters"
	--(chapters_id, settings_id)
	--VALUES
	--(currval('"Chapters_id_seq"'), currval('"Settings_id_seq"'));


--*********************************************
-- create default security setup
--*********************************************
-- type definitions	
SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbDictionaries', '');
SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbDictionary', 'MLifter.DAL.DB.DbDictionaries');
--SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbChapters', 'MLifter.DAL.DB.DbDictionary');
SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbChapter', 'MLifter.DAL.DB.DbDictionary');
--SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbCards', 'MLifter.DAL.DB.DbChapter');
SELECT "InsertTypeDefinitions"('MLifter.DAL.DB.DbCard', 'MLifter.DAL.DB.DbChapter');

-- permissions
-- permissions DbDictionaries
SELECT "InsertPermissions"('IsAdmin', 'MLifter.DAL.DB.DbDictionaries', false);
SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertPermissions"('CanPrint', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertPermissions"('CanExport', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbDictionaries', false);
SELECT "InsertPermissions"('CanModifyMedia', 'MLifter.DAL.DB.DbDictionaries', false);
SELECT "InsertPermissions"('CanModifySettings', 'MLifter.DAL.DB.DbDictionaries', false);
SELECT "InsertPermissions"('CanModifyStyles', 'MLifter.DAL.DB.DbDictionaries', false);
SELECT "InsertPermissions"('CanModifyProperties', 'MLifter.DAL.DB.DbDictionaries', false);
-- permissions DbDictionary
SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertPermissions"('CanPrint', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertPermissions"('CanExport', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbDictionary', false);
SELECT "InsertPermissions"('CanModifyMedia', 'MLifter.DAL.DB.DbDictionary', false);
SELECT "InsertPermissions"('CanModifySettings', 'MLifter.DAL.DB.DbDictionary', false);
SELECT "InsertPermissions"('CanModifyStyles', 'MLifter.DAL.DB.DbDictionary', false);
SELECT "InsertPermissions"('CanModifyProperties', 'MLifter.DAL.DB.DbDictionary', false);
-- permissions DbChapters
--SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbChapters', true);
--SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbChapters', false);
-- permissions DbChapter
SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbChapter', true);
SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbChapter', false);
-- permissions DbCards
--SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbCards', true);
--SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbCards', false);
-- permissions DbCard
SELECT "InsertPermissions"('Visible', 'MLifter.DAL.DB.DbCard', true);
SELECT "InsertPermissions"('CanModify', 'MLifter.DAL.DB.DbCard', false);

-- insert a dummy object
INSERT INTO "ObjectList"(locator) VALUES ('DUMMYOBJECT');

-- create user groups
SELECT "InsertUserGroups"('Administrator');
SELECT "InsertUserGroups"('Teacher');
SELECT "InsertUserGroups"('Student');
    
-- assign default permissions to groups
SELECT "InsertGroupPermission"('Administrator', 'IsAdmin', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanPrint', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanExport', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyMedia', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifySettings', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyStyles', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyProperties', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanPrint', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanExport', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyMedia', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifySettings', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyStyles', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModifyProperties', 'MLifter.DAL.DB.DbDictionary', true);
--SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbChapters', true);
--SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbChapters', true);
SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbChapter', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbChapter', true);
--SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbCards', true);
--SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbCards', true);
SELECT "InsertGroupPermission"('Administrator', 'Visible', 'MLifter.DAL.DB.DbCard', true);
SELECT "InsertGroupPermission"('Administrator', 'CanModify', 'MLifter.DAL.DB.DbCard', true);

SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyMedia', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifySettings', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyStyles', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyProperties', 'MLifter.DAL.DB.DbDictionaries', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyMedia', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifySettings', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyStyles', 'MLifter.DAL.DB.DbDictionary', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModifyProperties', 'MLifter.DAL.DB.DbDictionary', true);
--SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbChapters', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbChapter', true);
--SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbCards', true);
SELECT "InsertGroupPermission"('Teacher', 'CanModify', 'MLifter.DAL.DB.DbCard', true);

-- create user profiles and assign groups
SELECT "InsertUserProfile"('admin', 'admin', '', 'FormsAuthentication'); SELECT "AddGroupToUserByName"('admin', 'Administrator');
SELECT "InsertUserProfile"('teacher', 'teacher', '', 'FormsAuthentication'); SELECT "AddGroupToUserByName"('teacher', 'Teacher');
SELECT "InsertUserProfile"('student', 'student', '', 'FormsAuthentication'); SELECT "AddGroupToUserByName"('student', 'Student');
	
--*********************************************
-- create indices (general)
--*********************************************
-- Index: tc_cards_id
DROP INDEX IF EXISTS tc_cards_id;
CREATE INDEX tc_cards_id
  ON "TextContent"
  USING btree
  (cards_id);
ALTER TABLE "TextContent" CLUSTER ON tc_cards_id;
	
-- Index: cm_cards_id
DROP INDEX IF EXISTS cm_cards_id;
CREATE INDEX cm_cards_id
  ON "Cards_MediaContent"
  USING btree
  (cards_id);
ALTER TABLE "Cards_MediaContent" CLUSTER ON cm_cards_id;	

-- Index: cm_cards_id_type_side
DROP INDEX IF EXISTS cm_cards_id_type_side;
CREATE INDEX cm_cards_id_type_side
  ON "Cards_MediaContent"
  USING btree
  (cards_id, side, type);

-- Index: slc_settings_id
DROP INDEX IF EXISTS slc_settings_id;
CREATE INDEX slc_settings_id
  ON "SelectedLearnChapters"
  USING btree
  (settings_id);
ALTER TABLE "SelectedLearnChapters" CLUSTER ON slc_settings_id;
	
-- Index: idx_user_card
DROP INDEX IF EXISTS idx_user_card;
CREATE INDEX idx_user_card
  ON "UserCardState"
  USING btree
  (user_id, cards_id);
  
-- Index: idx_extensions
DROP INDEX IF EXISTS idx_extensions;
CREATE INDEX idx_extensions
	ON "Extensions"
	USING btree
	(guid);
	
-- Index: idx_extensionactions
DROP INDEX IF EXISTS idx_extensionactions;
CREATE INDEX idx_extensionactions
	ON "ExtensionActions"
	USING btree
	(guid);
  
--*********************************************
-- create indices for security framework
--*********************************************
-- Index: idx_objectlist_locator
DROP INDEX IF EXISTS idx_objectlist_locator;
CREATE INDEX idx_objectlist_locator
  ON "ObjectList"
  USING btree
  (locator);
 
-- Index: idx_permissions_name
DROP INDEX IF EXISTS idx_permissions_name;
CREATE INDEX idx_permissions_name
  ON "Permissions"
  USING btree
  (name);

-- Index: idx_permissions_id_name
DROP INDEX IF EXISTS idx_permissions_id_name;
CREATE INDEX idx_permissions_id_name
  ON "Permissions"
  USING btree
  (id, name);

-- Index: idx_permissions_id_name_types
DROP INDEX IF EXISTS idx_permissions_id_name_types;
CREATE INDEX idx_permissions_id_name_types
  ON "Permissions"
  USING btree
  (types_id, name, id);

-- Index: idx_permissions_types_id
DROP INDEX IF EXISTS idx_permissions_types_id;
CREATE INDEX idx_permissions_types_id
  ON "Permissions"
  USING btree
  (types_id);

-- Index: idx_typedefinitions_clr_name
DROP INDEX IF EXISTS idx_typedefinitions_clr_name;
CREATE INDEX idx_typedefinitions_clr_name
  ON "TypeDefinitions"
  USING btree
  (clr_name);

