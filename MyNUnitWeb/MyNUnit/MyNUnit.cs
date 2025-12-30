// <copyright file="MyNUnit.cs" company="Alexander Kuchin">
// Copyright (c) Alexander Kuchin. All rights reserved.
// </copyright>

namespace MyNUnit;

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using RunTestsUtils;

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
    public static async Task<ConcurrentBag<TestResult>> RunAsync(string path)
    {
        var result = new ConcurrentBag<TestResult>();
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
                         LogResult(result, $"Class - {type}, Method {method.Name} is not static");
                         continue;
                     }

                     try
                     {
                         method.Invoke(null, null);
                         LogResult(result, $"Class - {type}, BeforeClass Method {method.Name} worked");
                     }
                     catch
                     {
                         LogResult(result, $"Class - {type}, BeforeClass Method {method.Name} failed");
                     }
                 }

                 Parallel.ForEach(methodsDictionary[typeof(Test)], method =>
                 {
                     var instance = Activator.CreateInstance(type);

                     var test = testAttributes[method];
                     if (test.Ignore is not null)
                     {
                         LogResult(result, $"Class - {type}, {method.Name} ignored - {test.Ignore}");
                         return;
                     }

                     foreach (var methodIn in methodsDictionary[typeof(Before)])
                     {
                         try
                         {
                             methodIn.Invoke(instance, null);
                             LogResult(result, $"Class - {type}, Before Method {method.Name} worked");
                         }
                         catch
                         {
                             LogResult(result, $"Class - {type}, Before Method {method.Name} failed");
                         }
                     }

                     var stopwatch = Stopwatch.StartNew();

                     try
                     {
                         method.Invoke(instance, null);
                         stopwatch.Stop();

                         if (test.Expected is not null)
                         {
                             LogResult(result, $"Class - {type}, {method.Name} failed - {test.Expected.Name}");
                         }
                         else
                         {
                             LogResult(result, $"Class - {type}, {method.Name} passed in {stopwatch.ElapsedMilliseconds} ms");
                         }
                     }
                     catch (Exception e)
                     {
                         var exception = e.InnerException ?? e;

                         if (test.Expected is not null)
                         {
                             if (exception.GetType() == test.Expected)
                             {
                                 LogResult(result, $"Class - {type}, {method.Name} passed in  {stopwatch.ElapsedMilliseconds} ms");
                             }
                             else
                             {
                                 LogResult(result, $"Class - {type}, {method.Name} failed - {exception.GetType().Name} != {test.Expected?.Name ?? "null"}");
                             }
                         }
                         else
                         {
                             LogResult(result, $"Class - {type}, {method.Name} failed - {exception.Message}");
                             result.Add(new TestResult(assembly.GetName().Name!, method.Name, false, stopwatch.ElapsedMilliseconds, exception.Message, null));
                         }
                     }
                     finally
                     {
                         stopwatch.Stop();
                     }

                     foreach (var methodIn in methodsDictionary[typeof(After)])
                     {
                        methodIn.Invoke(instance, null);                     }
                 });

                 foreach (var method in methodsDictionary[typeof(AfterClass)])
                 {
                     if (!method.IsStatic)
                     {
                         throw new Exception($"Class - {type}, AfterClass Method {method.Name} is not static");
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
}

/// <summary>
/// Test result data.
/// </summary>
/// <param name="AssemblyName">Name of assembly.</param>
/// <param name="TestName">Name of test method.</param>
/// <param name="IsPassed">Passed parameter.</param>
/// <param name="WorkTime">Start time.</param>
/// <param name="ErrorMessage">Error message.</param>
/// <param name="IgnoreReason">Ignore reason.</param>
public record struct TestResult(
    string AssemblyName,
    string TestName,
    bool IsPassed,
    long WorkTime,
    string? ErrorMessage,
    string? IgnoreReason);