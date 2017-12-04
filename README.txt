                                    DIDA-PROJECT
                                Online Gaming Platform

Introduction
------------

This project is a simplified real-time distributed gaming platform. 
It's main components are a multi-player version of the PacMan game 
and a chat that allows communication among players involved in the
game.

Usage
-----

For starting this project first you open it with visual studio.
Then you click on the button Start to start the OGP_PacMan_Server. 
The server will ask you what game you want to play. You have to 
write PacMan then he asks the number of milliseconds per round that 
you want and finally the amount of players. After starting the
server you start as many instances of OGP_PacMan_Client as the
number of players you selected. After all the players are connected
the game will start.


Missing Features
----------------
DONE - Causal order on the clients' chat with vector clocks - Daniel
- Fault tolerance:
    DONE/NEEDS FIX - Passive Replication server-side - Diogo
    DONE - Server-side support clients death - Diogo
    DONE - Causal order vector clock handle fault tolerance - Daniel/(help will be needed)
- Connecting the Puppet Master to the client and server - Diogo/Daniel
- Finish implementing Puppet Master and PCS - Pedro
- Finish implementing ClientPuppet - Daniel
- Finish implementing ServerPuppet - Diogo


FIX:
---
DONE - Concurrent server problems (server add new client)
DONE - Input file for client from args
DONE - Input file accepting keys after reading input file
DONE - Server recieve in args the ip
DONE - Client recieve in args the ip
DONE - Remove coins concurrent problem
- Slave should try to kill master when it becomes the new master
- Try to improve master timeout
- Add roundID to server
