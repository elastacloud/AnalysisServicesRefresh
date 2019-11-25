using System;

namespace AnalysisServicesRefresh.BLL.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }
    }
}