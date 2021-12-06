using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace qRPC.Server
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a server listener for the implementation T
        /// </summary>
        /// <typeparam name="T">The implementation type</typeparam>
        /// <param name="services"></param>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static IServiceCollection AddQRPCServer<T>(this IServiceCollection services, T srv, string hostname, int port, Encoding encoding)
        {
            return services.AddSingleton(x => new QrpcServer<T>(srv, hostname, port, encoding));
        }


    }
}
