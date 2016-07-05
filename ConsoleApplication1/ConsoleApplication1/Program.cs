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
                Perfil = new List<Perfil> 
                {
                    new Perfil
                    { 
                        Tipo = "Normal",
                        Entrega = new Entrega 
                        {
                            Proveedor = "Nicolas"
                        }
                    },
                    new Perfil 
                    {
                        Tipo = "Punchi",
                        Entrega = new Entrega 
                        {
                            Proveedor = "Jebus"
                        }
                    }
                }
            };

            var listOfObjects = new List<Usuario>();
            listOfObjects.Add(objeto);
            listOfObjects.Add(objeto);

            //Inicializamos el mapper con nuestra configuracion para adaptarla a viewModel
            Mapper.ConfigureMapper(new MapperConfiguration { Name = "ViewModel" });

            /*Convetimos Entidad a DTO*/
            List<UsuarioViewModel> resultadosViewModel = listOfObjects.MapList();

            /*Convertimos los ViewModel a Entidades*/
            List<Usuario> entidadesDeViewModel = resultadosViewModel.MapToEntityList();

            /*
            //Inicializamos el mapper con nuestra configuracion para adaptarla a DTOS
            Mapper.ConfigureMapper(new MapperConfiguration { Name = "DTO" });

            //Convertimos el resultado de ViewModels a DTO
            List<UsuarioDTO> resultadosDTO = resultadosViewModel.MapList();

            //Convetimos los DTO a entidad
            var resultEntity = resultadosDTO.MapToEntityList();*/

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Todo correcto");
            Console.ReadLine();

        }
    }
}
