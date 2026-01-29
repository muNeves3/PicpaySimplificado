using PicpaySimplificado.Utils;
using Xunit;

namespace PicpaySimplificado.Tests.Utils
{
    public class CPFCNPJValidatorTests
    {
        [Theory]
        [InlineData("08456709999")]
        [InlineData("12345678909")]
        public void IsCpf_DeveRetornarTrue_ParaCpfsValidos(string cpf)
        {
            Assert.True(CPFCNPJValidator.IsCpf(cpf));
        }

        [Theory]
        [InlineData("12345678900")] 
        [InlineData("123")]         
        [InlineData("ABC56789011")] 
        [InlineData("")]            
        public void IsCpf_DeveRetornarFalse_ParaCpfsInvalidos(string cpf)
        {
            Assert.False(CPFCNPJValidator.IsCpf(cpf));
        }

        [Theory]
        [InlineData("18545231000146")]
        [InlineData("11222333000181")]
        public void IsCnpj_DeveRetornarTrue_ParaCnpjsValidos(string cnpj)
        {
            Assert.True(CPFCNPJValidator.IsCnpj(cnpj));
        }

        [Fact]
        public void IsValidCpfCnpj_DeveIdentificarCorretamente_CpfOuCnpj()
        {
            Assert.True(CPFCNPJValidator.IsValidCpfCnpj("08456709999")); 
            Assert.True(CPFCNPJValidator.IsValidCpfCnpj("18545231000146")); 
            Assert.False(CPFCNPJValidator.IsValidCpfCnpj("123")); 
        }
    }
}