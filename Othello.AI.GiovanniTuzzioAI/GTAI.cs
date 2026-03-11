using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Othello.Contract;

namespace Othello.AI.GTAI;

public class GTAI : IOthelloAI
{
    public string Name => "Giovanni Tuzzio AI";

    private int EvaluateBoard(BoardState board, DiscColor color)
    {
        int[,] weights = {
        {25, -5, 5, 5, 5, 5, -5, 25},
        {-5, -10, 1, 1, 1, 1, -10, -5},
        {5, 1, 1, 1, 1, 1, 1, 5},
        {5, 1, 1, 1, 1, 1, 1, 5},
        {5, 1, 1, 1, 1, 1, 1, 5},
        {5, 1, 1, 1, 1, 1, 1, 5},
        {-5, -10, 1, 1, 1, 1, -10, -5},
        {25, -5, 5, 5, 5, 5, -5, 25}
    }; // this is a weighted table that helps the ai decide what moves are best. Corners have the highest score, edges medium, and negative points were given to squares next to corners since those are more risky and I want the ai to avoid it
       // the numbers represent each square on the 8x8 grid, with the corners having the highest value, if my ai can put a disc in these high value spots, it increases the chance of it winning

        int score = 0;
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (board.Grid[r, c] == color) score += weights[r, c];
                else if (board.Grid[r, c] != DiscColor.None) score -= weights[r, c];
            }
        }
        return score;
    }

    private int NegaMax(BoardState board, DiscColor color, int depth, int alpha, int beta)
    {
        var validMoves = GetValidMoves(board, color);
        if (depth == 0 || validMoves.Count == 0) //decides if depth limit was reached or if we ran out moves
        {
            return EvaluateBoard(board, color); // Base case: evaluate board
        }

        int maxScore = int.MinValue;
        DiscColor opponent = color == DiscColor.Black ? DiscColor.White : DiscColor.Black;

        foreach (var move in validMoves)
        {
            var newBoard = CloneBoard(board); // Copies board to simulate potential moves
            ApplyMove(newBoard, move, color); // Apply move to the board      

            int score = -NegaMax(newBoard, opponent, depth - 1, -beta, -alpha); // Recursively evaluate opponent's perspective, flip score for zero-sum game
            maxScore = Math.Max(maxScore, score);
            alpha = Math.Max(alpha, score);
            if (alpha >= beta) break; // Alpha-beta pruning: skip remaining moves
        }

        return maxScore;
    }

    private void ApplyMove(BoardState board, Move move, DiscColor color)
    {
        board.Grid[move.Row, move.Column] = color; // place the disc

        int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };
        DiscColor opponent = color == DiscColor.Black ? DiscColor.White : DiscColor.Black;

        for (int i = 0; i < 8; i++)
        {
            int r = move.Row + dr[i];
            int c = move.Column + dc[i];
            List<(int, int)> piecesToFlip = new List<(int, int)>();

            while (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == opponent) //checks for potental opponents discs to flip
            {
                piecesToFlip.Add((r, c));
                r += dr[i];
                c += dc[i];
            }

            if (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == color)
            {
                foreach (var (fr, fc) in piecesToFlip)
                {
                    board.Grid[fr, fc] = color; // flip opponent discs
                }
            }
        }
    }

    private BoardState CloneBoard(BoardState board)
    {
        var newBoard = new BoardState();
        for (int r = 0; r < 8; r++) //copy the board to simulate moves safely without messing up original board
        {
            for (int c = 0; c < 8; c++)
            {
                newBoard.Grid[r, c] = board.Grid[r, c];
            }
        }
        return newBoard;
    }

    //default code from here on
    public async Task<Move?> GetMoveAsync(BoardState board, DiscColor yourColor, CancellationToken ct)
    {
        var validMoves = GetValidMoves(board, yourColor);
        if (validMoves.Count == 0) return null;

        Move bestMove = validMoves[0];
        int bestScore = int.MinValue;

        foreach (var move in validMoves)
        {
            var newBoard = CloneBoard(board);    
            ApplyMove(newBoard, move, yourColor); 
            DiscColor opponent = yourColor == DiscColor.Black ? DiscColor.White : DiscColor.Black;

            int score = -NegaMax(newBoard, opponent, 3, int.MinValue, int.MaxValue); // depth = 3
            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private List<Move> GetValidMoves(BoardState board, DiscColor color)
    {
        var moves = new List<Move>();
        for (int r = 0; r < 8; r++)
        {
            for (int c = 0; c < 8; c++)
            {
                if (IsValidMove(board, new Move(r, c), color))
                {
                    moves.Add(new Move(r, c));
                }
            }
        }
        return moves;
    }

    private bool IsValidMove(BoardState board, Move move, DiscColor color)
    {
        if (board.Grid[move.Row, move.Column] != DiscColor.None) return false;
        
        int[] dr = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dc = { -1, 0, 1, -1, 1, -1, 0, 1 };
        DiscColor opponent = color == DiscColor.Black ? DiscColor.White : DiscColor.Black;

        for (int i = 0; i < 8; i++)
        {
            int r = move.Row + dr[i];
            int c = move.Column + dc[i];
            int count = 0;

            while (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == opponent)
            {
                r += dr[i];
                c += dc[i];
                count++;
            }

            if (r >= 0 && r < 8 && c >= 0 && c < 8 && board.Grid[r, c] == color && count > 0)
            {
                return true;
            }
        }
        return false;
    }
}
