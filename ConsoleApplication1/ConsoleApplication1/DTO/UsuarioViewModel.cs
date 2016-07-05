using System.Collections.Generic;

namespace ConsoleApplication1.DTO
{
    public class UsuarioViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<PerfilViewModel> Perfil { get; set; }
    }
}
