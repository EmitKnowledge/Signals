using App.Domain.Entities.Common;
using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class GetUsersListingRequestDto : IDtoData
    {
        /// <summary>
        /// Filtering query
        /// </summary>
        public SortableQueryOptions Query { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            if (!Query.OrderBy.IsNull()) Query.OrderBy = sanitizer.Sanitize(Query.OrderBy);
        }
    }
}