
/* ovo ce da ide direktno u @section scripts { } */

let mainContainerId = 'glavnaTabla';
let sideContainerId = 'sporednaTabla';

let mainBoardObj = new MainChessBoard(mainContainerId, null, [], []);
let sideBoardObj = new MainChessBoard(sideContainerId, null, [], [], 'black');

let mainBoard = mainBoardObj.board;
let mainGame = mainBoardObj.game;

let sideBoard = sideBoardObj.board;
let sideGame = sideBoardObj.game;

// SREDITI PROBLEM NEVIDLJIVOG POTEZA - EN PASSANT PROVERITI

mainBoard.setOnDropFunction((source, target, piece) => {
	$('#' + mainContainerId + ' .cbjs-square').css('background', '');

	let move = mainGame.move({
		from: source,
		to: target,
		color: piece.substring(0, 1),
		piece: piece.substring(1, 2).toLowerCase(),
		promotion: 'p' // ovo treba da se prosiri
	});

	if (move === null) {
		return 'snapback';
	} else if (move.from === 'spare') {
		mainBoard.removeSparePiece(piece);
	} else if (move.captured !== undefined) {
		let capturedColor = (move.color === 'w') ? 'b' : 'w';
		// salje se signalR-om capturedColor + move.captured.toUpperCase()
		// tj linija ispod ce tad da bude za sporednu staticku tablu
		sideBoard.addSparePiece(capturedColor + move.captured.toUpperCase(), capturedColor);
	}

	console.log('Glavna tabla:\n' + mainGame.fen(), '\n', JSON.stringify(move));
});

sideBoard.setOnDropFunction((source, target, piece) => {
	$('#' + sideContainerId + ' .cbjs-square').css('background', '');

	let move = sideGame.move({
		from: source,
		to: target,
		color: piece.substring(0, 1),
		piece: piece.substring(1, 2).toLowerCase(),
		promotion: 'p' // ovo treba da se prosiri
	});

	if (move === null) {
		return 'snapback';
	} else if (move.from === 'spare') {
		sideBoard.removeSparePiece(piece);
	} else if (move.captured !== undefined) {
		let capturedColor = (move.color === 'w') ? 'b' : 'w';
		// salje se signalR-om capturedColor + move.captured.toUpperCase()
		// linija ispod ce tad da bude za sporednu staticku tablu
		mainBoard.addSparePiece(capturedColor + move.captured.toUpperCase(), capturedColor);
	}

	console.log('Sporedna tabla:\n' + sideGame.fen(), '\n', JSON.stringify(move));
});

// dodati metodu koja pristiglu figuru oblika bojaFIGURA dodaje u odgovarajuci deo pored table
$(window).resize(() => {
	mainBoard.resize();
	sideBoard.resize();
});
