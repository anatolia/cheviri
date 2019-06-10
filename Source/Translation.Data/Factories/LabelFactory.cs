﻿using Translation.Common.Models.DataTransferObjects;
using Translation.Common.Models.Requests.Label;
using Translation.Data.Entities.Domain;

namespace Translation.Data.Factories
{
    public class LabelFactory
    {
        public Label CreateEntityFromRequest(string key, Project projectEntity)
        {
            var entity = new Label();
            entity.Name = key;
            entity.Key = key;
            entity.IsActive = true;

            entity.ProjectId = projectEntity.Id;
            entity.ProjectUid = projectEntity.Uid;
            entity.ProjectName = projectEntity.Name;

            entity.OrganizationId = projectEntity.OrganizationId;
            entity.OrganizationUid = projectEntity.OrganizationUid;
            entity.OrganizationName = projectEntity.OrganizationName;

            return entity;
        }

        public Label CreateEntityFromRequest(LabelCloneRequest request, Label label)
        {
            var entity = new Label();
            entity.Description = request.Description;
            entity.Key = request.LabelKey;
            entity.Name = request.LabelKey;

            entity.ProjectId = label.Id;
            entity.ProjectUid = label.Uid;
            entity.ProjectName = label.Name;
            entity.OrganizationId = label.OrganizationId;
            entity.OrganizationUid = label.OrganizationUid;
            entity.OrganizationName = label.OrganizationName;
            entity.IsActive = true;

            return entity;
        }

        public Label CreateEntityFromRequest(LabelEditRequest request, Label entity)
        {
            entity.UpdatedBy = request.CurrentUserId;
            entity.Key = request.LabelKey;
            entity.Name = request.LabelKey;
            entity.Description = request.Description;

            return entity;
        }

        public Label CreateEntityFromRequest(LabelCreateRequest request, Project project)
        {
            var entity = new Label();
            entity.CreatedBy = request.CurrentUserId;
            entity.Key = request.LabelKey;
            entity.Name = request.LabelKey;
            entity.Description = request.Description;

            entity.OrganizationId = project.OrganizationId;
            entity.OrganizationUid = project.OrganizationUid;
            entity.OrganizationName = project.OrganizationName;
            entity.ProjectId = project.Id;
            entity.ProjectUid = project.Uid;
            entity.ProjectName = project.Name;
            entity.IsActive = true;

            return entity;
        }

        public LabelDto CreateDtoFromEntity(Label entity)
        {
            var dto = new LabelDto();
            dto.Uid = entity.Uid;
            dto.CreatedAt = entity.CreatedAt;
            dto.UpdatedAt = entity.UpdatedAt;
            dto.Key = entity.Key;
            dto.Name = entity.Name;
            dto.Description = entity.Description;

            dto.OrganizationUid = entity.OrganizationUid;
            dto.OrganizationName = entity.OrganizationName;
            dto.ProjectUid = entity.ProjectUid;
            dto.ProjectName = entity.ProjectName;
            dto.IsActive = entity.IsActive;

            return dto;
        }

        public Label UpdateEntityForChangeActivation(Label entity)
        {
            entity.IsActive = !entity.IsActive;
            return entity;
        }
    }
}