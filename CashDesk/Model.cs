using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CashDesk
{
    class Member : IMember
    {
        [Key]
        public int MemberNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        public List<Membership> Membership { get; set; }
    }

    class Membership : IMembership
    {
        public int MembershipId { get; set; }

        [Required]
        public Member Member { get; set; }

        [Required]
        public DateTime Begin { get; set; }

        private DateTime end = DateTime.MaxValue;

        public DateTime End {
            get { return end; }
            set
            {
                if (Begin > value)
                    throw new ArgumentException("Enddate mustn't be before Begindate");
                end = value;
            }
        }

        IMember IMembership.Member => Member;
    }

    class Deposit : IDeposit
    {
        public int DepositId { get; set; }

        [Required]
        public Membership Membership { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        public decimal Amount { get; set; }

        IMembership IDeposit.Membership => Membership;
    }

    class DepositStatistics : IDepositStatistics
    {

        public IMember Member { get; set; }

        public int Year { get; set; }

        public decimal TotalAmount { get; set; }
    }

}

