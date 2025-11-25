// <copyright file="MyNUnit.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Reflection;

/// <summary>
/// Test system.
/// </summary>
public class MyNUnit
{
    /// <summary>
    /// Run tests.
    /// </summary>
    /// <param name="path">Path of the directory with dll and exe.</param>
    /// <returns>List of logs about tests.</returns>
    public static List<string> Run(string path)
    {
        var result = new List<string>();
        var assemblies = LoadAssembliesByPath(path);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            Parallel.ForEach(types, type =>
            {
                 var methodsDictionary = new Dictionary<Type, List<MethodInfo>>()
                {
                    [typeof(Before)] = [],
                    [typeof(After)] = [],
                    [typeof(Test)] = [],
                    [typeof(AfterClass)] = [],
                    [typeof(BeforeClass)] = [],
                };

                 foreach (var method in type.GetMethods())
                 {
                     foreach (var attribute in method.GetCustomAttributes())
                     {
                         if (methodsDictionary.ContainsKey(attribute.GetType()))
                         {
                             methodsDictionary[attribute.GetType()].Add(method);
                         }
                     }
                 }

                 var instance = Activator.CreateInstance(type);

                 foreach (var method in methodsDictionary[typeof(BeforeClass)])
                 {
                     if (!method.IsStatic)
                     {
                         LogResult(result, $"Method {method.Name} is not static");
                     }

                     method.Invoke(null, null);
                 }

                 Parallel.ForEach(methodsDictionary[typeof(Test)], method =>
                 {
                     foreach (var methodIn in methodsDictionary[typeof(Before)])
                     {
                         methodIn.Invoke(instance, null);
                     }

                     foreach (var attribute in Attribute.GetCustomAttributes(method))
                     {
                         if (typeof(Test) != attribute.GetType())
                         {
                             continue;
                         }

                         var test = (Test)attribute;
                         if (test.Ignore is not null)
                         {
                             LogResult(result, $"Test {method.Name} ignored - {test.Ignore}");
                             continue;
                         }

                         try
                         {
                             instance = Activator.CreateInstance(type);
                             method.Invoke(instance, null);

                             if (test.Expected is not null)
                             {
                                 LogResult(result, $"Test {method.Name} failed - {test.Expected.Name}");
                             }
                             else
                             {
                                 LogResult(result, $"Test {method.Name} passed");
                             }
                         }
                         catch (Exception e)
                         {
                             var exception = e.InnerException ?? e;

                             if (test.Expected is not null && exception.GetType() == test.Expected)
                             {
                                 LogResult(result, $"Test {method.Name} passed");
                             }
                             else
                             {
                                 LogResult(result, $"Test {method.Name} failed - {exception.GetType().Name} != {test.Expected?.Name ?? "null"}");
                             }
                         }
                     }

                     foreach (var methodIn in methodsDictionary[typeof(After)])
                     {
                         methodIn.Invoke(instance, null);
                     }
                 });

                 foreach (var method in methodsDictionary[typeof(AfterClass)])
                 {
                     if (!method.IsStatic)
                     {
                         LogResult(result, $"Method {method.Name} is not static");
                     }

                     method.Invoke(null, null);
                 }
            });
        }

        return result;
    }

    private static List<Assembly> LoadAssembliesByPath(string path)
    {
        var assemblies = new List<Assembly>();

        var files = Directory.GetFiles(path, "*.*").Where(f => f.EndsWith(".dll") || f.EndsWith(".exe"));

        foreach (var file in files)
        {
            try
            {
                var assembly = Assembly.LoadFrom(file);
                assemblies.Add(assembly);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return assemblies;
    }

    private static void LogResult(List<string> result, string resultTest)
    {
        result.Add(resultTest);
        Console.WriteLine(resultTest);
    }
}