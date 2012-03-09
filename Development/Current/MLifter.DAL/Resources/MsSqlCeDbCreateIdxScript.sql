DROP INDEX TextContent.idx_textcontent_cardsbyposition;
CREATE NONCLUSTERED INDEX idx_textcontent_cardsbyposition ON TextContent 
(
	type ASC,
	cards_id ASC,
    [position] ASC
);
DROP INDEX Chapters.idx_chapters_lm_id;
CREATE NONCLUSTERED INDEX idx_chapters_lm_id ON Chapters 
(
	lm_id ASC,
    [position] ASC
);
DROP INDEX UserCardState.idx_textcontent_user_cards;
CREATE NONCLUSTERED INDEX idx_textcontent_user_cards ON UserCardState 
(
	cards_id ASC,
    [user_id] ASC
);
DROP INDEX Cards.idx_cards_query;
CREATE INDEX idx_cards_query ON Cards
(
	chapters_id ASC,
	lm_id ASC
);