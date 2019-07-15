﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using StandardRepository.PostgreSQL;

using Translation.Data.Entities.Domain;
using Translation.Data.Repositories.Contracts;
using Translation.Data.UnitOfWorks.Contracts;

namespace Translation.Data.UnitOfWorks
{
    public class LabelUnitOfWork : ILabelUnitOfWork
    {
        private readonly PostgreSQLTransactionalExecutor _transactionalExecutor;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ILabelRepository _labelRepository;
        private readonly ILabelTranslationRepository _labelTranslationRepository;

        public LabelUnitOfWork(PostgreSQLTransactionalExecutor transactionalExecutor,
                               IOrganizationRepository organizationRepository,
                               IUserRepository userRepository,
                               IProjectRepository projectRepository,
                               ILabelRepository labelRepository,
                               ILabelTranslationRepository labelTranslationRepository)
        {
            _transactionalExecutor = transactionalExecutor;
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _labelRepository = labelRepository;
            _labelTranslationRepository = labelTranslationRepository;
        }

        public async Task<bool> DoCreateWork(long currentUserId, Label label)
        {
            await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);

                await _labelRepository.Insert(currentUserId, label);

                var organization = await _organizationRepository.SelectById(label.OrganizationId);
                organization.LabelCount++;
                await _organizationRepository.Update(currentUserId, organization);

                var project = await _projectRepository.SelectById(label.ProjectId);
                project.LabelCount++;
                await _projectRepository.Update(currentUserId, project);

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelCount++;
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }

        public async Task<bool> DoCreateWorkBulk(long currentUserId, List<Label> labels, List<LabelTranslation> labelTranslationsToInsert,
                                                 List<LabelTranslation> labelTranslationsToUpdate)
        {
            var result = await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
             {
                 _organizationRepository.SetSqlExecutorForTransaction(connection);
                 _userRepository.SetSqlExecutorForTransaction(connection);
                 _projectRepository.SetSqlExecutorForTransaction(connection);
                 _labelRepository.SetSqlExecutorForTransaction(connection);
                 _labelTranslationRepository.SetSqlExecutorForTransaction(connection);

                 var first = labelTranslationsToInsert.FirstOrDefault();
                 if (first == null)
                 {
                     first = labelTranslationsToUpdate.FirstOrDefault();
                     if (first == null)
                     {
                         return false;
                     }
                 }

                 var organizationId = first.OrganizationId;
                 var projectId = first.ProjectId;

                 var organization = await _organizationRepository.SelectById(organizationId);
                 var project = await _projectRepository.SelectById(projectId);
                 var user = await _userRepository.SelectById(currentUserId);

                 for (var i = 0; i < labels.Count; i++)
                 {
                     var label = labels[i];
                     var labelsTranslations = labelTranslationsToInsert.Where(x => x.LabelName == label.Key).ToList();

                     label.LabelTranslationCount = labelsTranslations.Count;
                     var labelId = await _labelRepository.Insert(currentUserId, label);

                     for (var j = 0; j < labelsTranslations.Count; j++)
                     {
                         var labelTranslation = labelsTranslations[j];
                         labelTranslation.LabelId = labelId;

                         await _labelTranslationRepository.Insert(currentUserId, labelTranslation);
                         organization.LabelTranslationCount++;
                         project.LabelTranslationCount++;
                         user.LabelTranslationCount++;
                     }
                 }

                 for (var j = 0; j < labelTranslationsToUpdate.Count; j++)
                 {
                     var labelTranslation = labelTranslationsToUpdate[j];
                     await _labelTranslationRepository.Update(currentUserId, labelTranslation);
                 }

                 organization.LabelCount = organization.LabelCount + labels.Count;
                 await _organizationRepository.Update(currentUserId, organization);

                 project.LabelCount = project.LabelCount + labels.Count;
                 await _projectRepository.Update(currentUserId, project);

                 user.LabelCount = user.LabelCount + labels.Count;
                 await _userRepository.Update(currentUserId, user);

                 return true;
             });

