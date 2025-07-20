using Microsoft.Extensions.VectorData;

namespace ChatApp.Core.Domain.Models
{
    public class DocumentChunkRecord
    {
        [VectorStoreKey]
        public required Guid Key { get; init; }

        [VectorStoreData]
        public required string FileName { get; init; }

        [VectorStoreData]
        public required int ChunkIndex { get; init; }

        [VectorStoreData]
        public required string Text { get; init; }

        //adjusted to nomic-embed-text model
        [VectorStoreVector(768)]
        public ReadOnlyMemory<float> Embedding { get; set; }
    }
}
