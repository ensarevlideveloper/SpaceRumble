var shortID = require('shortid');
var Vector2D = require('./Vector2D.js');

module.exports = class Player {
    constructor() {
        this.username = '';
        this.id = shortID.generate();
        this.lobby = 0;
        this.position = new Vector2D();
        this.rotation = 0;
        this.score = new Number(0);
        this.damage = new Number(10);
        this.health = new Number(100);
        this.isDead = false;
    }

    displayerPlayerInformation() {
        let player = this;
        return '(' + player.username + ':' + player.id + ')';
    }

    

    doDamage(amount = Number) {
        this.health -= amount;

        if (this.health <= 0) {
            this.isDead = true;
        }

        return this.isDead;
    }

    setScore (amount = Number) {
        this.score = amount;
    }

    getScore () {
        return this.score;
    }

    playerReset () {
        this.position = new Vector2D();
        this.rotation = 0;
        this.score = new Number (0);
        this.damage = new Number(10);
        this.health = new Number(100);
        this.isDead = false;
    }
}