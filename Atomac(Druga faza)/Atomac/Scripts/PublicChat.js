var chat;
var mainBoard;
var sideBoard;

$(function () {
    chat = $.connection.chatHub;

    chat.client.addNewMessageToPage = function (name, message) {
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };

    chat.client.addNewMessageToGamePane = function (nick, message,color) {
        $('#GameDiscussion').append('<li style="color:'+color+'"><strong>' + htmlEncode(nick)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };

    chat.client.sendTeamRequest = function (username, teamName) {
        let yes = function () {
            chat.server.approveTeamRequest(username, teamName, "yes");
        }
        let no = function () {
            chat.server.approveTeamRequest(username, teamName, "no");
        }
        document.body.appendChild(GlobalPopUp("User " + htmlEncode(username) + " has sent you team invitation. Team name is: " + htmlEncode(teamName) + ".", "Do you accept it?", YesNoPopUp(yes, no)));
        $('#myModal').modal('show');
    };

    chat.client.sendActivateTeamRequest = function (username, teamName) {
                let yes = function () {
                    chat.server.approveActivateTeamRequest(username, teamName, "yes");
                }
                let no = function () {
                    chat.server.approveActivateTeamRequest(username, teamName, "no");
                }
                document.body.appendChild(GlobalPopUp("User " + htmlEncode(username) + " has sent you team activation invite. Team name is: " + htmlEncode(teamName) + ".", "Do you accept it?", YesNoPopUp(yes, no)));
                $('#myModal').modal('show');
    };

    chat.client.sendGameRequest = function (teamName, username, oppTeamName) {
        let yes = function () {
            chat.server.approveGameRequest(teamName, username, oppTeamName, "yes");
        }
        let no = function () {
            chat.server.approveGameRequest(teamName, username, oppTeamName, "no");
        }
        document.body.appendChild(GlobalPopUp("User " + htmlEncode(username) + " has sent you game invitation. Team name is: " + htmlEncode(teamName) + ".", "Do you accept it?", YesNoPopUp(yes, no)));
        $('#myModal').modal('show');
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

    chat.client.activateTeam = function (result) {
        if (result == "yes") {
            $('#recent-table').DataTable().ajax.reload(null, false);
            $('#opponents-table').DataTable().ajax.reload(null, false);
            setTimeout(function () {
                poveziDugmiceZaRecentTeams(chat);
                poveziDugmiceZaOpponentTeams(chat);
            }, 3000);
        }
    };

    chat.client.gameCreationConfirmationApprove = function (result) {
        setTimeout(function () {
            var objResult = JSON.parse(result);
            var id = objResult.Id;
            var query = "http://localhost:59310/NewGame/Index" + "//?id=" + id;
            document.location.href = query;
        }, 3000);
    };

    chat.client.resetControl = function () {
        var controls = document.getElementsByClassName('control');
        for (var i = 0; i < controls.length; i++) {
            controls[i].disabled = false;
        }
    };

    chat.client.sendSubmitOfOne = function () {
        var controls = document.getElementsByClassName('control');
        for (var i = 0; i < controls.length; i++) {
            controls[i].disabled = true;
        }
    };

    chat.client.returnDroppedCheck = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if(firstTeam==senderTeamId)
        {
            if(value=="no")
            {
                $('#t1check').css('background-color', 'red');
            }
            else
            {
                $('#t1check').css('background-color', 'green');
            }
        }
        else
        {
            if (value == "no") {
                $('#t2check').css('background-color', 'red');
            }
            else {
                $('#t2check').css('background-color', 'green');
            }
        }
    };

    chat.client.returnDroppedCheckMate = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId) {
            if (value == "no") {
                $('#t1checkMate').css('background-color', 'red');
            }
            else {
                $('#t1checkMate').css('background-color', 'green');
            }
        }
        else {
            if (value == "no") {
                $('#t2checkMate').css('background-color', 'red');
            }
            else {
                $('#t2checkMate').css('background-color', 'green');
            }
        }
    };

    chat.client.returnDroppedPawnOnFirstLine = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId) {
            if (value == "no") {
                $('#t1pawnFirstLine').css('background-color', 'red');
            }
            else {
                $('#t1pawnFirstLine').css('background-color', 'green');
            }
        }
        else {
            if (value == "no") {
                $('#t2pawnFirstLine').css('background-color', 'red');
            }
            else {
                $('#t2pawnFirstLine').css('background-color', 'green');
            }
        }
    };

    chat.client.returnDroppedPawnOnLastLine = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId) {
            if (value == "no") {
                $('#t1pawnLastLine').css('background-color', 'red');
            }
            else {
                $('#t1pawnLastLine').css('background-color', 'green');
            }
        }
        else {
            if (value == "no") {
                $('#t2pawnLastLine').css('background-color', 'red');
            }
            else {
                $('#t2pawnLastLine').css('background-color', 'green');
            }
        }
    };

    chat.client.returnDroppedFigureOnLastLine = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId) {
            if (value == "no") {
                $('#t1figureLastLine').css('background-color', 'red');
            }
            else {
                $('#t1figureLastLine').css('background-color', 'green');
            }
        }
        else {
            if (value == "no") {
                $('#t2figureLastLine').css('background-color', 'red');
            }
            else {
                $('#t2figureLastLine').css('background-color', 'green');
            }
        }
    };

    chat.client.returnGameDuration = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId)
        {
            $('#t1Duration').val(value);
            $('#t1Duration').html(value);
        }
        else
        {
            $('#t2Duration').val(value);
            $('#t2Duration').html(value);
        }
    };

    chat.client.returnGameTokens = function (value, senderTeamId) {
        let firstTeam = $('#firstTeam').val();
        if (firstTeam == senderTeamId) {
            $('#t1Tokens').val(value);
            $('#t1Tokens').html(value);
        }
        else {
            $('#t2Tokens').val(value);
            $('#t2Tokens').html(value);
        }
    };

    chat.client.sendStartGame = function (dtoGame) {
            $('#leftSide').empty();
            $('#rightTop').empty();
            //ovo ispod odvojiti u posebnu funkciju
            var leftSide = document.getElementById('leftSide');
            leftSide.classList.add("mainContainer");
            var glavnaTabla=document.createElement("div");
            glavnaTabla.className = "bigBoardContainer";
            glavnaTabla.id = "glavnaTabla";
            leftSide.appendChild(glavnaTabla);

            var rightTop = document.getElementById("rightTop");
            rightTop.classList.add("sideContainer");
            var malaTabla = document.createElement("div");
            malaTabla.className = "smallBoardContainer";
            malaTabla.id = "sporednaTabla";
            rightTop.appendChild(malaTabla);

            var rightBottom = document.getElementById("rightBottom");
            rightBottom.classList.add("chatContainer");

            LoadTables();
    };

    chat.client.moveFigureOnTable = function (move, sndEmail) {
        let myId = $('#myId').val();
        if (myId === sndEmail) {
            return;
        }
        let dtoMove = JSON.parse(move);
        let myCapId = $('#myCapitenId').val();
        //let myTeamId = $('#myTeamId').val();
        //let izazivaci = $('#firstTeam').val();
        if (move.From === 'spare') {
            dtoMove.Piece = move.Piece.substring(0, 1);
        }
        if (myId === myCapId) {
            if (dtoMove.Board == "1") {
                mainBoard.makeMove(dtoMove.From, dtoMove.To, dtoMove.Color + dtoMove.Piece.toUpperCase());
            }
            else {
                //sideBoard.position(dtoMove.State.slice());
                if (dtoMove.From === 'spare') {
                    sideBoard.removeSparePiece(dtoMove.Color + dtoMove.Piece.toUpperCase());
                }
                sideBoard.position(dtoMove.State, false);
                sideBoard.resize();
                if (dtoMove.Captured !== "") {
                    let capturedColor = (dtoMove.Color === 'w') ? 'b' : 'w';
                    mainBoard.addSparePiece(capturedColor + dtoMove.Captured.toUpperCase());
                }
            }
        }
        else {
            if (dtoMove.Board != "1") {
                mainBoard.makeMove(dtoMove.From, dtoMove.To, dtoMove.Color + dtoMove.Piece.toUpperCase());
            }
            else {
                //sideBoard.position(dtoMove.State.slice());
                if (dtoMove.From === 'spare') {
                    sideBoard.removeSparePiece(dtoMove.Color + dtoMove.Piece.toUpperCase());
                }
                sideBoard.position(dtoMove.State, false);
                sideBoard.resize();
                if (dtoMove.Captured !== "") {
                    let capturedColor = (dtoMove.Color === 'w') ? 'b' : 'w';
                    mainBoard.addSparePiece(capturedColor + dtoMove.Captured.toUpperCase());
                }
            }
        }
    }

    chat.client.moveFigureAndFinishGame = function (move, sndEmail, sndTeamId, poruka) {
        let headerText = poruka;
        let bodyText = "";
        let myId = $('#myId').val();
        let myTeamId = $('#myTeamId').val();
        if (myTeamId === sndTeamId) {
            if (poruka === "Checkmate")
                bodyText = "Your team has won this game. Congratulations!";
            else
                bodyText = "Fair and square. It's tie game.";
        }
        else {
            if (poruka === "Checkmate")
                bodyText = "Your team has lost this game. Better luck next time!";
            else
                bodyText = "Fair and square. It's tie game.";
        }
        if (myId === sndEmail) {
            document.body.appendChild(GlobalPopUp(headerText, bodyText, InfoPopUpForFinishGame()));
            $('#myModal').modal('show');
            return;
        }
        let dtoMove = JSON.parse(move);
        let myCapId = $('#myCapitenId').val();
        if (move.From === 'spare') {
            dtoMove.Piece = move.Piece.substring(0, 1);
        }
        if (myId === myCapId) {
            if (dtoMove.Board == "1") {
                mainBoard.makeMove(dtoMove.From, dtoMove.To, dtoMove.Color + dtoMove.Piece.toUpperCase());
            }
            else {

                if (dtoMove.From === 'spare') {
                    sideBoard.removeSparePiece(dtoMove.Color + dtoMove.Piece.toUpperCase());
                }
                sideBoard.position(dtoMove.State, false);
                sideBoard.resize();
                if (dtoMove.Captured !== "") {
                    let capturedColor = (dtoMove.Color === 'w') ? 'b' : 'w';
                    mainBoard.addSparePiece(capturedColor + dtoMove.Captured.toUpperCase());
                }
            }
        }
        else {
            if (dtoMove.Board != "1") {
                mainBoard.makeMove(dtoMove.From, dtoMove.To, dtoMove.Color + dtoMove.Piece.toUpperCase());
            }
            else {
                if (dtoMove.From === 'spare') {
                    sideBoard.removeSparePiece(dtoMove.Color + dtoMove.Piece.toUpperCase());
                }
                sideBoard.position(dtoMove.State, false);
                sideBoard.resize();
                if (dtoMove.Captured !== "") {
                    let capturedColor = (dtoMove.Color === 'w') ? 'b' : 'w';
                    mainBoard.addSparePiece(capturedColor + dtoMove.Captured.toUpperCase());
                }
            }
        }
        document.body.appendChild(GlobalPopUp(headerText, bodyText, InfoPopUpForFinishGame()));
        $('#myModal').modal('show');
        return;
    }

    $('#message').focus();

    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            chat.server.send($('#uName').val(), $('#message').val());
            $('#message').val('').focus();
        });
        $('#sendmessageInGame').click(function () {
            chat.server.sendMessageInGame($('#uNameGame').val(), $('#messageInGame').val(), $('#myGameId').val(), $('#myTeamId').val());
            $('#messageInGame').val('').focus();
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
        if (ValidacijaRecentTable(this.id)) {
            let x = this.id;
            let sendFun = function (teamName) {
                SendRequest(chat, x, teamName);
            }
            document.body.appendChild(
                GlobalPopUp("Team creation", "Write team name in input field", SendRequestPopUp(sendFun)));
            $('#myModal').modal('show');
        }
        else {
            document.body.appendChild(
                GlobalPopUp("Team creation", "It is not possible to send request to " + this.dataset.nickname + ". You two are already teammates."
                , InfoPopUp()));
            $('#myModal').modal('show');
        }
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
    $('.gameRequest').click(function () {
        let userEmail = $('#userEmail').val();
        if (ValidacijaGameRequest(userEmail)) {
            let oppButton = $('.gameRequest');
            let opponentTeamName = oppButton.data('oppteamname');
            let opponentCptEmail = oppButton.attr('id');
            let teamName = $('.currentTeam').data('teamname');
            chat.server.sendGameRequest(teamName, userEmail, opponentTeamName, opponentCptEmail);
        }
        else
        {
            document.body.appendChild(
                GlobalPopUp("Game Invitation", "It is not possible to send game invitation. Either you are not captain or you dont have activated team."
                , InfoPopUp()));
            $('#myModal').modal('show');
        }
    });
}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

//provera se vrsi da li se u listi skorasnjih timova ne nalazi rcvMail kao kapiten tima(NE VRSIMO PROVERU DA LI SE NALAZI KAO TEAM MEMBER!!!)
function ValidacijaRecentTable(rcvMail) {
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

function ValidacijaGameRequest(userEmail) {
    let captainIds = $('.currentTeam').attr('id');
    if (userEmail == captainIds)
        return true;
    return false;
}

function SendRequest(chat, rcvMail, teamName)
{
    chat.server.sendTeamRequest(rcvMail, teamName);
}

function CreatePopUp(chat, rcvMail, name, modalText, buttonText)
{
    let boolButtonText = buttonText;
    var div1=document.createElement('div');
    div1.id = "myModal";
    div1.className="modal fade";
    div1.setAttribute('role', 'dialog');
    div1.setAttribute('data', 'deletePopUp: true');
    var div2=document.createElement('div');
    div2.className="modal-dialog";
    div1.appendChild(div2);
    var div3=document.createElement('div');
    div3.className="modal-content";
    div2.appendChild(div3);
    var div4=document.createElement('div');
    div4.className="modal-header";
    var btn1=document.createElement('span');
    btn1.class = "close"; //
    btn1.style.cssFloat = "right";
    btn1.style.fontSize = '32px';
    btn1.addEventListener('onmouseover', function (e) { e.currentTarget.style.cursor = 'grab'; })
    btn1.dataset.dismiss='modal';
    btn1.innerHTML = '\u02DF';
    div4.appendChild(btn1);
    var h4=document.createElement('h4');
    h4.innerHTML=name;
    div4.appendChild(h4);
    div3.appendChild(div4);
    var div5=document.createElement('div');
    div5.class = "modal-body";
    if (boolButtonText)
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
    btn2.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
    });
    if (boolButtonText) {
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

function GlobalPopUp(headerText, bodyText, footerStrategy)
{
    var div1 = document.createElement('div');
    div1.id = "myModal";
    div1.className = "modal fade";
    div1.setAttribute('role', 'dialog');
    div1.setAttribute('data', 'deletePopUp: true');
    var div2 = document.createElement('div');
    div2.className = "modal-dialog";
    div1.appendChild(div2);
    var div3 = document.createElement('div');
    div3.className = "modal-content";
    div2.appendChild(div3);
    var div4 = document.createElement('div');
    div4.className = "modal-header";
    var btn1 = document.createElement('span');
    btn1.class = "close"; //
    btn1.style.cssFloat = "right";
    btn1.style.fontSize = '32px';
    btn1.addEventListener('onmouseover', function (e) { e.currentTarget.style.cursor = 'grab'; })
    btn1.dataset.dismiss = 'modal';
    btn1.innerHTML = '\u02DF';
    div4.appendChild(btn1);
    var h4 = document.createElement('h4');
    h4.innerHTML = headerText;
    div4.appendChild(h4);
    div3.appendChild(div4);
    var div5 = document.createElement('div');
    div5.class = "modal-body";
    var p = document.createElement('p');
    p.innerHTML = bodyText;
    div5.appendChild(p);
    div3.appendChild(div5);
    var div6 = document.createElement('div');
    div6.className = "modal-footer";
    div6.appendChild(footerStrategy);
    div3.appendChild(div6);
    return div1;
}

function YesNoPopUp(yesFun, noFun)
{
    var div = document.createElement('div');
    yesBtn = document.createElement('input');
    yesBtn.type = 'button';
    yesBtn.value = "Yes";
    yesBtn.class = "btn btn-default";
    yesBtn.dataset.dismiss = 'modal';
    yesBtn.addEventListener('click', function()
    {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
        yesFun();
    });
    noBtn = document.createElement('input');
    noBtn.type = 'button';
    noBtn.value = "No";
    noBtn.class = "btn btn-default";
    noBtn.dataset.dismiss = 'modal';
    noBtn.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
        noFun();
    });
    div.appendChild(yesBtn);
    div.appendChild(noBtn);
    return div;
}

