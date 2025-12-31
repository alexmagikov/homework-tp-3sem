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

            try
            {
                var types = assembly.GetTypes();

                Parallel.ForEach(types, type =>
                {
                    try
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
                                var attrType = attribute.GetType();
                                if (methodsDictionary.ContainsKey(attrType))
                                {
                                    if (attrType == typeof(Test))
                                    {
                                        testAttributes.Add(method, (Test)attribute);
                                    }

                                    methodsDictionary[attrType].Add(method);
                                }
                            }
                        }

                        if (methodsDictionary[typeof(Test)].Count == 0)
                        {
                            return;
                        }

                        foreach (var method in methodsDictionary[typeof(BeforeClass)])
                        {
                            if (!method.IsStatic)
                            {
                                continue;
                            }

                            method.Invoke(null, null);
                        }

                        Parallel.ForEach(methodsDictionary[typeof(Test)], method =>
                        {
                            var instance = Activator.CreateInstance(type);
                            var test = testAttributes[method];

                            if (test.Ignore is not null)
                            {
                                result.Add(new TestResult(assembly.GetName().Name!, method.Name, true, 0, null, test.Ignore));
                                return;
                            }

                            foreach (var methodIn in methodsDictionary[typeof(Before)]) methodIn.Invoke(instance, null);

                            var stopwatch = Stopwatch.StartNew();
                            try
                            {
                                method.Invoke(instance, null);
                                stopwatch.Stop();

                                if (test.Expected is not null)
                                {
                                    result.Add(new TestResult(assembly.GetName().Name!, method.Name, false, 0, $"Expected {test.Expected} but passed", null));
                                }
                                else
                                {
                                    result.Add(new TestResult(assembly.GetName().Name!, method.Name, true, stopwatch.ElapsedMilliseconds, null, null));
                                }
                            }
                            catch (Exception e)
                            {
                                stopwatch.Stop();
                                var exception = e.InnerException ?? e;

                                if (test.Expected is not null && exception.GetType() == test.Expected)
                                {
                                    result.Add(new TestResult(assembly.GetName().Name!, method.Name, true, stopwatch.ElapsedMilliseconds, null, null));
                                }
                                else
                                {
                                    result.Add(new TestResult(assembly.GetName().Name!, method.Name, false, stopwatch.ElapsedMilliseconds, exception.Message, null));
                                }
                            }

                            foreach (var methodIn in methodsDictionary[typeof(After)]) methodIn.Invoke(instance, null);
                        });

                        foreach (var method in methodsDictionary[typeof(AfterClass)])
                        {
                            if (!method.IsStatic)
                            {
                                continue;
                            }

                            method.Invoke(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Add(new TestResult(assembly.GetName().Name!, type.Name, false, 0, $"Class Error: {ex.Message}", null));
                    }
                });
            }
            catch (ReflectionTypeLoadException rtle)
            {
                foreach (var loaderException in rtle.LoaderExceptions)
                {
                    result.Add(new TestResult(assembly.GetName().Name!, "AssemblyLoad", false, 0, loaderException?.Message, null));
                }
            }
            catch (Exception e)
            {
                result.Add(new TestResult(assembly.GetName().Name!, "AssemblyError", false, 0, e.Message, null));
            }
        }

        return result;
    }

    private static async Task<List<Assembly?>> LoadAssembliesByPath(string path)
    {
        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                             .Where(f => f.EndsWith(".dll") || f.EndsWith(".exe"));

        var tasks = files.Select(file => Task.Run(() =>
        {
            try
            {
                var bytes = File.ReadAllBytes(file);
                return Assembly.Load(bytes);
            }
            catch
            {
                return null;
            }
        }));

        var assembliesArray = await Task.WhenAll(tasks);
        return assembliesArray.Where(a => a != null).ToList();
    }
}