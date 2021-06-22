module.exports = class Connection {
    constructor() {
        this.socket;
        this.player;
        this.server;
        this.lobby;
    }

    createEvents() {
        let connection = this;
        let socket = connection.socket;
        let server = connection.server;
        let player = connection.player;

        socket.on('disconnect', function() {
            server.onDisconnected(connection);
        });

        socket.on('joinGame', function(data) {
            connection.player.username = data.username;
            server.onAttemptToJoinGame(connection);
        });

        socket.on('fireBullet', function(data) {
            connection.lobby.onFireBullet(connection, data);
        });

        socket.on('collisionDestroy', function(data) {
            connection.lobby.onCollisionDestroy(connection, data);
        });

        socket.on('updatePositionPlayer', function(data) {
            player.position.x = data.position.x;
            player.position.y = data.position.y;
            player.rotation = data.rotation.z;
            socket.broadcast.to(connection.lobby.id).emit('updatePositionPlayer', player);
        });

        socket.on('respawnPlayer', function() {
            player.playerReset();
            //server.onAttemptToJoinGame(connection);
            var returnData = {
                id: connection.player.id,
                username: connection.player.username,
                position: {
                    x: connection.player.position.x,
                    y: connection.player.position.y
                }
            }
            console.log("send respawning request.");
            socket.emit('respawnPlayer', returnData); //tell myself I have spawned
            socket.broadcast.to(connection.lobby.id).emit('respawnPlayer', returnData);
        });

        socket.on('leaveGame', function() {
            player.playerReset();
            socket.broadcast.to(connection.player.lobby).emit('disconnected', {
                id: player.id
            });     
            server.onSwitchLobby(connection, 0);
        });
    }


}