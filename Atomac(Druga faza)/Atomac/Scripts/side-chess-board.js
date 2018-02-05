
function SideChessBoard(containerId, styleUrl, whitePieces, blackPieces) {
	
	let board = new ChessBoard(containerId, {
		orientation: 'black',
		position: 'start',
		showNotation: false,
		draggable: false,
		pieceTheme: styleUrl,
		spareWhitePieces: whitePieces,
		spareBlackPieces: blackPieces
	});
	
	return board;
}
