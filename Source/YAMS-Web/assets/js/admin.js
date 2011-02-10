// DTS Control JS Function
// (c) 2010 Dixcart Technical Solutions Ltd..
// Authored by Richard Benson
// Portions Copyright (c) 2008, Yahoo! Inc.
// All rights reserved.

YAMS.namespace("admin");

YAMS.admin = {
	name: "YAMS Admin",
	version: "1.0",
	
    servers: [],

    timer: 0,
    lastLogId: 0,
	
	selectedServer: 0,
	lastServerLogId: 0,
	serverTimer: 0,
	
	log: function(strMessage) {
		if (typeof(console) != "undefined") console.log(strMessage);
	},
	
	init: function() {
		var panelLoader = new YAMS.L({
			require: ["container"],
			loadOptional: true,
			combine: true,
			onSuccess: function() {
				YAMS.admin.loading = new YAHOO.widget.Panel("wait", {
					width: "240px",
					fixedcenter: true,
					close: false,
					draggable: false,
					zindex: 4,
					modal: true,
					visible: true,
					filterWord: true
				});
				YAMS.admin.loading.setHeader("Loading, please wait...");
				YAMS.admin.loading.setBody('<img src="http://l.yimg.com/a/i/us/per/gr/gp/rel_interstitial_loading.gif" />');
				YAMS.admin.loading.render(document.body);
			}
		});
		panelLoader.insert();
		
		var loader = new YAMS.L({
			require: ["layout", "connection", "json", "tabview"],
			loadOptional: true,
			combine: true,
			filter: 'debug',
			onSuccess: function() {
				YAMS.admin.layout = new YAHOO.widget.Layout({
					units: [
						{ position: 'top', height: 40, header: 'Yet Another Minecraft Server', collapse: false, resize: false },
						{ position: 'right', header: 'Server Status', width: 300, resize: false, gutter: '0px 5px', collapse: true, scroll: true, body: 'server-status', animate: true },
						{ position: 'bottom', header: 'Global Log', height: 200, resize: true, body: 'yams-log', gutter: '5px', collapse: true, scroll: true },
						{ position: 'left', header: 'Menu', width: 200, resize: false, body: 'left-menu', gutter: '0px 5px', collapse: true, close: false, scroll: true, animate: true },
						{ position: 'center', body: 'main' }
					]
				});
				YAMS.admin.layout.on('render', function() {
					YAMS.admin.getServers();
					YAMS.admin.updateGlobalLog();
					YAMS.admin.timer = setInterval("YAMS.admin.updateGlobalLog();", 10000);
				});
				YAMS.admin.layout.render();
			}
		});
		loader.insert();
		
	},
	
	getServers: function() { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.getServers_callback, 'action=list'); },
	
	getServers_callback: {
		success: function(o) {
			var results = [];
			try { results = YAHOO.lang.JSON.parse(o.responseText); }
			catch (x) { YAMS.admin.log('JSON parse failed'); return; }
			
			var ul = document.createElement('ul');
			for (var i = 0, len = results.servers.length; i < len; ++i) {
				var s = new YAMS.admin.server(results.servers[i].id, results.servers[i].title, results.servers[i].ver);
				YAMS.admin.servers.push(s);
				var li = document.createElement('li');
				var a = document.createElement('a');
				a.href = "#";
				a.innerHTML = results.servers[i].title;
				YAMS.E.on(a, 'click', YAMS.admin.setServer, YAMS.admin.servers.length - 1);
				li.appendChild(a);
				ul.appendChild(li);
			}
			YAMS.admin.layout.getUnitByPosition('left').body.appendChild(ul);
			//Build Tab view
			YAMS.admin.serverTabs = new YAHOO.widget.TabView('server-console');
			YAMS.admin.serverTabs.addTab(new YAHOO.widget.Tab({
				label: "Console",
				content: '<div id="console" class="log"></div><div class="command console"><input type="text" class="console-input" id="console-input" /><button id="console-send" class="send">Send</button></div>',
				active: true
			}));
			YAMS.admin.serverTabs.addTab(new YAHOO.widget.Tab({
				label: "Chat",
				content: '<div id="chat" class="log"></div><div class="command chat"><input type="text" class="chat-input" id="chat-input" /><button id="chat-send" class="send">Say</button></div>'
			}));
			YAMS.admin.serverTabs.addTab(new YAHOO.widget.Tab({
				label: "Settings",
				content: 'Some Settings'
			}));
			
			YAMS.admin.layout.on('resize', function() {
				var height = YAMS.admin.layout.getUnitByPosition('center').getSizes().body.h;
				var width = YAMS.admin.layout.getUnitByPosition('center').getSizes().body.w;
				YAMS.D.setStyle(['console','chat'], 'height', (height - 97) + 'px');
				YAMS.D.setStyle(['console-input', 'chat-input'], 'width', (width - 67) + 'px');
			});
			YAMS.admin.layout.resize();

			//Set initial server
			YAMS.admin.setServer(0, 0);
			
		},
		failure: function() { YAMS.admin.log('Failed to list servers'); }
	},
	
	setServer: function(e, serverid) {
		YAHOO.util.Event.preventDefault(e);
		//Clear out any previous contents
		YAMS.D.get('console').innerHTML = '';
		YAMS.D.get('chat').innerHTML = '';
		clearInterval(YAMS.admin.serverTimer);
		//Fix the server
		YAMS.admin.selectedServer = YAMS.admin.servers[serverid].id;
		//Set the title
		YAMS.S('#main h1')[0].innerHTML = YAMS.admin.servers[serverid].name + ' (' + YAMS.admin.servers[serverid].ver + ')';
		//Load console
		YAMS.admin.updateServerConsole();
		YAMS.admin.updateServerChat();
		//Set the timer
		YAMS.admin.serverTimer = setInterval("YAMS.admin.updateServerConsole();YAMS.admin.updateServerChat();", 10000);
		//Configure buttons
		YAMS.E.on('console-send', 'click', YAMS.admin.consoleSend);
		YAMS.E.on('chat-send', 'click', YAMS.admin.chatSend);
		YAMS.E.on('console-input', 'keydown', function(e) {
			if (e && (e.keyCode == 13)) { YAMS.admin.consoleSend(); }
		});
		YAMS.E.on('chat-input', 'keydown', function(e) {
			if (e && (e.keyCode == 13)) { YAMS.admin.chatSend(); }
		});
	},
	
	consoleSend: function() {
		var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.consoleSend_callback, 'action=command&serverid=' + YAMS.admin.selectedServer + '&message=' + escape(YAMS.D.get('console-input').value));
	},
	
	chatSend: function() {
		var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.chatSend_callback, 'action=command&serverid=' + YAMS.admin.selectedServer + '&message=' + escape('say ' + YAMS.D.get('chat-input').value));
	},
	
	consoleSend_callback: {
		success: function(o) { YAMS.D.get('console-input').value = ''; },
		failure: function(o) { YAMS.admin.log('ConsoleSend Failed'); }
	},
	
	chatSend_callback: {
		success: function(o) { YAMS.D.get('chat-input').value = ''; },
		failure: function(o) { YAMS.admin.log('ChatSend Failed'); }
	},
	
	updateServerConsole: function() { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateServerConsole_callback, 'action=log&start=' + YAMS.admin.lastServerLogId + '&rows=0&serverid=' + YAMS.admin.selectedServer + '&level=all'); },
	
	updateServerConsole_callback: {
		success: function(o) {
			var results = [];
			try { results = YAHOO.lang.JSON.parse(o.responseText); }
			catch (x) {	YAMS.admin.log('JSON Parse Failed'); return; }
			
			var l = YAMS.D.get('console');
			for (var i = 0, len = results.Table.length; i < len; ++i) {
				var r = results.Table[i];
				var s = document.createElement('div');
				YAMS.D.addClass(s, 'message');
				YAMS.D.addClass(s, r.LogLevel);
				var d = eval('new ' + r.LogDateTime.replace(/\//g, '').replace('+0000', ''));
				var m = document.createTextNode('[' + d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + '] ' + r.LogMessage);
				s.appendChild(m);
				YAMS.D.get('console').appendChild(s);
				YAMS.admin.lastServerLogId = r.LogID;
				l.scrollTop = l.scrollHeight;
			}
			YAMS.admin.loading.cfg.setProperty('visible', false);
		},
		failure: function(o) {
			YAMS.admin.log('updateServerConsole failed');
		}
	},

	updateServerChat: function() { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateServerChat_callback, 'action=log&start=' + YAMS.admin.lastServerLogId + '&rows=0&serverid=' + YAMS.admin.selectedServer + '&level=chat'); },
	
	updateServerChat_callback: {
		success: function(o) {
			var results = [];
			try { results = YAHOO.lang.JSON.parse(o.responseText); }
			catch (x) {	YAMS.admin.log('JSON Parse Failed'); return; }
			
			var l = YAMS.D.get('chat');
			for (var i = 0, len = results.Table.length; i < len; ++i) {
				var r = results.Table[i];
				var s = document.createElement('div');
				YAMS.D.addClass(s, 'message');
				YAMS.D.addClass(s, r.LogLevel);
				var d = eval('new ' + r.LogDateTime.replace(/\//g, '').replace('+0000', ''));
				var m = document.createTextNode('[' + d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + '] ' + r.LogMessage);
				s.appendChild(m);
				l.appendChild(s);
				YAMS.admin.lastServerLogId = r.LogID;
				l.scrollTop = l.scrollHeight;
			}
		},
		failure: function(o) {
			YAMS.admin.log('updateServerConsole failed');
		}
	},

	updateGlobalLog: function() { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateGlobalLog_callback, 'action=log&start=' + YAMS.admin.lastLogId + '&rows=0&serverid=0&level=all');	},
	
	updateGlobalLog_callback: {
		success: function(o) {
			var results = [];
			try { results = YAHOO.lang.JSON.parse(o.responseText); }
			catch (x) {	YAMS.admin.log('JSON Parse Failed'); return; }
			
			var l = YAMS.admin.layout.getUnitByPosition('bottom');
			for (var i = 0, len = results.Table.length; i < len; ++i) {
				var r = results.Table[i];
				var s = document.createElement('div');
				YAMS.D.addClass(s, 'message');
				YAMS.D.addClass(s, r.LogLevel);
				var d = eval('new ' + r.LogDateTime.replace(/\//g, '').replace('+0000', ''));
				var m = document.createTextNode('[' + d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + '] ' + r.LogMessage);
				s.appendChild(m);
				l.body.appendChild(s);
				YAMS.admin.lastLogId = r.LogID;
				l.body.scrollTop = l.body.scrollHeight;
			}
		},
		failure: function(o) {
			YAMS.admin.log('updateGlobalLog failed');
		}
	},
	
	server: function(id, name, ver) {
		this.id = id;
		this.name = name;
		this.ver = ver;
	}
	
}


YAMS.E.onDOMReady(YAMS.admin.init);

// Register with YAHOO
YAHOO.register("admin", YAMS.admin, {version: YAMS.admin.version});