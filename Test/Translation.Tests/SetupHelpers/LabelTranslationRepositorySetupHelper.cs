﻿using System;
using System.Linq.Expressions;

using Moq;

using Translation.Data.Entities.Domain;
using Translation.Data.Repositories.Contracts;
using static Translation.Tests.TestHelpers.FakeEntityTestHelper;

namespace Translation.Tests.SetupHelpers
{
    public static class LabelTranslationRepositorySetupHelper
    {
        public static void Setup_SelectById_Returns_ParkNetLabelTranslation(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.SelectById(It.IsAny<long>()))
                      .ReturnsAsync(GetOrganizationOneProjectOneLabelOneLabelTranslationOne());

        }

        public static void Verify_SelectById(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.SelectById(It.IsAny<long>()));
        }

        public static void Setup_Select_Returns_ParkNetLabelTranslation(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Select(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), false))
                      .ReturnsAsync(GetOrganizationOneProjectOneLabelOneLabelTranslationOne());
        }

        public static void Verify_Select(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.Select(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), false));
        }

        public static void Verify_SelectMany(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.SelectMany(It.IsAny<Expression<Func<LabelTranslation, bool>>>(),
                                               It.IsAny<int>(),
                                               It.IsAny<int>(),
                                               It.IsAny<Expression<Func<LabelTranslation, object>>>(),
                                               It.IsAny<bool>(), false));
        }

        public static void Setup_Insert_Success(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Insert(It.IsAny<long>(),
                                           It.IsAny<LabelTranslation>()))
                      .ReturnsAsync(1);
        }

        public static void Setup_Insert_Failed(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Insert(It.IsAny<long>(),
                                           It.IsAny<LabelTranslation>()))
                      .ReturnsAsync(0);
        }

        public static void Verify_Insert(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.Insert(It.IsAny<long>(),
                It.IsAny<LabelTranslation>()));
        }

        public static void Setup_Delete_Success(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Delete(It.IsAny<long>(),
                                           It.IsAny<long>()))
                      .ReturnsAsync(true);
        }

        public static void Setup_Delete_Failed(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Delete(It.IsAny<long>(),
                                           It.IsAny<long>()))
                      .ReturnsAsync(false);
        }

        public static void Verify_Delete(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.Delete(It.IsAny<long>(),
                                            It.IsAny<long>()));
        }

        public static void Setup_Update_Success(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Update(It.IsAny<long>(),
                                           It.IsAny<LabelTranslation>()))
                      .ReturnsAsync(true);
        }

        public static void Setup_Update_Failed(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Update(It.IsAny<long>(),
                                           It.IsAny<LabelTranslation>()))
                      .ReturnsAsync(false);
        }

        public static void Verify_Update(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.Update(It.IsAny<long>(),
                                            It.IsAny<LabelTranslation>()));
        }

        public static void Setup_LabelAlreadyExist(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Any(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), false))
                      .ReturnsAsync(true);
        }

        public static void Setup_LabelNotExist(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Any(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), false))
                      .ReturnsAsync(false);
        }

        public static void Setup_Any_Return_False(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Any(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), It.IsAny<bool>()))
                      .ReturnsAsync(false);
        }

        public static void Setup_Any_Return_True(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Setup(x => x.Any(It.IsAny<Expression<Func<LabelTranslation, bool>>>(),                           It.IsAny<bool>()))
                      .ReturnsAsync(true);
        }

        public static void Verify_Any(this Mock<ILabelTranslationRepository> repository)
        {
            repository.Verify(x => x.Any(It.IsAny<Expression<Func<LabelTranslation, bool>>>(), false));
        }
    }
}