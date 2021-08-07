mergeInto(LibraryManager.library, {

  initSocket: function () {
    socket = io();

    socket.on('connect', function() {
        console.log("connected to server");
    });
  },
  stopConnection: function () {
    socket.disconnect();
  },

  initListeners: function () {
    socket.on('chat', function(data) {
        myGameInstance.SendMessage("SocketManager", "msgResponse", JSON.stringify(data));
    });
    socket.on('joined', function(data) {
        myGameInstance.SendMessage("SocketManager", "joinedRoom",JSON.stringify(data));
    });
    socket.on('disconnected', function(data) {
        myGameInstance.SendMessage("SocketManager", "playerDisconnected",JSON.stringify(data));
    });
    socket.on('spawn', function(data) {
        myGameInstance.SendMessage("SocketManager", "spawnPlayer", JSON.stringify(data));
    });
    socket.on('fichaMoved', function(data) {
        myGameInstance.SendMessage("SocketManager", "fichaMoved", JSON.stringify(data));
    });
    socket.on('startGame', function(data) {
        myGameInstance.SendMessage("SocketManager", "startGame", data);
    });
    socket.on('gameEnded', function(data) {
        myGameInstance.SendMessage("SocketManager", "gameEnded", JSON.stringify(data));
    });
    socket.on('playerPassed', function(data) {
        myGameInstance.SendMessage("SocketManager", "playerPassed", JSON.stringify(data));
    });
  },

  goRoom: function (player) {
    var data = JSON.parse(Pointer_stringify(player));

    socket.emit('join', data);
    // socket.emit('join', {roomName: Pointer_stringify(room), username: Pointer_stringify(user)});
  },

  sendMsg: function (msg) {
    var data = JSON.parse(Pointer_stringify(msg));
    
    socket.emit('chat', data);
    // socket.emit('chat', {msg: Pointer_stringify(m), user: Pointer_stringify(u)});
  },

  sendFicha: function (ficha) {
    var data = JSON.parse(Pointer_stringify(ficha));
    
    socket.emit('moveFicha', data);
    // socket.emit('chat', {msg: Pointer_stringify(m), user: Pointer_stringify(u)});
  },

  restartGame: function (data) {

    socket.emit('resetGame', Pointer_stringify(data));
  },

  emitPass: function (player) {
    var data = JSON.parse(Pointer_stringify(player));
    
    socket.emit('passTurn', data);

  },

  finishGame: function (data) {

    socket.emit('endGame', Pointer_stringify(data));

  },

});