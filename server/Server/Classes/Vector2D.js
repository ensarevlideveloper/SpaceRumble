module.exports = class Vector2D {
    constructor(X = 0, Y = 0) {
        this.x = X;
        this.y = Y;
    }

    Magnitude() {
        return Math.sqrt((this.x * this.x) + (this.y * this.y));
    }

    Normalized () {
        var mag = this.Magnitude();
        return new Vector2D(this.x / mag, this.y / mag);
    }

    Distance(OtherVector = Vector2D) {
        var direction = new Vector2D();
        direction.x = OtherVector.x - this.x;
        direction.y = OtherVector.y - this.y;
        return direction.Magnitude();
    }

    ConsoleOutput() {
        return "(" + this.x + "," + this.y + ")";
    }
}