using System;

namespace App.Client.Migrations.Base
{
    public class SchemaVersion
    {
        public int Id { get; set; }
        public long Version { get; set; }
        public DateTime AppliedOn { get; set; }
        public string Description { get; set; }
    }
}