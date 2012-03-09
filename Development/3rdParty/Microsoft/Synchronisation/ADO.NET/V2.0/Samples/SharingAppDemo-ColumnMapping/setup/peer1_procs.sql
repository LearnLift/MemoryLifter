--
-- ========================================================================================================================================================================
--  PEER 1 - Stored Procs
-- ========================================================================================================================================================================
--

use peer1
go

if object_id(N'dbo.sp_ordertable_selectchanges', 'P') is not null
	drop procedure dbo.sp_ordertable_selectchanges

if object_id(N'dbo.sp_order_details_selectchanges', 'P') is not null
	drop procedure dbo.sp_order_details_selectchanges

if object_id(N'dbo.sp_ordertable_applyinsert', 'P') is not null
	drop procedure dbo.sp_ordertable_applyinsert

if object_id(N'dbo.sp_ordertable_insertmetadata', 'P') is not null
	drop procedure dbo.sp_ordertable_insertmetadata

if object_id(N'dbo.sp_order_details_applyinsert', 'P') is not null
	drop procedure dbo.sp_order_details_applyinsert

if object_id(N'dbo.sp_order_details_insertmetadata', 'P') is not null
	drop procedure dbo.sp_order_details_insertmetadata

if object_id(N'dbo.sp_ordertable_applyupdate', 'P') is not null
	drop procedure dbo.sp_ordertable_applyupdate

if object_id(N'dbo.sp_ordertable_updatemetadata', 'P') is not null
	drop procedure dbo.sp_ordertable_updatemetadata

if object_id(N'dbo.sp_order_details_applyupdate', 'P') is not null
	drop procedure dbo.sp_order_details_applyupdate

if object_id(N'dbo.sp_order_details_updatemetadata', 'P') is not null
	drop procedure dbo.sp_order_details_updatemetadata

if object_id(N'dbo.sp_ordertable_applydelete', 'P') is not null
	drop procedure dbo.sp_ordertable_applydelete

if object_id(N'dbo.sp_ordertable_deletemetadata', 'P') is not null
	drop procedure dbo.sp_ordertable_deletemetadata

if object_id(N'dbo.sp_order_details_applydelete', 'P') is not null
	drop procedure dbo.sp_order_details_applydelete

if object_id(N'dbo.sp_order_details_deletemetadata', 'P') is not null
	drop procedure dbo.sp_order_details_deletemetadata

if object_id(N'dbo.sp_ordertable_selectrow', 'P') is not null
	drop procedure dbo.sp_ordertable_selectrow

if object_id(N'dbo.sp_order_details_selectrow', 'P') is not null
	drop procedure dbo.sp_order_details_selectrow

if object_id(N'dbo.sp_ordertable_selecttombstones', 'P') is not null
	drop procedure dbo.sp_ordertable_selecttombstones

if object_id(N'dbo.sp_order_details_selecttombstones', 'P') is not null
	drop procedure dbo.sp_order_details_selecttombstones
go

--
--  ********************************************************************
--     Select Incremental Changes Procs for ordertable and order_details
--  ********************************************************************
--

create procedure dbo.sp_ordertable_selectchanges (				
		@sync_min_timestamp bigint,		
		@sync_metadata_only int)
as    
    select  t.orderid, 
            o.orderdate, 	                
            t.sync_row_is_tombstone, 
			t.sync_row_timestamp, 	                       
            t.sync_update_peer_key, 
            t.sync_update_peer_timestamp, 
            t.sync_create_peer_key, 
            t.sync_create_peer_timestamp 
    from ordertable o right join ordertable_tracking t on o.orderid = t.orderid
    where t.sync_row_timestamp > @sync_min_timestamp		
    order by t.orderid asc
go
  
create procedure dbo.sp_order_details_selectchanges (				
		@sync_min_timestamp bigint,		
		@sync_metadata_only int)		
as    
    select  t.order_id, 
            o.order_details_id, 
            o.product, 
            o.quantity,              
            t.sync_row_is_tombstone,
			t.sync_row_timestamp,
            t.sync_update_peer_key, 
            t.sync_update_peer_timestamp, 
            t.sync_create_peer_key, 
            t.sync_create_peer_timestamp 
    from order_details o right join order_details_tracking t on o.order_id = t.order_id 
    where t.sync_row_timestamp > @sync_min_timestamp		
    order by t.order_id asc
go


--
--  ***********************************************
--     Insert Procs for ordertable and order_details
--  ***********************************************
--

