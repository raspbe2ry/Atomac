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

//provera se vrsi da li se u listi skorasnjih timova ne nalazi rcvMail kao kapiten tima(NE VRSIMO PROVERU DA LI SE NALAZI KAO TEAM MEMBER!!!)
function Validacija(rcvMail) {
    let captainIds = $('#recent-table .listItem > button');
    var indikator = false;
    captainIds.each((el, num) => {
        //console.log(divovi[el].id);
        if (rcvMail == captainIds[el].id) {
            indikator = true;
        }
    });
    if (indikator == true)
        return false;
    else
    {
        let teamMemberIds = $('#recent-table .listItem .teamMember');
        teamMemberIds.each((el, num) => {
            //console.log(divovi[el].id);
            if (rcvMail == teamMemberIds[el].id) {
                indikator = true;
            }
        });
        if (indikator == true)
            return false;
        else return true;
    }
}