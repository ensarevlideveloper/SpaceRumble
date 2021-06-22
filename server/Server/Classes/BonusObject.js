var ServerObject = require('./ServerObject.js');
var Vector2D = require('./Vector2D.js');

module.exports = class BonusObject extends ServerObject {
    constructor () {
        super();
        this.isDestroyed = false;
    }

    onUpdate() {

        this.position.x += this.direction.x * this.speed;
        this.position.y += this.direction.y * this.speed;

        return this.isDestroyed;
    }
}