﻿using System;
using System.Linq.Expressions;

using Moq;

using Translation.Data.Entities.Main;
using Translation.Data.Repositories.Contracts;
using static Translation.Tests.TestHelpers.FakeEntityTestHelper;
using static Translation.Tests.TestHelpers.FakeConstantTestHelper;

namespace Translation.Tests.SetupHelpers
{
    public static class TokenRequestLogRepositorySetupHelper
    {
        public static void Setup_Count_Returns_POSITIVE_INT_NUMBER_10(this Mock<ITokenRequestLogRepository> repository)
        {
            repository.Setup(x => x.Count(It.IsAny<Expression<Func<TokenRequestLog, bool>>>(), false))
                      .ReturnsAsync(Ten);
        }

        public static void Verify_Count(this Mock<ITokenRequestLogRepository> repository)
        {
            repository.Verify(x => x.Count(It.IsAny<Expression<Func<TokenRequestLog, bool>>>(), false));
        }

        public static void Verify_SelectMany(this Mock<ITokenRequestLogRepository> repository)
        {
            repository.Verify(x =>
                    x.SelectMany(It.IsAny<Expression<Func<TokenRequestLog, bool>>>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<Expression<Func<TokenRequestLog, object>>>(),
                        It.IsAny<bool>(), false));
        }

        public static void Verify_SelectAfter(this Mock<ITokenRequestLogRepository> repository)
        {
            repository.Verify(x =>
                    x.SelectAfter(It.IsAny<Expression<Func<TokenRequestLog, bool>>>(),
                        It.IsAny<Guid>(),
                        It.IsAny<int>(),
                        It.IsAny<Expression<Func<TokenRequestLog, object>>>(),
                        It.IsAny<bool>(), false));
        }
    }
}