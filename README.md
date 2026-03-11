# Othello Negamax CGT 390 Giovanni Tuzzio

This project contains the custom AI I made using NegaMax AB pruning to play the game Othello.

--How the AI works--
Generating movements
- The AI will first identify what moves are actually valid according to the rules of Othello.
- The AI will then flip the opponent's disc(s) if it's a valid move.

Negamax AB Pruning
- The Negamax algorithm is used to explore the AI's potential moves up to a depth of 3.
- Alpha Beta pruning is used to skip  branches if it can't improve the AI's current score.
- Before actually placing a move, the algorithm simulates potential moves on a simulated board to determine what move would give the best score.
- This does not disrupt anything on the current board.

Scoring algorithm
- I gave each square on the 8x8 grid a weighted value.
- Squares that give the player a better chance at scoring more points are given a higher number.
- Corners have the highest value.
- Edges have medium values.
- Squares near corners or unfavorable positions are given a negative value so the AI avoids them.

Choosing the best move
- After the AI simulates what the best move is using the Negamax AB pruning algorithm, it'll select the move with the highest potential value.
- That chosen move is also returned asynchronously through the GetMoveAsync function.

--Project structure--
-The folder titled OthelloAiChanges_GiovanniTuzzio contains a pdf titled OthelloAIChanges_GiovanniTuzzio.pdf that also explains how this AI works (it has the same information as this README).
- The folder titled Othello.AI.GiovanniTuzzioAI contains the .csproj and .cs code I made for this.
- Also in this folder, inside the folders bin -> Debug -> net8.0 is where you'll find the Othello.AI.GiovanniTuzzioAI.dll I made.
- I also created a folder titled Othello.Ai.AllRandomAIDLLs that contain all the Othello.AI.RandomN DLLs just so they're all in one spot.
