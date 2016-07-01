using System.Collections.Generic;

namespace ConsoleApplication1.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public ICollection<Perfil> Perfil { get; set; }
    }
}
