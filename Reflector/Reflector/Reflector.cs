// <copyright file="Reflector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Reflector;

using System.Reflection;
using System.Text;

public static class Reflector
{
    private static readonly Dictionary<Type, string> _typeAliases = new()
    {
        { typeof(void), "void" },
        { typeof(int), "int" },
        { typeof(string), "string" },
        { typeof(bool), "bool" },
        { typeof(double), "double" },
        { typeof(float), "float" },
        { typeof(long), "long" },
        { typeof(short), "short" },
        { typeof(byte), "byte" },
        { typeof(char), "char" },
        { typeof(object), "object" },
    };

    public static string PrintStructure(Type someClass)
    {
        var className = someClass.Name;
        var fileName = className + ".cs";

        var classCode = GetClassCode(someClass);

        File.WriteAllText(fileName, classCode);

        return string.Empty;
    }

    public static List<Type> DiffClasses()
    {
        return new List<Type>();
    }

    private static string GetClassCode(Type someClass)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(someClass.Namespace))
        {
            sb.AppendLine($"namespace {someClass.Namespace}");
            sb.AppendLine("{");
            GetTypes(someClass, sb, 1);
            sb.AppendLine("}");
        }
        else
        {
            GetTypes(someClass, sb, 0);
        }

        return sb.ToString();
    }

    private static void GetTypes(Type someClass, StringBuilder sb, int indent)
    {
        var indentStr = new string(' ', indent * 4);

        var modifiers = GetTypeModifiers(someClass);

        var keyword = GetTypeKeyword(someClass);

        var name = someClass.Name;

        var inheritance = GetInheritance(someClass);

        sb.AppendLine($"{indentStr}{modifiers}{keyword} {name} {inheritance}");
        sb.AppendLine($"{indentStr}{{");

        WriteFields(someClass, sb, indent + 1);
        WriteConstructors(someClass, sb, indent + 1);
        WriteMethods(someClass, sb, indent + 1);
        WriteNestedTypes(someClass, sb, indent + 1);

        sb.AppendLine($"{indentStr}}}");
    }

    /// <summary>
    /// Get class modifiers.
    /// </summary>
    /// <returns>Modifiers.</returns>
    private static string GetTypeModifiers(Type someClass)
    {
        var result = string.Empty;

        if (someClass.IsPublic || someClass.IsNestedPublic)
        {
            result += "public ";
        }
        else if (someClass.IsNestedPrivate)
        {
            result += "private ";
        }
        else if (someClass.IsNestedFamily)
        {
            result += "protected ";
        }
        else if (someClass.IsNestedAssembly)
        {
            result += "internal ";
        }
        else if (someClass.IsNestedFamORAssem)
        {
            result += "protected internal ";
        }
        else
        {
            result += "internal ";
        }

        if (someClass.IsAbstract && someClass.IsSealed)
        {
            result += "static ";
        }
        else if (someClass.IsAbstract && !someClass.IsInterface)
        {
            result += "abstract ";
        }
        else if (someClass.IsSealed && !someClass.IsValueType)
        {
            result += "sealed ";
        }

        return result;
    }

    private static string GetTypeKeyword(Type someClass)
        => someClass.IsInterface ? "interface" : "class";

    private static string GetFriendlyTypeName(Type someClass)
    {
        if (_typeAliases.TryGetValue(someClass, out var alias))
        {
            return alias;
        }

        if (someClass.IsGenericParameter)
        {
            return someClass.Name;
        }

        if (someClass.IsGenericType)
        {
            var name = someClass.Name.Split('`')[0];
            var args = someClass.GetGenericArguments();
            var argNames = new string[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                argNames[i] = GetFriendlyTypeName(args[i]);
            }

            return name + "<" + string.Join(", ", argNames) + ">";
        }

        return someClass.Name;
    }

    private static string GetInheritance(Type someClass)
    {
        var bases = new List<string>();

        if (someClass.BaseType != null &&
            someClass.BaseType != typeof(object) &&
            someClass.BaseType != typeof(ValueType))
        {
            bases.Add(GetFriendlyTypeName(someClass.BaseType));
        }

        var interfaces = someClass.GetInterfaces();
        foreach (var item in interfaces)
        {
            var isDeclaredHere = true;
            if (someClass.BaseType != null)
            {
                var parentInterfaces = someClass.BaseType.GetInterfaces();
                foreach (var pi in parentInterfaces)
                {
                    if (pi == item)
                    {
                        isDeclaredHere = false;
                        break;
                    }
                }
            }

            if (isDeclaredHere)
            {
                bases.Add(GetFriendlyTypeName(item));
            }
        }

        if (bases.Count == 0)
        {
            return string.Empty;
        }

        return " : " + string.Join(", ", bases);
    }

    private static void WriteFields(Type someClass, StringBuilder sb, int indent)
    {
        var tab = GetIndent(indent);

        var flags = BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.Static |
                             BindingFlags.DeclaredOnly;

        var fields = someClass.GetFields(flags);

        foreach (var field in fields)
        {
            if (field.Name.Contains('<') && field.Name.Contains('>'))
            {
                continue;
            }

            var modifiers = GetFieldModifiers(field);
            var fieldType = GetFriendlyTypeName(field.FieldType);
            var fieldName = field.Name;

            sb.AppendLine($"{tab}{modifiers}{fieldType} {fieldName};");
        }

        if (fields.Length > 0)
        {
            sb.AppendLine();
        }
    }

    private static string GetFieldModifiers(FieldInfo field)
    {
        var result = string.Empty;

        if (field.IsPublic)
        {
            result += "public ";
        }
        else if (field.IsPrivate)
        {
            result += "private ";
        }
        else if (field.IsFamily)
        {
            result += "protected ";
        }
        else if (field.IsAssembly)
        {
            result += "internal ";
        }
        else if (field.IsFamilyOrAssembly)
        {
            result += "protected internal ";
        }

        if (field.IsStatic)
        {
            result += "static ";
        }

        if (field.IsInitOnly)
        {
            result += "readonly ";
        }

        if (field.IsLiteral)
        {
            result = result.Replace("static ", string.Empty) + "const ";
        }

        return result;
    }

    private static void WriteConstructors(Type someClass, StringBuilder sb, int indent)
    {
        var tab = GetIndent(indent);

        var flags = BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.DeclaredOnly;

        var constructors = someClass.GetConstructors(flags);

        foreach (var ctor in constructors)
        {
            var modifiers = GetConstructorModifiers(ctor);
            var name = someClass.Name.Split('`')[0];
            var parameters = GetParameters(ctor.GetParameters());

            sb.AppendLine($"{tab}{modifiers}{name}({parameters})");
            sb.AppendLine($"{tab}{{");
            sb.AppendLine($"{tab}}}");
            sb.AppendLine();
        }
    }

    private static string GetConstructorModifiers(ConstructorInfo ctor)
    {
        var result = string.Empty;

        if (ctor.IsPublic)
        {
            result += "public ";
        }
        else if (ctor.IsPrivate)
        {
            result += "private ";
        }
        else if (ctor.IsFamily)
        {
            result += "protected ";
        }
        else if (ctor.IsAssembly)
        {
            result += "internal ";
        }

        return result;
    }

    private static void WriteMethods(Type someClass, StringBuilder sb, int indent)
    {
        var tab = GetIndent(indent);

        var flags = BindingFlags.Public | BindingFlags.NonPublic |
                             BindingFlags.Instance | BindingFlags.Static |
                             BindingFlags.DeclaredOnly;

        var methods = someClass.GetMethods(flags);

        foreach (var method in methods)
        {
            var modifiers = GetMethodModifiers(method);
            var returnType = GetFriendlyTypeName(method.ReturnType);
            var methodName = GetMethodName(method);
            var parameters = GetParameters(method.GetParameters());

            sb.AppendLine($"{tab}{modifiers}{returnType} {methodName}({parameters})");
            sb.AppendLine($"{tab}{{");

            WriteMethodBody(method, sb, indent + 1);

            sb.AppendLine($"{tab}}}");
            sb.AppendLine();
        }
    }

    private static string GetMethodModifiers(MethodInfo method)
    {
        var result = string.Empty;

        if (method.IsPublic)
        {
            result += "public ";
        }
        else if (method.IsPrivate)
        {
            result += "private ";
        }
        else if (method.IsFamily)
        {
            result += "protected ";
        }
        else if (method.IsAssembly)
        {
            result += "internal ";
        }
        else if (method.IsFamilyOrAssembly)
        {
            result += "protected internal ";
        }

        if (method.IsStatic)
        {
            result += "static ";
        }

        if (method.IsAbstract)
        {
            result += "abstract ";
        }
        else if (method.IsVirtual && !method.IsFinal)
        {
            if (method.GetBaseDefinition() != method)
            {
                result += "override ";
            }
            else
            {
                result += "virtual ";
            }
        }

        return result;
    }

    private static string GetMethodName(MethodInfo method)
    {
        var name = method.Name;

        if (method.IsGenericMethod)
        {
            var args = method.GetGenericArguments();
            var argNames = new string[args.Length];

            for (var i = 0; i < args.Length; i++)
            {
                argNames[i] = args[i].Name;
            }

            name += "<" + string.Join(", ", argNames) + ">";
        }

        return name;
    }

    private static string GetParameters(ParameterInfo[] parameters)
    {
        if (parameters.Length == 0)
        {
            return string.Empty;
        }

        var parts = new string[parameters.Length];

        for (var i = 0; i < parameters.Length; i++)
        {
            var p = parameters[i];
            var paramType = GetFriendlyTypeName(p.ParameterType);

            var modifier = string.Empty;
            if (p.IsOut)
            {
                modifier = "out ";
                paramType = paramType.TrimEnd('&');
            }
            else if (p.ParameterType.IsByRef)
            {
                modifier = "ref ";
                paramType = paramType.TrimEnd('&');
            }

            if (p.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
            {
                modifier = "params ";
            }

            parts[i] = modifier + paramType + " " + p.Name;
        }

        return string.Join(", ", parts);
    }

    private static void WriteMethodBody(MethodInfo method, StringBuilder sb, int indent)
    {
        var tab = GetIndent(indent);
        var returnType = method.ReturnType;

        if (returnType == typeof(void))
        {
            return;
        }

        var defaultValue = GetDefaultValue(returnType);
        sb.AppendLine($"{tab}return {defaultValue};");
    }

    private static string GetDefaultValue(Type someClass)
    {
        if (!someClass.IsValueType || Nullable.GetUnderlyingType(someClass) != null)
        {
            return "default";
        }

        if (someClass == typeof(bool))
        {
            return "false";
        }

        if (someClass == typeof(int) || someClass == typeof(long) || someClass == typeof(short) ||
            someClass == typeof(byte))
        {
            return "0";
        }

        if (someClass == typeof(float))
        {
            return "0f";
        }

        return "default";
    }

    private static void WriteNestedTypes(Type type, StringBuilder sb, int indent)
    {
        var flags = BindingFlags.Public | BindingFlags.NonPublic;

        var nestedTypes = type.GetNestedTypes(flags);

        foreach (var nested in nestedTypes)
        {
            GetTypes(nested, sb, indent);
            sb.AppendLine();
        }
    }

    private static string GetIndent(int tab)
    {
        return new string(' ', tab * 4);
    }
}
