using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BLREdit.UI
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Image LastSelectedImage = null;
        private ImportItem FilterWeapon = null;
        public static Loadout ActiveLoadout = null;

        

        public MainWindow()
        {
            profilechanging = true;
            textchnaging = true;

            

            InitializeComponent();

            ItemList.Items.Filter += new Predicate<object>(o =>
            {
                if (o != null)
                {
                    return ((ImportItem)o).IsValidFor(FilterWeapon);
                }
                else
                {
                    return false;
                }
            });


            PlayerNameTextBox.Text = ExportSystem.ActiveProfile.PlayerName;
            ProfileComboBox.ItemsSource = ExportSystem.Profiles;
            ProfileComboBox.SelectedIndex = 0;

            SetLoadout(ExportSystem.ActiveProfile.Loadout1);

            Loadout1Button.IsEnabled = false;

            profilechanging = false;
            textchnaging = false;
        }

        private void UpdatePrimaryStats()
        {
            UpdateStats(
                PrimaryRecieverImage.DataContext as ImportItem,
                PrimaryBarrelImage.DataContext as ImportItem,
                PrimaryMagazineImage.DataContext as ImportItem,
                PrimaryMuzzleImage.DataContext as ImportItem,
                PrimaryScopeImage.DataContext as ImportItem,
                PrimaryStockImage.DataContext as ImportItem,
                PrimaryDamageLabel,
                PrimaryRateOfFireLabel,
                PrimaryAmmoLabel,
                PrimaryReloadLabel,
                PrimarySwapLabel,
                PrimaryAimLabel,
                PrimaryHipLabel,
                PrimaryMoveLabel,
                PrimaryRecoilLabel,
                PrimaryZoomRecoilLabel,
                PrimaryZoomLabel,
                PrimaryScopeInLabel,
                PrimaryRangeLabel,
                PrimaryRunLabel,
                PrimaryDescriptorLabel
                );
        }

        private void UpdateSecondaryStats()
        {
            UpdateStats(
                SecondaryRecieverImage.DataContext as ImportItem,
                SecondaryBarrelImage.DataContext as ImportItem,
                SecondaryMagazineImage.DataContext as ImportItem,
                SecondaryMuzzleImage.DataContext as ImportItem,
                SecondaryScopeImage.DataContext as ImportItem,
                SecondaryStockImage.DataContext as ImportItem,
                SecondaryDamageLabel,
                SecondaryRateOfFireLabel,
                SecondaryAmmoLabel,
                SecondaryReloadLabel,
                SecondarySwapLabel,
                SecondaryAimLabel,
                SecondaryHipLabel,
                SecondaryMoveLabel,
                SecondaryRecoilLabel,
                SecondaryZoomRecoilLabel,
                SecondaryZoomLabel,
                SecondaryScopeInLabel,
                SecondaryRangeLabel,
                SecondaryRunLabel,
                SecondaryDescriptorLabel
        );
        }

        private static double Lerp(double start, double target, double time)
        {
            return start * (1.0d - time) + target * time;
        }

        private static bool CheckCalculationReady(ImportItem item)
        {
            return item != null && item.WikiStats != null && item.IniStats != null && item.stats != null;
        }

        private static void UpdateStat(ImportItem[] items, ref double ROF, ref double AmmoMag, ref double AmmoRes, ref double Reload, ref double Swap, ref double Zoom, ref double ScopeIn, ref double Run)
        {
            foreach (ImportItem item in items)
            {
                ROF += item?.WikiStats?.firerate ?? 0;
                AmmoMag += item?.WikiStats?.ammoMag ?? 0;
                AmmoRes += item?.WikiStats?.ammoReserve ?? 0;
                Reload += item?.WikiStats?.reload ?? 0;
                Swap += item?.WikiStats?.swaprate ?? 0;
                Zoom += item?.WikiStats?.zoom ?? 0;
                ScopeIn += item?.WikiStats?.scopeInTime ?? 0;
                Run += item?.WikiStats?.run ?? 0;
            }
        }

        private static void UpdateStats(ImportItem Reciever, ImportItem Barrel, ImportItem Magazine, ImportItem Muzzle, ImportItem Scope, ImportItem Stock, Label DamageLabel, Label ROFLabel, Label AmmoLabel, Label ReloadLabel, Label SwapLabel, Label AimLabel, Label HipLabel, Label MoveLabel, Label RecoilLabel, Label ZoomRecoilLabel, Label ZoomLabel, Label ScopeInLabel, Label RangeLabel, Label RunLabel, Label Descriptor)
        {
            var watch = LoggingSystem.LogInfo("Updating Stats", "");

            double Damage = 0, DamageFar = 0, ROF = 0, AmmoMag = 0, AmmoRes = 0, Reload = 0, Swap = 0, Aim = 0, Hip = 0, Move = 0, Recoil = 0, RecoilZoom = 0, Zoom = 0, ScopeIn = 0, RangeClose = 0, RangeFar = 0, RangeMax = 0, Run = 0;

            if (CheckCalculationReady(Reciever))
            {
                List<ImportItem> items = new List<ImportItem>();
                if (Reciever != null)
                    items.Add(Reciever);
                if (Barrel != null)
                    items.Add(Barrel);
                if (Magazine != null)
                    items.Add(Magazine);
                if (Muzzle != null)
                    items.Add(Muzzle);
                if (Scope != null)
                    items.Add(Scope);
                if (Stock != null)
                    items.Add(Stock);
                if (Stock != null)
                    items.Add(Stock);

                UpdateStat(items.ToArray(), ref ROF, ref AmmoMag, ref AmmoRes, ref Reload, ref Swap, ref Zoom, ref ScopeIn, ref Run);

                double allRecoil = Barrel?.weaponModifiers?.recoil ?? 0;
                allRecoil += Muzzle?.weaponModifiers?.recoil ?? 0;
                allRecoil += Stock?.weaponModifiers?.recoil ?? 0;
                allRecoil += Scope?.weaponModifiers?.recoil ?? 0;
                allRecoil += Magazine?.weaponModifiers?.recoil ?? 0;
                allRecoil /= 100.0f;
                allRecoil = Math.Min(Math.Max(allRecoil, -1.0f), 1.0f);
                Recoil = CalculateRecoil(Reciever, allRecoil);
                RecoilZoom = Recoil * Reciever.IniStats.RecoilZoomMultiplier * 0.8;


                double allDamage = Barrel?.weaponModifiers?.damage ?? 0;
                allDamage += Muzzle?.weaponModifiers?.damage ?? 0;
                allDamage += Stock?.weaponModifiers?.damage ?? 0;
                allDamage += Scope?.weaponModifiers?.damage ?? 0;
                allDamage += Magazine?.weaponModifiers?.damage ?? 0;
                allDamage /= 100.0f;
                allDamage = Math.Min(Math.Max(allDamage, -1.0f), 1.0f);
                var damages = CalculateDamage(Reciever, allDamage);
                Damage = damages[0];
                DamageFar = damages[1];

                double allRange = Barrel?.weaponModifiers?.range ?? 0;
                allRange += Muzzle?.weaponModifiers?.range ?? 0;
                allRange += Stock?.weaponModifiers?.range ?? 0;
                allRange += Scope?.weaponModifiers?.range ?? 0;
                allRange += Magazine?.weaponModifiers?.range ?? 0;
                allRange /= 100.0f;
                allRange = Math.Min(Math.Max(allRange, -1.0f), 1.0f);
                var ranges = CalculateRange(Reciever, allRange);
                RangeClose = ranges[0];
                RangeFar = ranges[1];
                RangeMax = ranges[2];

                float allMovementSpread = Barrel?.weaponModifiers?.movementSpeed ?? 0;  // For specifically my move spread change, hence no mag/scope added.
                allMovementSpread += Muzzle?.weaponModifiers?.movementSpeed ?? 0;
                allMovementSpread += Stock?.weaponModifiers?.movementSpeed ?? 0;
                allMovementSpread /= 100.0f;
                allMovementSpread = Math.Min(Math.Max(allMovementSpread, -1.0f), 1.0f);

                float allAccuracy = Barrel?.weaponModifiers?.accuracy ?? 0;
                allAccuracy += Muzzle?.weaponModifiers?.accuracy ?? 0;
                allAccuracy += Stock?.weaponModifiers?.accuracy ?? 0;
                allAccuracy += Scope?.weaponModifiers?.accuracy ?? 0;
                allAccuracy += Magazine?.weaponModifiers?.accuracy ?? 0;
                allAccuracy /= 100.0f;
                allAccuracy = Math.Min(Math.Max(allAccuracy,-1.0f),1.0f);
                var spreads = CalculateSpread(Reciever, allAccuracy, allMovementSpread);
                Aim = spreads[0];
                Hip = spreads[1];
                Move = spreads[2];
            }

            List<ImportItem> mods = new List<ImportItem>();
            if (Barrel != null)
                mods.Add(Barrel);
            if (Magazine != null)
                mods.Add(Magazine);
            if (Muzzle != null)
                mods.Add(Muzzle);
            if (Scope != null)
                mods.Add(Scope);
            if (Stock != null)
                mods.Add(Stock);
            if (Stock != null)
                mods.Add(Stock);

            string barrelVSmag = CompareItemDescriptor1(Barrel, Magazine);
            string stockVSmuzzle = CompareItemDescriptor2(Stock, Muzzle, Scope);

            string weaponDescriptor = Reciever.GetDescriptorName(TotalPoints(mods));

            DamageLabel.Content = Damage.ToString("0.0") + " / " + DamageFar.ToString("0.0");
            ROFLabel.Content = ROF.ToString("0");
            AmmoLabel.Content = AmmoMag.ToString("0") + " / " + (AmmoMag * Reciever.IniStats.InitialMagazines).ToString("0");
            ReloadLabel.Content = Reload.ToString("0.00") + "s";
            SwapLabel.Content = Swap.ToString("0.00");
            AimLabel.Content = Aim.ToString("0.00") + "°";
            HipLabel.Content = Hip.ToString("0.00") + "°";
            MoveLabel.Content = Move.ToString("0.00") + "°";
            RecoilLabel.Content = Recoil.ToString("0.00") + "°";
            ZoomRecoilLabel.Content = RecoilZoom.ToString("0.00") + "°";
            ZoomLabel.Content = Zoom.ToString("0.00");
            ScopeInLabel.Content = ScopeIn.ToString("0.000") + "s";
            RangeLabel.Content = RangeClose.ToString("0.0") + " / " + RangeFar.ToString("0.0") + " / " + RangeMax.ToString("0");
            RunLabel.Content = Run.ToString("0.00");
            Descriptor.Content = barrelVSmag + " " + stockVSmuzzle + " " + weaponDescriptor;
            LoggingSystem.LogInfoAppend(watch);
        }

        private static int TotalPoints(IEnumerable<ImportItem> items)
        {
            int points = 0;
            foreach (ImportItem item in items)
            {
                points += item.weaponModifiers.rating;
            }
            return points;
        }

        private static string CompareItemDescriptor1(ImportItem item1, ImportItem item2)
        {
            if (item1 == null && item2 != null)
            {
                return item2.descriptorName;
            }
            else if (item1 != null && item2 == null)
            {
                return item1.descriptorName;
            }
            else if (item1 == null && item2 == null)
            {
                return "Standard";
            }

            if (item1.weaponModifiers.rating > item2.weaponModifiers.rating)
            {
                return item1.descriptorName;
            }
            else
            {
                return item2.descriptorName;
            }
        }

        private static string CompareItemDescriptor2(ImportItem item1, ImportItem item2, ImportItem item3)
        {
            if (item1 == null && item2 == null && item3 == null)
            {
                return "Basic";
            }

            if (item1 == null && item2 != null)
            {
                return item2.descriptorName;
            }
            else if (item1 != null && item2 == null)
            {
                return item1.descriptorName;
            }
            else if (item1 == null && item2 == null && item3 != null)
            {
                return item3.descriptorName;
            }

            if ( (item1.weaponModifiers.rating >= item2.weaponModifiers.rating) && (item1.weaponModifiers.rating >= item3.weaponModifiers.rating) )
            {
                if (item1.weaponModifiers.rating > 0)
                {
                    return item1.descriptorName;
                }
                return "Basic";
            }
            else if ( (item2.weaponModifiers.rating >= item1.weaponModifiers.rating) && (item2.weaponModifiers.rating >= item3.weaponModifiers.rating) )
            {
                return item2.descriptorName;
            }
            else if ( (item3.weaponModifiers.rating >= item1.weaponModifiers.rating) && (item3.weaponModifiers.rating >= item2.weaponModifiers.rating) )
            {
                return item3.descriptorName;
            }

            return item1.descriptorName;
        }

        private static double[] CalculateRange(ImportItem Reciever, double allRange)
        {
            if (Reciever != null && Reciever.WikiStats != null && Reciever.IniStats != null)
            {
                double idealRange;
                double maxRange;
                double alpha = Math.Abs(allRange);
                if (allRange > 0)
                {
                    idealRange = (int)Lerp(Reciever?.IniStats?.ModificationRangeIdealDistance.Z ?? 0, Reciever?.IniStats?.ModificationRangeIdealDistance.Y ?? 0, alpha);
                    maxRange = Lerp(Reciever?.IniStats?.ModificationRangeMaxDistance.Z ?? 0, Reciever?.IniStats?.ModificationRangeMaxDistance.Y ?? 0, alpha);
                }
                else
                {
                    idealRange = (int)Lerp(Reciever?.IniStats?.ModificationRangeIdealDistance.Z ?? 0, Reciever?.IniStats?.ModificationRangeIdealDistance.X ?? 0, alpha);
                    maxRange = Lerp(Reciever?.IniStats?.ModificationRangeMaxDistance.Z ?? 0, Reciever?.IniStats?.ModificationRangeMaxDistance.X ?? 0, alpha);
                }

                return new double[] { idealRange / 100, maxRange / 100, (Reciever?.IniStats?.MaxTraceDistance ?? 0) / 100 };
            }
            else
            {
                return new double[] { 0, 0, 0 };
            }
        }

        private static double[] CalculateSpread(ImportItem Reciever, double allAccuracy, double allMovementSpread)
        {
            if (Reciever != null && Reciever.WikiStats != null && Reciever.IniStats != null)
            {
                double accuracyBaseModifier;
                double accuracyTABaseModifier;
                double alpha = Math.Abs(allAccuracy);
                if (allAccuracy > 0)
                {
                    accuracyBaseModifier = Lerp(Reciever?.IniStats?.ModificationRangeBaseSpread.Z ?? 0, Reciever?.IniStats?.ModificationRangeBaseSpread.Y ?? 0, alpha);
                    accuracyTABaseModifier = Lerp(Reciever?.IniStats?.ModificationRangeTABaseSpread.Z ?? 0, Reciever?.IniStats?.ModificationRangeTABaseSpread.Y ?? 0, alpha);
                }
                else
                {
                    accuracyBaseModifier = Lerp(Reciever?.IniStats?.ModificationRangeBaseSpread.Z ?? 0, Reciever?.IniStats?.ModificationRangeBaseSpread.X ?? 0, alpha);
                    accuracyTABaseModifier = Lerp(Reciever?.IniStats?.ModificationRangeTABaseSpread.Z ?? 0, Reciever?.IniStats?.ModificationRangeTABaseSpread.X ?? 0, alpha);
                }

                double hip = accuracyBaseModifier * (180 / Math.PI);
                double aim = (accuracyBaseModifier * Reciever.IniStats.ZoomSpreadMultiplier) * (180 / Math.PI);
                if (Reciever.IniStats.UseTABaseSpread)
                {
                    aim = accuracyTABaseModifier * (float)(180 / Math.PI);
                }

                double weight_alpha = Math.Abs(Reciever.IniStats.Weight / 80.0);
                double weight_clampalpha = Math.Min(Math.Max(weight_alpha, -1.0), 1.0); // Don't ask me why they clamp the absolute value with a negative, I have no idea.
                double weight_multiplier;
                if (Reciever.IniStats.Weight > 0)   // It was originally supposed to compare the total weight of equipped mods, but from what I can currently gather from the scripts, nothing modifies weapon weight so I'm just comparing base weight for now.
                {
                    weight_multiplier = Lerp(1.0, 0.5, weight_clampalpha);  // Originally supposed to be a weapon specific range, but they all set the same values so it's not worth setting elsewhere.
                }
                else
                {
                    weight_multiplier = Lerp(1.0, 2.0, weight_clampalpha);
                }

                double move_alpha = Math.Abs(allMovementSpread);
                double move_multiplier; // Applying movement to it like this isn't how it's done to my current knowledge, but seems to be consistently closer to how it should be in most cases so far.
                if (allMovementSpread > 0)
                {
                    move_multiplier = Lerp(1.0, 0.5, move_alpha);
                }
                else
                {
                    move_multiplier = Lerp(1.0, 2.0, move_alpha);
                }

                double movemultiplier_current = 1.0 + ((Reciever.IniStats.MovementSpreadMultiplier - 1.0) * (weight_multiplier * move_multiplier));
                double moveconstant_current = Reciever.IniStats.MovementSpreadConstant * (weight_multiplier * move_multiplier);

                double move = ((accuracyBaseModifier + moveconstant_current) * (180 / Math.PI)) * movemultiplier_current;
                //double move = (accuracyBaseModifier * Reciever.IniStats.MovementSpreadMultiplier) * (180 / Math.PI);  // Old.

                return new double[] { aim, hip, move };
            }
            else
            {
                return new double[] { 0, 0, 0 };
            }
        }

        private static double[] CalculateDamage(ImportItem Reciever, double allDamage)
        {
            if (Reciever != null && Reciever.WikiStats != null && Reciever.IniStats != null)
            {
                double damageModifier;
                double alpha = Math.Abs(allDamage);
                if (allDamage > 0)
                {
                    damageModifier = Lerp(Reciever?.IniStats?.ModificationRangeDamage.Z ?? 0, Reciever?.IniStats?.ModificationRangeDamage.Y ?? 0, alpha);
                }
                else
                {
                    damageModifier = Lerp(Reciever?.IniStats?.ModificationRangeDamage.Z ?? 0, Reciever?.IniStats?.ModificationRangeDamage.X ?? 0, alpha);
                }

                return new double[] { damageModifier, damageModifier * (Reciever?.IniStats?.MaxRangeDamageMultiplier ?? 0.1d) };
            }
            else
            {
                return new double[] { 0, 0 };
            }
        }

        private static double CalculateRecoil(ImportItem Reciever, double allRecoil)
        {
            if (Reciever != null && Reciever.WikiStats != null && Reciever.IniStats != null)
            {
                double recoilModifier;
                double alpha = Math.Abs(allRecoil);
                if (allRecoil > 0)
                {
                    recoilModifier = Lerp(Reciever?.IniStats?.ModificationRangeRecoil.Z ?? 0, Reciever?.IniStats?.ModificationRangeRecoil.Y ?? 0, alpha);
                }
                else
                {
                    recoilModifier = Lerp(Reciever?.IniStats?.ModificationRangeRecoil.Z ?? 0, Reciever?.IniStats?.ModificationRangeRecoil.X ?? 0, alpha);
                }
                if (Reciever?.WikiStats.ammoMag > 0)
                {
                    double averageShotCount = Math.Min(Reciever?.WikiStats.ammoMag ?? 0, 15.0f);
                    Vector3 averageRecoil = new Vector3(0, 0, 0);

                    for (int shot = 1; shot <= averageShotCount; shot++)
                    {
                        Vector3 newRecoil = new Vector3(0, 0, 0)
                        {
                            X = (Reciever.IniStats.RecoilVector.X * Reciever.IniStats.RecoilVectorMultiplier.X) / 4.0f,
                            Y = (Reciever.IniStats.RecoilVector.Y * Reciever.IniStats.RecoilVectorMultiplier.Y) / 2.0f
                        };

                        double previousMultiplier = Reciever.IniStats.RecoilSize * Math.Pow(shot / Reciever.IniStats.Burst, (Reciever.IniStats.RecoilAccumulation * Reciever.IniStats.RecoilAccumulationMultiplier));
                        double currentMultiplier = Reciever.IniStats.RecoilSize * Math.Pow(shot / Reciever.IniStats.Burst + 1.0f, (Reciever.IniStats.RecoilAccumulation * Reciever.IniStats.RecoilAccumulationMultiplier));
                        double multiplier = currentMultiplier - previousMultiplier;
                        newRecoil *= (float)multiplier;
                        averageRecoil += newRecoil;
                    }

                    if (averageShotCount > 0)
                    {
                        averageRecoil /= (float)averageShotCount;
                    }
                    if (Reciever.IniStats.ROF > 0 && Reciever.IniStats.ApplyTime > 60 / Reciever.IniStats.ROF)
                    {
                        averageRecoil *= (float)(60 / (Reciever.IniStats.ROF * Reciever.IniStats.ApplyTime));
                    }
                    double recoil = averageRecoil.Length() * recoilModifier;
                    recoil *= (180 / Math.PI);
                    return recoil;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void CheckPrimaryModsForValidity(ImportItem primary)
        {
            CheckValidity(PrimaryBarrelImage, primary);

            CheckValidity(PrimaryMagazineImage, primary);

            CheckValidity(PrimaryMuzzleImage, primary);

            CheckValidity(PrimaryScopeImage, primary);

            CheckValidity(PrimaryStockImage, primary);
        }

        private void CheckSecondaryModsForValidity(ImportItem secondary)
        {
            CheckValidity(SecondaryBarrelImage, secondary);

            CheckValidity(SecondaryMagazineImage, secondary);

            CheckValidity(SecondaryMuzzleImage, secondary);

            CheckValidity(SecondaryScopeImage, secondary);

            CheckValidity(SecondaryStockImage, secondary);
        }

        private static void CheckValidity(Image image, ImportItem item)
        {
            if(image.DataContext is ImportItem importItem)
            {
                if (!importItem.IsValidFor(item) || !item.IsValidModType(importItem.Category))
                { LoggingSystem.LogInfo(importItem.name + " was not a Valid Mod for " + item.name); image.DataContext = null; }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ItemList.ItemsSource = ImportSystem.Weapons.primary;
            LastSelectedImage = PrimaryRecieverImage;
            if (App.IsNewVersionAvailable && IOResources.Settings.ShowUpdateNotice)
            {
                System.Diagnostics.Process.Start("https://github.com/" + App.CurrentOwner + "/" + App.CurrentRepo + "/releases");
            }
            if (IOResources.Settings.DoRuntimeCheck || IOResources.Settings.ForceRuntimeCheck)
            {
                App.RuntimeCheck(IOResources.Settings.ForceRuntimeCheck);
            }
        }

        private void ItemList_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LoggingSystem.LogInfo("MouseDown in ListView/StackPanel");
            var pt = e.GetPosition(ItemList);
            var result = VisualTreeHelper.HitTest(ItemList, pt);
            if (result.VisualHit is Image image)
            {
                if (image.DataContext is ImportItem item)
                {
                    if (e.ClickCount >= 2)
                    {
                        SetItemToImage(LastSelectedImage, item);
                    }
                    else
                    {
                        LoggingSystem.LogInfo("Sending:" + item.name);
                        DragDrop.DoDragDrop(image, item, DragDropEffects.Copy);
                    }
                }
            }
            else if (result.VisualHit is StackPanel panel)
            {
                LoggingSystem.LogInfo(panel.DataContext.ToString());
                foreach (var child in panel.Children)
                {
                    if (child is Image imageChild)
                    {
                        if (imageChild.DataContext is ImportItem item)
                        {
                            if (e.ClickCount >= 2)
                            {
                                SetItemToImage(LastSelectedImage, item);
                            }
                            else
                            {
                                LoggingSystem.LogInfo("Sending:" + item.name);
                                DragDrop.DoDragDrop(imageChild, item, DragDropEffects.Copy);
                            }
                        }
                    }
                }
            }
        }

        private void Image_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Serializable))
            { e.Effects = DragDropEffects.Copy; }
            else
            { e.Effects = DragDropEffects.None; }
        }

        private void Image_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                if (e.Data.GetData(typeof(ImportItem)) is ImportItem item)
                {
                    LoggingSystem.LogInfo("Recieving:" + item.name);
                    if (border.Child is Image image)
                    {
                        SetItemToImage(image, item);
                    }
                }
            }
            
        }

        public void SetItemToImage(Image image, ImportItem item)
        {
            if (image.Name.Contains("Primary"))
            {
                if (image.Name.Contains("Scope") || image.Name.Contains("Crosshair"))
                {
                    if (ImportSystem.Mods.scopes.Contains(item))
                    {
                        PrimaryScopeImage.DataContext = item; LoggingSystem.LogInfo("Scope Set!");
                        PrimaryCrosshairImage.DataContext = item; LoggingSystem.LogInfo("Crosshair Set!");
                    }
                }
            }
            else
            {
                if (image.Name.Contains("Scope") || image.Name.Contains("Crosshair"))
                {
                    if (ImportSystem.Mods.scopes.Contains(item))
                    {
                        SecondaryScopeImage.DataContext = item; LoggingSystem.LogInfo("Scope Set!");
                        SecondaryCrosshairImage.DataContext = item; LoggingSystem.LogInfo("Crosshair Set!");
                    }
                }
            }
            if (image.Name.Contains("Reciever"))
            {
                if (image.Name.Contains("Primary") && ImportSystem.Weapons.primary.Contains(item))
                {
                    image.DataContext = item;
                    LoggingSystem.LogInfo("Primary Set!");
                    CheckPrimaryModsForValidity(item);
                    FillEmptyPrimaryMods(item);
                    UpdatePrimaryStats();
                    UpdateActiveLoadout();
                    return;
                }
                if (image.Name.Contains("Secondary") && ImportSystem.Weapons.secondary.Contains(item))
                {
                    image.DataContext = item;
                    LoggingSystem.LogInfo("Secondary Set!");
                    CheckSecondaryModsForValidity(item);
                    FillEmptySecondaryMods(item);
                    UpdateSecondaryStats();
                    UpdateActiveLoadout();
                    return;
                }
                LoggingSystem.LogInfo("Not a Valid Primary or Secondary!");
            }
            else
            {
                if (image.Name.Contains("Primary") && !item.IsValidFor(PrimaryRecieverImage.DataContext as ImportItem))
                { LoggingSystem.LogInfo("Not a Valid Mod for " + (PrimaryRecieverImage.DataContext as ImportItem).name); return; }

                if (image.Name.Contains("Secondary") && !item.IsValidFor(SecondaryRecieverImage.DataContext as ImportItem))
                { LoggingSystem.LogInfo("Not a Valid Mod for " + (SecondaryRecieverImage.DataContext as ImportItem).name); return; }

                if (image.Name.Contains("Muzzle") && ImportSystem.Mods.muzzles.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Muzzle with ID:" + ImportSystem.GetMuzzleID(item) + " Set!"); }
                if (image.Name.Contains("Barrel") && ImportSystem.Mods.barrels.Contains(item))
                {
                    if (image.Name.Contains("Secondary"))
                    {
                        if (SecondaryRecieverImage.DataContext is ImportItem reciever)
                        {
                            if (CheckForPistolAndBarrel(reciever))
                            {
                                    if (item.name == Weapon.NoBarrel)
                                    {
                                        SecondaryStockImage.DataContext = Weapon.GetDefaultSetupOfReciever(reciever).GetStock();
                                    }
                            }
                        }
                    }
                    image.DataContext = item; LoggingSystem.LogInfo("Barrel Set!");
                }
                if (image.Name.Contains("Magazine") && ImportSystem.Mods.magazines.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Magazine with ID:" + ImportSystem.GetMagazineID(item) + " Set!"); }
                if (image.Name.Contains("Tag") && ImportSystem.Gear.hangers.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Hanger with ID:" + ImportSystem.GetTagID(item) + " Set!"); }
                if (image.Name.Contains("CamoWeapon") && ImportSystem.Mods.camosWeapon.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Camo with ID:" + ImportSystem.GetCamoWeaponID(item) + " Set!"); }
                if (image.Name.Contains("CamoBody") && ImportSystem.Mods.camosBody.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Camo with ID:" + ImportSystem.GetCamoBodyID(item) + " Set!"); }
                if (image.Name.Contains("Stock") && ImportSystem.Mods.stocks.Contains(item))
                {
                    if (image.Name.Contains("Primary"))
                    {
                        SetStock((PrimaryRecieverImage.DataContext as ImportItem), PrimaryBarrelImage, PrimaryStockImage, item);
                        LoggingSystem.LogInfo("Stock Set!");
                    }
                    else
                    {
                        SetStock((SecondaryRecieverImage.DataContext as ImportItem), SecondaryBarrelImage, SecondaryStockImage, item);
                        LoggingSystem.LogInfo("Stock Set!");
                    }
                }

                if (image.Name.Contains("Helmet") && ImportSystem.Gear.helmets.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Helmet Set!"); }
                if (image.Name.Contains("UpperBody") && ImportSystem.Gear.upperBodies.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("UpperBody Set!"); }
                if (image.Name.Contains("LowerBody") && ImportSystem.Gear.lowerBodies.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("LowerBody Set!"); }

                if (image.Name.Contains("Gear") && ImportSystem.Gear.attachments.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Gear Set!"); }
                if (image.Name.Contains("Tactical") && ImportSystem.Gear.tactical.Contains(item))
                { image.DataContext = item; LoggingSystem.LogInfo("Tactical Set!"); }

            }
            UpdatePrimaryStats();
            UpdateSecondaryStats();
            UpdateActiveLoadout();
        }

        private static bool CheckForPistolAndBarrel(ImportItem item)
        {
            return item.name == "Light Pistol" || item.name == "Heavy Pistol" || item.name == "Prestige Light Pistol";
        }

        private static void SetStock(ImportItem reciever, Image barrel, Image stock, ImportItem item)
        {
            if (reciever != null)
            {
                if (CheckForPistolAndBarrel(reciever))
                {
                    if (barrel.DataContext is ImportItem Barrel)
                    {
                        if (!string.IsNullOrEmpty(Barrel.name) && Barrel.name != "No Barrel Mod")
                        {
                            stock.DataContext = item;
                        }
                        else
                        {
                            stock.DataContext = Weapon.GetDefaultSetupOfReciever(reciever).GetStock();
                        }
                    }
                }
                else
                {
                    stock.DataContext = item;
                }
            }
        }

        private void FillEmptyPrimaryMods(ImportItem reciever)
        {
            FillEmptyMods(reciever, PrimaryMuzzleImage, PrimaryBarrelImage, PrimaryMagazineImage, PrimaryScopeImage, PrimaryCrosshairImage, PrimaryStockImage);
        }

        private void FillEmptySecondaryMods(ImportItem reciever)
        {
            FillEmptyMods(reciever, SecondaryMuzzleImage, SecondaryBarrelImage, SecondaryMagazineImage, SecondaryScopeImage, SecondaryCrosshairImage, SecondaryStockImage);
        }

        private static void FillEmptyMods(ImportItem reciever, Image muzzle, Image barrel, Image magazine, Image scope, Image crosshair, Image stock)
        {
            Weapon weapon = Weapon.GetDefaultSetupOfReciever(reciever);
            if (muzzle.DataContext == null || (muzzle.DataContext as ImportItem).name == Weapon.NoMuzzle)
            {
                muzzle.DataContext = weapon.GetMuzzle();
            }
            if (barrel.DataContext == null || (barrel.DataContext as ImportItem).name == Weapon.NoBarrel)
            {
                barrel.DataContext = weapon.GetBarrel();
                if (CheckForPistolAndBarrel(reciever))
                {
                    stock.DataContext = null;
                }
            }
            if (stock.DataContext == null || (barrel.DataContext as ImportItem).name == Weapon.NoStock)
            {
                SetStock(reciever, barrel, stock, weapon.GetStock());
            }
            if (magazine.DataContext == null)
            {
                magazine.DataContext = weapon.GetMagazine();
            }
            if (scope.DataContext == null)
            {
                scope.DataContext = weapon.GetScope();
                crosshair.DataContext = weapon.GetScope();
            }
        }

        private void UpdateActiveLoadout()
        {
            UpdateLoadoutWeapon(ActiveLoadout.Primary, PrimaryRecieverImage.DataContext as ImportItem, PrimaryMuzzleImage.DataContext as ImportItem, PrimaryBarrelImage.DataContext as ImportItem, PrimaryMagazineImage.DataContext as ImportItem, PrimaryScopeImage.DataContext as ImportItem, PrimaryStockImage.DataContext as ImportItem, PrimaryTagImage.DataContext as ImportItem, PrimaryCamoWeaponImage.DataContext as ImportItem);
            UpdateLoadoutWeapon(ActiveLoadout.Secondary, SecondaryRecieverImage.DataContext as ImportItem, SecondaryMuzzleImage.DataContext as ImportItem, SecondaryBarrelImage.DataContext as ImportItem, SecondaryMagazineImage.DataContext as ImportItem, SecondaryScopeImage.DataContext as ImportItem, SecondaryStockImage.DataContext as ImportItem, SecondaryTagImage.DataContext as ImportItem, SecondaryCamoWeaponImage.DataContext as ImportItem);
            ActiveLoadout.Gear1 = ImportSystem.GetGearID(GearImage1.DataContext as ImportItem);
            ActiveLoadout.Gear2 = ImportSystem.GetGearID(GearImage2.DataContext as ImportItem);
            ActiveLoadout.Gear3 = ImportSystem.GetGearID(GearImage3.DataContext as ImportItem);
            ActiveLoadout.Gear4 = ImportSystem.GetGearID(GearImage4.DataContext as ImportItem);
            ActiveLoadout.Tactical = ImportSystem.GetTacticalID(TacticalImage.DataContext as ImportItem);
            ActiveLoadout.Helmet = ImportSystem.GetHelmetID(HelmetImage.DataContext as ImportItem);
            ActiveLoadout.UpperBody = ImportSystem.GetUpperBodyID(UpperBodyImage.DataContext as ImportItem);
            ActiveLoadout.LowerBody = ImportSystem.GetLowerBodyID(LowerBodyImage.DataContext as ImportItem);
            ActiveLoadout.Camo = ImportSystem.GetCamoBodyID((PlayerCamoBodyImage.DataContext as ImportItem));
        }

        private static void UpdateLoadoutWeapon(Weapon weapon, ImportItem reciever, ImportItem muzzle, ImportItem barrel, ImportItem magazine, ImportItem scope, ImportItem stock, ImportItem tag, ImportItem camo)
        {
            weapon.Receiver = reciever.name;
            weapon.Muzzle = ImportSystem.GetMuzzleID(muzzle);
            weapon.Barrel = barrel?.name ?? "No Barrel Mod";
            weapon.Magazine = ImportSystem.GetMagazineID(magazine);
            weapon.Scope = scope?.name ?? "No Optic Mod";
            weapon.Stock = stock?.name ?? "No Stock";
            weapon.Tag = ImportSystem.GetTagID(tag);
            weapon.Camo = ImportSystem.GetCamoBodyID(camo);
        }

        private void UpdatePrimaryImages(Image image)
        {
            FilterWeapon = PrimaryRecieverImage.DataContext as ImportItem;
            LoggingSystem.LogInfo("ItemList Filter set Primary:" + (FilterWeapon?.name ?? "None"));


            if (image.Name.Contains("Reciever"))
            {
                ItemList.ItemsSource = ImportSystem.Weapons.primary;
                LastSelectedImage = PrimaryRecieverImage;
                LoggingSystem.LogInfo("ItemList Set for Primary Reciever");
                return;
            }

            if (image.Name.Contains("Muzzle"))
            {
                if (!(PrimaryRecieverImage.DataContext as ImportItem).IsValidModType("muzzle"))
                { return; }
                ItemList.ItemsSource = ImportSystem.Mods.muzzles;
                LastSelectedImage = image;
                LoggingSystem.LogInfo("ItemList Set for Muzzles");
                return;
            }
            if (image.Name.Contains("Crosshair"))
            {
                var item = (PrimaryScopeImage.DataContext as ImportItem);
                item.LoadCrosshair();
                ItemList.ItemsSource = new ImportItem[] { item };
                LastSelectedImage = PrimaryScopeImage;
                LoggingSystem.LogInfo("ItemList Set for Scopes");
                return;
            }
            UpdateImages(image);
        }

        private void UpdateSecondaryImages(Image image)
        {
            FilterWeapon = SecondaryRecieverImage.DataContext as ImportItem;
            LoggingSystem.LogInfo("ItemList Filter set Secondary:" + (FilterWeapon?.name ?? "None"));

            if (image.Name.Contains("Reciever"))
            {
                ItemList.ItemsSource = ImportSystem.Weapons.secondary;
                LastSelectedImage = SecondaryRecieverImage;
                LoggingSystem.LogInfo("ItemList Set for Secondary Reciever");
                return;
            }

            if (image.Name.Contains("Muzzle"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.muzzles;
                LastSelectedImage = image;
                LoggingSystem.LogInfo("ItemList Set for Muzzles");
                return;
            }
            if (image.Name.Contains("Crosshair"))
            {
                var item = (SecondaryScopeImage.DataContext as ImportItem);
                item.LoadCrosshair();
                ItemList.ItemsSource = new ImportItem[] { item };
                LastSelectedImage = SecondaryScopeImage;
                LoggingSystem.LogInfo("ItemList Set for Scopes");
                return;
            }
            UpdateImages(image);
        }

        private void UpdateImages(Image image)
        {
            LastSelectedImage = image;
            
            if (image.Name.Contains("Reciever"))
            {
                ItemList.ItemsSource = ImportSystem.Weapons.primary;
                LoggingSystem.LogInfo("ItemList Set for Primary Reciever");
                return;
            }
            if (image.Name.Contains("Magazine"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.magazines;
                LoggingSystem.LogInfo("ItemList Set for Magazines");
                return;
            }
            if (image.Name.Contains("Stock"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.stocks;
                LoggingSystem.LogInfo("ItemList Set for Stocks");
                return;
            }
            if (image.Name.Contains("Scope"))
            {
                foreach (var item in ImportSystem.Mods.scopes)
                {
                    item.RemoveCrosshair();
                }
                ItemList.ItemsSource = ImportSystem.Mods.scopes;
                LoggingSystem.LogInfo("ItemList Set for Scopes");
                return;
            }
            if (image.Name.Contains("Barrel"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.barrels;
                LoggingSystem.LogInfo("ItemList Set for Barrels");
                return;
            }
            if (image.Name.Contains("Grip"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.grips;
                LoggingSystem.LogInfo("ItemList Set for Grips");
                return;
            }
            if (image.Name.Contains("Tag"))
            {
                ItemList.ItemsSource = ImportSystem.Gear.hangers;
                LoggingSystem.LogInfo("ItemList Set for Tags");
                return;
            }
            if (image.Name.Contains("CamoWeapon"))
            {
                ItemList.ItemsSource = ImportSystem.Mods.camosWeapon;
                LoggingSystem.LogInfo("ItemList Set for Weapon Camos");
                return;
            }
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                if (border.Child is Image image)
                {
                    ItemList.ItemsSource = null;
                    if (image.Name.Contains("Primary"))
                    {
                        UpdatePrimaryImages(image);
                        return;
                    }
                    if (image.Name.Contains("Secondary"))
                    {
                        UpdateSecondaryImages(image);
                        return;
                    }


                    if (image.Name.Contains("Gear"))
                    {
                        ItemList.ItemsSource = ImportSystem.Gear.attachments;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for Gear");
                        return;
                    }
                    if (image.Name.Contains("Tactical"))
                    {
                        ItemList.ItemsSource = ImportSystem.Gear.tactical;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for Tactical");
                        return;
                    }
                    if (image.Name.Contains("CamoBody"))
                    {
                        ItemList.ItemsSource = ImportSystem.Mods.camosBody;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for Body Camos");
                        return;
                    }
                    if (image.Name.Contains("Helmet"))
                    {
                        ItemList.ItemsSource = ImportSystem.Gear.helmets;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for Helemts");
                        return;
                    }
                    if (image.Name.Contains("UpperBody"))
                    {
                        ItemList.ItemsSource = ImportSystem.Gear.upperBodies;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for UpperBodies");
                        return;
                    }
                    if (image.Name.Contains("LowerBody"))
                    {
                        ItemList.ItemsSource = ImportSystem.Gear.lowerBodies;
                        LastSelectedImage = image;
                        LoggingSystem.LogInfo("ItemList Set for LowerBodies");
                        return;
                    }
                    LoggingSystem.LogInfo("ItemList Din't get set");
                }
            }
        }

        public void SetLoadout(Loadout loadout)
        {
            ActiveLoadout = loadout;
            GearImage1.DataContext = Loadout.GetGear(loadout.Gear1);
            GearImage2.DataContext = Loadout.GetGear(loadout.Gear2);
            GearImage3.DataContext = Loadout.GetGear(loadout.Gear3);
            GearImage4.DataContext = Loadout.GetGear(loadout.Gear4);
            TacticalImage.DataContext = loadout.GetTactical();

            HelmetImage.DataContext = loadout.GetHelmet();
            UpperBodyImage.DataContext = loadout.GetUpperBody();
            LowerBodyImage.DataContext = loadout.GetLowerBody();
            PlayerCamoBodyImage.DataContext = loadout.GetCamo();

            IsFemaleCheckBox.DataContext = loadout;

            SetPrimary(loadout.Primary);
            SetSecondary(loadout.Secondary);

        }

        public void SetPrimary(Weapon primary)
        {
            PrimaryRecieverImage.DataContext = primary.GetReciever();
            PrimaryMuzzleImage.DataContext = primary.GetMuzzle();
            PrimaryStockImage.DataContext = primary.GetStock();
            PrimaryBarrelImage.DataContext = primary.GetBarrel();
            PrimaryMagazineImage.DataContext = primary.GetMagazine();
            PrimaryScopeImage.DataContext = primary.GetScope();
            PrimaryCrosshairImage.DataContext = primary.GetScope();
            PrimaryTagImage.DataContext = primary.GetTag();
            PrimaryCamoWeaponImage.DataContext = primary.GetCamo();
            UpdatePrimaryStats();
        }

        public void SetSecondary(Weapon secondary)
        {
            SecondaryRecieverImage.DataContext = secondary.GetReciever();
            SecondaryMuzzleImage.DataContext = secondary.GetMuzzle();
            SecondaryStockImage.DataContext = secondary.GetStock();
            SecondaryBarrelImage.DataContext = secondary.GetBarrel();
            SecondaryMagazineImage.DataContext = secondary.GetMagazine();
            SecondaryScopeImage.DataContext = secondary.GetScope();
            SecondaryCrosshairImage.DataContext = secondary.GetScope();
            SecondaryTagImage.DataContext = secondary.GetTag();
            SecondaryCamoWeaponImage.DataContext = secondary.GetCamo();
            UpdateSecondaryStats();
        }
        bool textchnaging = false;
        private void ProfileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!textchnaging)
            {
                profilechanging = true;
                ExportSystem.ActiveProfile = ProfileComboBox.SelectedValue as Profile;
                PlayerNameTextBox.Text = ExportSystem.ActiveProfile.PlayerName;

                IsFemaleCheckBox.DataContext = ExportSystem.ActiveProfile;

                SetLoadout(ExportSystem.ActiveProfile.Loadout1);
                profilechanging = false;
            }
        }

        private void Loadout1Button_Click(object sender, RoutedEventArgs e)
        {
            SetLoadout(ExportSystem.ActiveProfile.Loadout1);
            Loadout1Button.IsEnabled = false;
            Loadout2Button.IsEnabled = true;
            Loadout3Button.IsEnabled = true;
        }

        private void Loadout2Button_Click(object sender, RoutedEventArgs e)
        {
            SetLoadout(ExportSystem.ActiveProfile.Loadout2);
            Loadout1Button.IsEnabled = true;
            Loadout2Button.IsEnabled = false;
            Loadout3Button.IsEnabled = true;
        }

        private void Loadout3Button_Click(object sender, RoutedEventArgs e)
        {
            SetLoadout(ExportSystem.ActiveProfile.Loadout3);
            Loadout1Button.IsEnabled = true;
            Loadout2Button.IsEnabled = true;
            Loadout3Button.IsEnabled = false;
        }

        private void CopyToClipboardButton_Click(object sender, RoutedEventArgs e)
        {
            ExportSystem.CopyToClipBoard(ExportSystem.ActiveProfile);
        }

        private void AddProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ExportSystem.AddProfile();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ExportSystem.SaveProfiles();
        }
        bool profilechanging = false;
        private void PlayerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!profilechanging)
            {
                textchnaging = true;
                int index = ProfileComboBox.SelectedIndex;
                ProfileComboBox.ItemsSource = null;
                ProfileComboBox.Items.Clear();
                ExportSystem.RemoveActiveProfileFromDisk();
                ExportSystem.ActiveProfile.PlayerName = PlayerNameTextBox.Text;
                ProfileComboBox.ItemsSource = ExportSystem.Profiles;
                ProfileComboBox.SelectedIndex = index;
                textchnaging = false;
            }
        }

        private void IsFemaleCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var upper = UpperBodyImage.DataContext as ImportItem;
            var lower = LowerBodyImage.DataContext as ImportItem;
            UpperBodyImage.DataContext = null;
            LowerBodyImage.DataContext = null;
            UpperBodyImage.DataContext = upper;
            LowerBodyImage.DataContext = lower;
            var source = ItemList.ItemsSource;
            ItemList.ItemsSource = null;
            ItemList.ItemsSource = source;
        }

        private void IsFemaleCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var upper = UpperBodyImage.DataContext as ImportItem;
            var lower = LowerBodyImage.DataContext as ImportItem;
            UpperBodyImage.DataContext = null;
            LowerBodyImage.DataContext = null;
            UpperBodyImage.DataContext = upper;
            LowerBodyImage.DataContext = lower;
            var source = ItemList.ItemsSource;
            ItemList.ItemsSource = null;
            ItemList.ItemsSource = source;
        }
    }
}
