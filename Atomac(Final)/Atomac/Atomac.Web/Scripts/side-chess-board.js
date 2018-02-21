
function SideChessBoard(containerId, styleUrl, whitePieces, blackPieces, player, tableContext) {
	
    let board = new ChessBoard(containerId, {
        tableContext: tableContext,
        orientation: player,
		position: 'start',
		showNotation: false,
		draggable: false,
		pieceTheme: styleUrl,
        sparePiecesWhite: whitePieces,
        sparePiecesBlack: blackPieces,
        squareSize: 31
	});
	
	return board;
}