            return result;
        }

        public async Task<bool> DoDeleteWork(long currentUserId, Label label)
        {
            await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);

                await _labelRepository.Delete(currentUserId, label.Id);

                var organization = await _organizationRepository.SelectById(label.OrganizationId);
                organization.LabelCount--;
                await _organizationRepository.Update(currentUserId, organization);

                var project = await _projectRepository.SelectById(label.ProjectId);
                project.LabelCount--;
                await _projectRepository.Update(currentUserId, project);

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelCount--;
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }

        public async Task<bool> DoCloneWork(long currentUserId, long labelId, Label newLabel)
        {
            await _transactionalExecutor.ExecuteAsync(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);

                var newLabelId = await _labelRepository.Insert(currentUserId, newLabel);

                var organization = await _organizationRepository.SelectById(newLabel.OrganizationId);
                organization.LabelCount++;

                var project = await _projectRepository.SelectById(newLabel.ProjectId);
                project.LabelCount++;

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelCount++;

                var labelTranslations = await _labelTranslationRepository.SelectAll(x => x.LabelId == labelId);
                for (var i = 0; i < labelTranslations.Count; i++)
                {
                    var labelTranslation = labelTranslations[i];
                    organization.LabelTranslationCount++;
                    project.LabelTranslationCount++;
                    user.LabelTranslationCount++;

                    labelTranslation.Uid = Guid.NewGuid();
                    labelTranslation.LabelId = newLabelId;
                    labelTranslation.LabelUid = newLabel.Uid;
                    labelTranslation.LabelName = newLabel.Name;
                    await _labelTranslationRepository.Insert(currentUserId, labelTranslation);
                }

                await _organizationRepository.Update(currentUserId, organization);
                await _projectRepository.Update(currentUserId, project);
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }

        public async Task<bool> DoCreateTranslationWork(long currentUserId, LabelTranslation labelTranslation)
        {
            await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);
                _labelTranslationRepository.SetSqlExecutorForTransaction(connection);

                await _labelTranslationRepository.Insert(currentUserId, labelTranslation);

                var organization = await _organizationRepository.SelectById(labelTranslation.OrganizationId);
                organization.LabelTranslationCount++;
                await _organizationRepository.Update(currentUserId, organization);

                var project = await _projectRepository.SelectById(labelTranslation.ProjectId);
                project.LabelTranslationCount++;
                await _projectRepository.Update(currentUserId, project);

                var label = await _labelRepository.SelectById(labelTranslation.LabelId);
                label.LabelTranslationCount++;
                await _labelRepository.Update(currentUserId, label);

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelTranslationCount++;
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }

        public async Task<bool> DoCreateTranslationWorkBulk(long currentUserId, List<LabelTranslation> labelTranslations)
        {
            await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);
                _labelTranslationRepository.SetSqlExecutorForTransaction(connection);

                var first = labelTranslations.First();
                var organizationId = first.OrganizationId;
                var projectId = first.ProjectId;
                var labelId = first.LabelId;

                for (var i = 0; i < labelTranslations.Count; i++)
                {
                    var labelTranslation = labelTranslations[i];
                    await _labelTranslationRepository.Insert(currentUserId, labelTranslation);
                }

                var organization = await _organizationRepository.SelectById(organizationId);
                organization.LabelTranslationCount = organization.LabelTranslationCount + labelTranslations.Count;
                await _organizationRepository.Update(currentUserId, organization);

                var project = await _projectRepository.SelectById(projectId);
                project.LabelTranslationCount = project.LabelTranslationCount + labelTranslations.Count;
                await _projectRepository.Update(currentUserId, project);

                var label = await _labelRepository.SelectById(labelId);
                label.LabelTranslationCount = label.LabelTranslationCount + labelTranslations.Count;
                await _labelRepository.Update(currentUserId, label);

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelTranslationCount = user.LabelTranslationCount + labelTranslations.Count;
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }

        public async Task<bool> DoDeleteTranslationWork(long currentUserId, LabelTranslation labelTranslation)
        {
            await _transactionalExecutor.ExecuteAsync<bool>(async connection =>
            {
                _organizationRepository.SetSqlExecutorForTransaction(connection);
                _userRepository.SetSqlExecutorForTransaction(connection);
                _projectRepository.SetSqlExecutorForTransaction(connection);
                _labelRepository.SetSqlExecutorForTransaction(connection);
                _labelTranslationRepository.SetSqlExecutorForTransaction(connection);

                await _labelTranslationRepository.Delete(currentUserId, labelTranslation.Id);

                var organization = await _organizationRepository.SelectById(labelTranslation.OrganizationId);
                organization.LabelTranslationCount--;
                await _organizationRepository.Update(currentUserId, organization);

                var project = await _projectRepository.SelectById(labelTranslation.ProjectId);
                project.LabelTranslationCount--;
                await _projectRepository.Update(currentUserId, project);

                var label = await _labelRepository.SelectById(labelTranslation.LabelId);
                label.LabelTranslationCount--;
                await _labelRepository.Update(currentUserId, label);

                var user = await _userRepository.SelectById(currentUserId);
                user.LabelTranslationCount--;
                await _userRepository.Update(currentUserId, user);

                return true;
            });

            return true;
        }
    }
}