function SendRequestPopUp(sendReqFun) {
    var div = document.createElement('div');
    var inputText = document.createElement('input');
    inputText.type = 'text';
    inputText.id = "teamName";
    sendBtn = document.createElement('input');
    sendBtn.type = 'button';
    sendBtn.value = "Send";
    sendBtn.class = "btn btn-default";
    sendBtn.dataset.dismiss = 'modal';
    sendBtn.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
        sendReqFun(inputText.value);
    });
    closeBtn = document.createElement('input');
    closeBtn.type = 'button';
    closeBtn.value = "Close";
    closeBtn.class = "btn btn-default";
    closeBtn.dataset.dismiss = 'modal';
    closeBtn.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
    });
    div.appendChild(inputText);
    div.appendChild(sendBtn);
    div.appendChild(closeBtn);
    return div;
}

function InfoPopUp()
{
    var div = document.createElement('div');
    closeBtn = document.createElement('input');
    closeBtn.type = 'button';
    closeBtn.value = "Close";
    closeBtn.class = "btn btn-default";
    closeBtn.dataset.dismiss = 'modal';
    closeBtn.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
    });
    div.appendChild(closeBtn);
    return div;
}

function InfoPopUpForFinishGame() {
    var div = document.createElement('div');
    closeBtn = document.createElement('input');
    closeBtn.type = 'button';
    closeBtn.value = "Close";
    closeBtn.class = "btn btn-default";
    closeBtn.dataset.dismiss = 'modal';
    closeBtn.addEventListener('click', function () {
        $('#myModal').remove();
        $('.modal-backdrop').remove();
        //treba da se izvrsi prebacivanje 
        setTimeout(function () {
            var query = "http://localhost:59310/Home/About";
            document.location.href = query;
        }, 1000);
    });
    div.appendChild(closeBtn);
    return div;
}

