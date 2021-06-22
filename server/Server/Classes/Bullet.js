var ServerObject = require('./ServerObject.js');
var Vector2D = require('./Vector2D.js');

module.exports = class Bullet extends ServerObject {
    constructor () {
        super();
        this.direction = new Vector2D();
        this.speed = 1.0;
        this.isDestroyed = false;
        this.activator = '';
    }

    onUpdate() {

        this.position.x += this.direction.x * this.speed;
        this.position.y += this.direction.y * this.speed;

        return this.isDestroyed;
    }


}