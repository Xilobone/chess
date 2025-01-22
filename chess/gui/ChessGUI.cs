using chess;
using chessPlayer;

namespace gui
{
    public class ChessGUI : Form
    {
        private static readonly int[] BOARD_OFFSET = [64, 64];
        private static readonly int SQUARE_SIZE = 64;

        private ComboBox bitboardComboBox;
        private TrackBar bitboardTeamTrackbar;

        private TextBox bitboardTextbox;
        private ulong bitboard = 0;

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

        public static ChessGUI Create()
        {
            ChessGUI gui = new ChessGUI(null);
            Thread guiThread = new Thread(() =>
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(gui);
            });
            guiThread.Start();

            return gui;
        }
        public ChessGUI(ChessPlayer? player)
        {
            if (player != null) player.onChange += OnChange;

            image = Image.FromFile("lib/chess_pieces.png");
            DoubleBuffered = true;

            Text = "Chess GUI";
            Size = new Size(600, 600);

            bitboardComboBox = new ComboBox();

            //center combobox above the board
            bitboardComboBox.Location = new Point(BOARD_OFFSET[0] + 4 * SQUARE_SIZE - bitboardComboBox.Width / 2, BOARD_OFFSET[1] / 2 - bitboardComboBox.Height / 2);
            bitboardComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            bitboardComboBox.Items.AddRange([
                "pawn",
                "knight",
                "bishop",
                "rook",
                "queen",
                "king",
                "white: any",
                "black: any",
                "pawn attack",
                "knight attack",
                "bishop attack",
                "rook attack",
                "queen attack",
                "king attack",
                "white: any attack",
                "black: any attack"

                ]);

            bitboardComboBox.SelectedValueChanged += OnSelectBitboardChange;
            Controls.Add(bitboardComboBox);

            bitboardTeamTrackbar = new TrackBar();
            bitboardTeamTrackbar.Location = new Point(bitboardComboBox.Location.X + bitboardComboBox.Width + 16, bitboardComboBox.Location.Y);
            bitboardTeamTrackbar.Size = new Size(48, 16);
            bitboardTeamTrackbar.Minimum = 0;
            bitboardTeamTrackbar.Maximum = 1;
            bitboardTeamTrackbar.ValueChanged += OnSelectBitboardChange;
            Controls.Add(bitboardTeamTrackbar);

            bitboardTextbox = new TextBox();
            bitboardTextbox.Location = new Point(bitboardTeamTrackbar.Location.X + bitboardTeamTrackbar.Width + 16, bitboardTeamTrackbar.Location.Y);
            bitboardTextbox.Size = new Size(128, 32);
            Controls.Add(bitboardTextbox);

        }

        private void OnSelectBitboardChange(object? sender, EventArgs e)
        {
            UpdateBitboard();
        }

        private void UpdateBitboard()
        {
            if (bitboardComboBox.SelectedItem == null)
            {
                bitboard = 0;
                Refresh();
                return;
            }

            string? selectedBitboard = bitboardComboBox.SelectedItem.ToString();

            bool white = bitboardTeamTrackbar.Value == 0;

            ulong[] pieceBitboards = new ulong[7];
            ulong[] attackBitboards = new ulong[7];
            if (currentBoard != null)
            {
                pieceBitboards = white ? currentBoard.bitboardsWhite : currentBoard.bitboardsBlack;
                attackBitboards = white ? currentBoard.bitboardsWhiteAttack : currentBoard.bitboardsBlackAttack;

            }

            switch (selectedBitboard)
            {
                case "pawn": bitboard = pieceBitboards[BitBoard.PAWN]; break;
                case "knight": bitboard = pieceBitboards[BitBoard.KNIGHT]; break;
                case "bishop": bitboard = pieceBitboards[BitBoard.BISHOP]; break;
                case "rook": bitboard = pieceBitboards[BitBoard.ROOK]; break;
                case "queen": bitboard = pieceBitboards[BitBoard.QUEEN]; break;
                case "king": bitboard = pieceBitboards[BitBoard.KING]; break;
                case "white: any": bitboard = currentBoard != null ? BitBoard.GetAny(currentBoard, true) : 0; break;
                case "black: any": bitboard = currentBoard != null ? BitBoard.GetAny(currentBoard, false) : 0; break;
                case "pawn attack": bitboard = attackBitboards[BitBoard.PAWN]; break;
                case "knight attack": bitboard = attackBitboards[BitBoard.KNIGHT]; break;
                case "bishop attack": bitboard = attackBitboards[BitBoard.BISHOP]; break;
                case "rook attack": bitboard = attackBitboards[BitBoard.ROOK]; break;
                case "queen attack": bitboard = attackBitboards[BitBoard.QUEEN]; break;
                case "king attack": bitboard = attackBitboards[BitBoard.KING]; break;
                case "white: any attack": bitboard = currentBoard != null ? BitBoard.GetAnyAttack(currentBoard, true) : 0; break;
                case "black: any attack": bitboard = currentBoard != null ? BitBoard.GetAnyAttack(currentBoard, false) : 0; break;
                default: bitboard = 0; break;
            }

            if (!string.IsNullOrEmpty(bitboardTextbox.Text)) bitboard = ulong.Parse(bitboardTextbox.Text);

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
            ulong btb = bitboard;

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

        public void OnChange(object? sender, ChessEventArgs e)
        {
            currentBoard = e.board;
            UpdateBitboard();
            Refresh();
        }
    }
}