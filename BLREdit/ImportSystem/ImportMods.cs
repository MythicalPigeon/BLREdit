﻿using System.Collections.Generic;

namespace BLREdit
{
    public class ImportMods
    {
        public ImportItem[] ammo { get; set; }
        public ImportItem[] ammos { get; set; }
        public ImportItem[] barrels { get; set; }
        public ImportItem[] camos { get; set; }
        public ImportItem[] grips { get; set; }
        public ImportItem[] magazines { get; set; }
        public ImportItem[] muzzles { get; set; }
        public ImportItem[] primarySkins { get; set; }
        public ImportItem[] scopes { get; set; }
        public object[] secondarySkins { get; set; }
        public ImportItem[] stocks { get; set; }

        internal void AssignWikiStats(WikiStats[] stats)
        {
            ImportSystem.AssignWikiStatsTo(barrels, stats);
            ImportSystem.AssignWikiStatsTo(magazines, stats);
            ImportSystem.AssignWikiStatsTo(muzzles, stats);
            ImportSystem.AssignWikiStatsTo(scopes, stats);
            ImportSystem.AssignWikiStatsTo(stocks, stats);
        }

        public WikiStats[] GetWikiStats()
        {
            List<WikiStats> stats = new List<WikiStats>();
            stats.AddRange(ImportSystem.GetWikiStats(barrels));
            stats.AddRange(ImportSystem.GetWikiStats(magazines));
            stats.AddRange(ImportSystem.GetWikiStats(muzzles));
            stats.AddRange(ImportSystem.GetWikiStats(scopes));
            stats.AddRange(ImportSystem.GetWikiStats(stocks));
            return stats.ToArray();
        }

        public void UpdateImages()
        {
            ImportSystem.UpdateImagesForImportItems(ammo, "ammo");
            ImportSystem.UpdateImagesForImportItems(ammos, "ammos");
            ImportSystem.UpdateImagesForImportItems(barrels, "barrel");
            ImportSystem.UpdateImagesForImportItems(camos, "camo");
            ImportSystem.UpdateImagesForImportItems(grips, "grip");
            ImportSystem.UpdateImagesForImportItems(magazines, "magazine");
            ImportSystem.UpdateImagesForImportItems(muzzles, "muzzle");
            ImportSystem.UpdateImagesForImportItems(primarySkins, "primarySkin");
            ImportSystem.UpdateImagesForImportItems(scopes, "scope");
            ImportSystem.UpdateImagesForImportItems(stocks, "stock");
        }

        public ImportMods() { }
        public ImportMods(ImportMods mods)
        {
            ammo = ImportSystem.CleanItems(mods.ammo, "ammo");
            ammos = ImportSystem.CleanItems(mods.ammos, "ammos");
            barrels = ImportSystem.CleanItems(mods.barrels, "barrel");
            camos = ImportSystem.CleanItems(mods.camos, "camo");
            grips = ImportSystem.CleanItems(mods.grips, "grip");
            magazines = ImportSystem.CleanItems(mods.magazines, "magazine");
            muzzles = ImportSystem.CleanItems(mods.muzzles, "muzzle");
            primarySkins = ImportSystem.CleanItems(mods.primarySkins, "primarySkin");
            scopes = ImportSystem.CleanItems(mods.scopes, "scope");
            stocks = ImportSystem.CleanItems(mods.stocks, "stock");
        }
        public override string ToString()
        {
            string TextWall = "{";

            TextWall += "Ammo:\n";
            foreach (ImportItem item in ammo)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nAmmos:\n";
            foreach (ImportItem item in ammos)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nBarrels:\n";
            foreach (ImportItem item in barrels)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nCamos:\n";
            foreach (ImportItem item in camos)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nGrips:\n";
            foreach (ImportItem item in grips)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nMagazines:\n";
            foreach (ImportItem item in magazines)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nMuzzles:\n";
            foreach (ImportItem item in muzzles)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nPrimarySkins:\n";
            foreach (ImportItem item in primarySkins)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nScopes:\n";
            foreach (ImportItem item in scopes)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            TextWall += "\nStocks:\n";
            foreach (ImportItem item in stocks)
            {
                TextWall += "\t" + item.ToString() + "\n";
            }

            return TextWall + "}";
        }
    }
}
