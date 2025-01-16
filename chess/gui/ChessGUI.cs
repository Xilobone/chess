using chess;
using chessPlayer;

namespace gui
{
    public class ChessGUI : Form
    {
        private static readonly int[] BOARD_OFFSET = [64, 64];
        private static readonly int SQUARE_SIZE = 64;

        private ComboBox bitboardComboBox;
        long bitboard = 0;

        private static Dictionary<int, int> IMAGE_INDEX = new Dictionary<int, int>() {
            {Piece.WHITE_KING, 0},
            {Piece.WHITE_QUEEN, 1},
            {Piece.WHITE_BISHOP, 2},
            {Piece.WHITE_KNIGHT, 3},
            {Piece.WHITE_ROOK, 4},
            {Piece.WHITE_PAWN, 5},
            {Piece.BLACK_KING, 6},
            {Piece.BLACK_QUEEN, 7},
            {Piece.BLACK_BISHOP, 8},
            {Piece.BLACK_KNIGHT, 9},
            {Piece.BLACK_ROOK, 10},
            {Piece.BLACK_PAWN, 11},
        };

        private static readonly int PIECE_SIZE = 133;

        private Image image;

        private Color light = Color.FromArgb(219, 198, 140);
        private Color dark = Color.FromArgb(128, 100, 25);
        private Color bitboardColor = Color.FromArgb(255, 10, 10);

        private Board? currentBoard;

