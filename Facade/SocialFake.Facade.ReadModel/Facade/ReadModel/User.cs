using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialFake.Facade.ReadModel
{
    public class User
    {
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsClustered = true)]
        public long SequenceId { get; set; }

        [Index]
        [StringLength(100)]
        public string Username { get; set; }

        public string DisplayNamesJson { get; set; }

        public string Bio { get; set; }
    }
}
