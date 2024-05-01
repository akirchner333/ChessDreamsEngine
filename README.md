# Chess Dreams Engine
An engine for generating legal moves for any given chess position. It's built on a entity-component model with the hopes of making it easy to create fairy pieces or new chess variants. It allows for move application and reversal and can take [FENs](https://en.wikipedia.org/wiki/Forsyth%E2%80%93Edwards_Notation) to build board states.

# Architecture
Each piece is it's own class and uses `Movers` to generate [bitboards](https://www.chessprogramming.org/Bitboards) and `MoveFactories` to conert those boards into the Move struct. They also use [magic bitboards](https://www.chessprogramming.org/Magic_Bitboards) for fast storage and retrieval of boards.

Play is run through `Rules` - each move is applied to all rules and the rules are responsible for updating relevant board information. For example, the `Capture` rule marks the targetted piece as having been captured and the `Promotion` rule converts pawns that get to the final row to their promotion.
