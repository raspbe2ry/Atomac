$(function () {
    var chat = $.connection.chatHub;

    chat.client.addNewMessageToPage = function (name, message) {
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };

    chat.client.sendTeamRequest = function (username, teamName) {
        var elm = '<div id="prijemZahtevaZaTim" style="width:300px; height:200px; background-color:#aabbcc"></div>';
        $('body').append(elm);
        $('#prijemZahtevaZaTim').append('<p>User ' + htmlEncode(username)
            + ' has sent you team invitation. Team name is: ' + htmlEncode(teamName) + '.</p><br />'
            + 'Do you accept it? <button class="prihvatiZahtev" value="Yes">Yes</button>'
            + ' <button class="prihvatiZahtev" value="No">No</button>');
        $('.prihvatiZahtev').click(function () {
            if (this.value == "Yes") {
                $('#prijemZahtevaZaTim').remove();
                chat.server.approveTeamRequest(username, teamName, "yes");
            }
            else {
                $('#prijemZahtevaZaTim').remove();
                chat.server.approveTeamRequest(username, teamName, "no");
            }

        });
    };

    chat.client.sendActivateTeamRequest = function (username, teamName) {
        var elm = '<div id="prijemZahtevaZaAktivacijuTima" style="width:300px; height:200px; background-color:#aabbcc"></div>';
        $('body').append(elm);
        $('#prijemZahtevaZaAktivacijuTima').append('<p>User ' + htmlEncode(username)
            + ' has sent you team invitation for team activation. Team name is: ' + htmlEncode(teamName) + '.</p><br />'
            + 'Do you accept it? <button class="prihvatiZahtev" value="Yes">Yes</button>'
            + ' <button class="prihvatiZahtev" value="No">No</button>');
        $('.prihvatiZahtev').click(function () {
            if (this.value == "Yes") {
                $('#prijemZahtevaZaAktivacijuTima').remove();
                chat.server.approveActivateTeamRequest(username, teamName, "yes");
            }
            else {
                $('#prijemZahtevaZaAktivacijuTima').remove();
                chat.server.approveActivateTeamRequest(username, teamName, "no");
            }

        });
    };

    chat.client.reloadRecentByLogIn = function () {
        $('#korisnici-table').DataTable().ajax.reload(null, false);
        $('#recent-table').DataTable().ajax.reload(null, false);
        setTimeout(function () {
            poveziDugmiceZaOnlineKorisnike(chat);
            poveziDugmiceZaRecentTeams(chat);
        }, 3000);
    };

    chat.client.reloadRecentByLogOut = function () {
        $('#korisnici-table').DataTable().ajax.reload(null, false);
        $('#recent-table').DataTable().ajax.reload(null, false);
        $('#opponents-table').DataTable().ajax.reload(null, false);
        //da sprecimo W-R hazarde-> da se prvo veze dugme pa da se obrise tabela i osvezi
        setTimeout(function () {
            poveziDugmiceZaOnlineKorisnike(chat);
            poveziDugmiceZaRecentTeams(chat);
            poveziDugmiceZaOpponentTeams(chat);
        }, 3000);
    };

    chat.client.makeTeam = function (result, teamName) {
        if (result == "yes") {
            $('#recent-table').DataTable().ajax.reload(null, false);
            setTimeout(function () {
                poveziDugmiceZaRecentTeams(chat);
            }, 3000);
        }
    };

    chat.client.activateTeam = function (result, teamName) {
        if (result == "yes") {
            $('#recent-table').DataTable().ajax.reload(null, false);
            $('#opponents-table').DataTable().ajax.reload(null, false);
            setTimeout(function () {
                poveziDugmiceZaRecentTeams(chat);
                poveziDugmiceZaOpponentTeams(chat);
            }, 3000);
        }
    };

    $('#message').focus();

    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            chat.server.send($('#uName').val(), $('#message').val());
            $('#message').val('').focus();
        });
        poveziDugmiceZaOnlineKorisnike(chat);
        poveziDugmiceZaRecentTeams(chat);
        poveziDugmiceZaOpponentTeams(chat);
    });

});

