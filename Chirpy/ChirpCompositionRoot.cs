using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Chirpy.Domain.Repository;
//using Raven.Client.Embedded;
using Raven.Client.Document;
using Raven.Client.Extensions;
using Raven.Abstractions.Data;

namespace Chirpy
{

    public class ChirpCompositionRoot
    {
        private readonly IControllerFactory _controllerFactory;

        public ChirpCompositionRoot()
        {
            this._controllerFactory = ChirpCompositionRoot.CreateControllerFactory();
        }

        public IControllerFactory ControllerFactory
        {
            get
            {
                return _controllerFactory;
            }
        }

        private static IControllerFactory CreateControllerFactory()
        {
            string assemblyName = ConfigurationManager.AppSettings["chirpRepositoryAssemblyName"];
            string typeName = ConfigurationManager.AppSettings["chirpRepositoryTypeName"];
            
            DocumentStore documentStore = null;
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
            parser.Parse();
            documentStore = new DocumentStore
            {
                ApiKey = parser.ConnectionStringOptions.ApiKey,
                Url = parser.ConnectionStringOptions.Url
            };
            documentStore.Initialize();

            var cacheRepository = Activator.CreateInstance(Type.GetType(typeName, true, true), new object[] { 
                documentStore 
            });

            var controllerFactory = new ChirpControllerFactory((IChirpRepository)cacheRepository);

            return controllerFactory;
        }

    }
}