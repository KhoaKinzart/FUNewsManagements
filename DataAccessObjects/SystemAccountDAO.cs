using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class SystemAccountDAO
    {
        public static List<SystemAccount> GetSystemAccounts()
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.SystemAccounts.ToList();
            }
        }
        public static void AddSystemAccount(SystemAccount systemAccount)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.SystemAccounts.Add(systemAccount);
                context.SaveChanges();
            }
        }
        public static void UpdateSystemAccount(SystemAccount systemAccount)
        {
            using (var context = new FuNewsManagementContext())
            {
                context.SystemAccounts.Update(systemAccount);
                context.SaveChanges();
            }
        }
        public static void DeleteSystemAccount(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                var systemAccount = context.SystemAccounts.Find(id);
                if (systemAccount != null)
                {
                    context.SystemAccounts.Remove(systemAccount);
                    context.SaveChanges();
                }
            }
        }
        public static SystemAccount GetSystemAccountById(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.SystemAccounts.Find(id);
            }
        }
        public static bool SystemAccountExists(short id)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.SystemAccounts.Any(a => a.AccountID == id);
            }
        }
        public static SystemAccount GetSystemAccountByEmail(string email)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email);
            }
        }
        public static List<SystemAccount> GetSystemAccountByUsername(string username)
        {
            using (var context = new FuNewsManagementContext())
            {
                return context.SystemAccounts.Where(a => a.AccountName == username).ToList();
            }
        }
    }
}
