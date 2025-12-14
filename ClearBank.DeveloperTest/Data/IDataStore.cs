using System.Threading.Tasks;
using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data;

public interface IDataStore
{
    Account GetAccount(string requestDebtorAccountNumber);

    void UpdateAccount(Account account);
}