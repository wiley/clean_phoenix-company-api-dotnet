using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyAPI.Domain
{
    public class OrganizationUserRole
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int OrganizationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationRoleId { get; set; }

        [Required]
        public int GrantedByUserId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
