using System.Collections.Generic;

namespace ConsoleApplication1.DTO
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<PerfilDTO> Perfil { get; set; }
    }
}
