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

    log: function (strMessage) {
        if (typeof (console) != "undefined") console.log(strMessage);
    },

    init: function () {
        var panelLoader = new YAMS.L({
            require: ["container"],
            loadOptional: true,
            combine: true,
            onSuccess: function () {
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
            onSuccess: function () {
                YAMS.admin.layout = new YAHOO.widget.Layout({
                    units: [
						{ position: 'top', height: 40, header: 'Yet Another Minecraft Server', collapse: false, resize: false },
						{ position: 'right', header: 'Server Status', width: 300, resize: false, gutter: '0px 5px', collapse: true, scroll: false, body: 'server-status', animate: true },
						{ position: 'bottom', header: 'Global Log', height: 200, resize: true, body: 'yams-log', gutter: '5px', collapse: true, scroll: true },
						{ position: 'left', header: 'Menu', width: 200, resize: false, body: 'left-menu', gutter: '0px 5px', collapse: true, close: false, scroll: true, animate: true },
						{ position: 'center', body: 'main' }
					]
                });
                YAMS.admin.layout.on('render', function () {
                    var r = YAMS.admin.layout.getUnitByPosition('right').body;
                    var s = document.createElement('div');
                    s.id = 'status';
                    r.appendChild(s);
                    var b1 = document.createElement('button');
                    b1.id = 'start-server';
                    b1.innerHTML = 'Start';
                    b1.disabled = true;
                    YAMS.E.on(b1, 'click', function (e) {
                        YAMS.admin.startServer();
                    });
                    r.appendChild(b1);
                    var b2 = document.createElement('button');
                    b2.id = 'stop-server';
                    b2.innerHTML = 'Stop';
                    b2.disabled = true;
                    YAMS.E.on(b2, 'click', function (e) {
                        YAMS.admin.stopServer();
                    });
                    r.appendChild(b2);
                    var b3 = document.createElement('button');
                    b3.id = 'restart-server';
                    b3.innerHTML = 'Restart';
                    b3.disabled = true;
                    YAMS.E.on(b3, 'click', function (e) {
                        YAMS.admin.restartServer();
                    });
                    r.appendChild(b3);
                    var p = document.createElement('div');
                    p.id = "players";
                    r.appendChild(p);
                    YAMS.admin.getServers();
                    YAMS.admin.updateGlobalLog();
                    YAMS.admin.timer = setInterval("YAMS.admin.updateGlobalLog();", 10000);
                });
                YAMS.admin.layout.render();
            }
        });
        loader.insert();

    },

    getServers: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.getServers_callback, 'action=list'); },

    getServers_callback: {
        success: function (o) {
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
            var settingsTab = YAMS.admin.serverTabs.getTab(2);
            settingsTab.addListener('click', YAMS.admin.getServerSettings);
            YAMS.D.setStyle(settingsTab.get('contentEl'), 'overflow', 'auto');

            //Configure buttons
            YAMS.E.on('console-send', 'click', YAMS.admin.consoleSend);
            YAMS.E.on('chat-send', 'click', YAMS.admin.chatSend);
            YAMS.E.on('console-input', 'keydown', function (e) {
                if (e && (e.keyCode == 13)) { YAMS.admin.consoleSend(); }
            });
            YAMS.E.on('chat-input', 'keydown', function (e) {
                if (e && (e.keyCode == 13)) { YAMS.admin.chatSend(); }
            });

            YAMS.admin.layout.on('resize', function () {
                //Main body elements
                var height = YAMS.admin.layout.getUnitByPosition('center').getSizes().body.h;
                var width = YAMS.admin.layout.getUnitByPosition('center').getSizes().body.w;
                YAMS.D.setStyle(['console', 'chat'], 'height', (height - 97) + 'px');
                YAMS.D.setStyle(['console-input', 'chat-input'], 'width', (width - 82) + 'px');

                //right side elements
                var height = YAMS.admin.layout.getUnitByPosition('right').getSizes().body.h;
                YAMS.D.setStyle('players', 'height', (height - 82) + 'px');
            });
            YAMS.admin.layout.resize();

            //Set initial server
            YAMS.admin.setServer(0, 0);

        },
        failure: function () { YAMS.admin.log('Failed to list servers'); }
    },

    setServer: function (e, serverid) {
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
        YAMS.admin.serverTimer = setInterval("YAMS.admin.updateServerConsole();YAMS.admin.updateServerChat();YAMS.admin.checkServerStatus();YAMS.admin.getPlayers();", 5000);
    },

    getServerSettings: function (e) { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.getServerSettings_callback, 'action=get-server-settings&serverid=' + YAMS.admin.selectedServer); },

    getServerSettings_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var strSettingsForm = '<p><label for="title">Title</label><input type="text" id="cfg_title" name="title" value="' + results.title + '" /></p>' +
                                  '<p><label for="optimisations">Java Optimisations</label><input type="text" id="cfg_optimisations" name="optimisations" value="' + results.optimisations + '" /></p>' +
                                  '<p><label for="memory">Assigned Memory</label><input type="text" id="cfg_memory" name="memory" value="' + results.memory + '" /></p>' +
                                  '<p><label for="autostart">Auto Start Server</label><input type="text" id="cfg_autostart" name="autostart" value="' + results.autostart + '" /></p>' +
                                  '<p><label for="logonmode">Logon Mode</label><input type="text" id="cfg_logonmode" name="logonmode" value="' + results.logonmode + '" /></p>' +
                                  '<p><label for="hellworld">Hellworld</label><input type="text" id="cfg_hellworld" name="hellworld" value="' + results.hellworld + '" /></p>' +
                                  '<p><label for="spawnmonsters">Spawn Monsters</label><input type="text" id="cfg_spawnmonsters" name="spawnmonsters" value="' + results.spawnmonsters + '" /></p>' +
                                  '<p><label for="onlinemode">Online Mode</label><input type="text" id="cfg_onlinemode" name="onlinemode" value="' + results.onlinemode + '" /></p>' +
                                  '<p><label for="spawnanimals">Spawn Animals</label><input type="text" id="cfg_spawnanimals" name="spawnanimals" value="' + results.spawnanimals + '" /></p>' +
                                  '<p><label for="maxplayers">Max online players</label><input type="text" id="cfg_maxplayers" name="maxplayers" value="' + results.maxplayers + '" /></p>' +
                                  '<p><label for="serverip">Server IP</label><input type="text" id="cfg_serverip" name="serverip" value="' + results.serverip + '" /></p>' +
                                  '<p><label for="pvp">PvP</label><input type="text" id="cfg_pvp" name="pvp" value="' + results.pvp + '" /></p>' +
                                  '<p><label for="serverport">Server Port</label><input type="text" id="cfg_serverport" name="serverport" value="' + results.serverport + '" /></p>';

            YAMS.admin.serverTabs.getTab(2).set('content', strSettingsForm);
        },
        failure: function (o) {
            YAMS.admin.log('updateServerConsole failed');
        }
    },

    getPlayers: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.getPlayers_callback, 'action=players&serverid=' + YAMS.admin.selectedServer); },

    getPlayers_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var l = YAMS.D.get('players');
            l.innerHTML = '';
            for (var i = 0, len = results.players.length; i < len; ++i) {
                var r = results.players[i];
                var s = document.createElement('div');
                s.innerHTML = r;
                l.appendChild(s);
            }
        },
        failure: function (o) {
            YAMS.admin.log('updateServerConsole failed');
        }
    },

    checkServerStatus: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.checkServerStatus_callback, 'action=status&serverid=' + YAMS.admin.selectedServer); },

    checkServerStatus_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var s = document.getElementById('status');
            s.innerHTML = '<p>Running: ' + results.status + '</p>' +
                    '<p>RAM: ' + results.ram + 'MB</p>' +
                    '<p>VM: ' + results.vm + 'MB</p>'
            if (results.status == "True") {
                document.getElementById('start-server').disabled = true;
                document.getElementById('stop-server').disabled = false;
                document.getElementById('restart-server').disabled = false;
            } else {
                document.getElementById('start-server').disabled = false;
                document.getElementById('stop-server').disabled = true;
                document.getElementById('restart-server').disabled = true;
            }
        },
        failure: function (o) {
            YAMS.admin.log('checkServerStatus failed');
        }
    },

    mapServer: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.statusCommand_callback, 'action=gmap&serverid=' + YAMS.admin.selectedServer); },
    startServer: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.statusCommand_callback, 'action=start&serverid=' + YAMS.admin.selectedServer); },
    stopServer: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.statusCommand_callback, 'action=stop&serverid=' + YAMS.admin.selectedServer); },
    restartServer: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.statusCommand_callback, 'action=restart&serverid=' + YAMS.admin.selectedServer); },
    statusCommand_callback: { success: function (o) { }, failure: function (o) { YAMS.admin.log('Status Command Failed'); } },

    consoleSend: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.consoleSend_callback, 'action=command&serverid=' + YAMS.admin.selectedServer + '&message=' + escape(YAMS.D.get('console-input').value)); },
    chatSend: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.chatSend_callback, 'action=command&serverid=' + YAMS.admin.selectedServer + '&message=' + escape('say ' + YAMS.D.get('chat-input').value)); },
    consoleSend_callback: { success: function (o) { YAMS.D.get('console-input').value = ''; }, failure: function (o) { YAMS.admin.log('ConsoleSend Failed'); } },
    chatSend_callback: { success: function (o) { YAMS.D.get('chat-input').value = ''; }, failure: function (o) { YAMS.admin.log('ChatSend Failed'); } },

    updateServerConsole: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateServerConsole_callback, 'action=log&start=' + YAMS.admin.lastServerLogId + '&rows=0&serverid=' + YAMS.admin.selectedServer + '&level=all'); },

    updateServerConsole_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var l = YAMS.D.get('console');
            if (l.scrollTop == l.scrollHeight) var bolScroll = true;
            for (var i = 0, len = results.Table.length - 1; len >= i; --len) {
                var r = results.Table[len];
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
            if (bolScroll) l.scrollTop = l.scrollHeight;
            YAMS.admin.loading.cfg.setProperty('visible', false);
        },
        failure: function (o) {
            YAMS.admin.log('updateServerConsole failed');
        }
    },

    updateServerChat: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateServerChat_callback, 'action=log&start=' + YAMS.admin.lastServerLogId + '&rows=200&serverid=' + YAMS.admin.selectedServer + '&level=chat'); },

    updateServerChat_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var l = YAMS.D.get('chat');
            if (l.scrollTop == l.scrollHeight) var bolScroll = true;
            for (var i = 0, len = results.Table.length - 1; len >= i; --len) {
                var r = results.Table[len];
                var s = document.createElement('div');
                YAMS.D.addClass(s, 'message');
                YAMS.D.addClass(s, r.LogLevel);
                var d = eval('new ' + r.LogDateTime.replace(/\//g, '').replace('+0000', ''));
                var m = document.createTextNode('[' + d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + '] ' + r.LogMessage);
                s.appendChild(m);
                l.appendChild(s);
                YAMS.admin.lastServerLogId = r.LogID;
            }
            if (bolScroll) l.scrollTop = l.scrollHeight;
        },
        failure: function (o) {
            YAMS.admin.log('updateServerConsole failed');
        }
    },

    updateGlobalLog: function () { var transaction = YAHOO.util.Connect.asyncRequest('POST', '/api/', YAMS.admin.updateGlobalLog_callback, 'action=log&start=' + YAMS.admin.lastLogId + '&rows=200&serverid=0&level=all'); },

    updateGlobalLog_callback: {
        success: function (o) {
            var results = [];
            try { results = YAHOO.lang.JSON.parse(o.responseText); }
            catch (x) { YAMS.admin.log('JSON Parse Failed'); return; }

            var l = YAMS.admin.layout.getUnitByPosition('bottom').body;
            if (l.scrollTop == l.scrollHeight) var bolScroll = true;
            for (var i = 0, len = results.Table.length - 1; len >= i; --len) {
                var r = results.Table[len];
                var s = document.createElement('div');
                YAMS.D.addClass(s, 'message');
                YAMS.D.addClass(s, r.LogLevel);
                var d = eval('new ' + r.LogDateTime.replace(/\//g, '').replace('+0000', ''));
                s.innerHTML = '[' + d.getFullYear() + '-' + d.getMonth() + '-' + d.getDate() + ' ' + d.getHours() + ':' + d.getMinutes() + '] ' + r.LogMessage;
                l.appendChild(s);
                YAMS.admin.lastLogId = r.LogID;
            }
            l.scrollTop = l.scrollHeight;
        },
        failure: function (o) {
            YAMS.admin.log('updateGlobalLog failed');
        }
    },

    server: function (id, name, ver) {
        this.id = id;
        this.name = name;
        this.ver = ver;
    }

}


YAMS.E.onDOMReady(YAMS.admin.init);

// Register with YAHOO
YAHOO.register("admin", YAMS.admin, {version: YAMS.admin.version});