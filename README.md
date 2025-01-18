This repo contains the source code of a C# chess program.

About
This program originally started out in november 2024 as a simple console application in Python developed over 3 days as part of my traineeship in software engineering. As an exercise to get a better understanding for the Python language. The program let the user play a game of chess against a simple ai, which used a minimax
algorithm with alpa beta pruning to play. After the deadline for this project passed I decided to continue developing this program, as there were a lot of things I would have done differently if I had programmed this project under different circumstances, and I'm determined
to make an engine that will outplay me (not that I'm that good of a chess player, about 1100 elo on chess.com).

One of the first things that got changed after the initial project was the switch from Python to C#. This change alone doubled the efficiency of the program. The program has also been expanded with other features, such as a gui and testing environment for engine comparisons
and engine efficiency testing. The most recent version of the engine uses a minimax algorithm with alpha-beta pruning with a max search depth of 3 ply.

Program features
- Console application
- Graphical user interface (gui), currently view-only
- Ability to play player-to-player, player-to-engine or engine-to-engine
- Application to compare two engines to each other, by means of playing X games from positions played on lichess.com
- Application to test the efficiency of an engine for a single test position

Features
- Minimax algorithm
- Alpha-beta pruning
- Bitboards
