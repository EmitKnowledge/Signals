using Signals.Core.Common.InputOutput;
using Signals.Core.Common.Instance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Signals.Core.Common.Reflection
{
    /// <summary>
    /// Helper for reflection
    /// </summary>
    public static class ReflectionsHelper
    {
        /// <summary>
        /// Check if a type is deriving from another type
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsDerivedFromGenericType(this Type givenType, Type genericType)
        {
            if (givenType == null) return false;
            if (genericType == null) return false;
            if (givenType == typeof(System.Object)) return false;
            if (genericType.IsAssignableFrom(givenType)) return true;
            if (givenType.IsAssignableFrom(genericType)) return true;
            if (givenType == genericType) return true;
            var baseType = givenType.BaseType;
            if (baseType == null) return false;
            if (baseType.IsGenericType)
            {
                if (baseType.GetGenericTypeDefinition() == genericType) return true;
            }
            return IsDerivedFromGenericType(baseType, genericType);
        }

        /// <summary>
        /// Load all types both hoste and referenced
        /// </summary>
        /// <param name="ass"></param>
        /// <returns></returns>
        public static List<Type> LoadAllTypesFromAssembly(this Assembly ass)
        {
            var types = ass.GetTypes().ToList();
            types.AddRange(ass.GetReferencedAssemblies()
                               .Select(assemblyName => { try { return Assembly.Load(assemblyName); } catch { return null; } })
                               .Where(assembly => !assembly.IsNull())
                               .SelectMany(assembly => { try { return assembly.GetTypes(); } catch { return new Type[0]; } }));

            return types;
        }

        /// <summary>
        /// Load all types in the executing assembly
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetRegisteredTypesInEntryAssembly(Func<Type, bool> filter = null)
        {
            var types = Assembly.GetEntryAssembly().LoadAllTypesFromAssembly();
            return filter == null ? types : types.Where(filter).ToList();
        }

        /// <summary>
        /// Load all types in the executing assembly
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetRegisteredTypesInAssemblyFromLocation(string assemblyLocation, bool isRelativePath = false)
        {
            if (isRelativePath)
            {
                assemblyLocation = IoHelper.CombinePaths(AppDomain.CurrentDomain.BaseDirectory, assemblyLocation);
            }

            var types = Assembly.LoadFrom(assemblyLocation).LoadAllTypesFromAssembly();
            return types;
        }

        /// <summary>
        /// Get path of the current running assemlby
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentAssemblyRunPath()
        {
            var filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var pathWithExcludedFilename = IoHelper.GetPathWithoutFileName(filePath);
            return pathWithExcludedFilename;
        }

        /// <summary>
        /// Get calling method name
        /// </summary>
        /// <returns></returns>
        public static string GetCallingMethodName()
        {
            var trace = new StackTrace();
            var frame = trace.GetFrame(1);
            if (frame == null)
            {
                frame = trace.GetFrame(0);
            }

            var method = frame.GetMethod();
            var methodName = method.Name;
            return methodName;
        }

        /// <summary>
        /// Get method execution name in chain
        /// </summary>
        /// <param name="stepsBehind"></param>
        /// <param name="returnOnlyMethodName"></param>
        /// <returns></returns>
        public static string GetCallingMethodName(int stepsBehind, bool returnOnlyMethodName = false)
        {
            try
            {
                // stack pointer validity check
                stepsBehind = stepsBehind < 0 ? 0 : stepsBehind;

                var trace = new StackTrace();
                var frame = trace.GetFrame(stepsBehind + 1);
                var method = frame.GetMethod();
                if (method.IsNull()) return @"Method info not available";

                var _params = method.GetParameters() ?? new ParameterInfo[0];
                var paramBuilder = new StringBuilder();
                for (int i = 0; i < _params.Length; i++)
                {
                    var parameterInfo = _params[i];
                    paramBuilder.Append(parameterInfo.ParameterType.Name ?? @"Param type N/A");
                    paramBuilder.Append(@" ");
                    paramBuilder.Append(parameterInfo.Name ?? @"Param name N/A");
                    if (i + 1 < _params.Length)
                    {
                        paramBuilder.Append(@", ");
                    }
                }

                if (returnOnlyMethodName)
                {
                    return method.Name ?? @"Method name not available";
                }

                var methodReflectedType = method.ReflectedType.IsNull() || method.ReflectedType.FullName.IsNull() ? @"Method namespace not available" : method.ReflectedType.FullName;
                var mName = method.Name ?? @"Method name not available";
                var methodName = string.Concat(methodReflectedType, @".", mName ?? @"Method name not available", @"(", paramBuilder.ToString(), ")");
                return methodName;
            }
            catch (Exception)
            {
                return @"GetCallingMethodName can not be retrieved";
            }
        }

        /// <summary>
        /// Return the member expression for the provided expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MemberExpression GetMemberExpression<T>(this Expression<Func<T, object>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return memberExpression;
            }
            else
            {
                var op = ((UnaryExpression)expression.Body).Operand;
                return ((MemberExpression)op);
            }
        }

        /// <summary>
        /// Return the member expression for the provided expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static object GetExpressionValue<T>(this Expression<Func<T, object>> expression, object arguments = null)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                var property = (PropertyInfo)memberExpression.Member;
                return property.Name;
            }
            if (expression.Body is UnaryExpression unaryExpression)
            {
                var op = unaryExpression.Operand;
                var property = (PropertyInfo)((MemberExpression)op).Member;
                return property.Name;
            }
            if (expression.Body is MethodCallExpression)
            {
                return expression.Compile().DynamicInvoke(arguments);
            }

            return null;
        }
    }
}