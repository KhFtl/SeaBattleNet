using System.Drawing.Drawing2D;
using SeaBattle.Protocol;

namespace SeaBattleNet
{
    public class BoardControl : Control
    {
        public const int BoardSize = 10;
        public ClientCellState[,] Cells { get; } = new ClientCellState[BoardSize, BoardSize];

        public bool IsOpponentBoard { get; set; } = false;
        public bool IsInteractive { get; set; } = false;
        public string Title { get; set; } = "Поле";

        // Підтримка режиму розміщення кораблів (Placement Preview)
        public bool IsPlacementMode { get; set; } = false;
        public int PlacementShipSize { get; set; } = 4;
        public bool PlacementIsHorizontal { get; set; } = true;
        public bool PlacementIsValid { get; set; } = false;

        private int _hoverX = -1;
        private int _hoverY = -1;

        public event Action<int, int>? CellClicked;
        public event Action<int, int, MouseButtons>? BoardClicked;
        public event Action<int, int>? HoverCellChanged;

        private readonly string[] _columnHeaders = { "А", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "К" };

        public BoardControl()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            Clear();
        }

        public void Clear()
        {
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    Cells[x, y] = ClientCellState.Empty;
                }
            }
            Invalidate();
        }

        public void SetShip(int x, int y)
        {
            if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize)
            {
                Cells[x, y] = ClientCellState.Ship;
                Invalidate();
            }
        }

        public void SetCellState(int x, int y, ClientCellState state)
        {
            if (x >= 0 && x < BoardSize && y >= 0 && y < BoardSize)
            {
                Cells[x, y] = state;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (!IsInteractive) return;

            var cell = GetCellFromPoint(e.Location);
            if (cell.HasValue)
            {
                if (_hoverX != cell.Value.x || _hoverY != cell.Value.y)
                {
                    _hoverX = cell.Value.x;
                    _hoverY = cell.Value.y;
                    HoverCellChanged?.Invoke(_hoverX, _hoverY);
                    Invalidate();
                }
            }
            else
            {
                if (_hoverX != -1 || _hoverY != -1)
                {
                    _hoverX = -1;
                    _hoverY = -1;
                    HoverCellChanged?.Invoke(-1, -1);
                    Invalidate();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverX != -1 || _hoverY != -1)
            {
                _hoverX = -1;
                _hoverY = -1;
                HoverCellChanged?.Invoke(-1, -1);
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (!IsInteractive) return;

            var cell = GetCellFromPoint(e.Location);
            if (cell.HasValue)
            {
                BoardClicked?.Invoke(cell.Value.x, cell.Value.y, e.Button);
                if (e.Button == MouseButtons.Left)
                {
                    CellClicked?.Invoke(cell.Value.x, cell.Value.y);
                }
            }
        }

        private (int x, int y)? GetCellFromPoint(Point pt)
        {
            int headerSize = 26;
            int availableWidth = Width - headerSize - 4;
            int availableHeight = Height - headerSize - 30; // 30 for title header

            int cellSize = Math.Min(availableWidth / BoardSize, availableHeight / BoardSize);
            if (cellSize <= 0) return null;

            int startX = (Width - (headerSize + cellSize * BoardSize)) / 2 + headerSize;
            int startY = 30 + headerSize;

            int relX = pt.X - startX;
            int relY = pt.Y - startY;

            if (relX >= 0 && relY >= 0)
            {
                int cx = relX / cellSize;
                int cy = relY / cellSize;
                if (cx >= 0 && cx < BoardSize && cy >= 0 && cy < BoardSize)
                {
                    return (cx, cy);
                }
            }

            return null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Background
            using (var bgBrush = new SolidBrush(Color.FromArgb(24, 30, 44)))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // Title
            using (var titleBrush = new SolidBrush(Color.FromArgb(220, 230, 242)))
            using (var titleFont = new Font("Segoe UI", 11f, FontStyle.Bold))
            {
                var titleRect = new RectangleF(0, 4, Width, 24);
                var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                g.DrawString(Title, titleFont, titleBrush, titleRect, sf);
            }

            int headerSize = 26;
            int availableWidth = Width - headerSize - 8;
            int availableHeight = Height - headerSize - 36;
            int cellSize = Math.Max(18, Math.Min(availableWidth / BoardSize, availableHeight / BoardSize));

            int startX = (Width - (headerSize + cellSize * BoardSize)) / 2 + headerSize;
            int startY = 32 + headerSize;

            // Draw Column Headers (А-К)
            using (var headerBrush = new SolidBrush(Color.FromArgb(140, 165, 195)))
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    var rect = new RectangleF(startX + x * cellSize, startY - headerSize, cellSize, headerSize);
                    g.DrawString(_columnHeaders[x], Font, headerBrush, rect, sf);
                }

                // Draw Row Headers (1-10)
                for (int y = 0; y < BoardSize; y++)
                {
                    var rect = new RectangleF(startX - headerSize, startY + y * cellSize, headerSize, cellSize);
                    g.DrawString((y + 1).ToString(), Font, headerBrush, rect, sf);
                }
            }

            // Colors
            Color waterColor = Color.FromArgb(16, 44, 87);
            Color waterGridPen = Color.FromArgb(40, 75, 120);
            Color shipColor = Color.FromArgb(46, 125, 50); // Зелений корабель
            Color shipBorder = Color.FromArgb(76, 175, 80);
            Color hitColor = Color.FromArgb(229, 57, 53); // Червоне влучання
            Color killColor = Color.FromArgb(183, 28, 28); // Темно-червоний знищений

            using (var gridPen = new Pen(waterGridPen, 1f))
            using (var waterBrush = new SolidBrush(waterColor))
            using (var shipBrush = new SolidBrush(shipColor))
            using (var shipPen = new Pen(shipBorder, 2f))
            using (var hitBrush = new SolidBrush(hitColor))
            using (var killBrush = new SolidBrush(killColor))
            using (var missDotBrush = new SolidBrush(Color.FromArgb(200, 225, 255)))
            using (var hitCrossPen = new Pen(Color.White, 2.5f))
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    for (int y = 0; y < BoardSize; y++)
                    {
                        var cellRect = new Rectangle(startX + x * cellSize, startY + y * cellSize, cellSize, cellSize);

                        // Fill base water
                        g.FillRectangle(waterBrush, cellRect);

                        // Hover effect
                        if (IsInteractive && _hoverX == x && _hoverY == y && Cells[x, y] != ClientCellState.Hit && Cells[x, y] != ClientCellState.Miss && Cells[x, y] != ClientCellState.Killed)
                        {
                            using (var hoverBrush = new SolidBrush(Color.FromArgb(60, 255, 255, 255)))
                            {
                                g.FillRectangle(hoverBrush, cellRect);
                            }
                        }

                        var state = Cells[x, y];

                        if (state == ClientCellState.Ship)
                        {
                            var innerRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                            g.FillRectangle(shipBrush, innerRect);
                            g.DrawRectangle(shipPen, innerRect);
                        }
                        else if (state == ClientCellState.Miss)
                        {
                            // Біла/блакитна крапка по центру
                            int dotSize = Math.Max(6, cellSize / 4);
                            int dotX = cellRect.X + (cellSize - dotSize) / 2;
                            int dotY = cellRect.Y + (cellSize - dotSize) / 2;
                            g.FillEllipse(missDotBrush, dotX, dotY, dotSize, dotSize);
                        }
                        else if (state == ClientCellState.Hit)
                        {
                            // Червоний фон з білим хрестиком
                            var innerRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                            g.FillRectangle(hitBrush, innerRect);

                            int pad = cellSize / 4;
                            g.DrawLine(hitCrossPen, cellRect.X + pad, cellRect.Y + pad, cellRect.Right - pad, cellRect.Bottom - pad);
                            g.DrawLine(hitCrossPen, cellRect.Right - pad, cellRect.Y + pad, cellRect.X + pad, cellRect.Bottom - pad);
                        }
                        else if (state == ClientCellState.Killed)
                        {
                            // Знищений корабель (темно-червоний з хрестиком)
                            var innerRect = new Rectangle(cellRect.X + 2, cellRect.Y + 2, cellRect.Width - 4, cellRect.Height - 4);
                            g.FillRectangle(killBrush, innerRect);

                            int pad = cellSize / 4;
                            g.DrawLine(hitCrossPen, cellRect.X + pad, cellRect.Y + pad, cellRect.Right - pad, cellRect.Bottom - pad);
                            g.DrawLine(hitCrossPen, cellRect.Right - pad, cellRect.Y + pad, cellRect.X + pad, cellRect.Bottom - pad);
                        }

                        // Grid border
                        g.DrawRectangle(gridPen, cellRect);
                    }
                }

                // Placement Preview
                if (IsInteractive && IsPlacementMode && _hoverX >= 0 && _hoverY >= 0)
                {
                    Color previewColor = PlacementIsValid ? Color.FromArgb(120, 76, 175, 80) : Color.FromArgb(120, 244, 67, 54);
                    Color previewBorder = PlacementIsValid ? Color.FromArgb(200, 129, 199, 132) : Color.FromArgb(200, 239, 83, 80);

                    using (var previewBrush = new SolidBrush(previewColor))
                    using (var previewPen = new Pen(previewBorder, 2f))
                    {
                        for (int i = 0; i < PlacementShipSize; i++)
                        {
                            int px = PlacementIsHorizontal ? _hoverX + i : _hoverX;
                            int py = PlacementIsHorizontal ? _hoverY : _hoverY + i;

                            if (px >= 0 && px < BoardSize && py >= 0 && py < BoardSize)
                            {
                                var pRect = new Rectangle(startX + px * cellSize + 2, startY + py * cellSize + 2, cellSize - 4, cellSize - 4);
                                g.FillRectangle(previewBrush, pRect);
                                g.DrawRectangle(previewPen, pRect);
                            }
                        }
                    }
                }

                // Outer border around whole grid
                using (var outerPen = new Pen(Color.FromArgb(70, 130, 180), 2f))
                {
                    g.DrawRectangle(outerPen, startX, startY, cellSize * BoardSize, cellSize * BoardSize);
                }
            }
        }
    }
}
