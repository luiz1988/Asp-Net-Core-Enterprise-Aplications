using FluentValidation.Results;
using NSE.Core.Data;
using System.Threading.Tasks;

namespace NSE.Core.Messages
{
    public abstract class CommandHandler
    {
        protected FluentValidation.Results.ValidationResult ValidationResult;

        protected CommandHandler()
        {
            ValidationResult = new FluentValidation.Results.ValidationResult();
        }

        protected void AdicionarErro(string mensagem)
        {
            ValidationResult.Errors.Add(new ValidationFailure(string.Empty, mensagem));
        }

        protected async Task<FluentValidation.Results.ValidationResult> PersistirDados(IUnitOfWork uow)
        {
            try
            {
                if (await uow.Commit()) AdicionarErro("Houve um erro ao persistir os dados");

                return ValidationResult;
            }
            catch (System.Exception EX)
            {

                throw EX;
            }
           
        }
    }
}
