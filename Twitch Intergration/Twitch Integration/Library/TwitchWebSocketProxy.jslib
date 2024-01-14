mergeInto(LibraryManager.library, {
	TwitchWSCreateInstance: function (instanceName, callingClass, targetAddress) {
		var iName = UTF8ToString(instanceName);
		var cClass = UTF8ToString(callingClass);
		
		if (typeof window.UnityTwitchWebSockets === 'undefined') {
			console.log('Creating UnityTwitchWebSockets object');
			window.UnityTwitchWebSockets = [];
		}
		
		try {
			if (typeof window.UnityTwitchWebSockets[iName + '_' + cClass] !== 'undefined' && window.UnityTwitchWebSockets[iName + '_' + cClass] != null) {
				console.log("Cleaning up websocket system for " + iName + '_' + cClass);
				window.UnityTwitchWebSockets[iName + '_' + cClass].close();
				delete window.UnityTwitchWebSockets[iName + '_' + cClass];
			}
		} catch(e) {
			console.warning('Exception while cleaning up TwitchWS ' + iName + '_' + cClass + ': ' + e);
		}
		
		console.log('Connecting WebSocket to Twitch using Standard Websocket: ' + UTF8ToString(targetAddress));
		window.UnityTwitchWebSockets[iName + '_' + cClass] = new WebSocket(UTF8ToString(targetAddress));
		
		window.UnityTwitchWebSockets[iName + '_' + cClass].onopen = function (e) {
			SendMessage(iName, 'TwitchWebSocketState_' + cClass, 'open');
		};
		
		window.UnityTwitchWebSockets[iName + '_' + cClass].onerror = function(e) {
			console.log(iName + '_' + cClass + " WebSocket Error: " , e);
			SendMessage(iName, 'TwitchWebSocketState_' + cClass, 'error');
		};
		
		window.UnityTwitchWebSockets[iName + '_' + cClass].onclose = function(e) {
			console.log(iName + '_' + cClass + " WebSocket Closed: " , e);
			SendMessage(iName, 'TwitchWebSocketState_' + cClass, 'close');
		};
		
		window.UnityTwitchWebSockets[iName + '_' + cClass].onmessage  = function (e) {
			var data = "";
			if (typeof e.data != "string") data = JSON.stringify(e.data);
			else data = e.data;
			console.log("MESSAGE FROM WEBSOCKET for " + iName + '_' + cClass + ": " + data);
			SendMessage(iName, 'TwitchWebSocketMessage_' + cClass, data);
		};
	},
	
	TwitchWSSend: function (instanceName, callingClass, message) {
		var iName = UTF8ToString(instanceName);
		var cClass = UTF8ToString(callingClass);
		window.UnityTwitchWebSockets[iName + '_' + cClass].send(UTF8ToString(message));
	},
	
	TwitchWSClose: function (instanceName, callingClass) {
		var iName = UTF8ToString(instanceName);
		var cClass = UTF8ToString(callingClass);
		window.UnityTwitchWebSockets[iName + '_' + cClass].close();
		try {
			if (typeof window.UnityTwitchWebSockets[iName + '_' + cClass] !== 'undefined' && window.UnityTwitchWebSockets[iName + '_' + cClass] != null) {
				console.log("Cleaning up websocket system for " + iName + '_' + cClass);
				window.UnityTwitchWebSockets[iName + '_' + cClass].close();
				delete window.UnityTwitchWebSockets[iName + '_' + cClass];
			}
		} catch(e) {
			//OK
		}
	}
});
