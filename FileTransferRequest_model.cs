using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferRequest_model
{
    public class FileTransferRequest
    {
        public int Id { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public string FilePath { get; set; }
        public bool? IsAccepted { get; set; }
    }


}
