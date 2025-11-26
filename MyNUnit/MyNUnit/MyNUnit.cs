// <copyright file="MyNUnit.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Test system.
/// </summary>
public static class MyNUnit
{
    /// <summary>
    /// Run tests.
    /// </summary>
    /// <param name="path">Path of the directory with dll and exe.</param>
    /// <returns>List of logs about tests.</returns>
    public static async Task<ConcurrentBag<string>> Run(string path)
    {
        var result = new ConcurrentBag<string>();
        var assemblies = await LoadAssembliesByPath(path);

        foreach (var assembly in assemblies)
        {
            if (assembly is null)
            {
                continue;
            }

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

                 var testAttributes = new Dictionary<MethodInfo, Test>();

                 foreach (var method in type.GetMethods())
                 {
                     foreach (var attribute in method.GetCustomAttributes())
                     {
                         if (methodsDictionary.ContainsKey(attribute.GetType()))
                         {
                             if (attribute.GetType() == typeof(Test))
                             {
                                    testAttributes.Add(method, (Test)attribute);
                             }

                             methodsDictionary[attribute.GetType()].Add(method);
                         }
                     }
                 }

                 foreach (var method in methodsDictionary[typeof(BeforeClass)])
                 {
                     if (!method.IsStatic)
                     {
                         LogResult(result, $"Method {method.Name} is not static");
                         continue;
                     }

                     method.Invoke(null, null);
                 }

                 Parallel.ForEach(methodsDictionary[typeof(Test)], method =>
                 {
                     var instance = Activator.CreateInstance(type);

                     foreach (var methodIn in methodsDictionary[typeof(Before)])
                     {
                         methodIn.Invoke(instance, null);
                     }

                     var test = testAttributes[method];
                     if (test.Ignore is not null)
                     {
                         LogResult(result, $"Test {method.Name} ignored - {test.Ignore}");
                         return;
                     }

                     var stopwatch = Stopwatch.StartNew();

                     try
                     {
                         method.Invoke(instance, null);
                         stopwatch.Stop();

                         if (test.Expected is not null)
                         {
                             LogResult(result, $"Test {method.Name} failed - {test.Expected.Name}");
                         }
                         else
                         {
                             LogResult(result, $"Test {method.Name} passed in {stopwatch.ElapsedMilliseconds} ms");
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
                     finally
                     {
                         stopwatch.Stop();
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
                         continue;
                     }

                     method.Invoke(null, null);
                 }
            });
        }

        return result;
    }

    private static async Task<List<Assembly?>> LoadAssembliesByPath(string path)
    {
        var files = Directory.GetFiles(path, "*.*").Where(f => f.EndsWith(".dll") || f.EndsWith(".exe"));

        var tasks = files.Select(file => Task.Run(() =>
        {
            try
            {
                return Assembly.LoadFrom(file);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }));

        var assembliesArray = await Task.WhenAll(tasks);
        var assemblies = assembliesArray.Where(a => a != null);

        return assemblies.ToList();
    }

    private static void LogResult(ConcurrentBag<string> result, string resultTest)
    {
        result.Add(resultTest);
        Console.WriteLine(resultTest);
    }
}