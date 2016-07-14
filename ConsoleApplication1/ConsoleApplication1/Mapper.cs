using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    public class MapperConfiguration
    {
        public string Name { get; set; }
        public IList<string> AssemblyList { get; set; }
        public List<Assembly> Classes { get; set; }
    }
    
    public static class Mapper
    {
        private static MapperConfiguration _mapperConfiguration = new MapperConfiguration { Name = "DTO" };

        public static void ConfigureMapper(MapperConfiguration mapperConfiguration)
        {
            //_mapperConfiguration.Name = mapperConfiguration.Name;
            _mapperConfiguration = mapperConfiguration;

            if ( mapperConfiguration.AssemblyList != null)
            {
                foreach(var item in mapperConfiguration.AssemblyList)
                {
                    var assemblyTempList = Assembly.ReflectionOnlyLoad(item);
                    _mapperConfiguration.Classes.Add(assemblyTempList);
                }
                
            }
        }

        #region Mapper con configuracion

        public static U MapProperty<T,U>(this T source, IDictionary<string,string> configurationMapper)
            where T : class, new()
            where U : class, new()
        {
            if ( source == null || configurationMapper == null)
            {
                return default(U);
            }

            /*Obtenemos las properties del object source*/
            var sourceProperties = source.GetType().GetProperties();

            var destinyInstance = Activator.CreateInstance(typeof(U));
            var destinyProperties = destinyInstance.GetType().GetProperties();

            foreach(var i in sourceProperties)
            {
                //Buscamos si la property que quiere copiar tiene source
                var firstFind = configurationMapper.FirstOrDefault(m => m.Key.Contains(i.Name));
                if (string.IsNullOrEmpty(firstFind.Key) || string.IsNullOrEmpty(firstFind.Value))
                    continue;

                var secondFind = destinyProperties.FirstOrDefault(m => m.Name.Contains(firstFind.Value));
                if ( secondFind == null)
                {
                    continue;
                }

                if ( i.Name.Contains("Collection"))
                {

                }

                secondFind.SetValue(destinyInstance, i.GetValue(source));

            }

            return (U)destinyInstance;
        }

        #endregion

        #region Formula 1

        /*Metodo para mapear simples objects*/
        public static dynamic Map<T>(this T source)
            where T : class, new()
        {
            //Check for source
            if (source == null )
                return null;

            //Donde almacenaremos el tipo de destino
            Type destinyType = null;

            //Obtenemos todas las clases
            var classList = Assembly.GetExecutingAssembly().GetTypes();

            //Obtenemos las properties del source
            var propertiesSource = source.GetType().GetProperties();

            //Si no podemos obtener clases o properties
            if ( propertiesSource == null || classList == null )
            {
                return new Exception("Error al obtener clases o properties desde el assembly");
            }

            if (source.GetType().ToString().Contains("Collection"))
            {
                if (source.GetType().Name.Contains("ViewModel") || source.GetType().Name.Contains("DTO") || source.GetType().Name.Contains(_mapperConfiguration.Name))
                {
                    var firstTest = Regex.Split(propertiesSource.FirstOrDefault().Name, "ViewModel")[0];
                    if (string.IsNullOrEmpty(firstTest))
                    {
                        firstTest = "DTO";
                    }
                    else
                    {
                        firstTest = "ViewModel";
                    }

                    destinyType = classList.FirstOrDefault(
                        m => m.Name.Contains(Regex.Split(propertiesSource.FirstOrDefault().Name,firstTest)[0] + _mapperConfiguration.Name));
                }
                else
                {
                    destinyType = classList.FirstOrDefault(
                        m => m.Name.Contains(propertiesSource.FirstOrDefault().Name + _mapperConfiguration.Name));
                }
            }
            else
            {
                if (source.GetType().Name.Contains("ViewModel") || source.GetType().Name.Contains("DTO") || source.GetType().Name.Contains(_mapperConfiguration.Name))
                {
                    var firstTest = Regex.Split(source.GetType().Name, "ViewModel")[0];
                    if (string.IsNullOrEmpty(firstTest))
                    {
                        firstTest = "DTO";
                    } else
                    {
                        firstTest = "ViewModel";
                    }

                    destinyType = classList.FirstOrDefault(
                        m => m.Name.Contains(Regex.Split(source.GetType().Name,firstTest)[0] + _mapperConfiguration.Name));
                }
                else
                {
                    destinyType = classList.FirstOrDefault(
                        m => m.Name.Contains(source.GetType().Name + _mapperConfiguration.Name));
                }
            }

            try
            {

                //Creamos la instancia del object
                var instance = Activator.CreateInstance(destinyType);

                //Obtenemos las properties del object de destino
                var propertyOfDestiny = instance.GetType().GetProperties();

                foreach (var item in propertyOfDestiny)
                {
                    //Buscamos la property
                    var result = propertiesSource.FirstOrDefault(m => m.Name == item.Name);
                    if (result != null)
                    {
                        //En caso de que no sea un Object de tipo distinto a los nativos de C#
                        if ((!(result).PropertyType.Name.Contains("String")) && (!(result).PropertyType.Name.Contains("Int"))
                            && (!(result).PropertyType.Name.Contains("Bool")) && (!(result).PropertyType.Name.Contains("Float"))
                            && (!(result).PropertyType.Name.Contains("Double")))
                        {
                            //En caso de ser lista
                            if (result.ToString().Contains("Collection"))
                            {
                                Type type = null;

                                if (result.PropertyType.GetGenericArguments().Single().Name.Contains("ViewModel") || result.PropertyType.GetGenericArguments().Single().Name.Contains("DTO") ||
                                    result.PropertyType.GetGenericArguments().Single().Name.Contains(_mapperConfiguration.Name))
                                {
                                    var firstTest = Regex.Split(result.PropertyType.GetGenericArguments().Single().Name, "ViewModel")[0];
                                    if (string.IsNullOrEmpty(firstTest))
                                    {
                                        firstTest = "DTO";
                                    } else
                                    {
                                        firstTest = "ViewModel";
                                    }

                                    type = classList.FirstOrDefault(m => m.FullName.Contains(Regex.Split(result.PropertyType.GetGenericArguments().Single().Name,firstTest)[0] + _mapperConfiguration.Name));
                                }
                                else
                                {
                                    type = classList.FirstOrDefault(m => m.FullName.Contains(result.PropertyType.GetGenericArguments().Single().Name + _mapperConfiguration.Name));
                                }

                                if (type == null)
                                {
                                    continue;
                                }

                                var listInstance = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(null);

                                item.SetValue(instance, ((ICollection)result.GetValue(source)).MapList());
                            }
                            else
                            {
                                Type type = null;
                                if (result.Name.Contains("ViewModel") || result.Name.Contains("DTO") ||
                                    result.Name.Contains(_mapperConfiguration.Name))
                                {

                                    var firstTest = Regex.Split(result.Name, "ViewModel")[0];
                                    if (string.IsNullOrEmpty(firstTest))
                                    {
                                        firstTest = "DTO";
                                    }
                                    else
                                    {
                                        firstTest = "ViewModel";
                                    }

                                    //En caso de ser un object
                                    type = classList.FirstOrDefault(m => m.FullName.Contains(Regex.Split(result.Name,firstTest)[0] + _mapperConfiguration.Name));
                                }
                                else
                                {
                                    //En caso de ser un object
                                    type = classList.FirstOrDefault(m => m.FullName.Contains(result.Name + _mapperConfiguration.Name));
                                }
                                if (type == null)
                                {
                                    continue;
                                }

                                item.SetValue(instance, result.GetValue(source).Map());
                            }
                        }
                        else
                        {
                            //Property normal
                            item.SetValue(instance, result.GetValue(source));
                        }
                    }
                }

                return instance;

            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        /*Metodos para mapear listas*/
        public static dynamic MapList<T>(this T source) 
            where T : ICollection
        {
            //Check for source
            if (source == null)
                return null;

            if (source.GetType().GetGenericArguments().Count() == 0)
                return null;

            Type destinyType = null;
            bool isCollection = false;

            if (source.GetType().ToString().Contains("Collection"))
            {
                foreach(var i in source)
                {
                    if ( i.GetType().Name.Contains("ViewModel") || i.GetType().Name.Contains("DTO") || i.GetType().Name.Contains(_mapperConfiguration.Name))
                    {
                        var firstTest = Regex.Split(i.GetType().Name, "ViewModel")[0];
                        if (string.IsNullOrEmpty(firstTest))
                        {
                            firstTest = Regex.Split(i.GetType().Name, "DTO")[0];
                        }

                        destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                            m => m.Name.Contains(firstTest + _mapperConfiguration.Name));
                    }
                    else
                    {
                        destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                            m => m.Name.Contains(i.GetType().Name + _mapperConfiguration.Name));
                    }

                    isCollection = true;

                    break;
                }
            }

            if (!isCollection)
                return new Exception("Este object no es una lista de tipo coleccion");

            var listType = source.GetType().GetGenericArguments().Single();

            //Check if the type of object is or not a collection list.
            if (source.GetType().ToString().Contains("Collection"))
            {
                try
                {
                    dynamic list;

                    //Create instance of list
                    if ( isCollection)
                    {
                        list = (IList)typeof(List<>).MakeGenericType(destinyType).GetConstructor(Type.EmptyTypes).Invoke(null);
                    } else
                    {
                        list = Activator.CreateInstance(destinyType);
                    }

                    //Loop in the list
                    foreach (var i in source)
                    {
                        if (i.GetType().ToString().Contains("Collection"))
                        {
                            var mapped = ((ICollection)i).MapList();

                            foreach (var item in mapped)
                            {
                                ((IList<object>)list).Add(item);
                            }

                        }
                        else
                        {
                            ((IList)list).Add(i.Map());
                        }

                    }

                    return list;
                }
                catch (Exception e)
                {
                    return e.Message;
                }

            }

            return null;
        }

        #endregion

        #region Formula 2

        /*Metodo para mapear simples objects*/
        public static dynamic MapToEntity<T>(this T source)
            where T : class, new()
        {
            //Check for source
            if (source == null)
                return null;

            Type destinyType = null;

            //Obtenemos todas las clases
            var classList = Assembly.GetExecutingAssembly().GetTypes();

            //Obtenemos las properties del source
            var propertiesSource = source.GetType().GetProperties();

            //Si no podemos obtener clases o properties
            if ( propertiesSource == null || classList == null )
            {
                return new Exception("Error al obtener clases o properties desde el assembly");
            }

            if (source.GetType().ToString().Contains("Collection"))
            {
                destinyType = classList.FirstOrDefault(
                    m => m.Name.Contains(Regex.Split(propertiesSource.FirstOrDefault().Name,_mapperConfiguration.Name)[0]));

            }
            else
            {
                destinyType = classList.FirstOrDefault(
                    m => m.Name.Contains(Regex.Split(source.GetType().Name,_mapperConfiguration.Name)[0]));
            }

            try
            {
                //Creamos la instancia del object
                var instance = Activator.CreateInstance(destinyType);

                //Obtenemos las properties del object de destino
                var propertyOfDestiny = instance.GetType().GetProperties();

                foreach (var item in propertyOfDestiny)
                {
                    //Buscamos la property
                    var result = propertiesSource.FirstOrDefault(m => m.Name == item.Name);
                    if (result != null)
                    {
                        //En caso de que no sea un Object de tipo distinto a los nativos de C#
                        if ((!(result).PropertyType.Name.Contains("String")) && (!(result).PropertyType.Name.Contains("Int"))
                            && (!(result).PropertyType.Name.Contains("Bool")) && (!(result).PropertyType.Name.Contains("Float"))
                            && (!(result).PropertyType.Name.Contains("Double")))
                        {
                            //En caso de ser lista
                            if (result.ToString().Contains("Collection"))
                            {
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(Regex.Split(result.PropertyType.GetGenericArguments()
                                    .Single().Name,_mapperConfiguration.Name)[0]));

                                if (type == null)
                                {
                                    continue;
                                }

                                var listInstance = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(null);

                                item.SetValue(instance, ((ICollection)result.GetValue(source)).MapToEntityList());
                            }
                            else
                            {
                                //En caso de ser un object
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(Regex.Split(result.Name,_mapperConfiguration.Name)[0]));

                                if (type == null)
                                {
                                    continue;
                                }

                                item.SetValue(instance, result.GetValue(source).MapToEntity());
                            }
                        }
                        else
                        {
                            //Property normal
                            item.SetValue(instance, result.GetValue(source));
                        }
                    }
                }

                return instance;

            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        /*Metodos para mapear listas*/
        public static dynamic MapToEntityList<T>(this T source)
            where T : ICollection
        {

            //Check for source
            if (source == null)
                return null;

            if (source.GetType().GetGenericArguments().Count() == 0)
                return null;

            Type destinyType = null;
            bool isCollection = false;

            if (source.GetType().ToString().Contains("Collection"))
            {
                foreach (var i in source)
                {
                    destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(Regex.Split(i.GetType().Name,_mapperConfiguration.Name)[0]));

                    isCollection = true;

                    break;
                }
            }

            if (!isCollection)
                return new Exception("Este object no es una lista de tipo coleccion");

            var listType = source.GetType().GetGenericArguments().Single();

            //Check if the type of object is or not a collection list.
            if (source.GetType().ToString().Contains("Collection"))
            {
                try
                {
                    dynamic list;

                    //Create instance of list
                    if (isCollection)
                    {
                        list = (IList)typeof(List<>).MakeGenericType(destinyType).GetConstructor(Type.EmptyTypes).Invoke(null);
                    }
                    else
                    {
                        list = Activator.CreateInstance(destinyType);
                    }

                    //Loop in the list
                    foreach (var i in source)
                    {
                        if (i.GetType().ToString().Contains("Collection"))
                        {
                            var mapped = ((ICollection)i).MapToEntityList();

                            foreach (var item in mapped)
                            {
                                ((IList<object>)list).Add(item);
                            }

                        }
                        else
                        {
                            ((IList)list).Add(i.MapToEntity());
                        }

                    }

                    return list;
                }
                catch (Exception e)
                {
                    return e.Message;
                }

            }

            return null;
        }

        #endregion
    }
}
