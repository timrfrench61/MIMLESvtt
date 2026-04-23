namespace MIMLESvtt.src.Domain.Models.Placement
{
    public static class SquareGridCoordinateHelper
    {
        public static IReadOnlyList<Coordinate> GetOrthogonalNeighbors(Coordinate origin)
        {
            ArgumentNullException.ThrowIfNull(origin);

            var (x, y) = ToGrid(origin);
            return
            [
                CreateCoordinate(x, y - 1),
                CreateCoordinate(x + 1, y),
                CreateCoordinate(x, y + 1),
                CreateCoordinate(x - 1, y)
            ];
        }

        public static IReadOnlyList<Coordinate> GetNeighborsIncludingDiagonals(Coordinate origin)
        {
            ArgumentNullException.ThrowIfNull(origin);

            var (x, y) = ToGrid(origin);
            return
            [
                CreateCoordinate(x - 1, y - 1),
                CreateCoordinate(x, y - 1),
                CreateCoordinate(x + 1, y - 1),
                CreateCoordinate(x - 1, y),
                CreateCoordinate(x + 1, y),
                CreateCoordinate(x - 1, y + 1),
                CreateCoordinate(x, y + 1),
                CreateCoordinate(x + 1, y + 1)
            ];
        }

        public static int ManhattanDistance(Coordinate from, Coordinate to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var (fromX, fromY) = ToGrid(from);
            var (toX, toY) = ToGrid(to);

            return Math.Abs(toX - fromX) + Math.Abs(toY - fromY);
        }

        public static int ChebyshevDistance(Coordinate from, Coordinate to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var (fromX, fromY) = ToGrid(from);
            var (toX, toY) = ToGrid(to);

            return Math.Max(Math.Abs(toX - fromX), Math.Abs(toY - fromY));
        }

        private static (int x, int y) ToGrid(Coordinate coordinate)
        {
            return ((int)MathF.Round(coordinate.X), (int)MathF.Round(coordinate.Y));
        }

        private static Coordinate CreateCoordinate(int x, int y)
        {
            return new Coordinate
            {
                X = x,
                Y = y
            };
        }
    }
}
