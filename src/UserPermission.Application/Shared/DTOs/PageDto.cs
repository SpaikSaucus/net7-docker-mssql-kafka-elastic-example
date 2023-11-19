using System.Collections.Generic;

namespace UserPermission.Application.Shared.DTOs
{
    public class PageDto<T> where T : class
    {
        public long Total { get; set; }

        public uint Offset { get; set; }

        public ushort Limit { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
