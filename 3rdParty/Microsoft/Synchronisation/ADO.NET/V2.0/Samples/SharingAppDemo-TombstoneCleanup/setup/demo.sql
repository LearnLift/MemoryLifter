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
--  DML - Test Code
-- ========================================================================================================================================================================
--

insert into orders (order_id, order_date) values(1, GetDate())
insert into orders (order_id, order_date) values(4, GetDate())

update orders set order_date = GetDate() where order_id = 4

delete orders where order_id = 3

insert into order_details (order_id, order_details_id, product) values(7, 2 , 'DVD')
insert into order_details (order_id, order_details_id, product) values(3, 3 , 'CD')
insert into order_details (order_id, order_details_id, product) values(4, 2 , 'Floppy Disk')

update order_details set product = 'HDTV' where order_id =2
update order_details set order_details_id = 11 where order_id in (20,21,23)

delete peer1..orders where order_id = 1282


select @@dbts

select * from peer1..orders
select * from peer2..orders
select * from peer3..orders

select * from peer1..orders_tracking
select * from peer2..orders_tracking
select * from peer3..orders_tracking

select * from peer1..order_details
select * from peer2..order_details
select * from peer3..order_details

select * from peer1..order_details_tracking
select * from peer2..order_details_tracking
select * from peer3..order_details_tracking

select * from peer1 ..scope_info
select * from peer2 ..scope_info
select * from peer3 ..scope_info


