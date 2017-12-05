using AuditingService.Lexicon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Models
{
    public class AuditModel
    {
        [JsonProperty(CommonFields.Id)]
        public string Id { get; set; }

        [Required]
        [JsonProperty(AuditFields.TimeStamp)]
        public DateTime Timestamp { get; set; }

        [JsonProperty(AuditFields.UserId)]
        public string UserId { get; set; }

        [JsonProperty(AuditFields.UserName)]
        public string UserName { get; set; }

        [Required]
        [JsonProperty(CommonFields.Type)]
        public string Type { get; set; }

        [JsonProperty(AuditFields.Information)]
        public string Information { get; set; }
    }
}
