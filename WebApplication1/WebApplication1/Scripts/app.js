//'use strict';

//angular.module("AspNetWebApiSignalRDemo", [])

//.service("SignalrService", function () {
//    var notificationHubProxy = null;

//    this.initialize = function () {
//        $.connection.hub.logging = true;
//        notificationHubProxy = $.connection.notifHub;

//        notificationHubProxy.client.hello = function () {
//            console.log("Hello from ASP.NET Web API");
//        };

//        $.connection.hub.start().done(function () {
//            console.log("started");
//            notificationHubProxy.server.hello();
//        }).fail(function (result) {
//            console.log(result);
//        });
//    };
//})

//.controller("AppController", ["SignalrService", function (SignalrService) {
//    SignalrService.initialize();
//}]);

//$(function () {
//    var notificationHubProxy = null;

//    $.connection.hub.logging = true;
//    notificationHubProxy = $.connection.notificationHub;

//    notificationHubProxy.client.hello = function () {
//        console.log("Hello from ASP.NET Web API");
//    };

//    $.connection.hub.start().done(function () {
//        console.log("started");
//        notificationHubProxy.server.hello();
//    }).fail(function (result) {
//        console.log(result);
//    });
//})

$(function () { 
    //var signalR = require('signalr-client');

var client  = new signalR.client(

	//signalR service URL
	"http://localhost:49538/signalr",

	// array of hubs to be supported in the connection
	['NotificationHub']
    //, 10 /* Reconnection Timeout is optional and defaulted to 10 seconds */
    //, false 
    /* doNotStart is option and defaulted to false. If set to true 
                 client will not start until .start() is called */
);

client.on(
// Hub Name (case insensitive)
'NotificationHub',

// Method Name (case insensitive)
'hello',

// Callback function with parameters matching call from hub
function () {
    console.log("Hello from ASP.NET Web API");
});

var hub = client.hub('NotificationHub'); // Hub Name (case insensitive)
hub.invoke(
'start',	// Method Name (case insensitive) 
'hub', 'invoked from hub' //additional parameters to match called signature
);

hub.invoke(
	'Hello',	// Method Name (case insensitive) 
	'hub', 'invoked from hub' //additional parameters to match called signature
	);

})