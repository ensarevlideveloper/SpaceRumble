var Connection = require('../Connection');


module.exports = class LobbyBase {

    constructor(id) {
        this.id = id;
        this.connections = [];
    }

    onUpdate() {
    }

    onEnterLobby(connection = Connection) {
        let player = connection.player;
        let connectionId = player.id;

        console.log('Player ' + player.displayerPlayerInformation() + ' has entered the lobby (' + this.id + ')');

        //Save the connection on the playerId
        this.connections[connectionId] = connection;

        player.lobby = this.id;
        connection.lobby = this;
    }

    onLeaveLobby(connection = Connection) {
        let player = connection.player;
        let connectionId = player.id;


        console.log('Player ' + player.displayerPlayerInformation() + ' has left the lobby (' + this.id + ')');

        connection.lobby = undefined;

        delete this.connections[connectionId];

    }
}