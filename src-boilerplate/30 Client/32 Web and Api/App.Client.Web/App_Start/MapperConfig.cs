using System;
using System.Linq.Expressions;
using App.Service.DomainEntities.Users;
using AutoMapper;
using AutoMapper.Mappers;

namespace App.Client.Web
{
    public static class MapperConfig
    {
        /// <summary>
        /// Register WEB project models to domain model maps
        /// </summary>
        public static void RegisterEntityMaps()
        {
            Mapper.Initialize(configuration =>
            {
                configuration.Mappers.Add(new ArrayMapper());
                configuration.Mappers.Add(new EnumerableMapper());
                configuration.Mappers.Add(new StringMapper());
            });
        }


        /// <summary>
        /// Automapper Extension for field ignore
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="map"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination>(this IMappingExpression<TSource, TDestination> map, Expression<Func<TDestination, object>> selector)
        {
            map.ForMember(selector, config => config.Ignore());
            return map;
        }

        /// <summary>
        /// User automapper to map to your type
        /// </summary>
        /// <typeparam name="T">Result type</typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Map<T>(this object value)
        {
            return Mapper.Map<T>(value);
        }
    }
}