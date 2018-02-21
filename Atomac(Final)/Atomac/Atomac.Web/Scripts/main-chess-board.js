
function DTOMoveCreationObject(move, board, game) {
    this.Id = "";
    this.Board = ""; //1-T1,2-T2 //nadji tablu
    this.From = move.from;
    this.To = move.to;
    this.Piece = move.piece;
    this.State = game.fen(); //game.fen();
    this.White = board.getWhiteSparePieces(); //board.getWhiteSparePieces();
    this.Black = board.getBlackSparePieces(); //board.getBlackSparePieces();
    this.GameId = $('#myGameId').val(); //
    this.Captured = ""; //vrednost figure koja je pojedena malim slovom
    this.Color = move.color;
}

function MainChessBoard(containerId, styleUrl, whitePieces, blackPieces, sideBoard, player, tableContext, ruleSet) {
	let game = new Chess();
	let moveValidator = createValidator();
    
	function createValidator() {
	    let rules = [];
	    let ruleInd = 0;

	    if (ruleSet.droppedCheck === false) {
	        rules[ruleInd++] = new Rule(() => {
	            if (game.in_check()) {
	                game.undo();
	                return false;
	            }
	        }, null);
	    }

	    if (ruleSet.droppedCheckMate === false) {
	        let newNext = (ruleInd > 0) ? rules[ruleInd - 1] : null;
	        rules[ruleInd++] = new Rule(() => {
	            if (game.in_checkmate()) {
	                game.undo();
	                return false;
	            }
	        }, newNext);
	    }

	    if (ruleSet.droppedPawnOnFirstLine === false) {
	        let newNext = (ruleInd > 0) ? rules[ruleInd - 1] : null;
	        rules[ruleInd++] = new Rule(() => {
	            let moveHistory = game.history({ verbose: true });
	            let lastMove = moveHistory[moveHistory.length - 1];
	            let lastMoveTo = lastMove.to[1];
	            
	            if (lastMove.piece[0] === 'p'
                    && (lastMove.color === 'w' && lastMoveTo === '1'
                        || lastMove.color === 'b' && lastMoveTo === '8')) {
	                game.undo();
	                return false;
	            }
	        }, newNext);
	    }

	    if (ruleSet.droppedPawnOnLastLine === false) {
	        let newNext = (ruleInd > 0) ? rules[ruleInd - 1] : null;
	        rules[ruleInd++] = new Rule(() => {
	            let moveHistory = game.history({ verbose: true });
	            let lastMove = moveHistory[moveHistory.length - 1];
	            let lastMoveTo = lastMove.to[1];

	            if (lastMove.piece[0] === 'p'
                    && (lastMove.color === 'w' && lastMoveTo === '8'
                        || lastMove.color === 'b' && lastMoveTo === '1')) {
	                game.undo();
	                return false;
	            }
	        }, newNext);
	    }

	    if (ruleSet.droppedFigureOnLastLine === false) {
	        let newNext = (ruleInd > 0) ? rules[ruleInd - 1] : null;
	        rules[ruleInd++] = new Rule(() => {
	            let moveHistory = game.history({ verbose: true });
	            let lastMove = moveHistory[moveHistory.length - 1];
	            let lastMoveTo = lastMove.to[1];

	            if (lastMove.piece[0] !== 'p'
                    && (lastMove.color === 'w' && lastMoveTo === '8'
                        || lastMove.color === 'b' && lastMoveTo === '1')) {
	                game.undo();
	                return false;
	            }
	        }, newNext);
	    }

	    if (ruleInd === 0) {
	        return new Rule(() => { return true; }, null);
	    }

	    return rules[ruleInd - 1];
	};

	function Rule(validator, next) {
	    this.validate = () => {
	        if (validator() === false) {
	            return false;
	        }

	        if (this.next !== null && this.next !== undefined) {
	            return this.next.validate();
	        }
	    };
	    this.next = next;
	};

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

    var board = null;
    
    let playMove = (source, target, piece) => {
        $('#' + containerId + ' .cbjs-square').css('background', '');

        let move = game.move({
            from: source,
            to: target,
            color: piece.substring(0, 1),
            piece: piece.substring(1, 3).toLowerCase(),
            promotion: 'p'
        });

        if (move === null || (move.from === 'spare' && moveValidator.validate() === false)) {
            return 'snapback';
        }

        if (move.from === 'spare') {
            board.removeSparePiece(piece);
        } else if (move.captured !== undefined) {
            let capturedColor = (move.color === 'w') ? 'b' : 'w';
            sideBoard.addSparePiece(capturedColor + move.captured.toUpperCase());
        }
        board.position(game.fen(), false);
        board.resize("main");
        return move;
    }

    board = new ChessBoard(containerId, {
        tableContext: tableContext,
		orientation: (player === 'black') ? 'black' : 'white',
		position: 'start',
		showNotation: false,
		draggable: true,
		dropOffBoard: 'snapback',
        pieceTheme: styleUrl,
        makeMove: playMove,
		onDragStart: (source, piece) => {
            if (game.game_over() === true || (game.turn() === 'w' && piece.search(/^b/) !== -1) ||
                (game.turn() === 'b' && piece.search(/^w/) !== -1) || (game.turn() !== player.substring(0, 1)))
                return false;
        },
        onDrop: (source, target, piece) => {
            if (target === 'offboard') {
                return 'snapback';
            }
            let move = playMove(source, target, piece);
            if (move === 'snapback') {
                return move;
            }
            let moveObject = new DTOMoveCreationObject(move, board, game);
            if (move.captured !== undefined) {
                moveObject.Captured = move.captured;
            }
            let myId = $('#myId').val();
            let myCapId = $('#myCapitenId').val();
            if (myId === myCapId) {
                moveObject.Board = "1";
            }
            else {
                moveObject.Board = "2";
            }
            if (game.game_over() === true) {
                var poruka = "";
                if (game.in_checkmate()) {
                    poruka = "Checkmate";
                }
                else if (game.in_stalemate()) {
                    poruka = "Stalemate";
                }
                else if (game.insufficient_material()) {
                    poruka = "Insuficient material";
                }
                else if (game.in_threefold_repetition()) {
                    poruka = "Threefold repetition";
                }
                MoveFigureAndFinishGame(moveObject, poruka);
            }
            else {
                MoveFigure(moveObject);
            }
        },
		onMouseoutSquare: (square, piece) => {
			removeGreySquares();
		},
        onMouseoverSquare: (square, piece) => {
            if (!piece)
                return;
            if (piece.substring(0, 1) !== player.substring(0, 1)) {
                return;
            }

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
		sparePiecesBlack: blackPieces,
        squareSize: 57
	});

	return board;
}
