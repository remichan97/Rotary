using System;
using System.Collections.Generic;
using System.Text;

namespace Rotary.Core.Collections.Records
{
    public sealed record CollectionIndexDefinition
    {
        public IList<CollectionIndexEntryDefinition> Collections { get; init; }
    }
}
