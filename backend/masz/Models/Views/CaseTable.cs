using System;
using System.Collections.Generic;
using DSharpPlus.Entities;
using masz.Models.Views;

namespace masz.Models
{
    public class CaseTable
    {
        public List<ModCaseTableEntry> Cases { get; set; }
        public int FullSize { get; set; }
        public CaseTable(List<ModCaseTableEntry> modCase, int fullSize)
        {
            Cases = modCase;
            FullSize = fullSize;
        }
    }
}
