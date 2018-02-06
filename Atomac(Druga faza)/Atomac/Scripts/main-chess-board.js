function DTOMoveCreationObject(move, board, game) {
    this.Id = "";
    this.Board = ""; //1-T1,2-T2 //nadji tablu
    this.From = move.from;
    this.To = move.to;
    this.Piece = move.piece;
    this.State = game.fen(); //game.fen();
    this.White = board.getWhiteSparePieces(); //board.getWhiteSparePieces();
    this.Black = board.getBlackSparePieces(); //board.getBlackSparePieces();
    this.GameId = $('#myGameId').val();; //
    this.Captured = ""; //vrednost figure koja je pojedena malim slovom
    this.Color = move.color;
}

var board;

function MainChessBoard(containerId, styleUrl, whitePieces, blackPieces, sideBoard, player) {
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

    let playMove = (source, target, piece) => {
        $('#' + containerId + ' .cbjs-square').css('background', '');

        let move = game.move({
            from: source,
            to: target,
            color: piece.substring(0, 1),
            piece: piece.substring(1, 2).toLowerCase(),
            promotion: 'q' // ovo treba da se prosiri
        });

        if (move === null) {
            return 'snapback';
        } else if (move.from === 'spare') {
            this.removeSparePiece(piece);
        } else if (move.captured !== undefined) {
            let capturedColor = (move.color === 'w') ? 'b' : 'w';
            sideBoard.addSparePiece(capturedColor + move.captured.toUpperCase(), capturedColor);
        }
        return move;
    }

	 board = new ChessBoard(containerId, {
		orientation: (player === 'black') ? 'black' : 'white',
		position: 'start',
		showNotation: false,
		draggable: true,
		dropOffBoard: 'snapback',
        pieceTheme: styleUrl,
        move: playMove,
		onDragStart: (source, piece) => {
			if (game.game_over() === true || (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
				(game.turn() === 'b' && piece.search(/^w/) !== -1))
				return false;
        },
        onDrop: (source, target, piece) => {
            let move = playMove(source, target, piece);
            //$('#' + containerId + ' .cbjs-square').css('background', '');

            //let move = game.move({
            //    from: source,
            //    to: target,
            //    color: piece.substring(0, 1),
            //    piece: piece.substring(1, 2).toLowerCase(),
            //    promotion: 'q' // ovo treba da se prosiri
            //});

            //if (move === null) {
            //    return 'snapback';
            //} else if (move.from === 'spare') {
            //    this.removeSparePiece(piece);
            //}

            // else if (move.captured !== undefined) {
            //    let capturedColor = (move.color === 'w') ? 'b' : 'w';
            //    // salje se signalR-om capturedColor + move.captured.toUpperCase()
            //    // tj linija ispod ce tad da bude za sporednu staticku tablu
            //    sideBoard.addSparePiece(capturedColor + move.captured.toUpperCase(), capturedColor);
                
            //}
            let moveObject = new DTOMoveCreationObject(move, board, game);
            if (move.captured !== undefined) {
                moveObject = move.captured;
            }
            let myId = $('#myId').val();
            let myCapId = $('#myCapitenId').val();
            if (myId === myCapId) {
                moveObject.Board = "1";
            }
            else {
                moveObject.Board = "2";
            }
            MoveFigure(moveObject);
            //console.log('Glavna tabla:\n' + game.fen(), '\n', JSON.stringify(move));
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

	return board;
}
