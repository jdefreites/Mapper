using ConsoleApplication1.DTO;
using ConsoleApplication1.Entidades;
using System;
using System.Collections.Generic;

public class Uno
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }

    public void Saludar(string message, string another)
    {
        Console.WriteLine(message + "\n" + another);
        return;
    }
}

public class Dos
{
    public string Name { get; set; }
    public string SurName { get; set; }

    public void Saludar(string message)
    {
        Console.WriteLine(message);
        return;
    }
}

namespace ConsoleApplication1
{
    public static class RemoteCall
    {
        /*
         * Assembly assembly = Assembly.LoadFile("...Assembly1.dll");
         * Type type = assembly.GetType("TestAssembly.Main");
         */
        public static T Instance<T>() 
            where T : class, new()
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public static T Invoke<T>(string methodName, object[] parameters)
            where T : class, new()
        {
            var classSource = Instance<T>();
            var methods = classSource.GetType().GetMethods();

            foreach(var i in methods)
            {
                if ( i.Name == methodName)
                {
                    i.Invoke(classSource, parameters);
                }
            }

            return default(T);
        }
    }

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

            //var instanciado = RemoteCall.Instance<Uno>();
            RemoteCall.Invoke<Dos>("Saludar", new object[] { "hola amigos" } );

            var source = new Uno { Nombre = "Nicolas", Apellido = "Buzzi" };

            var dic = new Dictionary<string, string>();

            dic.Add("Nombre", "Name");
            dic.Add("Apellido", "SurName");

            var mapeo = source.MapProperty<Uno,Dos>(dic);

            var listOfObjects = new List<Usuario>();
            listOfObjects.Add(objeto);
            listOfObjects.Add(objeto);

            //Inicializamos el mapper con nuestra configuracion para adaptarla a viewModel
            Mapper.ConfigureMapper(new MapperConfiguration { Name = "ViewModel" });

            /*Convetimos Entidad a DTO*/
            List<UsuarioViewModel> resultadosViewModel = listOfObjects.MapList();

            /*Convertimos los ViewModel a Entidades*/
            List<Usuario> entidadesDeViewModel = resultadosViewModel.MapToEntityList();

            
            //Inicializamos el mapper con nuestra configuracion para adaptarla a DTOS
            Mapper.ConfigureMapper(new MapperConfiguration { Name = "DTO" });

            //Convertimos el resultado de ViewModels a DTO
            List<UsuarioDTO> resultadosDTO = resultadosViewModel.MapList();

            //Convetimos los DTO a entidad
            var resultEntity = resultadosDTO.MapToEntityList();

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine("Todo correcto");
            Console.ReadLine();

        }
    }
}
