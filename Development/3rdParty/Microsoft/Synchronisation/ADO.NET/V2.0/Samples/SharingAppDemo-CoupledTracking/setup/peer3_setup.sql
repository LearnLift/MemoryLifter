--
-- ========================================================================================================================================================================
--  PEER 3 - Setup
-- ========================================================================================================================================================================
--


--
-- Create fresh new database called "peer3"
--
drop database peer3
go
create database peer3
go

ALTER DATABASE peer3 SET ALLOW_SNAPSHOT_ISOLATION ON

CREATE TABLE peer3..orders(order_id int NOT NULL primary key, order_date datetime NULL)
go

CREATE TABLE peer3..order_details(order_id int NOT NULL primary key, order_details_id int NOT NULL, product nvarchar(100) NULL, quantity int NULL)
go


--
-- Create scope info table
--
CREATE TABLE peer3..scope_info(    	
    scope_id uniqueidentifier default NEWID(), 	
    scope_name nvarchar(100) NULL,
    scope_sync_knowledge varbinary(max) NULL,
	scope_tombstone_cleanup_knowledge varbinary(max) NULL,
	scope_timestamp timestamp)
go


--
-- Create tombstone tables
--
CREATE TABLE peer3..orders_tombstone(
    order_id int NOT NULL primary key,              
    sync_row_timestamp timestamp, 
    sync_update_peer_key int default 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int default 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime default GetDate())


CREATE TABLE peer3..order_details_tombstone(
    order_id int NOT NULL primary key,             
    sync_row_timestamp timestamp, 
    sync_update_peer_key int default 0,
    sync_update_peer_timestamp bigint,        
    sync_create_peer_key int default 0,
    sync_create_peer_timestamp bigint,
	last_change_datetime datetime default GetDate())
go
   
--
-- Add tracking columns inline with the data table
--   
ALTER TABLE peer3..orders add sync_row_timestamp timestamp
ALTER TABLE peer3..order_details add sync_row_timestamp timestamp
go

ALTER TABLE peer3..orders add sync_update_peer_key int default 0
ALTER TABLE peer3..order_details add sync_update_peer_key int default 0
go

ALTER TABLE peer3..orders add sync_update_peer_timestamp bigint 
ALTER TABLE peer3..order_details add sync_update_peer_timestamp bigint 
go

ALTER TABLE peer3..orders add sync_create_peer_key int default 0
ALTER TABLE peer3..order_details add sync_create_peer_key int default 0
go

ALTER TABLE peer3..orders add sync_create_peer_timestamp bigint 
ALTER TABLE peer3..order_details add sync_create_peer_timestamp bigint 
go


-- 
-- Create Triggers
--

-- insert triggers
use peer3
go
CREATE TRIGGER orders_insert_trigger on orders for insert
as    
    update o    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,		
	    sync_create_peer_key = 0,
	    sync_create_peer_timestamp = @@DBTS+1
	from orders o join inserted i on o.order_id = i.order_id     	
go

CREATE TRIGGER order_details_insert_trigger on order_details for insert
as
    update o    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1,		
	    sync_create_peer_key = 0,
	    sync_create_peer_timestamp = @@DBTS+1
	from order_details o join inserted i on o.order_id = i.order_id     	
go


-- update triggers
use peer3
go
CREATE TRIGGER orders_update_trigger on orders for update
as    
    update o    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1	    
	from orders o join inserted i on o.order_id = i.order_id   
	where not UPDATE(sync_update_peer_timestamp)  	
go

CREATE TRIGGER order_details_update_trigger on order_details for update
as
    update o    
	set sync_update_peer_key = 0, 
		sync_update_peer_timestamp = @@DBTS+1	    
	from order_details o join inserted i on o.order_id = i.order_id  
	where not UPDATE(sync_update_peer_timestamp)   	
go


-- delete triggers
use peer3
go
CREATE TRIGGER orders_delete_trigger on orders for delete
as
    insert into orders_tombstone(order_id, sync_update_peer_key, sync_update_peer_timestamp, sync_create_peer_key, sync_create_peer_timestamp) 
	select order_id, 0, @@DBTS+1, 0, @@DBTS+1
	from deleted	
go	

CREATE TRIGGER order_details_delete_trigger on order_details for delete
as
    insert into order_details_tombstone(order_id, sync_update_peer_key, sync_update_peer_timestamp, sync_create_peer_key, sync_create_peer_timestamp) 
	select order_id, 0, @@DBTS+1, 0, @@DBTS+1
	from deleted	
go	


