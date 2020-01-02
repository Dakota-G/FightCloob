using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BattlePlanner.Models
{

	public class User
	{
	    [Key]
	    public int UserId { get; set; }

	    [Required(ErrorMessage="How will the bards sing songs of your glory without a name?")]
	    public string Name { get; set; }

	    [Required(ErrorMessage="Please, let us know what your fighting class is!")]
	    public string Class { get; set; }

	    [Required(ErrorMessage="We need to know your birthdate!")]
	    public DateTime Birthdate { get; set; }

	    [EmailAddress]
	    [Required(ErrorMessage="We need an email so you can log in!")]
	    public string Email { get; set; }

	    public List<Team> Events { get; set; }

	    public List<Taunt> Taunts { get; set; }

	    [DataType(DataType.Password)]
	    [Required]
	    [MinLength(8, ErrorMessage="Password must be longer than 8 characters!")]
	    public string Password { get; set; }

	    public DateTime CreatedAt { get; set; } = DateTime.Now;

	    [NotMapped]
	    [Compare("Password")]
	    [DataType(DataType.Password)]
	    public string Confirm { get; set; }

	}

	public class Fight
	{
		[Key]
		public int FightId { get; set; }

		[Required(ErrorMessage="Where will the blood be spilt?")]
		public string Location { get; set; }

		[Required(ErrorMessage="We need to know when the brawl will take place!")]
		public DateTime FightDate { get; set; }

		public List<Team> Roster { get; set; }
		public List<Taunt> Taunts { get; set; }
	    public DateTime CreatedAt { get; set; } = DateTime.Now;
	    public int UserId { get; set; }
	    public User Creator { get; set; }
	}

	public class Team
	{
		[Key]
		public int TeamId { get; set; }
		public int UserId { get; set; }
		public int FightId { get; set; }
		public User Participant { get; set; }
		public Fight Event { get; set; }
		public string TeamColor { get; set; }

	}

	public class Taunt
	{
		[Key]
		public int TauntID { get; set; }

		[Required(ErrorMessage="You need to provide a message, you weak-livered hobgoblin!")]
		public string Message { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public int UserId { get; set; }
		public User Creator { get; set; }
		public int FightId { get; set; }
		public Fight Fight { get; set; }
	}

	public class LoginUser
	{
		[Required(ErrorMessage="You must log in with an Email!")]
		public string Email { get; set; }

		[Required(ErrorMessage="Provide a passphrase so you may enter")]
		public string Password { get; set; }
	}
}