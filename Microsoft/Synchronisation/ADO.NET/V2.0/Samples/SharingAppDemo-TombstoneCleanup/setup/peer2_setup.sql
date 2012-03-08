--
-- ========================================================================================================================================================================
--  PEER 2 - Setup
-- ========================================================================================================================================================================
--


--
-- Create fresh new database called "peer2"
--
drop database peer2
go
create database peer2
go

ALTER DATABASE peer2 SET ALLOW_SNAPSHOT_ISOLATION ON

CREATE TABLE peer2..orders(order_id int NOT NULL primary key, order_date datetime NULL)
go

CREATE TABLE peer2..order_details(order_id int NOT NULL primary key, order_details_id int NOT NULL, product nvarchar(100) NULL, quantity int NULL)
go


--
-- Create scope info table
--
CREATE TABLE peer2..scope_info(    	
    scope_id uniqueidentifier default NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)
go


--
-- Create tracking tables
--
CREATE TABLE peer2..orders_tracking(
    order_id int NOT NULL primary key,          
    sync_row_is_tombstone int default 0,
    sync_row_timestamp timestamp, 
    sync_update_peer_key int default 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int default 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime default GetDate())


CREATE TABLE peer2..order_details_tracking(
    order_id int NOT NULL primary key,         
    sync_row_is_tombstone int default 0,
    sync_row_timestamp timestamp, 
    sync_update_peer_key int default 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int default 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime default GetDate())
go
     

-- 
-- Create Triggers
--

-- insert triggers
use peer2
go
CREATE TRIGGER orders_insert_trigger on orders for insert
as    
	insert into orders_tracking(order_id, sync_update_peer_key, sync_update_peer_timestamp, sync_create_peer_key, sync_create_peer_timestamp) 
	select order_id, 0, @@DBTS+1, 0, @@DBTS+1
	from inserted		
go

CREATE TRIGGER order_details_insert_trigger on order_details for insert
as
    insert into order_details_tracking(order_id, sync_update_peer_key, sync_update_peer_timestamp, sync_create_peer_key, sync_create_peer_timestamp) 
	select order_id, 0, @@DBTS+1, 0, @@DBTS+1
	from inserted		
go


-- update triggers
use peer2
go
CREATE TRIGGER orders_update_trigger on orders for update
as    
    update t    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,
	    last_change_datetime = GetDate()
	from orders_tracking t join inserted i on t.order_id = i.order_id     	
go

CREATE TRIGGER order_details_update_trigger on order_details for update
as
    update t    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,
	    last_change_datetime = GetDate()
	from order_details_tracking t join inserted i on t.order_id = i.order_id     	
go


-- delete triggers
use peer2
go
CREATE TRIGGER orders_delete_trigger on orders for delete
as
    update t    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,
		sync_row_is_tombstone = 1,
	    last_change_datetime = GetDate()
	from orders_tracking t join deleted d on t.order_id = d.order_id     	
go	

CREATE TRIGGER order_details_delete_trigger on order_details for delete
as
	update t    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,
		sync_row_is_tombstone = 1,
	    last_change_datetime = GetDate()
	from order_details_tracking t join deleted d on t.order_id = d.order_id  
go	



