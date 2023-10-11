namespace DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    // інформація про ігрову сесію
    public class GameInfo
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        [Required]
        [MaxLength(100)]
        public string Player1 { get; set; }
        [Required]
        [MaxLength(100)]
        public string Player2 { get; set; }
    }
}