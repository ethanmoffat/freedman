using Azure;
using Azure.Data.Tables;
using System;

namespace freedman
{
    public class PrecisionTableRecord : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; } = "precision";
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public long GuildId
        {
            get => long.Parse(PartitionKey);
            set => PartitionKey = value.ToString();
        }

        public int Precision { get; set; }
    }
}
