
function MainChessBoard(containerId, styleUrl, whitePieces, blackPieces, player) {
	let game = new Chess();

	let removeGreySquares = () => {
		$('#' + containerId + ' .cbjs-square').css('background', '');
	};

	let greySquare = (square) => {
		let squareEl = $('#' + containerId + ' .square-' + square);

		let background = '#a9a9a9';
		if (squareEl.hasClass('cbjs-black') === true) {
			background = '#696969';
		}

		squareEl.css('background', background);
	};
	
	let board = new ChessBoard(containerId, {
		orientation: (player === 'black') ? 'black' : 'white',
		position: 'start',
		showNotation: false,
		draggable: true,
		dropOffBoard: 'snapback',
		pieceTheme: styleUrl,
		onDragStart: (source, piece) => {
			if (game.game_over() === true || (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
				(game.turn() === 'b' && piece.search(/^w/) !== -1))
				return false;
		},
		onMouseoutSquare: (square, piece) => {
			removeGreySquares();
		},
		onMouseoverSquare: (square, piece) => {
			let moves = game.moves({
				square: square,
				verbose: true
			});

			if (moves.length === 0) return;

			greySquare(square);

			for (let i = 0; i < moves.length; i++) {
				greySquare(moves[i].to);
			}
		},
		onSnapEnd: () => {
			board.position(game.fen());
		},
		sparePiecesWhite: whitePieces,
		sparePiecesBlack: blackPieces
	});

	return { board: board, game: game };
}
