let Connection = require('./Connection');
let Player = require('./Player');

//Lobbies
let LobbyBase = require('./Lobby/LobbyBase')
let GameLobby = require('./Lobby/GameLobby')
let GameLobbySettings = require('./Lobby/GameLobbySettings')


module.exports = class Server {
    constructor() {
        this.connections = [];
        this.lobbys = [];

        this.lobbys[0] = new LobbyBase(0);


    }

    //Interval update every 100 milliseconds
    onUpdate() {
        let server = this;

        //Update also each lobby
        for (let i in server.lobbys) {
            server.lobbys[i].onUpdate();
        }
    }


    //Handle a new connection to the server
    onConnected(socket) {
        let server = this;

        /**
         * Create a new Connection for the client.
         * Pass information to the connection:
         * socket of Client
         * server of Client
         * player of Client
         */
        let connection = new Connection();
        connection.socket = socket;
        connection.player = new Player();
        connection.server = server;

        let player = connection.player;
        let lobbys = server.lobbys;

        /**
         * Save Connection of Client in connection array.
         * Saved under the id of the client.
         */
        console.log('Added new player to the server (' + player.id + ')');
        server.connections[player.id] = connection;


        socket.join(player.lobby);
        connection.lobby = lobbys[player.lobby];
        connection.lobby.onEnterLobby(connection);

        return connection;
    }

    onDisconnected(connection = Connection) {
        let server = this;
        let id = connection.player.id;

        delete server.connections[id];
        console.log('Player ' + connection.player.displayerPlayerInformation() + ' has disconnected');

        //Tell Other players currently in the lobby that we have disconnected from the game
        connection.socket.broadcast.to(connection.player.lobby).emit('disconnected', {
            id: id
        });

        //Perform lobby clean up
        server.lobbys[connection.player.lobby].onLeaveLobby(connection);
    }

    onAttemptToJoinGame (connection = Connection) {
        //Look through lobbies for a gamelobby
        //check if joinable
        //if not make a new game
        let server = this;
        let lobbyFound = false;

        let gameLobbies = server.lobbys.filter(item => {
            return item instanceof GameLobby;
        });
        console.log('Found (' + gameLobbies.length + ') lobbies on the server');

        gameLobbies.forEach(lobby => {
            if(!lobbyFound) {
                let canJoin = lobby.canEnterLobby(connection);

                if(canJoin) {
                    lobbyFound = true;
                    server.onSwitchLobby(connection, lobby.id);
                }
            }
        });

        //All game lobbies full or we have never created one
        if(!lobbyFound) {
            console.log('Making a new game lobby');
            let gamelobby = new GameLobby(gameLobbies.length + 1, new GameLobbySettings('FFA', 100));
            server.lobbys.push(gamelobby);
            server.onSwitchLobby(connection, gamelobby.id);
        }
    }

    onSwitchLobby(connection = Connection, lobbyID) {
        let server = this;
        let lobbys = server.lobbys;

        connection.socket.join(lobbyID); // Join the new lobby's socket channel
        connection.lobby = lobbys[lobbyID];//assign reference to the new lobby

        lobbys[connection.player.lobby].onLeaveLobby(connection);
        lobbys[lobbyID].onEnterLobby(connection);
    }


}