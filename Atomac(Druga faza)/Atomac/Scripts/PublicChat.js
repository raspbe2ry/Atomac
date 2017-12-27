$(function () {
    var chat = $.connection.chatHub;

    chat.client.addNewMessageToPage = function (name, message) {
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };

    chat.client.sendTeamRequest = function (username, teamName) {
        //treba i hidden polja za username, teamName -> tu da se upise username i teamName
        //$('#un').text(username);
        //$('#tn').text(teamName);
        var elm = '<div id="prijemZahtevaZaTim" style="width:300px; height:200px; background-color:#aabbcc"></div>';
        $('body').append(elm);
        $('#prijemZahtevaZaTim').append('<p>User ' + htmlEncode(username)
            + ' has sent you team invitation. Team name is: ' + htmlEncode(teamName) + '.</p><br />'
            + 'Do you accept it? <button id="prihvatiZahtev" value="Yes">Yes</button>'
            + ' <button id="prihvatiZahtev" value="No">No</button>');
        $('#prihvatiZahtev').click(function () {
            if(this.value=="Yes")
                chat.server.approveTeamRequest(username, teamName, "yes");
            else
                chat.server.approveTeamRequest(username, teamName, "no");
        });
    };

    chat.client.activateTeam = function (result, teamName) {
        $('#aktivni-table').DataTable().ajax.reload(null, false);
        //assetListRecentTeams.init();
    };

    $('#message').focus();

    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            chat.server.send($('#uName').val(), $('#message').val());
            $('#message').val('').focus();
        });

        $('.pDugme').click(function () { //mozda this umesto e
            //u about.cshtml treba da se doda input jedan gde se unosi naziv. taj input ima id=teamName
            if (Validacija(this.id)) {
                let teamName = $('#teamName').text();
                chat.server.sendTeamRequest(this.id, "ime tima");
            }
        });

        
    });

});


function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function Validacija(rcvMail) {
    let divovi = $('#recent-table .listItem > button');
    var indikator = false;
    divovi.each((el, num) => {
        console.log(divovi[el].id);
        if (rcvMail == divovi[el].id) {
            indikator = true;
        }
    });
    if (indikator == true)
        return false;
    else
        return true;
}