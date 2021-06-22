var io = require('socket.io')(process.env.PORT || 5000);
var Server = require('./Classes/Server');


let server = new Server ();

setInterval(() => {
    server.onUpdate();
}, 100, 0);

io.on('connection', function(socket) {
    let connection = server.onConnected(socket);
    connection.createEvents();
    socket.emit('register', {id: connection.player.id});
});

function interval (func, wait, times) {
    var interv = function(w, t) {
        return function() {
            if(typeof t == "undefined" || t-- > 0) {
                setTimeout(interv, w);
                try {
                    func.call(null);
                }
                catch(e) {
                    t = 0;
                    throw e.toString();
                }
            }
        };
    }(wait, times);

    setTimeout(interv, wait);
    
}