        public static void Create(ChessPlayer player)
        {
            Thread guiThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ChessGUI(player));
            });
            guiThread.Start();
        }
        public ChessGUI(ChessPlayer player)
        {
            player.onChange += OnChange;

            image = Image.FromFile("lib/chess_pieces.png");
            DoubleBuffered = true;

            Text = "Chess GUI";
            Size = new Size(600, 600);

            bitboardComboBox = new ComboBox();

            //center combobox above the board
            bitboardComboBox.Location = new Point(BOARD_OFFSET[0] + 4 * SQUARE_SIZE - bitboardComboBox.Width / 2, BOARD_OFFSET[1] / 2 - bitboardComboBox.Height / 2);
            bitboardComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            bitboardComboBox.Items.AddRange([
                "white: pawn",
                "white: knight",
                "white: bishop",
                "white: rook",
                "white: queen",
                "white: king",
                "black: pawn",
                "black: knight",
                "black: bishop",
                "black: rook",
                "black: queen",
                "black: king",
                "white: pawn attack",
                "white: knight attack",
                "white: bishop attack",
                "white: rook attack",
                "white: queen attack",
                "white: king attack",
                "black: pawn attack",
                "black: knight attack",
                "black: bishop attack",
                "black: rook attack",
                "black: queen attack",
                "black: king attack",
                ]);

            bitboardComboBox.SelectedValueChanged += OnSelectBitboardChange;
            Controls.Add(bitboardComboBox);

        }

        private void OnSelectBitboardChange(object? sender, EventArgs e)
        {
            UpdateBitboard();
        }

        private void UpdateBitboard()
        {
            if (bitboardComboBox.SelectedItem == null || currentBoard == null)
            {
                bitboard = 0;
                Refresh();
                return;
            }

            string? selectedBitboard = bitboardComboBox.SelectedItem.ToString();

            switch (selectedBitboard)
            {
                case "white: pawn": bitboard = currentBoard.bitboardsWhite[BitBoard.PAWN]; break;
                case "white: knight": bitboard = currentBoard.bitboardsWhite[BitBoard.KNIGHT]; break;
                case "white: bishop": bitboard = currentBoard.bitboardsWhite[BitBoard.BISHOP]; break;
                case "white: rook": bitboard = currentBoard.bitboardsWhite[BitBoard.ROOK]; break;
                case "white: queen": bitboard = currentBoard.bitboardsWhite[BitBoard.QUEEN]; break;
                case "white: king": bitboard = currentBoard.bitboardsWhite[BitBoard.KING]; break;
                case "black: pawn": bitboard = currentBoard.bitboardsBlack[BitBoard.PAWN]; break;
                case "black: knight": bitboard = currentBoard.bitboardsBlack[BitBoard.KNIGHT]; break;
                case "black: bishop": bitboard = currentBoard.bitboardsBlack[BitBoard.BISHOP]; break;
                case "black: rook": bitboard = currentBoard.bitboardsBlack[BitBoard.ROOK]; break;
                case "black: queen": bitboard = currentBoard.bitboardsBlack[BitBoard.QUEEN]; break;
                case "black: king": bitboard = currentBoard.bitboardsBlack[BitBoard.KING]; break;
                case "white: pawn attack": bitboard = currentBoard.bitboardsWhite[BitBoard.PAWN_ATTACK]; break;
                case "white: knight attack": bitboard = currentBoard.bitboardsWhite[BitBoard.KNIGHT_ATTACK]; break;
                case "white: bishop attack": bitboard = currentBoard.bitboardsWhite[BitBoard.BISHOP_ATTACK]; break;
                case "white: rook attack": bitboard = currentBoard.bitboardsWhite[BitBoard.ROOK_ATTACK]; break;
                case "white: queen attack": bitboard = currentBoard.bitboardsWhite[BitBoard.QUEEN_ATTACK]; break;
                case "white: king attack": bitboard = currentBoard.bitboardsWhite[BitBoard.KING_ATTACK]; break;
                case "black: pawn attack": bitboard = currentBoard.bitboardsBlack[BitBoard.PAWN_ATTACK]; break;
                case "black: knight attack": bitboard = currentBoard.bitboardsBlack[BitBoard.KNIGHT_ATTACK]; break;
                case "black: bishop attack": bitboard = currentBoard.bitboardsBlack[BitBoard.BISHOP_ATTACK]; break;
                case "black: rook attack": bitboard = currentBoard.bitboardsBlack[BitBoard.ROOK_ATTACK]; break;
                case "black: queen attack": bitboard = currentBoard.bitboardsBlack[BitBoard.QUEEN_ATTACK]; break;
                case "black: king attack": bitboard = currentBoard.bitboardsBlack[BitBoard.KING_ATTACK]; break;
                default: bitboard = 0; break;
            }

            Refresh();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            //draw board
            Brush lightBrush = new SolidBrush(light);
            Brush darkBrush = new SolidBrush(dark);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Brush brush = (x + y) % 2 == 0 ? lightBrush : darkBrush;

                    g.FillRectangle(brush, BOARD_OFFSET[0] + SQUARE_SIZE * x, BOARD_OFFSET[1] + SQUARE_SIZE * y, SQUARE_SIZE, SQUARE_SIZE);
                }
            }

            lightBrush.Dispose();
            darkBrush.Dispose();

            DrawBitboard(g);
            DrawPieces(g);
        }

        private void DrawBitboard(Graphics g)
        {
            Brush brush = new SolidBrush(bitboardColor);
            long btb = bitboard;

            for (int i = 0; i < 64; i++)
            {
                bool isAttacking = (btb & 1) == 1;
                btb >>= 1;

                if (!isAttacking)
                {
                    continue;
                }

                int y = i / 8;
                int x = i % 8;

                g.FillRectangle(brush, BOARD_OFFSET[0] + SQUARE_SIZE * x, BOARD_OFFSET[1] + SQUARE_SIZE * (7 - y), SQUARE_SIZE, SQUARE_SIZE);

            }


        }
        private void DrawPieces(Graphics g)
        {
            if (currentBoard == null)
            {
                return;
            }

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int piece = currentBoard.getPiece(new Position(x, y));
                    if (piece == Piece.EMPTY)
                    {
                        continue;
                    }

                    int imageX = IMAGE_INDEX[piece] % 6;
                    int imageY = IMAGE_INDEX[piece] < 6 ? 0 : 1;

                    Rectangle source = new Rectangle(PIECE_SIZE * imageX, PIECE_SIZE * imageY, PIECE_SIZE, PIECE_SIZE);

                    //board y positions inverted
                    Rectangle position = new Rectangle(BOARD_OFFSET[0] + SQUARE_SIZE * x, BOARD_OFFSET[1] + SQUARE_SIZE * (7 - y), SQUARE_SIZE, SQUARE_SIZE);
                    g.DrawImage(image, position, source, GraphicsUnit.Pixel);

                }
            }
        }

        private void OnChange(object? sender, ChessEventArgs e)
        {
            currentBoard = e.board;
            UpdateBitboard();
            Refresh();
        }
    }
}