using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleApplication1
{
    public static class Mapper
    {
        #region Formula 1

        /*Metodo para mapear simples objects*/
        public static dynamic Map<T>(this T source)
            where T : class, new()
        {
            //Check for source
            if (source == null )
                return null;

            Type destinyType = null;

            if (source.GetType().ToString().Contains("Collection"))
            {
                destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(source.GetType().GetProperties().FirstOrDefault().Name + "DTO"));

            } else
            {
                destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(source.GetType().Name + "DTO"));
            }

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

                                item.SetValue(instance, ((ICollection)result.GetValue(source)).MapList());
                            }
                            else
                            {
                                //En caso de ser un object
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(result.Name + "DTO"));
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
                    destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(i.GetType().Name + "DTO"));

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
        public static dynamic MapDTO<T>(this T source)
            where T : class, new()
        {
            //Check for source
            if (source == null)
                return null;

            Type destinyType = null;

            if (source.GetType().ToString().Contains("Collection"))
            {
                destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(source.GetType().GetProperties().FirstOrDefault().Name.Split(new char[] { 'D', 'T', 'O' })[0]));

            }
            else
            {
                destinyType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(
                    m => m.Name.Contains(source.GetType().Name.Split(new char[] { 'D', 'T', 'O' })[0]));
            }

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
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(result.PropertyType.GetGenericArguments()
                                    .Single().Name.Split(new char[] { 'D', 'T', 'O' })[0]));

                                if (type == null)
                                {
                                    continue;
                                }

                                var listInstance = (IList)typeof(List<>).MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(null);

                                item.SetValue(instance, ((ICollection)result.GetValue(source)).MapDTOList());
                            }
                            else
                            {
                                //En caso de ser un object
                                var type = classList.FirstOrDefault(m => m.FullName.Contains(result.Name.Split(new char[] { 'D', 'T', 'O' })[0]));

                                if (type == null)
                                {
                                    continue;
                                }

                                item.SetValue(instance, result.GetValue(source).MapDTO());
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
        public static dynamic MapDTOList<T>(this T source)
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
                    m => m.Name.Contains(i.GetType().Name.Split(new char[] { 'D', 'T', 'O' })[0]));

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
                            var mapped = ((ICollection)i).MapDTOList();

                            foreach (var item in mapped)
                            {
                                ((IList<object>)list).Add(item);
                            }

                        }
                        else
                        {
                            ((IList)list).Add(i.MapDTO());
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
