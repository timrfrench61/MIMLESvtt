using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src
{
    public class TableSessionSnapshot
    {
        public int Version { get; set; }

        public VttSession? TableSession { get; set; }
    }
}
