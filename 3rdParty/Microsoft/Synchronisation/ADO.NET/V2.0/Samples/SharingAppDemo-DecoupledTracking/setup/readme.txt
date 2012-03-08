Copyright (c) Microsoft Corporation.  All rights reserved.

SharingAppDemo Sample Application

This application demonstrate how to build peer to peer sync application using microsoft synchronization services for ADO.NET

What is demonstrated in this sample?
- Sync among three different database (3 peers)
- Use decoupled tracking model to track changes on the database
- Using TSQL\SProcs for sync adapter commands
 

How to install OfflineAppDemo application?

1- Fire SQL server and load and excute peer1_setup.sql and then peer1_procs.sql
2- Repeat step 1 for peer2 and peer3 sql scripts
3- Load demo.sql and excute the "configure scope members" section at the top of the script
4- Load VS solution (OfflineAppDemo-DecoupledTracking) 
5- Build the project 
6- You are ready to go

