//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class DrawMoney
    {
        public int Id { get; set; }
        public Nullable<int> UserId { get; set; }
        public string RemoveMoenyNumber { get; set; }
        public Nullable<int> State { get; set; }
        public string BankAccount { get; set; }
        public Nullable<decimal> RemoveMoeny { get; set; }
        public Nullable<System.DateTime> RemoveMoenyTime { get; set; }
        public string Bank { get; set; }
        public string BankName { get; set; }
        public Nullable<System.DateTime> TransactionTime { get; set; }
        public string Remarks { get; set; }
    }
}
