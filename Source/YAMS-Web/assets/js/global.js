// Global and universal object for all YAMS operations
// Requires yuiloader-dom-event.js to be loaded
// (c) 2011 Richard Benson
// Portions Copyright (c) 2008, Yahoo! Inc.
// All rights reserved.

// Create root object if it doesn't exist
if (typeof YAMS == "undefined" || !YAMS) {
    var YAMS = {};
}

YAMS = {

	name: "YAMS Global object",
	version: "1.0",
	YUIVersion: "2.8.2r1",
	
	emailRegex: /^[_a-zA-Z0-9-]+(\.[_a-zA-Z0-9-]+)*@[a-zA-Z0-9-]+(\.[a-zA-Z0-9-]+)*\.(([0-9]{1,3})|([a-zA-Z]{2,3})|(aero|coop|info|museum|name))$/,

	// Function to aid in making new namespaces
	namespace: function() {
		var a=arguments, o=null, i, j, d;
		for (i=0; i<a.length; i=i+1) {
			d=a[i].split(".");
			o=YAMS;
	
			// YAMS is implied, so it is ignored if it is included
			for (j=(d[0] == "YAMS") ? 1 : 0; j<d.length; j=j+1) {
				o[d[j]]=o[d[j]] || {};
				o=o[d[j]];
			}
		}
	
		return o;
	},

	// Define the modules available
	module: {
		admin: {
			friendlyName: "Admin",
			fileName: "admin.js"
		}
	
	},

	loadModules: function() {
		var moduleLoader = new YAMS.L();
		
		for (i = 0; i < YAMS.required.length; i++) {
			curr = YAMS.required[i];
			moduleLoader.addModule({
				name: YAMS.required[i],
				type: "js",
				fullpath: YAMS.basePath + YAMS.module[YAMS.required[i]].fileName
			});
			moduleLoader.require(YAMS.required[i]);
		};
		
		moduleLoader.insert();
	},
	
	init: function() {
	
		// Pull global vars into our object
		if (typeof YAMSBasePath == "undefined") {
			YAMS.basePath = "/assets/js/";
		} else { 
			YAMS.basePath = YAMSBasePath;
			YAMSBasePath = undefined;
		};
		if (typeof YUIBuildPath == "undefined") {
			YAMS.YUIBuildPath = "http://yui.yahooapis.com/2.8.2r1/build/";
		} else {
			YAMS.YUIBuildPath = YUIBuildPath;
			YUIBuildPath = undefined;
		};
		if (!(typeof YAMSModules == "undefined")) {
			YAMS.required = YAMSModules;
			YAMSModules = undefined;
		};
		
		// Check if the YAHOO object is available, if it is then re-use some elements
		if (typeof YAHOO == "object") {
			YAMS.L = YAHOO.util.YUILoader;
			YAMS.D = YAHOO.util.Dom;
			YAMS.E = YAHOO.util.Event;
			YAMS.S = YAHOO.util.Selector.query;
		}

		// Add the yui skin class to the body
		YAMS.E.onDOMReady(function(){
			YAMS.D.addClass(document.body, 'yui-skin-sam');
		});
	
		// Load the modules requested
		if (!typeof YAMS.required == "undefined" || YAMS.required) {
			YAMS.loadModules();
		}
		
	}
};

YAMS.init();

YAHOO.register("global", YAMS, {version: YAMS.version});