create procedure dbo.sp_ordertable_applyinsert (						
        @orderid int,
        @orderdate datetime,
		@sync_row_count int out)        
as
	insert into [ordertable] ([orderid], [orderdate]) 
	values (@orderid, @orderdate)
	set @sync_row_count = @@rowcount
go

create procedure dbo.sp_ordertable_insertmetadata (		
		@orderid int,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp bigint,
		@sync_row_is_tombstone int,		
		@sync_row_count int out)        
as
	-- insert metadta only		
	insert into ordertable_tracking ([orderid], [sync_update_peer_key], [sync_update_peer_timestamp], [sync_create_peer_key], [sync_create_peer_timestamp], [sync_row_is_tombstone])
	values (@orderid, @sync_update_peer_key, @sync_update_peer_timestamp, @sync_create_peer_key, @sync_create_peer_timestamp, @sync_row_is_tombstone)        			
	set @sync_row_count = @@rowcount           
go


create procedure dbo.sp_order_details_applyinsert (				
        @order_id int ,
        @order_details_id int,
        @product varchar(100),
        @quantity int,
		@sync_row_count int out)        
as	
	insert into [order_details] ([order_id], [order_details_id], [product], [quantity]) 
		values (@order_id, @order_details_id, @product, @quantity)
	set @sync_row_count = @@rowcount	
go

create procedure dbo.sp_order_details_insertmetadata (		
		@order_id int ,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp bigint,
		@sync_row_is_tombstone int,	
		@sync_row_count int out)        
as
	-- insert metadta only		
	insert into order_details_tracking ([order_id], [sync_update_peer_key], [sync_update_peer_timestamp], [sync_create_peer_key], [sync_create_peer_timestamp], [sync_row_is_tombstone])
	values (@order_id, @sync_update_peer_key, @sync_update_peer_timestamp, @sync_create_peer_key, @sync_create_peer_timestamp, @sync_row_is_tombstone)        			
	set @sync_row_count = @@rowcount           
go



--
--  ***********************************************
--     Update Procs for ordertable and order_details
--  ***********************************************
--

create procedure dbo.sp_ordertable_applyupdate (									
        @orderid int ,
        @orderdate datetime,
		@sync_min_timestamp bigint , 								
		@sync_force_write int ,
		@sync_row_count int out)        
as		
	update o
	set o.orderdate = @orderdate        
	from [ordertable] o join [ordertable_tracking] t on o.orderid = t.orderid
	where (t.sync_row_timestamp <= @sync_min_timestamp or @sync_force_write = 1)
		and t.orderid = @orderid  
	set @sync_row_count = @@rowcount                   		
go

create procedure dbo.sp_ordertable_updatemetadata (
		@orderid int,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,					
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp timestamp,						
		@sync_row_timestamp timestamp,
		@sync_check_concurrency int,
		@sync_row_count int out)        
as			
	-- update metadta only
	update ordertable_tracking set
		[sync_create_peer_key] = @sync_create_peer_key, 
		[sync_create_peer_timestamp] =  @sync_create_peer_timestamp,
		[sync_update_peer_key] = @sync_update_peer_key, 
		[sync_update_peer_timestamp] =  @sync_update_peer_timestamp 
	where orderid = @orderid and (@sync_check_concurrency = 0 or sync_row_timestamp = @sync_row_timestamp)
	set @sync_row_count = @@rowcount   
go

create procedure dbo.sp_order_details_applyupdate (											
        @order_id int,
        @order_details_id int ,
        @product varchar(100),
        @quantity int,
		@sync_min_timestamp bigint ,
		@sync_force_write int ,
		@sync_row_count int out )        
as    	
	update o
	set o.order_details_id = @order_details_id, 
		o.product = @product,
		o.quantity = @quantity	        	     
	from [order_details] o join [order_details_tracking] t on o.order_id = t.order_id
	where (t.sync_row_timestamp <= @sync_min_timestamp or @sync_force_write = 1)  
		and t.order_id = @order_id			
	set @sync_row_count = @@rowcount
        	
go

create procedure dbo.sp_order_details_updatemetadata (
		@order_id int,
		@sync_create_peer_key int ,
		@sync_create_peer_timestamp bigint,					
		@sync_update_peer_key int ,
		@sync_update_peer_timestamp timestamp,						
		@sync_row_timestamp timestamp,
		@sync_check_concurrency int,
		@sync_row_count int out)        
