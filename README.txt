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
- Causal order on the clients' chat
- Fault tolerance
- Connecting the Puppet Master to the client and server
- Implementing some parts of the puppet master
