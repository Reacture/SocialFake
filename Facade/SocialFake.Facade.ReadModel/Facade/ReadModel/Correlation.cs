using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialFake.Facade.ReadModel
{
    public class Correlation
    {
        public Guid Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsClustered = true)]
        public long SequenceId { get; set; }
    }
}
