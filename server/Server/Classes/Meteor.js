var ServerObject = require('./ServerObject.js');
var Vector2D = require('./Vector2D.js');

module.exports = class Meteor extends ServerObject {
    constructor () {
        super();
        this.isDestroyed = false;
    }
}