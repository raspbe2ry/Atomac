
/* ovo ce da ide direktno u @section scripts { } */

let mainContainerId = 'glavnaTabla';
let sideContainerId = 'sporednaTabla';

let sideBoard = new SideChessBoard(sideContainerId, null, [], [], 'black');
let mainBoard = new MainChessBoard(mainContainerId, null, [], [], sideBoard);

$(window).resize(() => {
	mainBoard.resize();
	sideBoard.resize();
});
