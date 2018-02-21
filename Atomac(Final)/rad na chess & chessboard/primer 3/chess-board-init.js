
/* ovo ce da ide direktno u @section scripts { } */

let mainBoard = new MainChessBoard(
	'glavnaTabla',
	null, // @Url.path kako god
	['wQ', 'wP', 'wP', 'wB', 'wN', 'wR'],
	['bP', 'bQ', 'bP']
);

let sideBoard = new SideChessBoard(
	'sporednaTabla',
	null, // @Url.path kako god
	['wQ', 'wP', 'wP', 'wB', 'wN', 'wR'],
	['bP', 'bQ', 'bP']
);

$(window).resize(() => {
	mainBoard.resize();
	sideBoard.resize();
});
