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
                "white: rook",
                "white: knight"]);

            bitboardComboBox.SelectedValueChanged += OnSelectBitboard;
            Controls.Add(bitboardComboBox);

        }

        private void OnSelectBitboard(object? sender, EventArgs e)
        {
            if (bitboardComboBox.SelectedItem == null || currentBoard == null)
            {
                bitboard = 0;
                return;
            }

            string? selectedBitboard = bitboardComboBox.SelectedItem.ToString();

            switch (selectedBitboard)
            {
                case "white: pawn": bitboard = currentBoard.bitboardsWhite[Board.BITBOARD_PAWN]; break;
                case "white: rook": bitboard = currentBoard.bitboardsWhite[Board.BITBOARD_ROOK]; break;
                case "white: knight": bitboard = currentBoard.bitboardsWhite[Board.BITBOARD_KNIGHT]; break;
                default: bitboard = 0; break;
            }
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

                g.FillRectangle(brush, BOARD_OFFSET[0] + SQUARE_SIZE * x, BOARD_OFFSET[1] + SQUARE_SIZE * y, SQUARE_SIZE, SQUARE_SIZE);

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
            Refresh();
        }
    }
}