function poveziDugmiceZaOnlineKorisnike(chat) {
    //pDugme je dugme za slanje zahteva igracu za kreiranje tima
    $('.pDugme').click(function () {
        //u about.cshtml treba da se doda input jedan gde se unosi naziv. taj input ima id=teamName
        if (Validacija(this.id)) {

            document.body.appendChild(CreatePopUp(chat, this.id, "proba", "Send request", true));
        }
        else
            document.body.appendChild(CreatePopUp(null, this.id, "proba1", "It is not possible to send request to " + this.dataset.nickname + ". " +
                "You two are already teammates.", false));
    });
}

function poveziDugmiceZaRecentTeams(chat) {
    $('.activateTeam').click(function () {
        let teamName = this.dataset.teamname;
        let teamMember;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].className == "teamMember") {
                //vadim span element koji sadrzi mejl u id
                teamMember = this.childNodes[i];
                break;
            }
        }
        //this.id je captainMail 
        chat.server.sendActivateTeamRequest(this.id, teamMember.id, teamName);
    });
}

function poveziDugmiceZaOpponentTeams(chat) {

}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

//provera se vrsi da li se u listi skorasnjih timova ne nalazi rcvMail kao kapiten tima(NE VRSIMO PROVERU DA LI SE NALAZI KAO TEAM MEMBER!!!)
function Validacija(rcvMail) {
    let captainIds = $('#recent-table .listItem > button');
    var indikator = false;
    captainIds.each((el, num) => {
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
            if (rcvMail == teamMemberIds[el].id) {
                indikator = true;
            }
        });
        if (indikator == true)
            return false;
        else return true;
    }
}

function SendRequest(chat, rcvMail)
{
    let teamName = $('#teamName').val();
    chat.server.sendTeamRequest(rcvMail, teamName);
}

function CreatePopUp(chat, rcvMail, name, modalText, buttonText)
{
    var div1=document.createElement('div');
    div1.id="myModal";
    div1.className="modal fade";
    div1.setAttribute('role', 'dialog');
    var div2=document.createElement('div');
    div2.className="modal-dialog";
    div1.appendChild(div2);
    var div3=document.createElement('div');
    div3.className="modal-content";
    div2.appendChild(div3);
    var div4=document.createElement('div');
    div4.className="modal-header";
    var btn1=document.createElement('input');
    btn1.type='button';
    btn1.class="close";
    btn1.dataset.dismiss='modal';
    btn1.value = htmlEncode("&times;");
    div4.appendChild(btn1);
    var h4=document.createElement('h4');
    h4.innerHTML=name;
    div4.appendChild(h4);
    div3.appendChild(div4);
    var div5=document.createElement('div');
    div5.class = "modal-body";
    if (buttonText)
    {
        var inputText = document.createElement('input');
        inputText.type = 'text';
        inputText.id = "teamName";
        div5.appendChild(inputText);
    }
    else {
        var p = document.createElement('p');
        p.innerHTML = modalText;
        div5.appendChild(p);
    }
    div3.appendChild(div5);
    var div6=document.createElement('div');
    div6.className="modal-footer";
    var btn2=document.createElement('input');
    btn2.type='button';
    btn2.class="btn btn-default";
    btn2.dataset.dismiss='modal';
    btn2.value = "Close";
    if (buttonText) {
        var btn3 = document.createElement('input');
        btn3.type = 'button';
        btn3.class = "btn btn-default";
        btn3.dataset.dismiss = 'modal';
        btn3.value = modalText;
        btn3.addEventListener('click', function () { SendRequest(chat, rcvMail) });
        div6.appendChild(btn3);
    }
    div6.appendChild(btn2);
    div3.appendChild(div6);
    return div1;
}