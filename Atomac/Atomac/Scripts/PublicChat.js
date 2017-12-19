$(function () {
    // Reference the auto-generated proxy for the hub.  
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {
        // Add the message to the page. 
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };
    chat.client.sendTeamRequest = function (username, teamName) {
        //treba i hidden polja za username, teamName -> tu da se upise username i teamName
        $('#un').text(username);
        $('#tn').text(teamName);
        //div za prijem treba da se napravi, da ga ne menjam pre push lugica
        $('#prijemZahtevaZaTim').append('<p>User ' + htmlEncode(username)
            + ' has sent you team invitation. Team name is: ' + htmlEncode(teamName) + '.</p><br />'
            + '< p > Do you accept it? <button id="prihvatiZahtev">Yes</button>'
            + ' <button id="prihvatiZahtev">No</button>');
    }
    //lugic treba da doda tim u datatable
    chat.client.activateTeam = function () {
        //treba da se pozove metoda za refresh liste recentTeams da se iz baze pokupi taj novi tim
    }
    // Get the user name and store it to prepend to messages.
    //$('#displayname').val(prompt('Enter your name:', ''));
    // Set initial focus to message input box.  
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            // Call the Send method on the hub. 
            chat.server.send($('#uName').val(), $('#message').val());
            // Clear text box and reset focus for next comment. 
            $('#message').val('').focus();
        });
        $('.btn .btn-success').click(function (e) { //mozda this umesto e
            if (Validacija(e.id)) {
                //u about.cshtml treba da se doda input jedan gde se unosi naziv. taj input ima id=teamName
                let teamName = $('#teamName').text();
                chat.server.sendTeamRequest(e.id, teamName);
            }

        });
        $('#prihvatiZahtev').click(function (e) {
            //username je onaj od kojeg je stigao zahtev
            let username = $('#un').text();
            let teamName = $('#tn').text();
            if (e.text() == "Yes")
                chat.server.approveTeamRequest(username, teamName, "yes");
            else chat.server.approveTeamRequest(username, teamName, "no");
            //ili samo chat.server.approveTeamRequest(username, teamName, e.text());
        });
    });

});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function Validacija(rcvMail) {
    let divovi = $('#recent-table .listItem > button').each((el, num) => {
        if (rcvMail === el.attr('id'))
            return false;
    });
    return true;
}