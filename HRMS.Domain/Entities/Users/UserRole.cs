﻿using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HRMS.Domain.Entities.Users
{
    [Table("RolUsuario")]
    public class UserRole : AuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRolUsuario { get; set; }
        public string? Descripcion { get; set; }
    }
}