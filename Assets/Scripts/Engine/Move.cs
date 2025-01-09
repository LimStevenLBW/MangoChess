public struct Move
{
    public Move(int s, int e, char piece, bool isCapture, bool passantCapture, bool isPromotion, char promotionPiece = ' ')
    {
        start = s;
        end = e;
        this.piece = piece;
        this.isCapture = isCapture;
        this.passantCapture = passantCapture;
        this.isPromotion = isPromotion;
        this.promotionPiece = promotionPiece;
    }

    public override string ToString() => $"start{start}, end{end}; {char.ToUpper(piece)}" + (isCapture ? "x" : "");

    public int start;       //origin and destination squares (0 - 63)
    public int end;
    //Game.Side side;           

    public char piece;          //king=0, queens=2,rooks=4,knights=6,pawns=8
    public bool isCapture;
    public bool passantCapture;   //is this move a capture using en passant

    //char captured_piece; //king=0, queens=2,rooks=4,bishops=6,pawns=8        
    // bool canCastle_off[4];
    // bool isCastle[4];
    //char enPassant;         //if the pawn is pushed two places, set en passant square, along with the square the pawn is on for capture

    public bool isPromotion;
    public char promotionPiece;
}