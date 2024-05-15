using CryptoPortfolioMessageServer.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoPortfolioMessageServer.Models.Messages.Dtos
{
    internal struct MessageBusRetrievalMessage
    {
        [JsonInclude]
        public RetrievalType RetrievalType;

        [JsonInclude]
        public byte[] StructBuffer;
    }
}
