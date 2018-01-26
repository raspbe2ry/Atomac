function DTOGameCreationObject() {
    this.Id = $('#myGameId').val();
    this.Points = "";
    this.Tokens = $('#tokens').val();
    this.Duration = $('#duration').val();
    this.TeamId = $('#myTeamId').val();
    this.DroppedCheck = document.querySelector('input[name="check"]:checked').value;
    this.DroppedCheckMate = document.querySelector('input[name="checkMate"]:checked').value;
    this.DroppedPawnOnFirstLine = document.querySelector('input[name="pawnFirstLine"]:checked').value;
    this.DroppedPawnOnLastLine = document.querySelector('input[name="pawnLastLine"]:checked').value;
    this.DroppedFigureOnLastLine = document.querySelector('input[name="figureLastLine"]:checked').value;
}

function ValidateGameObject()
{
    var droppedCheck1 = document.querySelector('input[name="check"]:checked');
    if (droppedCheck1 == undefined || droppedCheck1 == null) {
        return false;
    }
    var droppedCheckMate1 = document.querySelector('input[name="checkMate"]:checked');
    if (droppedCheckMate1 == undefined || droppedCheckMate1 == null) {
        return false;
    }
    var droppedPawnOnFirstLine1 = document.querySelector('input[name="pawnFirstLine"]:checked');
    if (droppedPawnOnFirstLine1 == undefined || droppedPawnOnFirstLine1 == null) {
        return false;
    }
    var droppedPawnOnLastLine1 = document.querySelector('input[name="pawnLastLine"]:checked');
    if (droppedPawnOnLastLine1 == undefined || droppedPawnOnLastLine1 == null) {
        return false;
    }
    var droppedFigureOnLastLine1 = document.querySelector('input[name="figureLastLine"]:checked');
    if (droppedFigureOnLastLine1 == undefined || droppedFigureOnLastLine1 == null) {
        return false;
    }
    if ($('#tokens').val() == "")
    {
        return false;
    }
    return true;
}

function AddSubmitGameRules() {
    var br = document.createElement("br");
    $('#leftSide').append(br);
    var button = document.createElement("input");
    button.type = 'button';
    button.value = "OK";
    button.id = "okButton";

    button.addEventListener('click', function () {
        if (ValidateGameObject()) {
            let obj = new DTOGameCreationObject();
            //let obj1 = JSON.stringify(obj);
            chat.server.submitChanges(obj);
        }
    });
    $('#leftSide').append(button);
}

function AddRowForRule(name, idName)
{
    var row = document.createElement('tr');
    var td1 = document.createElement('td');
    td1.innerHTML = name;
    row.appendChild(td1);
    var td2 = document.createElement('td');
    td2.innerHTML = name;
    row.appendChild(td2);
    td2.appendChild(RadioButtons(name, idName));
    var td3 = document.createElement('td');
    td3.style.backgroundColor = "red";
    row.appendChild(td3);
    var td4 = document.createElement('td');
    td4.style.backgroundColor = "red";
    row.appendChild(td4);
    $('#tableBody').append(row);
}

function RadioButtons(name, idName)
{
    var div = document.createElement('div');
    var no = document.createElement('input');
    no.type = 'radio';
    no.value = 'no';
    no.name = idName;
    no.className = idName;  //mozda treba idName+"No"
    var labelNo = document.createElement('label');
    labelNo.innerHTML = "No";
    div.appendChild(labelNo);
    div.appendChild(no);
    var yes = document.createElement('input');
    yes.type = 'radio';
    yes.value = 'yes';
    yes.name = idName;
    yes.className = idName;  //
    var labelYes = document.createElement('label');
    labelYes.innerHTML = "Yes";
    div.appendChild(labelYes);
    div.appendChild(yes);
    return div;
}