as			
	-- update metadta only
	update order_details_tracking set
		[sync_create_peer_key] = @sync_create_peer_key, 
		[sync_create_peer_timestamp] =  @sync_create_peer_timestamp, 
		[sync_update_peer_key] = @sync_update_peer_key, 
		[sync_update_peer_timestamp] =  @sync_update_peer_timestamp 
	where order_id = @order_id and (@sync_check_concurrency =0 or sync_row_timestamp = @sync_row_timestamp)
	set @sync_row_count = @@rowcount   
go


--
--  ***********************************************
--     Delete Procs for ordertable and order_details
--  ***********************************************
--

create procedure dbo.sp_ordertable_applydelete(
	@orderid int ,	
	@sync_min_timestamp bigint , 	     	
	@sync_force_write int ,	     	
	@sync_row_count int out)	 
as  
	delete o
	from [ordertable] o join [ordertable_tracking] t on o.orderid = t.orderid
	where (t.sync_row_timestamp <= @sync_min_timestamp or @sync_force_write = 1)
		and t.orderid = @orderid            
	set @sync_row_count = @@rowcount              
go

create procedure dbo.sp_ordertable_deletemetadata(
    @orderid int ,			
	@sync_row_timestamp timestamp,	
	@sync_check_concurrency int,	
	@sync_row_count int out) 	
as    
	-- delete metadata only
	delete t
	from [ordertable_tracking] t 
	where t.orderid = @orderid and (@sync_check_concurrency = 0 or t.sync_row_timestamp = @sync_row_timestamp)
	set @sync_row_count = @@rowcount           	
go

create procedure dbo.sp_order_details_applydelete(
	@order_id int ,
	@sync_min_timestamp bigint ,
	@sync_force_write int ,			
	@sync_row_count int out)	    
as	
	delete o
	from [order_details] o join [ordertable_details_tracking] t on o.order_id = t.sync_order_id
	where (t.sync_row_timestamp <= @sync_min_timestamp or @sync_force_write = 1)
		and t.order_id = @order_id            
	set @sync_row_count = @@rowcount              
go

create procedure dbo.sp_order_details_deletemetadata(	
    @order_id int ,			
	@sync_row_timestamp timestamp,	
    @sync_check_concurrency int,	
	@sync_row_count int out) 	
as    
	-- delete metadata only
	delete t
	from [order_details_tracking] t 
	where t.order_id = @order_id and (@sync_check_concurrency = 0 or t.sync_row_timestamp = @sync_row_timestamp)
	set @sync_row_count = @@rowcount           	
go

--
--  ***********************************************
--     Get conflicting row procs
--  ***********************************************
--

create procedure dbo.sp_ordertable_selectrow
        @orderid int
as

	select t.orderid, 
	       o.orderdate, 	    	       
		   t.sync_row_timestamp, 
	       t.sync_row_is_tombstone,
	       t.sync_update_peer_key, 
	       t.sync_update_peer_timestamp, 
	       t.sync_create_peer_key, 
	       t.sync_create_peer_timestamp 
	from ordertable o right join ordertable_tracking t on o.orderid = t.orderid 
	where t.orderid = @orderid 
go

create procedure dbo.sp_order_details_selectrow
        @order_id int
as

	select t.order_id, 
	       o.order_details_id, 
	       o.product, 
	       o.quantity,	       
	       t.sync_row_is_tombstone,
	       t.sync_row_timestamp,
	       t.sync_update_peer_key, 
	       t.sync_update_peer_timestamp, 
	       t.sync_create_peer_key, 
	       t.sync_create_peer_timestamp 
	from order_details o right join order_details_tracking t on o.order_id = t.order_id 
	where t.order_id = @order_id 
go


--
--  ***********************************************
--     Get tombstones for cleanup commands
--  ***********************************************
--

create procedure dbo.sp_ordertable_selecttombstones     
	@tombstone_aging_in_hours int
as
	select orderid,
		   sync_row_timestamp, 	       
	       sync_update_peer_key, 
	       sync_update_peer_timestamp, 
	       sync_create_peer_key, 
	       sync_create_peer_timestamp 
	from ordertable_tracking
	where sync_row_is_tombstone > 0 
go

create procedure dbo.sp_order_details_selecttombstones  
	@tombstone_aging_in_hours int   
as
	select order_id,
		   sync_row_timestamp, 	       
	       sync_update_peer_key, 
	       sync_update_peer_timestamp, 
	       sync_create_peer_key, 
	       sync_create_peer_timestamp 
	from order_details_tracking
	where sync_row_is_tombstone > 0 
go
