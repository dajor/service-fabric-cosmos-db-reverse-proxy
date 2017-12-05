using CommonBase.Lexicon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommonBase.Models
{
    public class ResponseMessage
    {
        [JsonProperty(ResponseMessageFields.ErrorCode)]
        public int ErrorCode { get; set; }

        [JsonProperty(ResponseMessageFields.Message)]
        public string Message { get; set; }

        [JsonProperty(ResponseMessageFields.Parameters)]
        public string[] Parameters { get; set; }
    }
}
