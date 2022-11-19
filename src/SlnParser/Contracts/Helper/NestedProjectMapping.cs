using System;

namespace SlnParser.Contracts.Helper
{
    internal class NestedProjectMapping
    {
        public NestedProjectMapping(
            string targetId,
            string destinationId)
        {
            TargetId = new Guid(targetId);
            DestinationId = new Guid(destinationId);
        }

        public Guid TargetId { get; }

        public Guid DestinationId { get; }
    }
}
