﻿
namespace HRMS.Domain.Entities.RoomManagement;

public sealed class Servicios 
{
    public short Id { get; set; }
    public string Nombre { get; set; } = String.Empty;
    public string Descripcion { get; set; } = String.Empty;
}