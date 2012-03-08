--
-- ========================================================================================================================================================================
--  Configure scope Memebers
-- ========================================================================================================================================================================
--
delete peer1..scope_info
insert into peer1..scope_info(scope_name) values ('sales')

delete peer2..scope_info
insert into peer2..scope_info(scope_name) values ('sales')

delete peer3..scope_info
insert into peer3..scope_info(scope_name) values ('sales')
go
