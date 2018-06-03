﻿using ApiGateway.Common.Constants;
using Xunit;

namespace ApiGateway.Common.Test
{
    public class ApiKeyTypesTest
    {
        [Fact]
        public void CheckCountValue()
        {
            Assert.Equal(ApiKeyTypes.ToList().Count, ApiKeyTypes.Count);
        }

        [Fact]
        public void Check_IsValid_With_Valid_Input()
        {
            Assert.True(ApiKeyTypes.IsValid(ApiKeyTypes.ClientSecret));
         
            Assert.True(ApiKeyTypes.IsValid(ApiKeyTypes.JwtToken));
        }

        [Fact]
        public void Check_IsValid_With_InValid_Input()
        {
            Assert.False(ApiKeyTypes.IsValid("some invalid text"));
        }
    }
}