----(1) independed Tables
--DROP TABLE Cards_MediaContent;
--DROP TABLE MediaProperties;
--DROP TABLE CommentarySounds;
--DROP TABLE LearningModules_Cards;
--DROP TABLE Chapters_Cards;
--DROP TABLE SelectedLearnChapters;
--DROP TABLE TextContent;
--DROP TABLE LearnLog;
--DROP TABLE UserCardState;
--DROP TABLE UserProfilesLearningModulesSettings;
--DROP TABLE LearningModules_Tags;
--DROP TABLE MediaContent_Tags;
--DROP TABLE MediaContent_CardStyles

--DROP TABLE UserGroups_AccessControlList;
--DROP TABLE UserProfiles_AccessControlList;
--DROP TABLE AccessControlList;
--DROP TABLE ObjectList;
--DROP TABLE Permissions;
--DROP TABLE TypeDefinitions;
--DROP TABLE UserProfiles_UserGroups;
--DROP TABLE UserGroups;

----depends on (1) or deeper
--DROP TABLE Cards;
--DROP TABLE Chapters;
--DROP TABLE LearningSessions;
--DROP TABLE UserProfiles;
--DROP TABLE LearningModules;
--DROP TABLE Settings;
--DROP TABLE QueryDirections;
--DROP TABLE SynonymGradings;
--DROP TABLE QueryTypes;
--DROP TABLE MediaContent;
--DROP TABLE MultipleChoiceOptions;
--DROP TABLE TypeGradings;
--DROP TABLE CardStyles;
--DROP TABLE StyleSheets;
--DROP TABLE Boxes;
--DROP TABLE SnoozeOptions;
--DROP TABLE Categories;
--DROP TABLE DatabaseInformation;
--DROP TABLE Extensions;
--DROP TABLE ExtensionActions;
--DROP TABLE Tags;

CREATE TABLE Tags
(
	id int PRIMARY KEY IDENTITY,
	text ntext NOT NULL
);

CREATE TABLE Categories
(
	id int PRIMARY KEY IDENTITY,
	global_id int,
	name nvarchar(100)
);

CREATE TABLE MediaContent
(
	id int PRIMARY KEY IDENTITY,
	data image,
	media_type nvarchar(100)
);

CREATE TABLE QueryDirections
(
	id int PRIMARY KEY IDENTITY,
	question2answer bit,
	answer2question bit,
	mixed bit
);

CREATE TABLE SynonymGradings
(
	id int PRIMARY KEY IDENTITY,
	all_known bit,
	half_known bit,
	one_known bit,
	first_known bit,
	prompt bit
);

CREATE TABLE QueryTypes
(
	id int PRIMARY KEY IDENTITY,
	image_recognition bit,
	listening_comprehension bit,
	multiple_choice bit,
	sentence bit,
	word bit
);

CREATE TABLE MultipleChoiceOptions
(
	id int PRIMARY KEY IDENTITY,
	allow_multiple_correct_answers bit,
	allow_random_distractors bit,
	max_correct_answers int,
	number_of_choices int
);

CREATE TABLE TypeGradings
(
	id int PRIMARY KEY IDENTITY,
	all_correct bit,
	half_correct bit,
	none_correct bit,
	prompt bit
);

CREATE TABLE CardStyles
(
	id int PRIMARY KEY IDENTITY,
	value ntext
);

CREATE TABLE StyleSheets
(
	id int PRIMARY KEY IDENTITY,
	value ntext NOT NULL
);

CREATE TABLE Boxes
(
	id int PRIMARY KEY IDENTITY,
	box1_size int,
	box2_size int,
	box3_size int,
	box4_size int,
	box5_size int,
	box6_size int,
	box7_size int,
	box8_size int,
	box9_size int
);

CREATE TABLE SnoozeOptions
(
	id int PRIMARY KEY IDENTITY,
	cards_enabled bit,
	rights_enabled bit,
	time_enabled bit,
	snooze_cards int,
	snooze_high int,
	snooze_low int,
	snooze_mode nvarchar(100),
	snooze_rights int,
	snooze_time int
);

