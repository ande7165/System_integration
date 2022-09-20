using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BookingSite.Model
{
	public class Booking
	{
		[Required]
		[NotNull]
		public string Name { get; set; }
		[Required]
		[NotNull]
		public string Email { get; set; }
		[Required]
		[NotNull]
		public string Tour { get; set; }
		[Required]
		[NotNull]
		public string Book { get; set; }
	}
}
