public struct Move
{
    public Move(int s, int e, char piece, bool isCastle, 
        bool passantCapture, bool isPromotion, char capturedPiece = ' ', char promotionPiece = ' ')
    {
        start = s;
        end = e;
        this.piece = piece;
        this.isCastle = isCastle;
        this.passantCapture = passantCapture;
        this.isPromotion = isPromotion;

        this.promotionPiece = promotionPiece;
        this.capturedPiece = capturedPiece;
    }

    public override string ToString() => $"start{start}, end{end}; {char.ToUpper(piece)}" + 
        (capturedPiece != ' ' ? "x" + capturedPiece : "" + (isCastle ? "O-O" : ""));

    public int start;       //origin and destination squares (0 - 63)
    public int end;

    public char piece;          //king=0, queens=2,rooks=4,knights=6,pawns=8
    public bool isCastle;
    public bool passantCapture;   //is this move a capture using en passant
    //char enPassant;         //if the pawn is pushed two places, set en passant square, along with the square the pawn is on for capture

    public bool isPromotion;
    public char promotionPiece;
    public char capturedPiece;        //The piece that was captured, whitespace char otherwise

}