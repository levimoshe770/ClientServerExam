# ClientServerExam

The solution includes two executable assemblies:

1. FileServer - Creates the server applicatoin
2. Client - Creates the client application

A few words about the other assemblies:

Communicator - Handles all the TCP/IP communication, at the lower level. (70% reuse from an old project)
  Classes: 
    ICommInterface - A common interface inherited by CommClient, and CommConnection, that provides services for sending and receiving messages as packets
    CommClient - TCP/IP client class. Attempts connection to server in an inner thread until succeeded. Provides services implemented for ICommInterface
    CommServer - TCP/IP server. Opens a listener socket and waits for connections. For every connection established, creates a CommConnection class that handles the communications for every client that connects
    CommConnection - Implements ICommInteface, and handles communication between the server and a single client
    TcpReaderWriter - Implements Send and Receive methods, includes a thread that waits for messages. Used both by CommClient, and CommConnection symetrically.
    
CryptoManager - Includes one class for encoding passwords before sending them through the wire, and for storing. Uses SHA-256 hash algorithm

ControlMessages - Message types 

DataLayer - Handles all the data manipulation with the database for the server

FileTransfer - Includes one class for file transfer management for both client and server sides (symetrically)

Logger - Includes one class for writing to the audit.log file

MessageHandler - Includes classes that use reflexion in a sophisticated matter, for translating messages (as classes with properties as message fields) to buffer of bytes, and vice versa. (90% Code reuse from an old project of mine)

The server creates an instance of ClientHandler class for handling all the communication and logic with every client connected and verified. The instances are holded in a dictionary at main form class

The client creates a single serverproxy class for handling all the communication and logic with the server.

Tests:
The following tests have been made:
1. Transmission of large files (60GB), without growing in memory usage
2. Transmission with two clients simultaneously, without interference between them
3. There is no way to move to another user folder by the client UI. (There may be a way, if someone simulates our client, and injects the same message's sequence as our client does, but it's a long shot, he will have to break the password, which is transmitted encoded with SHA-256)
4. Time performance tuning 

Thanks
Moshe