var sideBoard = null;
var mainBoard = null;

function LoadTables()
{
    let myId = $('#myId').val();
    let myCapId = $('#myCapitenId').val();
    let myTeamId = $('#myTeamId').val();
    let izazivaci = $('#firstTeam').val();

    $.ajax(
         {
             type: "GET", 
             url: "http://localhost:59310/Profile/MyTableContext",
             data: {userEmail: myId},
             contentType: "application/json; charset=utf-8",
             processdata: true,
             dataType: "json",
             success: function (msg) 
             {
                 LoadTablesResult(msg, myId, myCapId, myTeamId, izazivaci);
             },
             error: function (jqXHR, textStatus, errorThrown) 
             {
                 alert("Some error");
             }
         }
     );
}

function LoadTablesResult(msg, myId, myCapId, myTeamId, izazivaci)
{
    let mainContainerId = 'glavnaTabla';
    let sideContainerId = 'sporednaTabla';

    if (myId === myCapId) {
        if (myTeamId == izazivaci) {
            sideBoard = new SideChessBoard(sideContainerId, '/Content/img/chesspieces/'+msg.figure+'/{piece}.png', [], [], 'black', msg);
            mainBoard = new MainChessBoard(mainContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], sideBoard, 'white', msg);
        }
        else {
            sideBoard = new SideChessBoard(sideContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], 'white', msg);
            mainBoard = new MainChessBoard(mainContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], sideBoard, 'black', msg);
        }
    }
    else {
        if (myTeamId == izazivaci) {
            sideBoard = new SideChessBoard(sideContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], 'white', msg);
            mainBoard = new MainChessBoard(mainContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], sideBoard, 'black', msg);
        }
        else {
            sideBoard = new SideChessBoard(sideContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], 'black', msg);
            mainBoard = new MainChessBoard(mainContainerId, '/Content/img/chesspieces/' + msg.figure + '/{piece}.png', [], [], sideBoard, 'white', msg);
        }
    }

    $(window).resize(() => {
        mainBoard.resize();
        sideBoard.resize();
    });
}

function MoveFigure(moveObject) {
    chat.server.moveFigure(JSON.stringify(moveObject));
}

function MoveFigureAndFinishGame(moveObject, poruka) {
    chat.server.moveFigureAndFinishGame(JSON.stringify(moveObject), poruka);
}