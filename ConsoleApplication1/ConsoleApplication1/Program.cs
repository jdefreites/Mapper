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

            var resultado = Mapper.MapList(listOfObjects, typeof(List<UsuarioDTO>));

            Console.ReadLine();

        }
    }
}
