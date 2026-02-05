using System.Collections.Generic;

namespace UnlockOKR.OKRSupoort.Domain.ResponseModels
{
    public class Payload<T>
    {
        public bool IsSuccess { get; set; } = true;
        public int Status { get; set; }
        public string MessageType { get; set; }
        public Dictionary<string, string> MessageList { get; set; } = new Dictionary<string, string>();
        public PageInfo PagingInfo { get; set; }
        public T Entity { get; set; }
        public List<T> EntityList { get; set; }

    }

    public class PageInfo
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
    }
}
