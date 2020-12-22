using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using GTEventGenerator.PDUtils;
using GTEventGenerator.PDUtils.Enums;

namespace GTEventGenerator.CarParameter
{
    /// <summary>
    /// Interaction logic for PurchasedPartsWindow.xaml
    /// </summary>
    public partial class PurchasedPartsWindow : Window
    {
        public MCarParameter Parameter { get; set; }
        public PurchasedPartsWindow(MCarParameter parameter)
        {
            Parameter = parameter;
            InitializeComponent();
            Populate();
        }

        public void cb_Brake1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE, (int)PARTS_BRAKE.NORMAL, cb_Brake1.IsChecked == true);
        public void cb_Brake2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE, (int)PARTS_BRAKE._4PISTON, cb_Brake2.IsChecked == true);
        public void cb_Brake3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE, (int)PARTS_BRAKE._6PISTON, cb_Brake3.IsChecked == true);
        public void cb_Brake4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE, (int)PARTS_BRAKE._8PISTON, cb_Brake4.IsChecked == true);
        public void cb_Brake5_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE, (int)PARTS_BRAKE.CARBON, cb_Brake5.IsChecked == true);

        public void cb_Susp1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.NORMAL, cb_Susp1.IsChecked == true);
        public void cb_Susp2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS1, cb_Susp2.IsChecked == true);
        public void cb_Susp3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS2, cb_Susp3.IsChecked == true);
        public void cb_Susp4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS3, cb_Susp4.IsChecked == true);
        public void cb_Susp5_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.RACING, cb_Susp5.IsChecked == true);
        public void cb_Susp6_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.FULL_ACTIVE, cb_Susp6.IsChecked == true);

        public void cb_Natune1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL1, cb_Natune1.IsChecked == true);
        public void cb_Natune2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL2, cb_Natune2.IsChecked == true);
        public void cb_Natune3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL3, cb_Natune3.IsChecked == true);
        public void cb_Natune4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL4, cb_Natune4.IsChecked == true);
        public void cb_Natune5_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL5, cb_Natune5.IsChecked == true);

        public void cb_Turbo1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.NO, cb_Turbo1.IsChecked == true);
        public void cb_Turbo2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL1, cb_Turbo2.IsChecked == true);
        public void cb_Turbo3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL2, cb_Turbo3.IsChecked == true);
        public void cb_Turbo4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL3, cb_Turbo4.IsChecked == true);
        public void cb_Turbo5_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL4, cb_Turbo5.IsChecked == true);
        public void cb_Turbo6_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL5, cb_Turbo6.IsChecked == true);

        public void cb_Clutch1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.NORMAL, cb_Clutch1.IsChecked == true);
        public void cb_Clutch2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.HIGH_CAPACITY, cb_Clutch2.IsChecked == true);
        public void cb_Clutch3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.TWIN, cb_Clutch3.IsChecked == true);
        public void cb_Clutch4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.TRIPLE, cb_Clutch4.IsChecked == true);

        public void cb_Flywheel1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.FLYWHEEL, (int)PARTS_FLYWHEEL.LIGHT, cb_Flywheel1.IsChecked == true);
        public void cb_Flywheel2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.FLYWHEEL, (int)PARTS_FLYWHEEL.Cr_Mo, cb_Flywheel2.IsChecked == true);
        public void cb_Flywheel3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.FLYWHEEL, (int)PARTS_FLYWHEEL.LIGHT_Cr_Mo, cb_Flywheel3.IsChecked == true);

        public void cb_LSD1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LSD, (int)PARTS_LSD.NORMAL, cb_LSD1.IsChecked == true);
        public void cb_LSD2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LSD, (int)PARTS_LSD.VARIABLE, cb_LSD2.IsChecked == true);
        public void cb_LSD3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LSD, (int)PARTS_LSD.AYCC, cb_LSD3.IsChecked == true);

        public void cb_Disp1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL1, cb_Disp1.IsChecked == true);
        public void cb_Disp2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL2, cb_Disp2.IsChecked == true);
        public void cb_Disp3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL3, cb_Disp3.IsChecked == true);

        public void cb_Drivetrain1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.NORMAL, cb_Drivetrain1.IsChecked == true);
        public void cb_Drivetrain2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.VARIABLE_CENTER_DIFF, cb_Drivetrain2.IsChecked == true);
        public void cb_Drivetrain3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.ACTIVE_CENTER_DIFF, cb_Drivetrain3.IsChecked == true);

        public void cb_Muffler1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.SPORTS, cb_Muffler1.IsChecked == true);
        public void cb_Muffler2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.SEMI_RACING, cb_Muffler2.IsChecked == true);
        public void cb_Muffler3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.RACING, cb_Muffler3.IsChecked == true);

        public void cb_Intercooler1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.S, cb_Intercooler1.IsChecked == true);
        public void cb_Intercooler2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.M, cb_Intercooler2.IsChecked == true);
        public void cb_Intercooler3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.L, cb_Intercooler3.IsChecked == true);
        public void cb_Intercooler4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.LL, cb_Intercooler4.IsChecked == true);

        public void cb_Lightweight1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE1, cb_Lightweight1.IsChecked == true);
        public void cb_Lightweight2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE2, cb_Lightweight2.IsChecked == true);
        public void cb_Lightweight3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE3, cb_Lightweight3.IsChecked == true);
        public void cb_Lightweight4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE4, cb_Lightweight4.IsChecked == true);
        public void cb_Lightweight5_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE5, cb_Lightweight5.IsChecked == true);
        public void cb_Lightweight6_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE6, cb_Lightweight6.IsChecked == true);
        public void cb_Lightweight7_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE7, cb_Lightweight7.IsChecked == true);
        public void cb_Lightweight8_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE8, cb_Lightweight8.IsChecked == true);

        public void cb_Computer1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.COMPUTER, (int)PARTS_COMPUTER.LEVEL1, cb_Computer1.IsChecked == true);
        public void cb_Computer2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.COMPUTER, (int)PARTS_COMPUTER.LEVEL2, cb_Computer2.IsChecked == true);

        public void cb_Gear1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.GEAR, (int)PARTS_GEAR.NORMAL, cb_Gear1.IsChecked == true);
        public void cb_Gear2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.GEAR, (int)PARTS_GEAR.CLOSE, cb_Gear2.IsChecked == true);
        public void cb_Gear3_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.GEAR, (int)PARTS_GEAR.SUPER_CLOSE, cb_Gear3.IsChecked == true);
        public void cb_Gear4_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.GEAR, (int)PARTS_GEAR.VARIABLE, cb_Gear4.IsChecked == true);

        public void cb_Catalyst1_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CATALYST, (int)PARTS_CATALYST.SPORTS, cb_Computer1.IsChecked == true);
        public void cb_Catalyst2_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.CATALYST, (int)PARTS_CATALYST.RACING, cb_Computer2.IsChecked == true);

        public void cb_AirCleaner1_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.AIR_CLEANER, (int)PARTS_AIR_CLEANER.SPORTS, cb_AirCleaner1.IsChecked == true);
        public void cb_AirCleaner2_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.AIR_CLEANER, (int)PARTS_AIR_CLEANER.RACING, cb_AirCleaner2.IsChecked == true);

        public void cb_Bonnet1_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.BONNET, (int)PARTS_BONNET.CARBON, cb_Bonnet1.IsChecked == true);
        public void cb_Bonnet2_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.BONNET, (int)PARTS_BONNET.PAINT_CARBON, cb_Bonnet2.IsChecked == true);

        public void cb_Tire1_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_HARD, cb_Tire1.IsChecked == true);
        public void cb_Tire2_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_MEDIUM, cb_Tire2.IsChecked == true);
        public void cb_Tire3_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_SOFT, cb_Tire3.IsChecked == true);
        public void cb_Tire4_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_HARD, cb_Tire4.IsChecked == true);
        public void cb_Tire5_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_MEDIUM, cb_Tire5.IsChecked == true);
        public void cb_Tire6_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_SOFT, cb_Tire6.IsChecked == true);
        public void cb_Tire7_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_SUPER_SOFT, cb_Tire7.IsChecked == true);
        public void cb_Tire8_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_HARD, cb_Tire8.IsChecked == true);
        public void cb_Tire9_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_MEDIUM, cb_Tire9.IsChecked == true);
        public void cb_Tire10_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_SOFT, cb_Tire10.IsChecked == true);
        public void cb_Tire11_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_SUPER_SOFT, cb_Tire11.IsChecked == true);
        public void cb_Tire12_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RAIN_INTERMEDIATE, cb_Tire12.IsChecked == true);
        public void cb_Tire13_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RAIN_HEAVY_WET, cb_Tire13.IsChecked == true);
        public void cb_Tire14_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.DIRT, cb_Tire14.IsChecked == true);
        public void cb_Tire15_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SNOW, cb_Tire15.IsChecked == true);

        public void cb_NOS_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.NOS, (int)PARTS_NOS.ONE, cb_NOS.IsChecked == true);

        public void cb_Supercharger_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.SUPERCHARGER, (int)PARTS_SUPERCHARGER.ONE, cb_Supercharger.IsChecked == true);

        public void cb_IntakeManifold_Click(object sender, RoutedEventArgs e)
           => Parameter.TogglePurchasedPart(CarPartsType.INTAKE_MANIFOLD, (int)PARTS_INTAKE_MANIFOLD.ONE, cb_IntakeManifold.IsChecked == true);

        public void cb_ExhaustManifold_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.EXHAUST_MANIFOLD, (int)PARTS_EXHAUST_MANIFOLD.ONE, cb_ExhaustManifold.IsChecked == true);

        public void cb_LightweightWindow_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.LIGHT_WEIGHT_WINDOW, (int)PARTS_LIGHT_WEIGHT_WINDOW.ONE, cb_LightweightWindow.IsChecked == true);

        public void cb_ASCC_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.ASCC, (int)PARTS_ASCC.ONE, cb_ASCC.IsChecked == true);

        public void cb_TCSC_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.TCSC, (int)PARTS_TCSC.ONE, cb_TCSC.IsChecked == true);

        public void cb_BrakeController_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.BRAKE_CONTROLLER, (int)PARTS_BRAKE_CONTROLLER.ONE, cb_BrakeController.IsChecked == true);

        public void cb_PropellerShaft_Click(object sender, RoutedEventArgs e)
            => Parameter.TogglePurchasedPart(CarPartsType.PROPELLERSHAFT, (int)PARTS_PROPELLERSHAFT.ONE, cb_PropellerShaft.IsChecked == true);

        public void Populate()
        {
            cb_Brake1.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE, (int)PARTS_BRAKE.NORMAL);
            cb_Brake2.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE, (int)PARTS_BRAKE._4PISTON);
            cb_Brake3.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE, (int)PARTS_BRAKE._6PISTON);
            cb_Brake4.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE, (int)PARTS_BRAKE._8PISTON);
            cb_Brake5.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE, (int)PARTS_BRAKE.CARBON);

            cb_Susp1.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.NORMAL);
            cb_Susp2.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS1);
            cb_Susp3.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS2);
            cb_Susp4.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.SPORTS3);
            cb_Susp5.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.RACING);
            cb_Susp6.IsChecked = Parameter.IsHavingParts(CarPartsType.SUSPENSION, (int)PARTS_SUSPENSION.FULL_ACTIVE);

            cb_Natune1.IsChecked = Parameter.IsHavingParts(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL1);
            cb_Natune2.IsChecked = Parameter.IsHavingParts(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL2);
            cb_Natune3.IsChecked = Parameter.IsHavingParts(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL3);
            cb_Natune4.IsChecked = Parameter.IsHavingParts(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL4);
            cb_Natune5.IsChecked = Parameter.IsHavingParts(CarPartsType.NATUNE, (int)PARTS_NATUNE.LEVEL5);

            cb_Turbo1.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.NO);
            cb_Turbo2.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL1);
            cb_Turbo3.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL2);
            cb_Turbo4.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL3);
            cb_Turbo5.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL4);
            cb_Turbo6.IsChecked = Parameter.IsHavingParts(CarPartsType.TURBINEKIT, (int)PARTS_TURBINEKIT.LEVEL5);

            cb_Clutch1.IsChecked = Parameter.IsHavingParts(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.NORMAL);
            cb_Clutch2.IsChecked = Parameter.IsHavingParts(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.HIGH_CAPACITY);
            cb_Clutch3.IsChecked = Parameter.IsHavingParts(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.TWIN);
            cb_Clutch4.IsChecked = Parameter.IsHavingParts(CarPartsType.CLUTCH, (int)PARTS_CLUTCH.TRIPLE);

            cb_Disp1.IsChecked = Parameter.IsHavingParts(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL1);
            cb_Disp2.IsChecked = Parameter.IsHavingParts(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL2);
            cb_Disp3.IsChecked = Parameter.IsHavingParts(CarPartsType.DISPLACEMENT, (int)PARTS_DISPLACEMENT.LEVEL3);

            cb_Drivetrain1.IsChecked = Parameter.IsHavingParts(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.NORMAL);
            cb_Drivetrain2.IsChecked = Parameter.IsHavingParts(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.VARIABLE_CENTER_DIFF);
            cb_Drivetrain3.IsChecked = Parameter.IsHavingParts(CarPartsType.DRIVETRAIN, (int)PARTS_DRIVETRAIN.ACTIVE_CENTER_DIFF);

            cb_Muffler1.IsChecked = Parameter.IsHavingParts(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.SPORTS);
            cb_Muffler2.IsChecked = Parameter.IsHavingParts(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.SEMI_RACING);
            cb_Muffler3.IsChecked = Parameter.IsHavingParts(CarPartsType.MUFFLER, (int)PARTS_MUFFLER.RACING);

            cb_Intercooler1.IsChecked = Parameter.IsHavingParts(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.S);
            cb_Intercooler2.IsChecked = Parameter.IsHavingParts(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.M);
            cb_Intercooler3.IsChecked = Parameter.IsHavingParts(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.L);
            cb_Intercooler4.IsChecked = Parameter.IsHavingParts(CarPartsType.INTERCOOLER, (int)PARTS_INTERCOOLER.LL);

            cb_Lightweight1.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE1);
            cb_Lightweight2.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE2);
            cb_Lightweight3.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE3);
            cb_Lightweight4.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE4);
            cb_Lightweight5.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE5);
            cb_Lightweight6.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE6);
            cb_Lightweight7.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE7);
            cb_Lightweight8.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT, (int)PARTS_LIGHT_WEIGHT.STAGE8);

            cb_Computer1.IsChecked = Parameter.IsHavingParts(CarPartsType.COMPUTER, (int)PARTS_COMPUTER.LEVEL1);
            cb_Computer2.IsChecked = Parameter.IsHavingParts(CarPartsType.COMPUTER, (int)PARTS_COMPUTER.LEVEL2);

            cb_Gear1.IsChecked = Parameter.IsHavingParts(CarPartsType.GEAR, (int)PARTS_GEAR.NORMAL);
            cb_Gear2.IsChecked = Parameter.IsHavingParts(CarPartsType.GEAR, (int)PARTS_GEAR.CLOSE);
            cb_Gear3.IsChecked = Parameter.IsHavingParts(CarPartsType.GEAR, (int)PARTS_GEAR.SUPER_CLOSE);
            cb_Gear4.IsChecked = Parameter.IsHavingParts(CarPartsType.GEAR, (int)PARTS_GEAR.VARIABLE);

            cb_Catalyst1.IsChecked = Parameter.IsHavingParts(CarPartsType.CATALYST, (int)PARTS_CATALYST.SPORTS);
            cb_Catalyst2.IsChecked = Parameter.IsHavingParts(CarPartsType.CATALYST, (int)PARTS_CATALYST.RACING);

            cb_AirCleaner1.IsChecked = Parameter.IsHavingParts(CarPartsType.AIR_CLEANER, (int)PARTS_AIR_CLEANER.SPORTS);
            cb_AirCleaner2.IsChecked = Parameter.IsHavingParts(CarPartsType.AIR_CLEANER, (int)PARTS_AIR_CLEANER.RACING);

            cb_Bonnet1.IsChecked = Parameter.IsHavingParts(CarPartsType.BONNET, (int)PARTS_BONNET.CARBON);
            cb_Bonnet2.IsChecked = Parameter.IsHavingParts(CarPartsType.BONNET, (int)PARTS_BONNET.PAINT_CARBON);

            cb_Tire1.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_HARD);
            cb_Tire2.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_MEDIUM);
            cb_Tire3.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.COMFORT_SOFT);
            cb_Tire4.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_HARD);
            cb_Tire5.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_MEDIUM);
            cb_Tire6.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_SOFT);
            cb_Tire7.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SPORTS_SUPER_SOFT);
            cb_Tire8.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_HARD);
            cb_Tire9.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_MEDIUM);
            cb_Tire10.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_SOFT);
            cb_Tire11.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RACING_SUPER_SOFT);
            cb_Tire12.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RAIN_INTERMEDIATE);
            cb_Tire13.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.RAIN_HEAVY_WET);
            cb_Tire14.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.DIRT);
            cb_Tire14.IsChecked = Parameter.IsHavingParts(CarPartsType.FRONT_TIRE, (int)PARTS_TIRE.SNOW);

            cb_NOS.IsChecked = Parameter.IsHavingParts(CarPartsType.NOS, (int)PARTS_NOS.ONE);
            cb_Supercharger.IsChecked = Parameter.IsHavingParts(CarPartsType.SUPERCHARGER, (int)PARTS_SUPERCHARGER.ONE);
            cb_IntakeManifold.IsChecked = Parameter.IsHavingParts(CarPartsType.INTAKE_MANIFOLD, (int)PARTS_INTAKE_MANIFOLD.ONE);
            cb_ExhaustManifold.IsChecked = Parameter.IsHavingParts(CarPartsType.EXHAUST_MANIFOLD, (int)PARTS_EXHAUST_MANIFOLD.ONE);
            cb_LightweightWindow.IsChecked = Parameter.IsHavingParts(CarPartsType.LIGHT_WEIGHT_WINDOW, (int)PARTS_LIGHT_WEIGHT_WINDOW.ONE);
            cb_ASCC.IsChecked = Parameter.IsHavingParts(CarPartsType.ASCC, (int)PARTS_ASCC.ONE);
            cb_TCSC.IsChecked = Parameter.IsHavingParts(CarPartsType.TCSC, (int)PARTS_TCSC.ONE);
            cb_BrakeController.IsChecked = Parameter.IsHavingParts(CarPartsType.BRAKE_CONTROLLER, (int)PARTS_BRAKE_CONTROLLER.ONE);
            cb_PropellerShaft.IsChecked = Parameter.IsHavingParts(CarPartsType.PROPELLERSHAFT, (int)PARTS_PROPELLERSHAFT.ONE);

        }
    }
}
