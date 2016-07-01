using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApplication1
{
    public static class Mapper
    {

        /*Metodo para mapear simples objects*/
        public static dynamic Map(object source, Type destinyType)
        {
            //Check for source
            if (source == null || destinyType == null)
                return null;

            try
            {

                //Obtenemos todas las clases
                var classList = Assembly.GetExecutingAssembly().GetTypes();

                //Creamos la instancia del object
                var instance = Activator.CreateInstance(destinyType);

                //Obtenemos las properties del source
                var propertiesSource = source.GetType().GetProperties();

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
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(result.PropertyType.GetGenericArguments().Single().Name + "DTO"));
                                if (type == null)
                                {
                                    continue;
                                }

                                var listInstance = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(null);

                                item.SetValue(instance, MapList(result.GetValue(source), listInstance.GetType()));
                            }
                            else
                            {
                                //En caso de ser un object
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(result.Name + "DTO"));
                                if (type == null)
                                {
                                    continue;
                                }

                                item.SetValue(instance, Map(result.GetValue(source), type));
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
            catch ( Exception e)
            {
                return e.Message;
            }

        }

        /*Metodos para mapear listas*/
        public static dynamic MapList(object source, Type destinyType)
        {
            //Check for source
            if (source == null || destinyType == null)
                return null;

            var listType = source.GetType().GetGenericArguments().Single();

            //Check if the type of object is or not a collection list.
            if (source.GetType().ToString().Contains("Collection"))
            {
                try
                {
                    //Create instance of list
                    var list = Activator.CreateInstance(destinyType);
                
                    //Loop in the list
                    foreach (var i in (ICollection)source)
                    {
                        if (i.GetType().ToString().Contains("Collection"))
                        {
                            var mapped = MapList(i, i.GetType());

                            foreach (var item in mapped)
                            {
                                ((IList<object>)list).Add(item);
                            }

                        }
                        else
                        {
                            ((IList)list).Add(Map(i, destinyType.GetGenericArguments().Single()));
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

    }
}
