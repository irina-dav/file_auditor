using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    [Table("RuleAudit")]
    public class CRule
    {
        public static CRule CreateRule(Guid ruleId, string folder, bool includeSubfolders, string email, bool notify,
            string masksInclude, string masksExclude, EFileEvents fileEvents, ERuleState state, Guid clientId)
        {
            return new CRule()
            {
                RuleId = ruleId,
                Folder = folder,
                IncludeSubfolders = includeSubfolders,
                Email = email,
                Notify = notify,
                MasksExclude = masksExclude,
                MasksInclude = masksInclude,
                FileEvents = fileEvents,
                State = state,
                ClientInfoId = clientId
            };
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid RuleId { get; set; }

        [NotMapped]
        public CFileSystemPath FolderPath => new CFileSystemPath(Folder);

        public string Folder { get; set; }

        public bool IncludeSubfolders { get; set; }

        public string Email { get; set; }

        public bool Notify { get; set; }

        public string MasksInclude { get; set; }

        public string MasksExclude { get; set; }

        public EFileEvents FileEvents { get; set; }

        public ERuleState State { get; set; }

        public Guid ClientInfoId { get; set; }

    }
}
