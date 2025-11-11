using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public void AddSystemAccount(SystemAccount account)
        {
            SystemAccountDAO.AddSystemAccount(account);
        }

        public void DeleteSystemAccount(short id)
        {
            SystemAccountDAO.DeleteSystemAccount(id);
        }

        public SystemAccount? GetSystemAccountByEmail(string email)
        {
            return SystemAccountDAO.GetSystemAccountByEmail(email);
        }

        public SystemAccount? GetSystemAccountById(short id)
        {
            return SystemAccountDAO.GetSystemAccountById(id);
        }

        public List<SystemAccount> GetSystemAccountByUsername(string keyword)
        {
            return SystemAccountDAO.GetSystemAccountByUsername(keyword);
        }

        public List<SystemAccount> GetSystemAccounts()
        {
            return SystemAccountDAO.GetSystemAccounts();
        }

        public void UpdateSystemAccount(SystemAccount account)
        {
            SystemAccountDAO.UpdateSystemAccount(account);
        }
    }
}