CREATE TABLE Settings
(
	id int PRIMARY KEY IDENTITY,
	autoplay_audio bit,
	case_sensitive bit,
	confirm_demote bit,
	enable_commentary bit,
	correct_on_the_fly bit,
	enable_timer bit,
	synonym_gradings int,
	type_gradings int,
	multiple_choice_options int,
	query_directions int,
	query_types int,
	random_pool bit,
	self_assessment bit,
	show_images bit,
	stripchars nvarchar(100),
	question_culture nvarchar(100),	
	answer_culture nvarchar(100),
	question_caption nvarchar(100),
	answer_caption nvarchar(100),
	logo int,
	question_stylesheet int,
	answer_stylesheet int,
	auto_boxsize bit,
	pool_empty_message_shown bit,
	show_statistics bit,
	skip_correct_answers bit,
	snooze_options int,
	use_lm_stylesheets bit,
	cardstyle int,
	boxes int,
	isCached bit DEFAULT '1',
	FOREIGN KEY (logo) REFERENCES MediaContent(id),
	FOREIGN KEY (synonym_gradings) REFERENCES SynonymGradings(id),
	FOREIGN KEY (type_gradings) REFERENCES TypeGradings(id),
	FOREIGN KEY (multiple_choice_options) REFERENCES MultipleChoiceOptions(id),
	FOREIGN KEY (query_directions) REFERENCES QueryDirections(id),
	FOREIGN KEY (query_types) REFERENCES QueryTypes(id),
	FOREIGN KEY (question_stylesheet) REFERENCES StyleSheets(id),
	FOREIGN KEY (answer_stylesheet) REFERENCES StyleSheets(id),
	FOREIGN KEY (cardstyle) REFERENCES CardStyles(id),
	FOREIGN KEY (snooze_options) REFERENCES SnoozeOptions(id),
	FOREIGN KEY (boxes) REFERENCES Boxes(id)
);


CREATE TABLE LearningModules
(
	id int PRIMARY KEY IDENTITY,
	categories_id int NOT NULL,
	default_settings_id int NOT NULL,
	allowed_settings_id int NOT NULL,
	creator_id int,
	guid nvarchar(36),
	author ntext,
	title ntext,
	description ntext,
	licence_key ntext,
	content_protected bit,
	cal_count int,
	FOREIGN KEY (categories_id) REFERENCES Categories(id),
	FOREIGN KEY (default_settings_id) REFERENCES Settings(id),
	FOREIGN KEY (allowed_settings_id) REFERENCES Settings(id)
);

CREATE TABLE Chapters
(
	id int PRIMARY KEY IDENTITY,
	lm_id int NOT NULL,
	title ntext,
	description ntext,
	position int NOT NULL,
	settings_id int NOT NULL,
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id)
);

CREATE TABLE Cards
(
	id int PRIMARY KEY IDENTITY,
	chapters_id integer NOT NULL DEFAULT 0,
	lm_id integer NOT NULL DEFAULT 0,
	settings_id int NOT NULL,
	FOREIGN KEY (settings_id) REFERENCES Settings(id)
);

CREATE TABLE UserProfiles
(
	id int PRIMARY KEY IDENTITY,
	username nvarchar(100) NOT NULL,
	password nvarchar(100),
	local_directory_id nvarchar(100),
	user_type nvarchar(100) NOT NULL,
	enabled bit NOT NULL DEFAULT 0
);

CREATE TABLE UserGroups
(
	id int PRIMARY KEY IDENTITY,
	name nvarchar(100)
);

CREATE TABLE UserProfiles_UserGroups
(
	users_id int NOT NULL,
	groups_id int NOT NULL,
	CONSTRAINT UserProfiles_UserGroups_PK PRIMARY KEY (users_id, groups_id),
	FOREIGN KEY (users_id) REFERENCES UserProfiles(id),
	FOREIGN KEY (groups_id) REFERENCES UserGroups(id)
);

CREATE TABLE TypeDefinitions
(
	id int PRIMARY KEY IDENTITY,
	clr_name nvarchar(1000) NOT NULL,
	parent_id int
);

CREATE TABLE Permissions
(
	id int PRIMARY KEY IDENTITY,
	types_id int NOT NULL,
	name nvarchar(100),
	"default" bit NOT NULL,
	FOREIGN KEY (types_id) REFERENCES TypeDefinitions(id)
);

CREATE TABLE ObjectList
(
	id int PRIMARY KEY IDENTITY,
	locator nvarchar(100) NOT NULL,
	parent_id int
);

