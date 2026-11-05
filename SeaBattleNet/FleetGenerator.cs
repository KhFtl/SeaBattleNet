using SeaBattle.Protocol;

namespace SeaBattleNet
{
    public enum ClientCellState
    {
        Empty = 0,
        Ship = 1,
        Miss = 2,
        Hit = 3,
        Killed = 4
    }

    public class ClientShip
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Size { get; set; }
        public bool IsHorizontal { get; set; }

        public ClientShip(int x, int y, int size, bool isHorizontal)
        {
            X = x;
            Y = y;
            Size = size;
            IsHorizontal = isHorizontal;
        }

        public List<(int X, int Y)> GetCells()
        {
            var cells = new List<(int X, int Y)>();
            for (int i = 0; i < Size; i++)
            {
                int cx = IsHorizontal ? X + i : X;
                int cy = IsHorizontal ? Y : Y + i;
                cells.Add((cx, cy));
            }
            return cells;
        }
    }

    public static class FleetGenerator
    {
        public static List<ShipPlacementDto> GenerateRandomFleet()
        {
            var fleetSizes = new[] { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            bool[,] occupied = new bool[10, 10];
            var ships = new List<ShipPlacementDto>();

            foreach (var size in fleetSizes)
            {
                bool placed = false;
                int attempts = 0;

                while (!placed && attempts < 1000)
                {
                    attempts++;
                    bool isHorizontal = Random.Shared.Next(2) == 0;
                    int maxX = isHorizontal ? 10 - size : 9;
                    int maxY = isHorizontal ? 9 : 10 - size;

                    int x = Random.Shared.Next(maxX + 1);
                    int y = Random.Shared.Next(maxY + 1);

                    if (CanPlace(x, y, size, isHorizontal, occupied))
                    {
                        for (int i = 0; i < size; i++)
                        {
                            int cx = isHorizontal ? x + i : x;
                            int cy = isHorizontal ? y : y + i;
                            occupied[cx, cy] = true;
                        }

                        ships.Add(new ShipPlacementDto
                        {
                            X = x,
                            Y = y,
                            Size = size,
                            IsHorizontal = isHorizontal
                        });
                        placed = true;
                    }
                }
            }

            return ships;
        }

        private static bool CanPlace(int x, int y, int size, bool isHorizontal, bool[,] occupied)
        {
            for (int i = 0; i < size; i++)
            {
                int cx = isHorizontal ? x + i : x;
                int cy = isHorizontal ? y : y + i;

                if (cx < 0 || cx >= 10 || cy < 0 || cy >= 10) return false;

                // Перевірка 1 клітинки навколо включно з діагоналями
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        int nx = cx + dx;
                        int ny = cy + dy;
                        if (nx >= 0 && nx < 10 && ny >= 0 && ny < 10)
                        {
                            if (occupied[nx, ny]) return false;
                        }
                    }
                }
            }
            return true;
        }
    }
}
