﻿using HRMS.Application.DTOs.BaseDTO;

namespace HRMS.Application.DTOs.UsersDTOs.UserDTOs
{
    public class RemoveUserClientDTO : SoftDeleteBaseDTO
    {
        public int IdUsuario { get; set; }
    }
}
