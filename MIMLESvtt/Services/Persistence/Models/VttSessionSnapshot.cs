using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Persistence.Models
{
    public class VttSessionSnapshot
    {
        public int Version { get; set; }

        public VttSession? VttSession { get; set; }
    }
}
