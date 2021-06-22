var LobbyBase = require('./LobbyBase');
var GameLobbySettings = require('./GameLobbySettings');
var Connection = require('../Connection');
var Bullet = require('../Bullet');
var Meteor = require('../Meteor.js');
var Vector2D = require('../Vector2D.js');

//Count of meteors ingame
var meteorAmount = 100;

//Paramaters for score
var killScore = new Number(10);
var meteorScore = new Number(1);

//GameField paramater
var gameFieldWidth = 100;
var gameFieldHeight = 100;



module.exports = class GameLobbby extends LobbyBase {
    constructor(id, settings = GameLobbySettings) {
        super(id);
        this.settings = settings;
        this.bullets = [];
        this.meteors = [];
        this.spawnMeteor(meteorAmount);
    }

    onUpdate() {

        this.updateBullets();

    }

    canEnterLobby(connection = Connection) {
        let maxPlayerCount = this.settings.maxPlayers;
        let currentPlayerCount = this.connections.length;

        if(currentPlayerCount + 1 > maxPlayerCount) {
            return false;
        }

        return true;
    }

    onEnterLobby(connection = Connection) {

        super.onEnterLobby(connection);

        this.addPlayer(connection);

        //Handle spawning any server spawned objects here
        //Example: loot, perhaps flying bullets etc
    }

    onLeaveLobby(connection = Connection) {

        super.onLeaveLobby(connection);

        this.removePlayer(connection);

        //Handle unspawning any server spawned objects here
        //Example: loot, perhaps flying bullets etc
    }

    findSpawnablePosition () {
        let spawnPosition = new Vector2D();
        spawnPosition.x = Math.floor(Math.random() * 50);
        spawnPosition.x*= Math.floor(Math.random()*2) == 1 ? 1 : -1;
        spawnPosition.y = Math.floor(Math.random() * 50);
        spawnPosition.y*= Math.floor(Math.random()*2) == 1 ? 1 : -1;
    
        while (this.isPlaceBusy(spawnPosition) == false) {
            spawnPosition.x = Math.floor(Math.random() * 50);
            spawnPosition.x*= Math.floor(Math.random()*2) == 1 ? 1 : -1;
            spawnPosition.y = Math.floor(Math.random() * 50);
            spawnPosition.y*= Math.floor(Math.random()*2) == 1 ? 1 : -1;
        }
        return spawnPosition;
    }

    isPlaceBusy (position) {
        let meteors = this.meteors;
        let connections = this.connections;


        for(var m in meteors) {
            let meteor = meteors[m];
            if (meteor.position == position) {
                return false;
            }
        }
        for(var c in connections) {
            let connection = connections[c];
            if(connection.player.position == position) {
                return false;
            }
        }
        return true;
    }

    updateBullets() {
        let bullets = this.bullets;
        let connections = this.connections;

        for(var b in bullets) {
            let bullet = bullets[b];
            let isDestroyed = bullet.onUpdate();

            if(isDestroyed) {
                this.despawnBullet(bullet);
            } else {
                var returnData = {
                    id: bullet.id,
                    position: {
                        x: bullet.position.x,
                        y: bullet.position.y
                    }
                }

                for(var c in connections) {
                    let connection = connections[c];
                    connection.socket.emit('updatePosition', returnData);
                }
            }
        }
    }


    onFireBullet(connection = Connection, data) {

        //Create a new bullet with @param data
        let bullet = new Bullet();
        bullet.name = 'Bullet';
        bullet.activator = data.activator;
        bullet.position.x = data.position.x;
        bullet.position.y = data.position.y;
        bullet.direction.x = data.direction.x;
        bullet.direction.y = data.direction.y;

        //Since all bullets are equal its uninteresting on which index each bullet is
        this.bullets.push(bullet);

        var returnData = {
            name: bullet.name,
            id: bullet.id,
            activator: bullet.activator,
            position: {
                x: bullet.position.x,
                y: bullet.position.y
            },
            direction: {
                x: bullet.direction.x,
                y: bullet.direction.y
            },
            speed: bullet.speed
        }

        connection.socket.emit('serverSpawn', returnData);
        //Only broadcast to those in the same lobby as us
        connection.socket.broadcast.to(this.id).emit('serverSpawn', returnData);
    }

    onCollisionDestroy(connection = Connection, data) {
        let meteors = this.meteors;
        let connections = this.connections;

        //In this case I won't change lambda typing...
        let returnBullets = this.bullets.filter(bullet => {
            return bullet.id == data.id
        });

        for(var rB in returnBullets) {
            let bullet = returnBullets[rB];
            let playerHit = false;
            let meteorHit = false;
            let activator = connections[bullet.activator].player;


            //Idea: Dont loop over each player and each meteor. If bullet hit meteor, can't hit player anymore ?!
            for (var c in connections) {
                let connection = connections[c];
                let player = connection.player;

                if(bullet.activator != player.id) {

                    let distance = bullet.position.Distance(player.position);

                    if(distance < 2) {
                        playerHit = true;
                        let isDead = player.doDamage(activator.damage);
                        let returnData = {
                            id: player.id,
                            health: player.health
                        }
                        connections[player.id].socket.emit('playerTookDamage', returnData);

                        if(isDead) {
                            /**
                             * Activator is the player who shot the bullet.
                             * He gets 10 points if the player who got shot dies.
                             */
                            activator.setScore(activator.getScore() + killScore);
                            let scoreData = {
                                id: activator.id,
                                score: activator.getScore()
                            }
                            connection.socket.emit('updatePlayerScore', scoreData);
                            connection.socket.broadcast.to(this.id).emit('updatePlayerScore', scoreData);

                            /**
                             * Player died so send this information to all.
                             */
                            console.log('Player with id: ' + player.id + ' has died');
                            connection.socket.emit('playerDied', returnData);
                            connection.socket.broadcast.to(this.id).emit('playerDied', returnData);

                            let respawnData = {
                                id: player.id,
                                rank: this.getRank(connections[player.id])
                            }
                            connections[player.id].socket.emit('playerRank', respawnData);

                        } else {
                            console.log('Player with id: ' + player.id + ' has (' + player.health + ') health left');
                        }
                        this.despawnBullet(bullet);
                    }
                }
            }


            for(var m in meteors) {
                let meteor = meteors[m];
                let activator = connections[bullet.activator].player;
                let distance = bullet.position.Distance(meteor.position);

                if(distance < 1.5) {
                    meteorHit = true;
                    activator.setScore(activator.getScore() + meteorScore);
                    let scoreData = {
                        id: activator.id,
                        score: activator.getScore()
                    }
                    connection.socket.emit('updatePlayerScore', scoreData);
                    connection.socket.broadcast.to(this.id).emit('updatePlayerScore', scoreData);
                    this.despawnBullet(bullet);
                    this.despawnMeteor(meteor);
                }
            }


            if(!playerHit || !meteorHit) {
                bullet.isDestroyed = true;
            }


        }    
    }

    despawnBullet(bullet = Bullet) {
        let bullets = this.bullets;
        let connections = this.connections;

        console.log('Destroying bullet (' + bullet.id + ')');
        var index = bullets.indexOf(bullet);
        if(index > -1) {
            console.log('Found bullet (' + bullet.id + ')');

            bullets.splice(index, 1);

            var returnData = {
                id: bullet.id
            }

            //Send remove bullet command to players
            for(var c in connections) {
                let connection = connections[c];
                connection.socket.emit('serverUnspawn', returnData);
                console.log('Destroying bullet (' + bullet.id + ')');

            }
        }
    }

    despawnMeteor(meteor = Meteor) {
        let meteors = this.meteors;
        let connections = this.connections;

        console.log("Destroying Meteor (" + meteor.id + ")");
        var index = meteors.indexOf(meteor);
        if (index > -1) {
            meteors.splice(index, 1);
    
            var returnData = {
                id: meteor.id
            }
    
            //Send remove bullet command to players
            for(var c in connections) {
                let connection = connections[c];
                connection.socket.emit('serverUnspawn', returnData);
                console.log('Destroying meteor (' + meteor.id + ')');

            }
            
            this.spawnMeteor (meteorAmount - meteors.length);
        }
    }

    spawnMeteor (amount) {
        let meteors= this.meteors;
        let connections = this.connections;

        console.log("spawn meteor method");
        for(var i = 0; i < amount; i++) {
            var meteor = new Meteor();
            meteor.name = "Meteor";
            meteor.position = this.findSpawnablePosition();
            meteors.push(meteor);
    
            var returnData = {
                name: meteor.name,
                id: meteor.id,
                position: {
                    x: meteor.position.x,
                    y: meteor.position.y
                }
            }

            for(var c in connections) {
                let connection = connections[c];
                connection.socket.emit('serverSpawn', returnData);
            }
            
    
        }

        console.log("meteors spawned");
    }

    addPlayer(connection = Connection) {
        let connections = this.connections;
        let socket = connection.socket;
        let meteors = this.meteors;
        let spawningPosition = this.findSpawnablePosition();

        connection.player.position = spawningPosition;


        var returnData = {
            id: connection.player.id,
            username: connection.player.username,
            position: {
                x: connection.player.position.x,
                y: connection.player.position.y
            }
        }

        socket.emit('spawn', returnData); //tell myself I have spawned
        socket.broadcast.to(this.id).emit('spawn', returnData); // Tell others

        //Tell myself about everyone else already in the lobby
        for(var oC in connections) {
            let otherConnection = connections[oC];
            if(otherConnection.player.id != connection.player.id) {
                socket.emit('spawn', {
                    id: otherConnection.player.id,
                    username: otherConnection.player.username,
                    position: {
                        x: otherConnection.player.position.x,
                        y: otherConnection.player.position.y
                    }
                });
            }
        }


        for(var m in meteors) {
            let meteor = meteors[m];
            var returnData = {
                name: meteor.name,
                id: meteor.id,
                position: {
                    x: meteor.position.x,
                    y: meteor.position.y
                }
            }
                socket.emit('serverSpawn', returnData);
        }     
    }

    removePlayer(connection = Connection) {
        connection.socket.broadcast.to(this.id).emit('disconnected', {
            id: connection.player.id
        });
    }

    getRank (connection = Connection) {
        let connections = this.connections;
        let player = connection.player;
        let playerScore = player.score;
        let playersCount = connections.length + 1;
        let rank = playersCount;
        for (var oC in connections) {
            if (connections[oC] == connection) {

            }
            else {
                let otherConnection = connections[oC];
                let otherPlayer = otherConnection.player;
                let otherPlayerScore = otherPlayer.score;
                if (playerScore >= otherPlayerScore) {
                    rank = rank - 1;
                }
            }
        }
        rank = rank + 1; //Because we forgot ourselves.
        return rank;
    }
}