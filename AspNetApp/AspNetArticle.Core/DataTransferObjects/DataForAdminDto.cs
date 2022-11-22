using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetArticle.Core.DataTransferObjects
{
    public class DataForAdminDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public bool Spam { get; set; }
        public int CountCommentaries { get; set; }
        public DateTime LastVisit { get; set; }
        public DateTime AccountCreated { get; set; }
    }
}