CREATE TABLE AccessControlList
(
	id int PRIMARY KEY IDENTITY,
	object_id int NOT NULL,
	permissions_id int NOT NULL,
	access bit NOT NULL,
	FOREIGN KEY (object_id) REFERENCES ObjectList(id),
	FOREIGN KEY (permissions_id) REFERENCES Permissions(id)
);

CREATE TABLE UserProfiles_AccessControlList
(
	users_id int NOT NULL,
	acl_id int NOT NULL,
	CONSTRAINT UserProfiles_AccessControlList_PK PRIMARY KEY (users_id, acl_id),
	FOREIGN KEY (acl_id) REFERENCES AccessControlList(id),
	FOREIGN KEY (users_id) REFERENCES UserProfiles(id)
);


CREATE TABLE UserGroups_AccessControlList
(
	groups_id int NOT NULL,
	acl_id int NOT NULL,
	CONSTRAINT UserGroups_AccessControlList_PK PRIMARY KEY (groups_id, acl_id),
	FOREIGN KEY (acl_id) REFERENCES AccessControlList(id),
	FOREIGN KEY (groups_id) REFERENCES UserGroups(id)
);

CREATE TABLE LearningSessions
(
	id int IDENTITY,
	user_id int NOT NULL,
	lm_id int NOT NULL,
	starttime datetime,
	endtime datetime,
	sum_right int default 0,
	sum_wrong int default 0,
	pool_content int,
	box1_content int,
	box2_content int,
	box3_content int,
	box4_content int,
	box5_content int,
	box6_content int,
	box7_content int,
	box8_content int,
	box9_content int,
	box10_content int,
	CONSTRAINT LearningSessions_PK PRIMARY KEY (id, user_id, lm_id),
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id),
	FOREIGN KEY (user_id) REFERENCES UserProfiles(id)
);

----------------------<
-- Independed Tables <<
----------------------<

CREATE TABLE LearningModules_Tags
(
	lm_id int NOT NULL,
	tags_id int NOT NULL,
	CONSTRAINT LearningModule_Tags_PK PRIMARY KEY (lm_id, tags_id),
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id),
	FOREIGN KEY (tags_id) REFERENCES Tags(id)
);

CREATE TABLE MediaContent_Tags
(
	media_id int NOT NULL,
	tags_id int NOT NULL,
	CONSTRAINT MediaContent_Tags_PK PRIMARY KEY (media_id, tags_id),
	FOREIGN KEY (media_id) REFERENCES MediaContent(id),
	FOREIGN KEY (tags_id) REFERENCES Tags(id)
);

CREATE TABLE UserProfilesLearningModulesSettings
(
	user_id int NOT NULL,
	lm_id int NOT NULL,
	settings_id int NOT NULL,
	highscore numeric(18,10) default 0,
	CONSTRAINT UserProfilesLearningModulesSettings_PK PRIMARY KEY (user_id, lm_id),
	FOREIGN KEY (user_id) REFERENCES UserProfiles(id),
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id),
	FOREIGN KEY (settings_id) REFERENCES Settings(id)
);

CREATE TABLE LearnLog
(
	id int PRIMARY KEY IDENTITY,
	session_id int NOT NULL,
	user_id int NOT NULL,
	lm_id int NOT NULL,
	cards_id int NOT NULL,
	old_box int,
	new_box int,
	timestamp datetime,
	duration bigint,
	learn_mode nvarchar(100),
	move_type nvarchar(100),
	answer ntext,
	direction nvarchar(100),
	case_sensitive bit,
	correct_on_the_fly bit,
	percentage_known int,
	percentage_required int,
	FOREIGN KEY (session_id, user_id, lm_id) REFERENCES LearningSessions(id, user_id, lm_id),
	FOREIGN KEY (cards_id) REFERENCES Cards(id)
);

CREATE TABLE UserCardState
(
	user_id int NOT NULL,
	cards_id int NOT NULL,
	box int,
	active bit,
	timestamp datetime,
	CONSTRAINT UserCardState_PK PRIMARY KEY (user_id, cards_id),
	FOREIGN KEY (user_id) REFERENCES UserProfiles(id),
	FOREIGN KEY (cards_id) REFERENCES Cards(id)
);

CREATE TABLE TextContent
(
	id int PRIMARY KEY IDENTITY,
	cards_id int NOT NULL,
	text nvarchar(4000),
	side nvarchar(100),
	type nvarchar(100),
	position int,
	is_default bit,
	FOREIGN KEY (cards_id) REFERENCES Cards(id)
);

