﻿using System.Linq;
using Common.Logging;
using Octopus.Client;
using Octopus.Client.Model;
using OctopusProjectBuilder.Model;
using OctopusProjectBuilder.Uploader.Converters;

namespace OctopusProjectBuilder.Uploader
{
    public class ModelDownloader
    {
        private static readonly ILog Logger = LogManager.GetLogger<ModelDownloader>();
        private readonly IOctopusRepository _repository;

        public ModelDownloader(string octopusUrl, string octopusApiKey)
            : this(new OctopusRepository(new OctopusClient(new OctopusServerEndpoint(octopusUrl, octopusApiKey))))
        {
        }

        public ModelDownloader(IOctopusRepository repository)
        {
            _repository = repository;
        }

        public SystemModel DownloadModel()
        {
            return new SystemModel(
                _repository.MachinePolicies.FindAll().Select(ReadMachinePolicy),
                _repository.Lifecycles.FindAll().Select(ReadLifecycle),
                _repository.ProjectGroups.FindAll().Select(ReadProjectGroup),
                _repository.LibraryVariableSets.FindAll().Select(ReadLibraryVariableSet),
                _repository.Projects.FindAll().Select(ReadProject),
                _repository.Environments.FindAll().Select(ReadEnvironment),
                _repository.UserRoles.FindAll().Select(ReadUserRole),
                _repository.Teams.FindAll().Select(ReadTeam));
        }

        private MachinePolicy ReadMachinePolicy(MachinePolicyResource resource)
        {
            Logger.Info($"Downloading {nameof(MachinePolicyResource)}: {resource.Name}");
            return resource.ToModel();
        }

        private LibraryVariableSet ReadLibraryVariableSet(LibraryVariableSetResource resource)
        {
            Logger.Info($"Downloading {nameof(LibraryVariableSetResource)}: {resource.Name}");
            return resource.ToModel(_repository);
        }

        private Lifecycle ReadLifecycle(LifecycleResource resource)
        {
            Logger.Info($"Downloading {nameof(LifecycleResource)}: {resource.Name}");
            return resource.ToModel(_repository);
        }

        private Project ReadProject(ProjectResource resource)
        {
            Logger.Info($"Downloading {nameof(ProjectResource)}: {resource.Name}");
            return resource.ToModel(_repository);
        }

        private static ProjectGroup ReadProjectGroup(ProjectGroupResource resource)
        {
            Logger.Info($"Downloading {nameof(ProjectGroupResource)}: {resource.Name}");
            return resource.ToModel();
        }

        private static Environment ReadEnvironment(EnvironmentResource resource)
        {
            Logger.Info($"Downloading {nameof(EnvironmentResource)}: {resource.Name}");
            return resource.ToModel();
        }

        private static UserRole ReadUserRole(UserRoleResource resource)
        {
            Logger.Info($"Downloading {nameof(UserRoleResource)}: {resource.Name}");
            return resource.ToModel();
        }

        private Team ReadTeam(TeamResource resource)
        {
            Logger.Info($"Downloading {nameof(TeamResource)}: {resource.Name}");
            return resource.ToModel(_repository);
        }
    }
}