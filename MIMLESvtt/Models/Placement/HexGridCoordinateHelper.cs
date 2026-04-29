namespace MIMLESvtt.src.Domain.Models.Placement
{
    public static class HexGridCoordinateHelper
    {
        private static readonly (int dq, int dr)[] NeighborOffsets =
        [
            (1, 0),
            (1, -1),
            (0, -1),
            (-1, 0),
            (-1, 1),
            (0, 1)
        ];

        public static IReadOnlyList<HexCoordinate> GetNeighbors(HexCoordinate origin)
        {
            ArgumentNullException.ThrowIfNull(origin);

            return NeighborOffsets
                .Select(offset => new HexCoordinate
                {
                    Q = origin.Q + offset.dq,
                    R = origin.R + offset.dr
                })
                .ToList();
        }

        public static int AxialDistance(HexCoordinate from, HexCoordinate to)
        {
            ArgumentNullException.ThrowIfNull(from);
            ArgumentNullException.ThrowIfNull(to);

            var qDistance = from.Q - to.Q;
            var rDistance = from.R - to.R;
            var sDistance = (-from.Q - from.R) - (-to.Q - to.R);

            return (Math.Abs(qDistance) + Math.Abs(rDistance) + Math.Abs(sDistance)) / 2;
        }
    }
}
