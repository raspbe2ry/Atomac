
function SideChessBoard(containerId, styleUrl, whitePieces, blackPieces, player) {
	
	let board = new ChessBoard(containerId, {
        orientation: player,
		position: 'start',
		showNotation: false,
		draggable: false,
		pieceTheme: styleUrl,
        sparePiecesWhite: whitePieces,
        sparePiecesBlack: blackPieces
	});
	
	return board;
}
