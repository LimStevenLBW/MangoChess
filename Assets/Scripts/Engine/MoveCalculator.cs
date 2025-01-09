using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCalculator
{
    public Move GetCalculation(List<Move> moves)
    {
        //For Testing, select a random move
        System.Random r = new System.Random();
        int num = r.Next(0, moves.Count);

        for(int i=0;i < moves.Count; i++)
        {
            if (i == num) return moves[i];
        }

        return new Move();
    }
}
