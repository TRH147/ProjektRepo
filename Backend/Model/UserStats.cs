using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisztracioTest.Model
{
    public class UserStats
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }
        public int Kills { get; set; }
        public int Death { get; set; }
        public int Score { get; set; }

        public virtual User User { get; set; }
    }
}