CREATE TABLE SelectedLearnChapters
(
	chapters_id int NOT NULL,
	settings_id int NOT NULL,
	CONSTRAINT SelectedLearnChapters_PK PRIMARY KEY(chapters_id, settings_id),
	FOREIGN KEY (chapters_id) REFERENCES Chapters(id),
	FOREIGN KEY (settings_id) REFERENCES Settings(id)
);

CREATE TABLE Chapters_Cards
(
	chapters_id int NOT NULL,
	cards_id int NOT NULL,
	FOREIGN KEY (chapters_id) REFERENCES Chapters(id),
	FOREIGN KEY (cards_id) REFERENCES Cards(id),
	CONSTRAINT Chapters_Cards_PK PRIMARY KEY (chapters_id, cards_id)
);

CREATE TABLE LearningModules_Cards
(
	lm_id int NOT NULL,
	cards_id int NOT NULL,
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id),
	FOREIGN KEY (cards_id) REFERENCES Cards(id),
	CONSTRAINT LearningModules_Cards_PK PRIMARY KEY (lm_id, cards_id)
);

CREATE TABLE CommentarySounds
(
	media_id int NOT NULL,
	settings_id int NOT NULL,
	side nvarchar(100) NOT NULL,
	type nvarchar(100) NOT NULL,
	CONSTRAINT CommentarySounds_PK PRIMARY KEY(media_id, settings_id, side, type),
	FOREIGN KEY (media_id) REFERENCES MediaContent(id),
	FOREIGN KEY (settings_id) REFERENCES Settings(id)
);

CREATE TABLE MediaProperties
(
	media_id int NOT NULL,
	property nvarchar(100),
	value nvarchar(100),
	CONSTRAINT MediaProperties_PK PRIMARY KEY (media_id, property),
	FOREIGN KEY (media_id) REFERENCES MediaContent(id)
);

CREATE TABLE Cards_MediaContent
(
	media_id int NOT NULL,
	cards_id int NOT NULL,
	side nvarchar(100) NOT NULL,
	type nvarchar(100) NOT NULL,
	is_default bit NOT NULL,
	CONSTRAINT Cards_MediaContent_PK PRIMARY KEY (media_id, cards_id, side, type),
	FOREIGN KEY (media_id) REFERENCES MediaContent(id),
	FOREIGN KEY (cards_id) REFERENCES Cards(id)
);

CREATE TABLE MediaContent_CardStyles
(
	media_id int NOT NULL,
	cardstyles_id int NOT NULL,
	CONSTRAINT MediaContent_CardStyles_PK PRIMARY KEY (media_id, cardstyles_id),
	FOREIGN KEY (media_id) REFERENCES MediaContent(id),
	FOREIGN KEY (cardstyles_id) REFERENCES CardStyles(id)
);

CREATE TABLE DatabaseInformation
(
	property nvarchar(100) PRIMARY KEY,
	value nvarchar(100)
);

CREATE TABLE Extensions
(
	guid nvarchar(36) PRIMARY KEY NOT NULL,
	lm_id int,
	name ntext NOT NULL,
	version nvarchar(10) NOT NULL,
	type nvarchar(100) NOT NULL,
	data image NOT NULL,
	startfile ntext,
	FOREIGN KEY (lm_id) REFERENCES LearningModules(id)
);

CREATE TABLE ExtensionActions
(
	guid nvarchar(36) NOT NULL,
	action nvarchar(100) NOT NULL,
	CONSTRAINT ExtensionActions_PK PRIMARY KEY(guid, action),
	execution nvarchar(100) NOT NULL,
	FOREIGN KEY (guid) REFERENCES Extensions(guid)
);

INSERT INTO Categories (global_id, name) VALUES (0, 'Applied Sciences');
INSERT INTO Categories (global_id, name) VALUES (1, 'Arts');
INSERT INTO Categories (global_id, name) VALUES (2, 'Languages');
INSERT INTO Categories (global_id, name) VALUES (3, 'Miscellaneous');
INSERT INTO Categories (global_id, name) VALUES (4, 'Natural Sciences');
INSERT INTO Categories (global_id, name) VALUES (5, 'Social Sciences');

INSERT INTO DatabaseInformation (property, value) VALUES ('Version', '1.0.2');
INSERT INTO DatabaseInformation (property, value) VALUES ('SupportedDataLayerVersions', '2.2.406,>2.2.406');
