--
-- ========================================================================================================================================================================
--  Cleanup content and metadata
-- ========================================================================================================================================================================
--
delete peer1..scope_info
delete peer1..orders
delete peer1..orders_tracking
delete peer1..order_details
delete peer1..order_details_tracking

delete peer2..scope_info
delete peer2..orders
delete peer2..orders_tracking
delete peer2..order_details
delete peer2..order_details_tracking


delete peer3..scope_info
delete peer3..orders
delete peer3..orders_tracking
delete peer3..order_details
delete peer3..order_details_tracking

go

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


