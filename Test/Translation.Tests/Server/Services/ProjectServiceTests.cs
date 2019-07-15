﻿using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using Shouldly;

using Translation.Common.Contracts;
using Translation.Common.Enumerations;
using Translation.Common.Models.Responses.Project;
using Translation.Tests.SetupHelpers;
using static Translation.Tests.TestHelpers.FakeRequestTestHelper;
using static Translation.Tests.TestHelpers.AssertViewModelTestHelper;

namespace Translation.Tests.Server.Services
{
    [TestFixture]
    public class ProjectServiceTests : ServiceBaseTests
    {
        public IProjectService SystemUnderTest { get; set; }

        [SetUp]
        public void run_before_every_test()
        {
            SystemUnderTest = Container.Resolve<IProjectService>();
        }

        [Test]
        public async Task ProjectService_GetProjects_InvalidOrganizationEntity()
        {
            // arrange
            var request = GetProjectReadListRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationTwoUserOne();

            // act
            var result = await SystemUnderTest.GetProjects(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Invalid);
            AssertReturnType<ProjectReadListResponse>(result);
            MockUserRepository.Verify_SelectById();
        }

        [Test]
        public async Task ProjectService_GetProjects_Success()
        {
            // arrange
            var request = GetProjectReadListRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();

            // act
            var result = await SystemUnderTest.GetProjects(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectReadListResponse>(result);
            MockUserRepository.Verify_SelectById();
        }

        [Test]
        public async Task ProjectService_GetProjectRevisions_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectRevisionReadListRequest();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.GetProjectRevisions(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectRevisionReadListResponse>(result);
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_GetProjectRevisions_Success()
        {
            // arrange
            var request = GetProjectRevisionReadListRequest();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_SelectRevisions_Returns_OrganizationOneProjectOneRevisions();

            // act
            var result = await SystemUnderTest.GetProjectRevisions(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectRevisionReadListResponse>(result);
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_SelectRevisions();
        }

        [Test]
        public async Task ProjectService_GetProject_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectReadRequest();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.GetProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectReadResponse>(result);
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_GetProject_Success()
        {
            // arrange
            var request = GetProjectReadRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();

            // act
            var result = await SystemUnderTest.GetProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectReadResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_CreateProject_ProjectAlreadyExist()
        {
            // arrange
            var request = GetProjectCreateRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.CreateProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            result.ErrorMessages.Any(x => x == "project_name_must_be_unique").ShouldBeTrue();
            AssertReturnType<ProjectCreateResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CreateProject_OrganizationAlreadyExist()
        {
            // arrange
            var request = GetProjectCreateRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockOrganizationRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.CreateProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseParentNotActive);
            AssertReturnType<ProjectCreateResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CreateProject_Failed()
        {
            // arrange
            var request = GetProjectCreateRequest();
            MockProjectRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.CreateProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            AssertReturnType<ProjectCreateResponse>(result);
            MockProjectRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CreateProject_Success()
        {
            // arrange
            var request = GetProjectCreateRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Any_Returns_False();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectUnitOfWork.Setup_DoCreateWork_Returns_True();

            // act
            var result = await SystemUnderTest.CreateProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectCreateResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Any();
            MockUserRepository.Verify_SelectById();
            MockProjectUnitOfWork.Verify_DoCreateWork();
        }

        [Test]
        public async Task ProjectService_EditProject_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectEditRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();
            MockOrganizationRepository.Setup_Any_Returns_False();

            // act
            var result = await SystemUnderTest.EditProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectEditResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_EditProject_OrganizationNotExist()
        {
            // arrange
            var request = GetProjectEditRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockOrganizationRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.EditProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseParentNotActive);
            AssertReturnType<ProjectEditResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_EditProject_ProjectAlreadyExist()
        {
            // arrange
            var request = GetProjectEditRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.EditProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            result.ErrorMessages.Any(x => x == "project_name_must_be_unique").ShouldBeTrue();
            AssertReturnType<ProjectEditResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_EditProject_Success()
        {
            // arrange
            var request = GetProjectEditRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Update_Success();
            MockProjectRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();

            // act
            var result = await SystemUnderTest.EditProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectEditResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Update();
            MockProjectRepository.Verify_Any();
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_EditProject_Failed()
        {
            // arrange
            var request = GetProjectEditRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Any_Returns_True();


            // act
            var result = await SystemUnderTest.EditProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            result.ErrorMessages.Any(x => x == "project_name_must_be_unique").ShouldBeTrue();
            AssertReturnType<ProjectEditResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CloneProject_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectCloneRequest();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.CloneProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectCloneResponse>(result);
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_CloneProject_OrganizationNotExist()
        {
            // arrange
            var request = GetProjectCloneRequest();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockOrganizationRepository.Setup_Any_Returns_False();

            // act
            var result = await SystemUnderTest.CloneProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            AssertReturnType<ProjectCloneResponse>(result);
            MockProjectRepository.Verify_Select();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CloneProject_ProjectAlreadyExist()
        {
            // arrange
            var request = GetProjectCloneRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.CloneProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            result.ErrorMessages.Any(x => x == "project_name_must_be_unique").ShouldBeTrue();
            AssertReturnType<ProjectCloneResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_CloneProject_Success()
        {
            // arrange
            var request = GetProjectCloneRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_Any_Returns_False();
            MockProjectUnitOfWork.Setup_DoCloneWork_Returns_True();

            // act
            var result = await SystemUnderTest.CloneProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectCloneResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_Any();
            MockProjectUnitOfWork.Verify_DoCloneWork();
        }

        [Test]
        public async Task ProjectService_DeleteProject__InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectDeleteRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.DeleteProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectDeleteResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_DeleteProject__OrganizationNotExist()
        {
            // arrange
            var request = GetProjectDeleteRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockOrganizationRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.DeleteProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Invalid);
            AssertReturnType<ProjectDeleteResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_DeleteProject_Success()
        {
            // arrange
            var request = GetProjectDeleteRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Delete_Success();

            // act
            var result = await SystemUnderTest.DeleteProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectDeleteResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Delete();
        }

        [Test]
        public async Task ProjectService_DeleteProject_Failed()
        {
            // arrange
            var request = GetProjectDeleteRequest();
            MockOrganizationRepository.Setup_Any_Returns_False();
            MockProjectRepository.Setup_Delete_Failed();

            // act
            var result = await SystemUnderTest.DeleteProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Failed);
            AssertReturnType<ProjectDeleteResponse>(result);
            MockOrganizationRepository.Verify_Any();
            MockProjectRepository.Verify_Delete();
        }

        [Test]
        public async Task ProjectService_ChangeActivationForProject_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectChangeActivationRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.ChangeActivationForProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectChangeActivationResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_ChangeActivationForProject_OrganizationAlreadyExist()
        {
            // arrange
            var request = GetProjectChangeActivationRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockOrganizationRepository.Setup_Any_Returns_True();

            // act
            var result = await SystemUnderTest.ChangeActivationForProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.Invalid);
            AssertReturnType<ProjectChangeActivationResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_ChangeActivationForProject_Success()
        {
            // arrange
            var request = GetProjectChangeActivationRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneAdminUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_Update_Success();
            MockOrganizationRepository.Setup_Any_Returns_False();

            // act
            var result = await SystemUnderTest.ChangeActivationForProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectChangeActivationResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_Update();
            MockOrganizationRepository.Verify_Any();
        }

        [Test]
        public async Task ProjectService_RestoreProject_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectRestoreRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneUserOne();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.RestoreProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            result.InfoMessages.Any(x => x == "project_not_found").ShouldBeTrue();
            AssertReturnType<ProjectRestoreResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_RestoreProject_InvalidRevisionEntity()
        {
            // arrange
            var request = GetProjectRestoreRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_SelectRevisions_Returns_InvalidRevision();

            // act
            var result = await SystemUnderTest.RestoreProject(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            result.InfoMessages.Any(x => x == "revision_not_found").ShouldBeTrue();
            AssertReturnType<ProjectRestoreResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_SelectRevisions();
        }

        [Test]
        public async Task ProjectService_RestoreProject_Success()
        {
            // arrange
            var request = GetProjectRestoreRequest();
            MockUserRepository.Setup_SelectById_Returns_OrganizationOneUserOne();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();
            MockProjectRepository.Setup_SelectRevisions_Returns_OrganizationOneProjectOneRevisions();
            MockProjectRepository.Setup_RestoreRevision_Returns_True();

            // act
            var result = await SystemUnderTest.RestoreProject(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectRestoreResponse>(result);
            MockUserRepository.Verify_SelectById();
            MockProjectRepository.Verify_Select();
            MockProjectRepository.Verify_SelectRevisions();
            MockProjectRepository.Verify_RestoreRevision();
        }

        [Test]
        public async Task ProjectService_GetPendingTranslations_InvalidProjectEntity()
        {
            // arrange
            var request = GetProjectPendingTranslationReadListRequest();
            MockProjectRepository.Setup_Select_Returns_InvalidProject();

            // act
            var result = await SystemUnderTest.GetPendingTranslations(request);

            // assert
            result.ErrorMessages.ShouldNotBeNull();
            result.Status.ShouldBe(ResponseStatus.InvalidBecauseEntityNotFound);
            AssertReturnType<ProjectPendingTranslationReadListResponse>(result);
            MockProjectRepository.Verify_Select();
        }

        [Test]
        public async Task ProjectService_GetPendingTranslations_Success()
        {
            // arrange
            var request = GetProjectPendingTranslationReadListRequest();
            MockProjectRepository.Setup_Select_Returns_OrganizationOneProjectOne();

            // act
            var result = await SystemUnderTest.GetPendingTranslations(request);

            // assert
            result.Status.ShouldBe(ResponseStatus.Success);
            result.ErrorMessages.ShouldNotBeNull();
            result.ErrorMessages.Count.ShouldBe(0);
            AssertReturnType<ProjectPendingTranslationReadListResponse>(result);
            MockProjectRepository.Verify_Select();
        }

    }
}