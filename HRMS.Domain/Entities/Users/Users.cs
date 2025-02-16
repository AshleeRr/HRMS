﻿using HRMS.Domain.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Users
{
    [Table("Usuario")]
    public class Users : UserAuditEntity
    {
        [Key]
        public int IdUsuario { get; set; }

        public string? Clave { get; set; }
        [ForeignKey("UserRole")]
        public int? IdRolUsuario { get; set; } // FK

    }
}

