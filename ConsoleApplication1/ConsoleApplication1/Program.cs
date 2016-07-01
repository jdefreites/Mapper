using ConsoleApplication1.DTO;
using ConsoleApplication1.Entidades;
using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            var objeto = new Usuario
            {
                Id = 1,
                Nombre = "Nicolas",
                Perfil = new List<Perfil> { new Perfil { Tipo = "Normal", Entrega = new Entrega { Proveedor = "Nicolas" } }, new Perfil { Tipo = "Punchi", Entrega = new Entrega { Proveedor = "Jebus" } } }
            };

            var listOfObjects = new List<Usuario>();
            listOfObjects.Add(objeto);
            listOfObjects.Add(objeto);

            /*Convetimos Entidad a DTO*/
            var resultado = listOfObjects.MapList(typeof(List<UsuarioDTO>));

            /*Otra forma de convertir*/
            /*
                 * var resultado = Mapper.MapList(listOfObjects, typeof(List<UsuarioDTO>));
                 * 
             */

            /*Convetimos DTO a Entidad*/
            var otro = ((IList<UsuarioDTO>) resultado).MapDTOList(typeof(List<Usuario>));

            /*Otra forma de convertir*/
            /*
                * var resultado = Mapper.MapDTOList(listOfObjects, typeof(List<Usuario>));
                * 
            */

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Todo correcto");
            Console.ReadLine();

        }
    }
}
