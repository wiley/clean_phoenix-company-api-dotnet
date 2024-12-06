using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyAPI.Domain
{
	public class OrganizationUserRoleHistory
	{
		[Required]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrganizationUserRoleHistoryId { get; set; }

		[Required]
		public int OrganizationId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int OrganizationRoleId { get; set; }

		[Required]
		public int ChangedByUserId { get; set; }

		[Required]
		public bool WasDeleted { get; set; }

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		public static OrganizationUserRoleHistory CreateOrganizationUserRoleHistory(OrganizationUserRole org, bool delete)
		{
			//Since group history just indicates the status of a Group before it is changed, create a Group History from a Group object
			return new OrganizationUserRoleHistory()
			{
				OrganizationId = org.OrganizationId,
				UserId = org.UserId,
				OrganizationRoleId = org.OrganizationRoleId,
				ChangedByUserId = org.GrantedByUserId,
				CreatedAt = DateTime.Now,
				WasDeleted = delete
			};
		}
	}
}
