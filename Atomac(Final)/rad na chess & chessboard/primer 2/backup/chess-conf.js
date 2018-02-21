
let board, game = new Chess();

let removeGreySquares = () => {
    $('#sahovskaTabla .cbjs-square').css('background', '');
};

let greySquare = (square) => {
    let squareEl = $('#sahovskaTabla .square-' + square);

    let background = '#a9a9a9';
    if (squareEl.hasClass('cbjs-black') === true) {
        background = '#696969';
    }

    squareEl.css('background', background);
};

let onDragStart = (source, piece) => {
    // do not pick up pieces if the game is over
    // or if it's not that side's turn
    if (game.game_over() === true || (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
            (game.turn() === 'b' && piece.search(/^w/) !== -1))
        return false;
};

let onDrop = (source, target, piece) => {
    removeGreySquares();
	
	// see if the move is legal
	let move = game.move({
	  from: source,
	  to: target,
	  color: piece.substring(0, 1),
	  piece: piece.substring(1, 2).toLowerCase(),
	  promotion: 'q' // NOTE: always promote to a queen for example simplicity
	});
	
	// illegal move
	if (move === null) {
	  return 'snapback';
	} else if (move.from === 'spare') {
	  board.removeSparePiece(piece);
	}
		
	console.log(game.fen(), '\n', JSON.stringify(move));
};

let onMouseoverSquare = (square, piece) => {
    // get list of possible moves for this square
    let moves = game.moves({
        square: square,
        verbose: true
    });

    // exit if there are no moves available for this square
    if (moves.length === 0) return;

    // highlight the square they moused over
    greySquare(square);

    // highlight the possible squares for this piece
    for (let i = 0; i < moves.length; i++) {
        greySquare(moves[i].to);
    }
};

let onMouseoutSquare = (square, piece) => {
    removeGreySquares();
};

let onSnapEnd = () => {
    board.position(game.fen());
};

let conf = {
    position: 'start',
    showNotation: false,
    draggable: true,
    dropOffBoard: 'snapback',
    onDragStart: onDragStart,
    onDrop: onDrop,
    onMouseoutSquare: onMouseoutSquare,
    onMouseoverSquare: onMouseoverSquare,
    onSnapEnd: onSnapEnd,
	spareWhitePieces: ['wQ', 'wP', 'wP', 'wB', 'wN', 'wR'],
	spareBlackPieces: ['bP', 'bQ', 'bP']
};

board = new ChessBoard('sahovskaTabla', conf);
$(window).resize(board.resize);