function AppendRulesRows()
{
    AddRowForRule("Umetnuti šah", "check");
    var pr1 = document.getElementsByClassName("check");
    for (let i = 0; i < pr1.length; i++)
    {
        pr1[i].addEventListener("click", function () { chat.server.sendDroppedCheck(pr1[i].value, $('#myTeamId').val(), $('#myGameId').val()) });
    }

    AddRowForRule("Umetnuti mat", "checkMate");
    var pr2 = document.getElementsByClassName("checkMate");
    for (let i = 0; i < pr2.length; i++) {
        pr2[i].addEventListener("click", function () { chat.server.sendDroppedCheckMate(pr2[i].value, $('#myTeamId').val(), $('#myGameId').val()) });
    }

    AddRowForRule("Umetanje piona za prvoj liniji", "pawnFirstLine");
    var pr3 = document.getElementsByClassName("pawnFirstLine");
    for (let i = 0; i < pr3.length; i++) {
        pr3[i].addEventListener("click", function () { chat.server.sendDroppedPawnOnFirstLine(pr3[i].value, $('#myTeamId').val(), $('#myGameId').val()) });
    }

    AddRowForRule("Umetanje piona na posldenjoj liniji", "pawnLastLine");
    var pr4 = document.getElementsByClassName("pawnLastLine");
    for (let i = 0; i < pr4.length; i++) {
        pr4[i].addEventListener("click", function () { chat.server.sendDroppedPawnOnLastLine(pr4[i].value, $('#myTeamId').val(), $('#myGameId').val()) });
    }

    AddRowForRule("Umetanje figure na zadnjoj liniji", "figureLastLine");
    var pr5 = document.getElementsByClassName("figureLastLine")
    for (let i = 0; i < pr5.length; i++) {
        pr5[i].addEventListener("click", function () { chat.server.sendDroppedFigureOnLastLine(pr5[i].value, $('#myTeamId').val(), $('#myGameId').val()) });
    }

    AddTokensForRow();
    AddDurationForRow();
    AddSubmitGameRules();
}

function AddTokensForRow() {
    var row = document.createElement('tr');
    var td1 = document.createElement('td');
    td1.innerHTML = "Uloženi tokeni";
    row.appendChild(td1);
    var td2 = document.createElement('td');
    var token = document.createElement('input');
    token.type = 'number';
    token.id = "tokens";
    token.addEventListener('change', function (e) { if (e.currentTarget.value != "") { chat.server.sendGameDuration(e.currentTarget.value, $('#myTeamId').val(), $('#myGameId').val()) } });
    td2.appendChild(token);
    row.appendChild(td2);
    var td3 = document.createElement('td');
    td3.id = "t1Tokens";
    td3.innerHTML = "0";
    row.appendChild(td3);
    var td4 = document.createElement('td');
    td4.id = "t2Tokens";
    td4.innerHTML = "0";
    row.appendChild(td4);
    $('#tableBody').append(row);
}

function AddDurationForRow() {
    var array = ["5", "10", "15", "20", "30"];

    var row = document.createElement('tr');
    var td1 = document.createElement('td');
    td1.innerHTML = "Dužina partije";
    row.appendChild(td1);
    var td2 = document.createElement('td');
    var duration = document.createElement('select');
    duration.id = "duration";
    for (let i = 0; i < array.length; i++) {
        var option = document.createElement("option");
        option.value = array[i];
        option.text = array[i];
        duration.appendChild(option);
    }
    duration.addEventListener('change', function (e) { chat.server.sendGameDuration(e.currentTarget.value, $('#myTeamId').val(), $('#myGameId').val()) });
    td2.appendChild(duration);
    row.appendChild(td2);
    var td3 = document.createElement('td');
    td3.id = "t1Duration";
    td3.innerHTML = "5";
    row.appendChild(td3);
    var td4 = document.createElement('td');
    td4.id = "t2Duration";
    td4.innerHTML = "5";
    row.appendChild(td4);
    $('#tableBody').append(row);
}

$('#okButton').remove();
$('#tableBody').empty();
AppendRulesRows();