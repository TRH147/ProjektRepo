using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisztracioTest.Model
{
    public class UserStats
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }  // UserId-t is jelenti egyben

        public int Matches { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Kills { get; set; }

        public virtual User User { get; set; }
    }
}
