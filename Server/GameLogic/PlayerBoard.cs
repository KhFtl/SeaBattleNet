using SeaBattle.Protocol;

namespace Server.GameLogic
{
    public class Ship
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int Size { get; set; }
        public bool IsHorizontal { get; set; }

        public List<(int X, int Y)> Cells { get; } = new();
        public HashSet<(int X, int Y)> HitCells { get; } = new();

        public bool IsSunk => HitCells.Count == Cells.Count;

        public Ship(int x, int y, int size, bool isHorizontal)
        {
            StartX = x;
            StartY = y;
            Size = size;
            IsHorizontal = isHorizontal;

            for (int i = 0; i < size; i++)
            {
                int cx = isHorizontal ? x + i : x;
                int cy = isHorizontal ? y : y + i;
                Cells.Add((cx, cy));
            }
        }

        public bool Contains(int x, int y) => Cells.Any(c => c.X == x && c.Y == y);

        public bool RegisterHit(int x, int y)
        {
            if (Contains(x, y))
            {
                HitCells.Add((x, y));
                return true;
            }
            return false;
        }

        public List<(int X, int Y)> GetSurroundingCells()
        {
            var result = new HashSet<(int X, int Y)>();
            foreach (var (cx, cy) in Cells)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = cx + dx;
                        int ny = cy + dy;
                        if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10)
                        {
                            if (!Contains(nx, ny))
                            {
                                result.Add((nx, ny));
                            }
                        }
                    }
                }
            }
            return result.ToList();
        }
    }

    public enum BoardCellState
    {
        Empty = 0,
        Ship = 1,
        Miss = 2,
        Hit = 3
    }

    public class PlayerBoard
    {
        public const int BoardSize = 10;
        public BoardCellState[,] Grid { get; } = new BoardCellState[BoardSize, BoardSize];
        public List<Ship> Ships { get; } = new();

        public bool AreAllShipsSunk => Ships.Count > 0 && Ships.All(s => s.IsSunk);

        public static (bool IsValid, string ErrorMessage, List<Ship>? Ships) ValidateAndCreate(List<ShipPlacementDto>? placements)
        {
            if (placements == null || placements.Count != 10)
            {
                return (false, "Кількість кораблів повинна дорівнювати 10.", null);
            }

            // Перевірка складу флоту: 1x4, 2x3, 3x2, 4x1
            var counts = new Dictionary<int, int> { { 4, 0 }, { 3, 0 }, { 2, 0 }, { 1, 0 } };
            foreach (var p in placements)
            {
                if (!counts.ContainsKey(p.Size))
                {
                    return (false, $"Некоректний розмір корабля: {p.Size}.", null);
                }
                counts[p.Size]++;
            }

            if (counts[4] != 1 || counts[3] != 2 || counts[2] != 3 || counts[1] != 4)
            {
                return (false, "Склад флоту має бути: 1x4-палубний, 2x3-палубних, 3x2-палубних, 4x1-палубних.", null);
            }

            var ships = new List<Ship>();
            bool[,] occupied = new bool[BoardSize, BoardSize];

            foreach (var p in placements)
            {
                // Перевірка виходу за межі поля
                int endX = p.IsHorizontal ? p.X + p.Size - 1 : p.X;
                int endY = p.IsHorizontal ? p.Y : p.Y + p.Size - 1;

                if (p.X < 0 || p.Y < 0 || endX >= BoardSize || endY >= BoardSize)
                {
                    return (false, $"Корабель розміром {p.Size} на ({p.X}, {p.Y}) виходить за межі поля 10x10.", null);
                }

                var ship = new Ship(p.X, p.Y, p.Size, p.IsHorizontal);

                // Перевірка накладання та дотику (мінімум 1 клітинка між кораблями, включаючи діагоналі)
                foreach (var (cx, cy) in ship.Cells)
                {
                    if (occupied[cx, cy])
                    {
                        return (false, $"Корабель на ({p.X}, {p.Y}) накладається на інший корабель.", null);
                    }
                }

                // Перевірка сусідніх клітинок з уже встановленими кораблями
                foreach (var existing in ships)
                {
                    foreach (var (ex, ey) in existing.Cells)
                    {
                        foreach (var (cx, cy) in ship.Cells)
                        {
                            if (Math.Abs(ex - cx) <= 1 && Math.Abs(ey - cy) <= 1)
                            {
                                return (false, $"Кораблі не можуть торкатися один одного (дистанція < 1). Позиції: ({cx},{cy}) і ({ex},{ey}).", null);
                            }
                        }
                    }
                }

                foreach (var (cx, cy) in ship.Cells)
                {
                    occupied[cx, cy] = true;
                }

                ships.Add(ship);
            }

            return (true, string.Empty, ships);
        }

        public void Initialize(List<Ship> ships)
        {
            Ships.Clear();
            Ships.AddRange(ships);

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    Grid[x, y] = BoardCellState.Empty;
                }
            }

            foreach (var s in ships)
            {
                foreach (var (cx, cy) in s.Cells)
                {
                    Grid[cx, cy] = BoardCellState.Ship;
                }
            }
        }

        public (ShotOutcome Outcome, Ship? HitShip, List<(int X, int Y)> SurroundingMisses) ProcessShot(int x, int y)
        {
            var surroundingMisses = new List<(int X, int Y)>();

            if (x < 0 || x >= BoardSize || y < 0 || y >= BoardSize)
            {
                return (ShotOutcome.Miss, null, surroundingMisses);
            }

            if (Grid[x, y] == BoardCellState.Hit || Grid[x, y] == BoardCellState.Miss)
            {
                // Клітинка вже обстріляна
                return (ShotOutcome.Miss, null, surroundingMisses);
            }

            var hitShip = Ships.FirstOrDefault(s => s.Contains(x, y));
            if (hitShip != null)
            {
                Grid[x, y] = BoardCellState.Hit;
                hitShip.RegisterHit(x, y);

                if (hitShip.IsSunk)
                {
                    // Відзначаємо сусідні клітинки як промахи
                    var surrounding = hitShip.GetSurroundingCells();
                    foreach (var (sx, sy) in surrounding)
                    {
                        if (Grid[sx, sy] == BoardCellState.Empty)
                        {
                            Grid[sx, sy] = BoardCellState.Miss;
                            surroundingMisses.Add((sx, sy));
                        }
                    }
                    return (ShotOutcome.Kill, hitShip, surroundingMisses);
                }

                return (ShotOutcome.Hit, hitShip, surroundingMisses);
            }
            else
            {
                Grid[x, y] = BoardCellState.Miss;
                return (ShotOutcome.Miss, null, surroundingMisses);
            }
        }
    }
}
