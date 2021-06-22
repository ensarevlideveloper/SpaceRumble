var shortID = require('shortid');
var Vector2D = require('./Vector2D.js');

module.exports = class ServerObject {
    constructor() {
        this.name= 'ServerObject';
        this.id = shortID.generate();
        this.position = new Vector2D();
    }
}