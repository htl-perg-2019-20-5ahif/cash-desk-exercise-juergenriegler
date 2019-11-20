using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashDesk
{
    /// <inheritdoc />
    public class DataAccess : IDataAccess
    {

        private CashDeskDataContext dataContext;

        /// <inheritdoc />
        public Task InitializeDatabaseAsync()
        {
            if (dataContext == null) dataContext = new CashDeskDataContext();
            else throw new InvalidOperationException("database already initialized");
            return Task.CompletedTask;
        }

        private void CheckDatabaseInitialized()
        {
            if (dataContext == null)
                throw new InvalidOperationException("database not initialized");
        }

        /// <inheritdoc />
        public async Task<int> AddMemberAsync(string firstName, string lastName, DateTime birthday)
        {
            CheckDatabaseInitialized();
            var newMember = new Member
            {
                FirstName = firstName,
                LastName = lastName,
                Birthday = birthday
            };
            dataContext.Members.Add(newMember);

            await dataContext.SaveChangesAsync();

            return newMember.MemberNumber;
        }

        /// <inheritdoc />
        public async Task DeleteMemberAsync(int memberNumber)
        {
            CheckDatabaseInitialized();
            dataContext.Members.Remove(dataContext.Members.Where(m => m.MemberNumber == memberNumber).FirstOrDefault());
            await dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IMembership> JoinMemberAsync(int memberNumber)
        {
            CheckDatabaseInitialized();
            var member = dataContext.Members.Where(m => m.MemberNumber == memberNumber).FirstOrDefault();
            if (member == default) throw new ArgumentException("no member found for given id");
            if (dataContext.Memberships.Where(ms => ms.Member.MemberNumber == memberNumber).LastOrDefault().End != null)
                throw new AlreadyMemberException();
            var newMembership = new Membership
            {
                Member = member,
                Begin = new DateTime()
            };
            dataContext.Memberships.Add(newMembership);
            await dataContext.SaveChangesAsync();
            return newMembership;
        }

        /// <inheritdoc />
        public async Task<IMembership> CancelMembershipAsync(int memberNumber)
        {
            CheckDatabaseInitialized();
            var membership = dataContext.Memberships.Where(ms => ms.Member.MemberNumber == memberNumber).FirstOrDefault();
            if (membership == default) throw new NoMemberException();
            membership.End = new DateTime();
            await dataContext.SaveChangesAsync();
            return membership;
        }

        /// <inheritdoc />
        public async Task DepositAsync(int memberNumber, decimal amount)
        {
            CheckDatabaseInitialized();
            var member = dataContext.Members.Where(m => m.MemberNumber == memberNumber).FirstOrDefault();
            if (member == default) throw new ArgumentException();
            var membership = dataContext.Memberships.Where(ms => ms.Member.MemberNumber == memberNumber).FirstOrDefault();
            if (membership == default) throw new NoMemberException();

            var deposit = new Deposit
            {
                Membership = membership,
                Amount = amount
            };
            await dataContext.Deposit.AddAsync(deposit);
            await dataContext.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDepositStatistics>> GetDepositStatisticsAsync() 
            => throw new NotImplementedException();

        /// <inheritdoc />
        public void Dispose()
        {
            if (dataContext != null)
            {
                dataContext.Dispose();
                dataContext = null;
            }
        }
    }
}
